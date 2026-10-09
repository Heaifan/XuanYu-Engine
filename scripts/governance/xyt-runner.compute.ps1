function Find-XytOwner($File, $Mapping) {
    $rule = @($Mapping.ownershipRules) | Where-Object { Test-XytPattern $File $_.pattern } | Select-Object -First 1
    if ($rule) { return [pscustomobject]@{ name = [string]$rule.owner; source = 'fixed-pattern' } }
    return [pscustomobject]@{ name = 'UNKNOWN'; source = 'unresolved' }
}
function Expand-XytTests($Repo, $Test) {
    if ($Test -notmatch '[*?]') { return @($Test) }
    $pattern = Join-Path $Repo ($Test -replace '/', '\')
    return @(Get-ChildItem -Path $pattern -File -ErrorAction SilentlyContinue | ForEach-Object { $_.FullName.Substring($Repo.Length + 1).Replace('\', '/') })
}
function Get-XytPlan($Repo, $Files, $Mapping, $Supplement, $AgentTests) {
    $required = [System.Collections.Generic.List[object]]::new(); $changed = @(); $disputes = [System.Collections.Generic.List[string]]::new()
    foreach ($file in $Files) {
        $owner = Find-XytOwner $file $Mapping
        $caps = @($Mapping.capabilityRules | Where-Object { Test-XytPattern $file $_.pattern })
        if ($owner.name -eq 'UNKNOWN' -or $caps.Count -eq 0) { [void]$disputes.Add("${file}: ownership/capability unresolved") }
        $tests = @($Mapping.testRules | Where-Object { $_.owner -eq $owner.name -or @($_.capabilities | Where-Object { $caps.capability -contains $_ }).Count -gt 0 })
        foreach ($test in $tests) { foreach ($resolved in (Expand-XytTests $Repo ([string]$test.test))) { [void]$required.Add($resolved) } }
        $changed += [pscustomobject]@{ path = $file; ownership = $owner.name; capabilities = @($caps.capability); mapping = $owner.source }
    }
    foreach ($test in @($AgentTests)) { if ($test.Trim()) { [void]$required.Add($test.Trim()) } }
    $supplementalItems = if ($null -eq $Supplement) { @() } else { @($Supplement.items) }
    foreach ($item in $supplementalItems) {
        if (-not [string]$item.test -or -not [string]$item.reason -or -not [string]$item.source) { [void]$disputes.Add('AI supplement requires test, reason, and source') }
    }
    $supplemental = @($supplementalItems | ForEach-Object { [string]$_.test } | Where-Object { $_ })
    foreach ($test in $supplemental) { [void]$required.Add($test) }
    $unique = @($required | Select-Object -Unique)
    return [pscustomobject]@{ schema = 'XYT-C/1'; status = $(if ($disputes.Count) { 'REVIEW_REQUIRED' } else { 'PASS' }); changedFiles = $changed; requiredTests = $unique; supplementalTests = @($supplemental | Select-Object -Unique); disputes = @($disputes); agentAdditions = @($AgentTests | Where-Object { $_.Trim() } | Select-Object -Unique); rule = 'fixed union agent additions and AI supplements; subtraction forbidden' }
}
