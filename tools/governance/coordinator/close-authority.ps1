[CmdletBinding()]
param(
    [Parameter(Mandatory)][ValidateSet('lane-close','global-close')][string]$Mode,
    [Parameter(Mandatory)][ValidateSet('xye','xyui','integration','governance')][string]$Scope,
    [Parameter(Mandatory)][string]$RepositoryRoot,
    [switch]$WorkingTreeDirty
)
$ErrorActionPreference = 'Stop'
$dir = Join-Path $RepositoryRoot '.git\xye-handoff'
$statePath = Join-Path $dir 'state.json'
function Stop-Close([string]$Code,[string]$Reason) { Write-Output "HANDOFF BLOCKED: $Code"; Write-Output "Reason: $Reason"; exit 1 }
function Read-Json([string]$Path) { if (Test-Path -LiteralPath $Path -PathType Leaf) { return Get-Content -Raw -LiteralPath $Path | ConvertFrom-Json }; return $null }
function Write-Json([string]$Path,$Value) { $tmp = "$Path.$([guid]::NewGuid().ToString('N')).tmp"; try { [IO.File]::WriteAllText($tmp,($Value|ConvertTo-Json -Depth 12),(New-Object Text.UTF8Encoding($false))); Move-Item -LiteralPath $tmp -Destination $Path -Force } finally { if (Test-Path -LiteralPath $tmp) { Remove-Item -LiteralPath $tmp -Force } } }
function Items($Value,[string]$Name) { if ($null -eq $Value) { return @() }; if ($Value.PSObject.Properties.Name -contains $Name) { return @($Value.$Name) }; return @($Value) }
function Is-Empty($Value) { return $null -eq $Value -or [string]::IsNullOrWhiteSpace([string]$Value) -or @($Value).Count -eq 0 }
function Lane-Entries($State) { if ($null -eq $State.PSObject.Properties['laneStates']) { return @() }; return @($State.laneStates.PSObject.Properties) }
function Active-Lanes($State) { @(Lane-Entries $State | Where-Object { [string]$_.Value.state -notin @('FROZEN','CLOSED') }) }
function Release-LaneTasks([string]$Lane) {
    $path = Join-Path $dir 'task-registry.json'; $registry = Read-Json $path
    if ($null -eq $registry) { return }
    foreach ($task in @(Items $registry 'tasks')) { if ([string]$task.Owner -eq $Lane -and [string]$task.Status -eq 'ACTIVE') { $task.Status='RELEASED'; $task.ReleasedAt=(Get-Date).ToUniversalTime().ToString('o') } }
    Write-Json $path $registry
}
function Release-LaneResources([string]$Lane) {
    foreach ($name in @('work-release.json','ownership-locks.json')) {
        $path = Join-Path $dir $name; $value = Read-Json $path; if ($null -eq $value) { continue }
        if ($name -eq 'work-release.json' -and [string]$value.laneId -eq $Lane) { $value.status='IDLE'; $value.token=''; $value.taskId=''; $value.laneId=''; $value.ownershipSet=@() }
        if ($name -eq 'ownership-locks.json' -and $null -ne $value.PSObject.Properties['locks']) { $value.locks=@($value.locks | Where-Object { [string]$_.laneId -ne $Lane -and [string]$_.owner -ne $Lane }) }
        Write-Json $path $value
    }
}
function Assert-GlobalReady($State) {
    $active = @(Active-Lanes $State)
    if ($active.Count -gt 0) { Stop-Close 'ACTIVE_LANES_EXIST' 'Active Lane remains.' }
    $stateTasks = @($State.activeTasks)
    if ($stateTasks.Count -gt 0) { Stop-Close 'ACTIVE_TASKS_EXIST' 'Active Task remains.' }
    $registry = Read-Json (Join-Path $dir 'task-registry.json')
    $tasks = if ($null -ne $registry -and $null -ne $registry.PSObject.Properties['tasks']) { @($registry.tasks) } else { @() }
    $activeTasks = @($tasks | Where-Object { $_.Status -eq 'ACTIVE' })
    if ($activeTasks.Count -gt 0) { Stop-Close 'ACTIVE_TASKS_EXIST' 'Active Task remains.' }
    if ($tasks.Count -gt 0) { Stop-Close 'UNRESOLVED_TASK_REGISTRATIONS' 'Task Registration remains.' }
    $lanes = Lane-Entries $State
    foreach ($lane in $lanes) {
        $ownership = if ($null -ne $lane.Value.PSObject.Properties['ownership']) { $lane.Value.ownership } else { $null }
        $workRelease = if ($null -ne $lane.Value.PSObject.Properties['workRelease']) { $lane.Value.workRelease } else { $null }
        if ($ownership -or $workRelease) { Stop-Close 'OWNERSHIP_NOT_RELEASED' "Lane $($lane.Name) resource remains." }
    }
    $release = Read-Json (Join-Path $dir 'work-release.json'); if ($release -and ([string]$release.status -ne 'IDLE' -or -not (Is-Empty $release.ownershipSet))) { Stop-Close 'OWNERSHIP_NOT_RELEASED' 'Work Release remains.' }
    $locks = Read-Json (Join-Path $dir 'ownership-locks.json'); if ($locks -and @($locks.locks).Count -gt 0) { Stop-Close 'OWNERSHIP_NOT_RELEASED' 'Ownership Lock remains.' }
    if (Test-Path -LiteralPath (Join-Path $dir 'commit-mutex.json') -PathType Leaf) { Stop-Close 'COMMIT_MUTEX_HELD' 'Commit Mutex is held.' }
}
$state = Read-Json $statePath
if ($null -eq $state -or -not [bool]$state.active) { Stop-Close 'NO_ACTIVE_WAVE' 'No Active Wave.' }
$dirty = @(git -C $RepositoryRoot status --porcelain=v1 --untracked-files=all)
if ($WorkingTreeDirty -or $dirty.Count -gt 0) { Stop-Close 'DIRTY_ON_CLOSE' 'Working tree is dirty.' }
if ($Mode -eq 'lane-close') {
    $entry = if ($null -ne $state.PSObject.Properties['laneStates']) { $state.laneStates.PSObject.Properties[$Scope] } else { $null }
    if ($null -ne $entry -and [string]$entry.Value.state -in @('FROZEN','CLOSED')) { Write-Output 'LANE CLOSE PASS: ALREADY_CLOSED'; exit 0 }
    if ($null -eq $state.PSObject.Properties['laneStates']) { $state | Add-Member NoteProperty laneStates ([pscustomobject]@{}) }
    $lane = [pscustomobject]@{ state='FROZEN'; closedAt=(Get-Date).ToUniversalTime().ToString('o'); ownership=$null; workRelease=$null }
    if ($null -eq $entry) { $state.laneStates | Add-Member NoteProperty $Scope $lane } else { $entry.Value=$lane }
    Release-LaneTasks $Scope; Release-LaneResources $Scope; Write-Json $statePath $state
    Write-Output 'LANE CLOSE PASS'; exit 0
}
$coordinator = if ($null -ne $state.PSObject.Properties['coordinatorScope']) { [string]$state.coordinatorScope } else { '' }
if ([string]::IsNullOrWhiteSpace($coordinator) -or $Scope -ne $coordinator) { Stop-Close 'COORDINATOR_REQUIRED' "Scope $Scope is not Coordinator." }
Assert-GlobalReady $state
if ($null -eq $state.PSObject.Properties['closedAt']) { $state | Add-Member NoteProperty closedAt $null }
$state.active=$false; $state.closedAt=(Get-Date).ToUniversalTime().ToString('o'); Write-Json $statePath $state
Write-Output 'HANDOFF CLOSE PASS'
