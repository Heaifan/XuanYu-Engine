[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$StatePath,
    [Parameter(Mandatory)][ValidateSet('xye','xyui','integration','governance')][string]$Scope,
    [string]$CoordinatorScope,
    [string]$Field,
    [string]$Value,
    [string]$Transition,
    [ValidateSet('BLOCKED','CONFLICT','UNKNOWN_DIRTY','CANDIDATE_MISMATCH','EVIDENCE_STALE','HARNESS_FAILURE')][string]$ExceptionStatus
)

$ErrorActionPreference = 'Stop'
$laneStates = @('JOINED','ACTIVE','IMPLEMENTATION_COMPLETE','IMPLEMENTATION_HANDOFF_READY','DEPENDENCY_RELEASED','REGRESSION_REQUIRED','PRODUCT_ACCEPTANCE_PENDING','ACCEPTED','FROZEN')
$globalFields = @('GATE STATUS','PRODUCT REGRESSION','UNRESOLVED UNKNOWN','CANDIDATE TREE MATCH','COMMIT ELIGIBILITY','GLOBAL PASS','RELEASE READY')
$laneFields = @('IMPLEMENTATION STATUS','OWN-SCOPE TEST STATUS','BUILD STATUS','HANDOFF READINESS','PRODUCT ACCEPTANCE STATUS')
$passTypes = @('IMPLEMENTATION_PASS','OWN_SCOPE_PASS','REGRESSION_PASS','PRODUCT_ACCEPTANCE_PASS','INTEGRATION_PASS')
$next = @{
    JOINED = 'ACTIVE'; ACTIVE = 'IMPLEMENTATION_COMPLETE'; IMPLEMENTATION_COMPLETE = 'IMPLEMENTATION_HANDOFF_READY'
    IMPLEMENTATION_HANDOFF_READY = 'DEPENDENCY_RELEASED'; DEPENDENCY_RELEASED = 'REGRESSION_REQUIRED'
    REGRESSION_REQUIRED = 'PRODUCT_ACCEPTANCE_PENDING'; PRODUCT_ACCEPTANCE_PENDING = 'ACCEPTED'; ACCEPTED = 'FROZEN'
}

function Stop-Authority([string]$Code, [string]$Reason) {
    Write-Output "AUTHORITY_REJECTED: $Code"
    Write-Output "Reason: $Reason"
    exit 1
}

function Write-Atomic($State) {
    $tmp = "$StatePath.$([guid]::NewGuid().ToString('N')).tmp"
    try {
        [IO.File]::WriteAllText($tmp, ($State | ConvertTo-Json -Depth 12), (New-Object Text.UTF8Encoding($false)))
        Move-Item -LiteralPath $tmp -Destination $StatePath -Force
    } finally { if (Test-Path -LiteralPath $tmp) { Remove-Item -LiteralPath $tmp -Force } }
}

if (-not (Test-Path -LiteralPath $StatePath -PathType Leaf)) { Stop-Authority 'STATE_NOT_FOUND' 'Active Wave state.json does not exist.' }
$state = Get-Content -Raw -LiteralPath $StatePath | ConvertFrom-Json
$isCoordinator = -not [string]::IsNullOrWhiteSpace($CoordinatorScope) -and $Scope -eq $CoordinatorScope
$forbiddenBare = @('PASS','ALL PASS','GLOBAL PASS','COMMIT ELIGIBLE')
if ($Value -and $forbiddenBare -contains $Value.Trim().ToUpperInvariant()) { Stop-Authority 'TYPED_PASS_REQUIRED' 'PASS 需要明确类型。' }
if ($Field -and $globalFields -contains $Field.ToUpperInvariant() -and -not $isCoordinator) { Stop-Authority 'GLOBAL_WRITE_FORBIDDEN' "Lane $Scope 无权写入 Global Status。" }
if ($Field -and $globalFields -notcontains $Field.ToUpperInvariant() -and $laneFields -notcontains $Field.ToUpperInvariant()) { Stop-Authority 'FIELD_NOT_ALLOWED' "未知或禁止字段：$Field" }
if ($Value -and $Value.ToUpperInvariant().Contains('PASS') -and $passTypes -notcontains $Value.ToUpperInvariant()) { Stop-Authority 'TYPED_PASS_REQUIRED' 'PASS 需要明确类型。' }
if ($Transition -and $laneStates -notcontains $Transition.ToUpperInvariant()) { Stop-Authority 'STATE_NOT_ALLOWED' "未知 Lane 状态：$Transition" }

if ($null -eq $state.PSObject.Properties['laneStates']) { $state | Add-Member -NotePropertyName laneStates -NotePropertyValue ([pscustomobject]@{}) }
$lane = $state.laneStates.PSObject.Properties[$Scope]
$current = if ($null -eq $lane) { 'JOINED' } else { [string]$lane.Value.state }
if ($null -eq $lane -and (($Field -and $laneFields -contains $Field.ToUpperInvariant()) -or $ExceptionStatus)) {
    $entry = [pscustomobject]@{ state = 'JOINED'; fields = [pscustomobject]@{}; exceptions = @() }
    $state.laneStates | Add-Member -NotePropertyName $Scope -NotePropertyValue $entry
    $lane = $state.laneStates.PSObject.Properties[$Scope]
}
if ($ExceptionStatus) {
    if ($current -eq 'FROZEN' -or $lane.Value.state -eq 'FROZEN') { Stop-Authority 'FROZEN_STATE' 'FROZEN 后禁止回写。' }
    $lane.Value.exceptions = @($lane.Value.exceptions) + $ExceptionStatus.ToUpperInvariant() | Select-Object -Unique
}
if ($Transition) {
    $target = $Transition.ToUpperInvariant()
    if ($current -eq 'FROZEN') { Stop-Authority 'FROZEN_STATE' 'FROZEN 后禁止回写。' }
    if ($target -ne $current -and ($next[$current] -ne $target)) { Stop-Authority 'ILLEGAL_STATE_TRANSITION' "$current -> $target 不允许。" }
    $entry = if ($null -eq $lane) { [pscustomobject]@{ state = $current; fields = [pscustomobject]@{}; exceptions = @() } } else { $lane.Value }
    $entry.state = $target
    if ($null -eq $lane) { $state.laneStates | Add-Member -NotePropertyName $Scope -NotePropertyValue $entry } else { $lane.Value = $entry }
}
if ($Field) {
    if ($current -eq 'FROZEN' -or ($lane -and $lane.Value.state -eq 'FROZEN')) { Stop-Authority 'FROZEN_STATE' 'FROZEN 后禁止回写。' }
    $targetSet = if ($globalFields -contains $Field.ToUpperInvariant()) { 'globalStatus' } else { 'fields' }
    if ($null -eq $state.PSObject.Properties[$targetSet]) { $state | Add-Member -NotePropertyName $targetSet -NotePropertyValue ([pscustomobject]@{}) }
    $container = if ($targetSet -eq 'globalStatus') { $state.globalStatus } else { $state.laneStates.$Scope.fields }
    if ($null -eq $container) { $container = [pscustomobject]@{}; if ($targetSet -eq 'globalStatus') { $state.globalStatus = $container } else { $state.laneStates.$Scope.fields = $container } }
    $container | Add-Member -NotePropertyName ($Field.ToUpperInvariant()) -NotePropertyValue $Value -Force
}
Write-Atomic $state
if ($isCoordinator -and $Field -and $globalFields -contains $Field.ToUpperInvariant()) { Write-Output 'AUTHORITY PASS' } else { Write-Output 'LANE STATE PASS' }
