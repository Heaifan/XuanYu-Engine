$ErrorActionPreference = 'Stop'
$gate = Join-Path $PSScriptRoot 'test-evidence-gate.ps1'
$failures = [System.Collections.Generic.List[string]]::new()

function Assert-Gate([string]$Name, [string]$IssueType, [string]$RequiredTier, [string]$AchievedTiers, [string]$RuntimeAcceptance, [string]$ProductAcceptance, [string]$Expected) {
    $output = @(& $gate -IssueType $IssueType -RequiredTier $RequiredTier -AchievedTiers $AchievedTiers -RuntimeAcceptance $RuntimeAcceptance -ProductAcceptance $ProductAcceptance)
    $actual = ($output | Where-Object { $_ -like 'TEST EVIDENCE GATE:*' }) -replace '^TEST EVIDENCE GATE: ', ''
    if ($actual -ne $Expected) { [void]$script:failures.Add("$Name expected $Expected but got $actual") }
}

Assert-Gate 'T1 required with T1 achieved' 'Pure Math' 'T1' 'T1' 'NOT_REQUIRED' 'NOT_REQUIRED' 'PASS'
Assert-Gate 'T2 required with T1 only' 'UiVm' 'T2' 'T1' 'NOT_REQUIRED' 'NOT_REQUIRED' 'BLOCKED'
Assert-Gate 'T3 required with T0/T1/T2' 'Vulkan Swapchain' 'T3' 'T0,T1,T2' 'NOT_RUN' 'NOT_REQUIRED' 'BLOCKED'
Assert-Gate 'T3 required with T3' 'Vulkan Swapchain' 'T3' 'T3' 'PASS' 'NOT_REQUIRED' 'PASS'
Assert-Gate 'T4 required with T3 only' 'Final Visual Effect' 'T4' 'T3' 'PASS' 'NOT_RUN' 'BLOCKED'
Assert-Gate 'Headless cannot map to T3' 'Avalonia Headless' 'T3' 'T2' 'NOT_RUN' 'NOT_REQUIRED' 'BLOCKED'
Assert-Gate 'Static contract cannot map to runtime' 'Static Contract' 'T3' 'T0' 'NOT_RUN' 'NOT_REQUIRED' 'BLOCKED'

if ($failures.Count -gt 0) { $failures | ForEach-Object { "FAIL: $_" }; exit 1 }
'TEST EVIDENCE GATE SELFTEST: PASS'
'Cases: 7/7'

