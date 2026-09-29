$ErrorActionPreference = 'Stop'
$gate = Join-Path $PSScriptRoot 'deferred-capability-gate.ps1'
$tempRoot = Join-Path ([IO.Path]::GetTempPath()) ('deferred-capability-selftest-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $tempRoot | Out-Null
$checks = New-Object System.Collections.Generic.List[string]

function Check([string]$Name, [scriptblock]$Action) {
    try { & $Action; $checks.Add("PASS $Name") } catch { $checks.Add("FAIL $Name :: $($_.Exception.Message)") }
}
function Write-Registry($Name, $Value) { $path = Join-Path $tempRoot "$Name.json"; $Value | ConvertTo-Json -Depth 12 | Set-Content -LiteralPath $path -Encoding UTF8; return $path }
function Run-Gate($Path, [string[]]$Tags = @()) { & $gate -RegistryPath $Path -CapabilityTags $Tags 2>&1 | Out-String | Out-Null; return $LASTEXITCODE }
function Run-GateOutput($Path, [string[]]$Tags = @()) { return (& $gate -RegistryPath $Path -CapabilityTags $Tags 2>&1 | Out-String) }
function Base-Debt([string]$Status = 'DEFERRED-SAFE') { return [pscustomobject]@{ DebtId='SAFE-001'; Title='Safe'; CurrentTemporarySolution='Temporary'; DeferredCapability='Future'; ReasonForDeferral='Scope'; ValidEnvelope='Small scope only'; ActivationTriggers=@([pscustomobject]@{TriggerId='TRIGGER-1'; Description='Explicit trigger'; CapabilityTags=@('future-capability')}); BlockedCapabilities=@([pscustomobject]@{Name='Blocked feature'; CapabilityTags=@('blocked-feature')}); RequiredClosure=@('Implement and verify'); TestDebt=@('Add regression'); IntroducedAt=[pscustomobject]@{Commit='abc';Version='v1';Date='2026-09-29'}; LastReviewedAt='2026-09-29'; Status=$Status; Evidence=@('Evidence'); ClosureEvidence=@() } }

Check '完整 SAFE → PASS' { $path = Write-Registry 'safe' ([pscustomobject]@{contract='Deferred Capability Contract';debts=@(Base-Debt)}); if ((Run-Gate $path) -ne 0) { throw 'expected PASS' } }
Check '无 ValidEnvelope → FAIL' { $debt = Base-Debt; $debt.ValidEnvelope = ''; $path = Write-Registry 'no-envelope' ([pscustomobject]@{debts=@($debt)}); if ((Run-Gate $path) -ne 1) { throw 'expected FAIL' } }
Check '无 ActivationTriggers → FAIL' { $debt = Base-Debt; $debt.ActivationTriggers = @(); $path = Write-Registry 'no-trigger' ([pscustomobject]@{debts=@($debt)}); if ((Run-Gate $path) -ne 1) { throw 'expected FAIL' } }
Check '重复 DebtId → FAIL' { $path = Write-Registry 'duplicate' ([pscustomobject]@{debts=@((Base-Debt), (Base-Debt))}); if ((Run-Gate $path) -ne 1) { throw 'expected FAIL' } }
Check 'BLOCKING 无 BlockedCapabilities → FAIL' { $debt = Base-Debt 'BLOCKING'; $debt.BlockedCapabilities = @(); $path = Write-Registry 'blocking-no-blocked' ([pscustomobject]@{debts=@($debt)}); if ((Run-Gate $path) -ne 1) { throw 'expected FAIL' } }
Check 'CLOSED 无 ClosureEvidence → FAIL' { $debt = Base-Debt 'CLOSED'; $path = Write-Registry 'closed-no-evidence' ([pscustomobject]@{debts=@($debt)}); if ((Run-Gate $path) -ne 1) { throw 'expected FAIL' } }
Check 'Activation trigger 命中 → BLOCKING' { $path = Write-Registry 'activation-match' ([pscustomobject]@{debts=@((Base-Debt))}); if ((Run-GateOutput $path @('future-capability')) -notmatch 'STATUS = BLOCKING') { throw 'expected BLOCKING status' } }
Check '任务 Capability 命中 BLOCKING → BLOCKED' { $debt = Base-Debt 'BLOCKING'; $path = Write-Registry 'blocking-match' ([pscustomobject]@{debts=@($debt)}); if ((Run-Gate $path @('blocked-feature')) -ne 2) { throw 'expected BLOCKED' } }
Check '不相关任务 → PASS' { $debt = Base-Debt 'BLOCKING'; $path = Write-Registry 'blocking-unrelated' ([pscustomobject]@{debts=@($debt)}); if ((Run-Gate $path @('unrelated')) -ne 0) { throw 'expected PASS' } }

$checks | ForEach-Object { Write-Output $_ }
if ($checks | Where-Object { $_ -like 'FAIL *' }) { exit 1 }
Write-Output 'DEFERRED CAPABILITY SELFTEST PASS'
