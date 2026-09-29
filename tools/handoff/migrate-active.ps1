[CmdletBinding()]
param(
    [Parameter(Mandatory)][ValidateSet('xye','integration','governance')][string]$CoordinatorScope,
    [string]$RepositoryRoot = (Get-Location).Path,
    [switch]$AllowTestWorkspace
)
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path -LiteralPath $RepositoryRoot).Path
$stateDir = Join-Path $root '.git\xye-handoff'
$statePath = Join-Path $stateDir 'state.json'
$releasePath = Join-Path $stateDir 'work-release.json'
$ownershipPath = Join-Path $stateDir 'ownership-locks.json'
function Stop-Migration([string]$Code, [string]$Reason) {
    Write-Output "ACTIVE WAVE MIGRATION DENIED: $Code"
    Write-Output "Reason: $Reason"
    exit 1
}
function Write-Atomic([string]$Path, $Value) {
    $tmp = "$Path.$([guid]::NewGuid().ToString('N')).tmp"
    try {
        [IO.File]::WriteAllText($tmp, ($Value | ConvertTo-Json -Depth 12), (New-Object Text.UTF8Encoding($false)))
        Move-Item -LiteralPath $tmp -Destination $Path -Force
    } finally {
        if (Test-Path -LiteralPath $tmp) { Remove-Item -LiteralPath $tmp -Force }
    }
}
if (-not (Test-Path -LiteralPath (Join-Path $root '.git') -PathType Container)) {
    Stop-Migration 'NOT_A_REPOSITORY' 'Repository root is not a Git worktree.'
}
if (-not (Test-Path -LiteralPath $statePath -PathType Leaf)) {
    Stop-Migration 'STATE_NOT_FOUND' 'Active Wave state.json does not exist.'
}
$state = Get-Content -Raw -LiteralPath $statePath | ConvertFrom-Json
if (-not [bool]$state.active) { Stop-Migration 'NON_ACTIVE_WAVE' 'Only an active Wave may be migrated.' }
$existing = if ($null -ne $state.PSObject.Properties['coordinatorScope']) { [string]$state.coordinatorScope } else { '' }
if (-not [string]::IsNullOrWhiteSpace($existing)) {
    Stop-Migration 'COORDINATOR_SCOPE_ALREADY_SET' "Existing scope is $existing; refusing overwrite."
}
$head = (git -C $root rev-parse HEAD).Trim()
$branch = (git -C $root branch --show-current).Trim()
$workspace = (git -C $root rev-parse --show-toplevel).Trim()
$dirty = @(git -C $root status --porcelain=v1 --untracked-files=all)
$dirtyText = $dirty -join "`n"
$sha = [Security.Cryptography.SHA256]::Create()
$dirtyFingerprint = ([BitConverter]::ToString($sha.ComputeHash([Text.Encoding]::UTF8.GetBytes($dirtyText))) -replace '-', '').ToLowerInvariant()
$state | Add-Member -MemberType NoteProperty -Name schemaVersion -Value 'XYE-HANDOFF/3' -Force
$state | Add-Member -MemberType NoteProperty -Name authorityModel -Value 'R3-LANE-COORDINATOR' -Force
if ($null -eq $state.PSObject.Properties['laneStates']) {
    $state | Add-Member -MemberType NoteProperty -Name laneStates -Value ([pscustomobject]@{})
}
$state.coordinatorScope = $CoordinatorScope
$state | Add-Member -MemberType NoteProperty -Name migration -Value ([pscustomobject]@{
    kind = 'LEGACY_ACTIVE_WAVE_MIGRATION'
    from = 'active-development-without-coordinator-scope'
    at = (Get-Date).ToUniversalTime().ToString('o')
    preservedWorkspace = $workspace
    preservedBranch = $branch
    preservedHead = $head
    preservedDirtyFingerprint = $dirtyFingerprint
}) -Force
if (Test-Path -LiteralPath $releasePath -PathType Leaf) {
    $release = Get-Content -Raw -LiteralPath $releasePath | ConvertFrom-Json
} else {
    $release = [pscustomobject]@{
        schema = 'XYT-WR/1'
        status = 'IDLE'
        coordinatorScope = $CoordinatorScope
        waveId = [string]$state.waveId
        baselineHead = $head
        token = ''
        taskId = ''
        laneId = ''
        ownershipSet = @()
        initializedAt = (Get-Date).ToUniversalTime().ToString('o')
    }
    Write-Atomic $releasePath $release
}
if (Test-Path -LiteralPath $ownershipPath -PathType Leaf) {
    $ownership = Get-Content -Raw -LiteralPath $ownershipPath | ConvertFrom-Json
} else {
    $ownership = [pscustomobject]@{
        schema = 'XYT-OM/1'
        coordinatorScope = $CoordinatorScope
        waveId = [string]$state.waveId
        locks = @()
        initializedAt = (Get-Date).ToUniversalTime().ToString('o')
    }
    Write-Atomic $ownershipPath $ownership
}
Write-Atomic $statePath $state

$afterDirty = @(git -C $root status --porcelain=v1 --untracked-files=all)
if (($afterDirty -join "`n") -ne $dirtyText) { Stop-Migration 'DIRTY_PROVENANCE_CHANGED' 'Dirty set changed during migration.' }
if ((git -C $root rev-parse HEAD).Trim() -ne $head) { Stop-Migration 'HEAD_CHANGED' 'HEAD changed during migration.' }
Write-Output 'ACTIVE WAVE MIGRATION PASS'
Write-Output 'R2_TO_R3 MIGRATION PASS'
Write-Output "CoordinatorScope: $CoordinatorScope"
Write-Output "WaveId: $($state.waveId)"
Write-Output "HEAD: $head"
Write-Output "DirtyFingerprint: $dirtyFingerprint"
