 $runnerPath = Join-Path $PSScriptRoot 'xyt-process-runner.ps1'

function Start-XytAttempt {
    param([pscustomobject]$Spec, [int]$Attempt)
    $job = Start-Job -ScriptBlock {
        param($command, $workingDirectory, $attempt, $runnerPath)
        . $runnerPath
        $ErrorActionPreference = 'Continue'
        $oldAttempt = $env:XYT_ATTEMPT
        $env:XYT_ATTEMPT = [string]$attempt
        try {
            if ($workingDirectory) { Set-Location -LiteralPath $workingDirectory }
            $shell = (Get-Command pwsh.exe -ErrorAction SilentlyContinue).Source
            if (!$shell) { $shell = (Get-Command powershell.exe).Source }
            $run = Invoke-XytProcess -FilePath $shell -WorkingDirectory $workingDirectory -Arguments @('-NoProfile','-NonInteractive','-ExecutionPolicy','Bypass','-Command',$command)
            [pscustomobject]@{ exitCode = [int]$run.ExitCode; output = $run.Stdout + $run.Stderr }
        } catch {
            [pscustomobject]@{ exitCode = 1; output = $_ | Out-String }
        } finally { $env:XYT_ATTEMPT = $oldAttempt }
    } -ArgumentList $Spec.command, $Spec.workingDirectory, $Attempt, $runnerPath
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
