function Invoke-XytSchedule([pscustomobject]$Plan) {
    $cycle = Find-XytDependencyCycle $Plan
    if ($cycle) { throw (New-XytDependencyError $cycle) }
    $pending = [Collections.Generic.List[object]]::new(); @($Plan.tests) | ForEach-Object { [void]$pending.Add($_) }
    $active = [Collections.Generic.List[object]]::new(); $results = [ordered]@{}
    while ($pending.Count -or $active.Count) {
        $changed = $false
        foreach ($spec in @($pending.ToArray())) {
            if ($active.Count -ge 4) { break }
            $deps = @($spec.dependsOn | Where-Object { $_ })
            if ([string]::IsNullOrWhiteSpace([string]$spec.command)) { $results[$spec.testId]=New-XytResult $spec 'UNCLASSIFIED' 0 'Missing command'; [void]$pending.Remove($spec); $changed=$true; continue }
            if (($deps | Where-Object { !$results.Contains($_) }).Count) { continue }
            if (($deps | Where-Object { $results[$_].status -ne 'PASS' }).Count) { $results[$spec.testId]=New-XytResult $spec 'BLOCKED_BY' 0 'Dependency did not PASS'; [void]$pending.Remove($spec); $changed=$true; continue }
            [void]$active.Add((Start-XytAttempt $spec 1)); [void]$pending.Remove($spec); $changed=$true
        }
        foreach ($item in @($active.ToArray())) {
            $seconds = if ($null -eq $item.spec.timeoutSeconds) {300} else {[double]$item.spec.timeoutSeconds}
            if ($item.job.State -in @('Completed','Failed','Stopped')) {
                $attempt = Complete-XytAttempt $item
                if ($attempt.status -eq 'FAIL' -and $item.attempt -eq 1) { [void]$active.Remove($item); [void]$active.Add((Start-XytAttempt $item.spec 2)); continue }
                $results[$item.spec.testId]=New-XytResult $item.spec $attempt.status $attempt.attempts '' $attempt
                [void]$active.Remove($item); $changed=$true; continue
            }
            if (([datetime]::UtcNow - $item.startedAt).TotalSeconds -ge $seconds) { $attempt=Stop-XytAttempt $item; $results[$item.spec.testId]=New-XytResult $item.spec 'TIMEOUT' $attempt.attempts '' $attempt; [void]$active.Remove($item); $changed=$true }
        }
        if (!$active.Count -and $pending.Count -and !$changed) { foreach ($spec in @($pending.ToArray())) { $results[$spec.testId]=New-XytResult $spec 'UNCLASSIFIED' 0 'Unresolved dependency graph'; [void]$pending.Remove($spec) } }
        if ($active.Count) { Start-Sleep -Milliseconds 25 }
    }
    @($results.Values)
}
