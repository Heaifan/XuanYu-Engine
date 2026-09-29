function New-XytResult {
    param([pscustomobject]$Spec, [string]$Status, [int]$Attempts, [string]$Output='', [pscustomobject]$AttemptResult)
    $end = if ($AttemptResult) { $AttemptResult.endedAt } else { [datetime]::UtcNow }
    $start = if ($AttemptResult) { $AttemptResult.startedAt } else { $end }
    $elapsed = ($end - $start).TotalSeconds
    $warning = $false
    if ($null -ne $Spec.maxDurationSeconds -and $elapsed -gt [double]$Spec.maxDurationSeconds) { $warning = $true }
    $finalStatus = if ($Status -eq 'FLAKY') { $Status } elseif ($Status -eq 'PASS' -and $Attempts -gt 1) { 'FLAKY' } else { $Status }
    [pscustomobject]@{
        testId=$Spec.testId; status=$finalStatus; attempts=$Attempts; startedAt=$start; endedAt=$end
        durationSeconds=[math]::Round($elapsed, 3); exitCode=if ($AttemptResult) { $AttemptResult.exitCode } else { $null }
        output=if ($AttemptResult) { $AttemptResult.output } else { $Output }; rootCause=if ($finalStatus -in @('FAIL','FLAKY')) { 'UNKNOWN' } else { $null }
        costWarning=$warning; warnings=if ($warning) { @('COST WARNING') } else { @() }
    }
}
