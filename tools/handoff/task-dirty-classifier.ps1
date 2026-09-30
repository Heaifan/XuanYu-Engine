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
function Result([string]$class, [string]$allowed, [string]$eligible, [string]$owner, [string]$status, $matched, [string]$consumed, [string]$reason) {
    [pscustomobject]@{ Classification=$class; CodingAllowed=$allowed; FinalEvidenceEligible=$eligible; OwnerTaskId=$owner; OwnerStatus=$status; MatchedWriteScope=@($matched); DependencyConsumed=$consumed; Reason=$reason } | ConvertTo-Json -Depth 8
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
if ($matches.Count -gt 1) { Result 'OWNERSHIP_CONFLICT' 'NO' 'NO' $matches[0].Task.TaskId $matches[0].Task.Status $matches.Scopes 'NO' 'Multiple ACTIVE tasks claim the dirty path.'; exit 0 }
if ($current -and @($current.WriteScope | Where-Object { Hit $DirtyPath ([string]$_) }).Count) { $m=@($current.WriteScope | Where-Object { Hit $DirtyPath ([string]$_) }); Result 'OWNED_DIRTY' 'YES' 'YES' $CurrentTaskId $current.Status $m 'NO' 'Current Task WriteScope claims the dirty path.'; exit 0 }
if ($matches.Count -eq 1) {
    $m=$matches[0]; $consumed=if ($current) { Consumes $current $m.Task.TaskId $DirtyPath $m.Scopes } else { $false }
    if ($consumed) { Result 'FRIENDLY_ACTIVE_DEPENDENCY' 'YES' 'NO' $m.Task.TaskId $m.Task.Status $m.Scopes 'YES' 'ACTIVE owner is declared in ExpectedDependencies.'; exit 0 }
    Result 'FRIENDLY_ACTIVE_DIRTY' 'YES' 'YES' $m.Task.TaskId $m.Task.Status $m.Scopes 'NO' 'Another ACTIVE Task owns the dirty path.'; exit 0
}
$released = @($tasks | Where-Object { [string]$_.Status -eq 'RELEASED' -and @($_.WriteScope | Where-Object { Hit $DirtyPath ([string]$_) }).Count }) | Select-Object -First 1
if ($released) { $m=@($released.WriteScope | Where-Object { Hit $DirtyPath ([string]$_) }); Result 'FRIENDLY_RELEASED_DIRTY' 'YES' 'NO' $released.TaskId $released.Status $m 'NO' 'RELEASED Task WriteScope explains the dirty path.'; exit 0 }
$files=if($dependency){Items $dependency 'files'}else{@()}; $record=@($files | Where-Object { Hit $DirtyPath ([string]$_.file) }) | Select-Object -First 1
if ($record -and [string]$record.status -eq 'RELEASED') { $owner=[string]$(if($record.ownerTaskId){$record.ownerTaskId}elseif($record.taskId){$record.taskId}else{$record.owner}); Result 'FRIENDLY_RELEASED_DIRTY' 'YES' 'NO' $owner 'RELEASED' @($record.file) 'NO' 'Dependency ownership record marks the path RELEASED.'; exit 0 }
Result 'UNKNOWN_DIRTY' 'NO' 'NO' '' '' @() 'NO' 'Task Registry and dependency ownership provide no explanation.'
