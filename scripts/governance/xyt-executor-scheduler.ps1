function Invoke-XytSchedule {
    param([pscustomobject]$Plan)
    $cycle = Find-XytDependencyCycle $Plan
    if ($cycle) { throw (New-XytDependencyError $cycle) }
    $pending = [System.Collections.Generic.List[object]]::new(); @($Plan.tests) | ForEach-Object { [void]$pending.Add($_) }
    $active = [System.Collections.Generic.List[object]]::new(); $results = [ordered]@{}
    while ($pending.Count -gt 0 -or $active.Count -gt 0) {
        $changed = $false
        foreach ($spec in @($pending.ToArray())) {
            $deps = @($spec.dependsOn | Where-Object { $_ })
            if ([string]::IsNullOrWhiteSpace([string]$spec.command)) {
                $results[$spec.testId] = New-XytResult -Spec $spec -Status 'UNCLASSIFIED' -Attempts 0 -Output 'Missing command'
                [void]$pending.Remove($spec); $changed = $true; continue
            }
            if ($deps.Count -gt 0 -and ($deps | Where-Object { -not $results.Contains($_) }).Count -gt 0) { continue }
            if (($deps | Where-Object { $results[$_].status -ne 'PASS' }).Count -gt 0) {
                $results[$spec.testId] = New-XytResult -Spec $spec -Status 'BLOCKED_BY' -Attempts 0 -Output 'Dependency did not PASS'
                [void]$pending.Remove($spec); $changed = $true; continue
            }
            [void]$active.Add((Start-XytAttempt -Spec $spec -Attempt 1)); [void]$pending.Remove($spec); $changed = $true
        }
        foreach ($item in @($active.ToArray())) {
            $timeoutValue = $item.spec.timeoutSeconds
            $timeout = if ($null -eq $timeoutValue) { 300 } else { [double]$timeoutValue }
            if ($item.job.State -eq 'Completed') {
                $attemptResult = Complete-XytAttempt $item
                if ($attemptResult.status -eq 'FAIL' -and $item.attempt -eq 1) { [void]$active.Remove($item); [void]$active.Add((Start-XytAttempt $item.spec 2)); continue }
                $results[$item.spec.testId] = New-XytResult -Spec $item.spec -Status $attemptResult.status -Attempts $attemptResult.attempts -AttemptResult $attemptResult
                [void]$active.Remove($item); $changed = $true; continue
            }
            if (([datetime]::UtcNow - $item.startedAt).TotalSeconds -ge $timeout) {
                $attemptResult = Stop-XytAttempt $item
                $results[$item.spec.testId] = New-XytResult -Spec $item.spec -Status 'TIMEOUT' -Attempts $attemptResult.attempts -AttemptResult $attemptResult
                [void]$active.Remove($item); $changed = $true
            }
        }
        if ($active.Count -eq 0 -and $pending.Count -gt 0 -and -not $changed) {
            foreach ($spec in @($pending.ToArray())) {
                $results[$spec.testId] = New-XytResult -Spec $spec -Status 'UNCLASSIFIED' -Attempts 0 -Output 'Unresolved dependency graph'
                [void]$pending.Remove($spec)
            }
        }
        if ($active.Count -gt 0) { Start-Sleep -Milliseconds 25 }
    }
    return @($results.Values)
}

function Find-XytDependencyCycle {
    param([pscustomobject]$Plan)
    $nodes = @($Plan.tests); $edges = @{}
    foreach ($node in $nodes) { $edges[[string]$node.testId] = @($node.dependsOn | Where-Object { $_ }) }
    function Visit-XytNode([string]$Node, [string[]]$Path) {
        if ($Path -contains $Node) { return (($Path + $Node) -join ' -> ') }
        foreach ($next in @($edges[$Node])) {
            if (!$edges.ContainsKey([string]$next)) { continue }
            $found = Visit-XytNode ([string]$next) ($Path + $Node)
            if ($found) { return $found }
        }
        $null
    }
    foreach ($node in $nodes) {
        $found = Visit-XytNode ([string]$node.testId) @()
        if ($found) { return $found }
    }
    $null
}
