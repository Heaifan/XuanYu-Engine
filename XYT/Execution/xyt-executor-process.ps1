function Start-XytAttempt([pscustomobject]$Spec, [int]$Attempt) {
    $job = Start-Job -ScriptBlock {
        param($command, $directory, $attempt)
        $old = $env:XYT_ATTEMPT; $env:XYT_ATTEMPT = [string]$attempt
        try {
            if ($directory) { Set-Location -LiteralPath $directory }
            $shell = (Get-Command pwsh.exe -ErrorAction SilentlyContinue).Source
            if (!$shell) { $shell = (Get-Command powershell.exe).Source }
            $output = @(& $shell -NoProfile -NonInteractive -ExecutionPolicy Bypass -Command $command 2>&1)
            [pscustomobject]@{ exitCode=[int]$LASTEXITCODE; output=($output | Out-String) }
        } catch { [pscustomobject]@{ exitCode=1; output=($_ | Out-String) } }
        finally { $env:XYT_ATTEMPT = $old }
    } -ArgumentList $Spec.command, $Spec.workingDirectory, $Attempt
    [pscustomobject]@{ spec=$Spec; job=$job; attempt=$Attempt; startedAt=[datetime]::UtcNow }
}

function Stop-XytAttempt([pscustomobject]$Active) {
    Stop-Job $Active.job -ErrorAction SilentlyContinue
    Remove-Job $Active.job -Force -ErrorAction SilentlyContinue
    [pscustomobject]@{ status='TIMEOUT'; attempts=$Active.attempt; startedAt=$Active.startedAt; endedAt=[datetime]::UtcNow; output='Execution timeout'; exitCode=$null }
}

function Complete-XytAttempt([pscustomobject]$Active) {
    $raw = @(Receive-Job $Active.job -ErrorAction SilentlyContinue)
    Remove-Job $Active.job -Force -ErrorAction SilentlyContinue
    $payload = if ($raw.Count -and $raw[-1].PSObject.Properties['exitCode']) { $raw[-1] } else { [pscustomobject]@{ exitCode=1; output=($raw | Out-String) } }
    [pscustomobject]@{ status=$(if ([int]$payload.exitCode -eq 0) {'PASS'} else {'FAIL'}); attempts=$Active.attempt; startedAt=$Active.startedAt; endedAt=[datetime]::UtcNow; output=$payload.output; exitCode=$payload.exitCode }
}
