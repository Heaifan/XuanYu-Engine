function Start-XytAttempt {
    param([pscustomobject]$Spec, [int]$Attempt)
    $job = Start-Job -ScriptBlock {
        param($command, $workingDirectory, $attempt)
        $ErrorActionPreference = 'Continue'
        $oldAttempt = $env:XYT_ATTEMPT
        $env:XYT_ATTEMPT = [string]$attempt
        try {
            if ($workingDirectory) { Set-Location -LiteralPath $workingDirectory }
            $lines = @(& pwsh -NoProfile -NonInteractive -ExecutionPolicy Bypass -Command $command 2>&1 | ForEach-Object { $_.ToString() })
            [pscustomobject]@{ exitCode = [int]$LASTEXITCODE; output = [string]::Join("`n", $lines) }
        } catch {
            [pscustomobject]@{ exitCode = 1; output = $_ | Out-String }
        } finally { $env:XYT_ATTEMPT = $oldAttempt }
    } -ArgumentList $Spec.command, $Spec.workingDirectory, $Attempt
    [pscustomobject]@{ spec=$Spec; job=$job; attempt=$Attempt; startedAt=[datetime]::UtcNow }
}

function Stop-XytAttempt {
    param([pscustomobject]$Active)
    Stop-Job -Job $Active.job -ErrorAction SilentlyContinue
    Remove-Job -Job $Active.job -Force -ErrorAction SilentlyContinue
    [pscustomobject]@{ status='TIMEOUT'; attempts=$Active.attempt; startedAt=$Active.startedAt; endedAt=[datetime]::UtcNow; output='Execution timeout'; exitCode=$null }
}

function Complete-XytAttempt {
    param([pscustomobject]$Active)
    $raw = @(Receive-Job -Job $Active.job -ErrorAction SilentlyContinue)
    Remove-Job -Job $Active.job -Force -ErrorAction SilentlyContinue
    if ($raw.Count -eq 0) { $payload = [pscustomobject]@{ exitCode=1; output='No execution result' } }
    else { $payload = $raw[-1] }
    [pscustomobject]@{ status=if ([int]$payload.exitCode -eq 0) { 'PASS' } else { 'FAIL' }; attempts=$Active.attempt; startedAt=$Active.startedAt; endedAt=[datetime]::UtcNow; output=$payload.output; exitCode=$payload.exitCode }
}
