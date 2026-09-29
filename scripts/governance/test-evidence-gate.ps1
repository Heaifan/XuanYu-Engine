[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$IssueType,
    [Parameter(Mandatory = $true)][ValidateSet('T0','T1','T2','T3','T4')][string]$RequiredTier,
    [Parameter(Mandatory = $true)][string[]]$AchievedTiers,
    [ValidateSet('PASS','FAIL','NOT_RUN','NOT_REQUIRED')][string]$RuntimeAcceptance = 'NOT_REQUIRED',
    [ValidateSet('PASS','FAIL','NOT_RUN','NOT_REQUIRED')][string]$ProductAcceptance = 'NOT_REQUIRED'
)

$tierOrder = @{ T0 = 0; T1 = 1; T2 = 2; T3 = 3; T4 = 4 }
$achieved = @($AchievedTiers | ForEach-Object { $_ -split ',' } | ForEach-Object { $_.ToUpperInvariant().Trim() } | Where-Object { $tierOrder.ContainsKey($_) } | Select-Object -Unique)
$requiredRank = $tierOrder[$RequiredTier]
$highest = -1
if ($achieved.Count -gt 0) { $highest = ($achieved | ForEach-Object { $tierOrder[$_] } | Measure-Object -Maximum).Maximum }
$tierReached = $highest -ge $requiredRank
$runtimeNeeded = $requiredRank -ge $tierOrder.T3
$productNeeded = $requiredRank -ge $tierOrder.T4
$runtimeOk = -not $runtimeNeeded -or $RuntimeAcceptance -eq 'PASS'
$productOk = -not $productNeeded -or $ProductAcceptance -eq 'PASS'
$gate = if ($tierReached -and $runtimeOk -and $productOk) { 'PASS' } else { 'BLOCKED' }
$automation = if ($tierReached) { 'PASS' } else { 'INCOMPLETE' }
$runtime = if ($runtimeNeeded) { $RuntimeAcceptance } else { 'NOT_REQUIRED' }
$product = if ($productNeeded) { $ProductAcceptance } else { 'NOT_REQUIRED' }
$gaps = [System.Collections.Generic.List[string]]::new()
if (-not $tierReached) { [void]$gaps.Add("Required $RequiredTier evidence is missing; highest achieved is $(if($highest -ge 0){'T' + $highest}else{'none'})") }
if (-not $runtimeOk) { [void]$gaps.Add('T3 real-runtime acceptance is required') }
if (-not $productOk) { [void]$gaps.Add('T4 product acceptance is required') }
if ($gaps.Count -eq 0) { [void]$gaps.Add('none') }

"ISSUE TYPE: $IssueType"
"REQUIRED EVIDENCE TIER: $RequiredTier"
"ACHIEVED EVIDENCE TIER: $(if($highest -ge 0){'T' + $highest}else{'NONE'})"
"ACHIEVED TIERS: $($achieved -join ',')"
"AUTOMATION STATUS: $automation"
"RUNTIME STATUS: $runtime"
"PRODUCT ACCEPTANCE: $product"
"EVIDENCE GAP: $($gaps -join '; ')"
"TEST EVIDENCE GATE: $gate"

