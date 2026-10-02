[CmdletBinding()]
param(
    [string]$RepositoryRoot = (Get-Location).Path,
    [Parameter(Mandatory)][string]$CurrentTaskId,
    [Parameter(Mandatory)][string]$DirtyPath,
    [string]$TaskRegistryPath,
    [string]$DependencyStatePath
)
$ErrorActionPreference = 'Stop'
function Read-OptionalJson([string]$path) {
    try { if (Test-Path -LiteralPath $path -PathType Leaf) { return (Get-Content -Raw -LiteralPath $path | ConvertFrom-Json) } } catch { return $null }
    return $null
}
function Items($value, [string]$property) { if ($null -eq $value) { return @() }; if ($value.PSObject.Properties.Name -contains $property) { return @($value.$property) }; return @($value) }
function Clean([string]$value) { return $value.Replace('\','/').TrimStart('./') }
function Hit([string]$path, [string]$scope) {
    $p = Clean $path; $s = (Clean $scope).TrimEnd('/')
    if ($s.EndsWith('/**')) { return $p.StartsWith($s.Substring(0,$s.Length-2), [StringComparison]::OrdinalIgnoreCase) }
    return $p -eq $s -or $p.StartsWith($s + '/', [StringComparison]::OrdinalIgnoreCase) -or $p -like $s
}
function Consumes($task, [string]$owner, [string]$path, $scopes) {
    foreach ($d in @($task.ExpectedDependencies)) {
        if ([string]$d -eq $owner -or (Hit $path ([string]$d))) { return $true }
        foreach ($scope in @($scopes)) { if ((Clean ([string]$d)) -eq (Clean ([string]$scope))) { return $true } }
    }
    return $false
}
function Result([string]$class, [string]$allowed, [string]$eligible, [string]$owner, [string]$status, $matched, [string]$reason, [string]$recovery='NONE') {
    [pscustomobject]@{ Classification=$class; CodingAllowed=$allowed; FinalEvidenceEligible=$eligible; CandidateAllowed=($class -ne 'UNAUTHORIZED_DIRTY'); ReleaseAllowed=($class -ne 'UNAUTHORIZED_DIRTY'); OwnerTaskId=$owner; OwnerStatus=$status; MatchedWriteScope=@($matched); RecoveryWindow=$recovery; Reason=$reason } | ConvertTo-Json -Depth 8
}
$root = [IO.Path]::GetFullPath($RepositoryRoot)
if (!$TaskRegistryPath) { $TaskRegistryPath = Join-Path $root '.git\xye-handoff\task-registry.json' }
if (!$DependencyStatePath) { $DependencyStatePath = Join-Path $root '.git\xye-handoff\dependency-state.json' }
$registry = Read-OptionalJson $TaskRegistryPath
$dependency = Read-OptionalJson $DependencyStatePath
$tasks = if ($registry) { Items $registry 'tasks' } else { @() }
$current = @($tasks | Where-Object { [string]$_.TaskId -eq $CurrentTaskId }) | Select-Object -First 1
$active = @($tasks | Where-Object { [string]$_.Status -eq 'ACTIVE' -and @($_.WriteScope | Where-Object { Hit $DirtyPath ([string]$_) }).Count })
$matches = @($active | ForEach-Object { [pscustomobject]@{ Task=$_; Scopes=@($_.WriteScope | Where-Object { Hit $DirtyPath ([string]$_) }) } })
if ($matches.Count -gt 1) { Result 'UNAUTHORIZED_DIRTY' 'NO' 'NO' '' '' @() 'Multiple ACTIVE tasks claim the dirty path; ownership is not unique.' 'RECOVERY_WINDOW'; exit 0 }
if ($current -and $current.Owner -and @($current.WriteScope | Where-Object { Hit $DirtyPath ([string]$_) }).Count) { $m=@($current.WriteScope | Where-Object { Hit $DirtyPath ([string]$_) }); Result 'FRIENDLY_ACTIVE' 'YES' 'YES' $CurrentTaskId $current.Status $m 'Task, Owner, and WriteScope explain the dirty path.'; exit 0 }
if ($matches.Count -eq 1) {
    $m=$matches[0]; if ($m.Task.Owner) { Result 'FRIENDLY_ACTIVE' 'YES' 'YES' $m.Task.TaskId $m.Task.Status $m.Scopes 'Task, Owner, and WriteScope explain the dirty path.'; exit 0 }
}
$released = @($tasks | Where-Object { [string]$_.Status -eq 'RELEASED' -and @($_.WriteScope | Where-Object { Hit $DirtyPath ([string]$_) }).Count }) | Select-Object -First 1
if ($released -and $released.Owner) { $m=@($released.WriteScope | Where-Object { Hit $DirtyPath ([string]$_) }); Result 'FRIENDLY_COMPLETED' 'YES' 'NO' $released.TaskId $released.Status $m 'Completed Task, Owner, and WriteScope explain the dirty path.'; exit 0 }
$files=if($dependency){Items $dependency 'files'}else{@()}; $record=@($files | Where-Object { Hit $DirtyPath ([string]$_.file) }) | Select-Object -First 1
if ($record -and [string]$record.status -in @('KNOWN','FOREIGN_KNOWN')) { $owner=[string]$(if($record.ownerTaskId){$record.ownerTaskId}elseif($record.taskId){$record.taskId}else{$record.owner}); Result 'FOREIGN_KNOWN' 'NO' 'NO' $owner ([string]$record.status) @($record.file) 'A recorded external owner is known; current Task may not claim it.'; exit 0 }
if ($record -and [string]$record.status -eq 'RELEASED') { $owner=[string]$(if($record.ownerTaskId){$record.ownerTaskId}elseif($record.taskId){$record.taskId}else{$record.owner}); Result 'FRIENDLY_COMPLETED' 'YES' 'NO' $owner 'RELEASED' @($record.file) 'Dependency ownership record marks the path completed.'; exit 0 }
Result 'UNAUTHORIZED_DIRTY' 'NO' 'NO' '' '' @() 'No Task, Owner, and WriteScope explain the dirty path.' 'RECOVERY_WINDOW'
