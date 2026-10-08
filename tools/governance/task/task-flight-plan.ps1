[CmdletBinding()]
param(
 [Parameter(Position=0)][ValidateSet('register','begin','list','inspect','amend','release','close','reap','transfer')][string]$Command='list',
 [string]$TaskId,[string]$Role,[string]$Owner,[string]$NewOwner,[string]$Workspace,[string[]]$WriteScope,
 [string[]]$ExpectedDependencies,[string[]]$AddWriteScope,[string]$Coordinator,[string]$Reason,[string]$CloseReason,
 [string]$AuthorizationEvidenceUrl,[string]$RepositoryRoot
)
$ErrorActionPreference='Stop'
. (Join-Path $PSScriptRoot '..\coordinator\authority-auth.ps1')
. (Join-Path $PSScriptRoot 'task-flight-plan.common.ps1')
function Find-Task($Registry,[string]$Id){@($Registry.tasks|?{[string]$_.TaskId -ceq $Id})|select -First 1}
$operationLock=$null;$failed=$false
try{
 $root=Repo $RepositoryRoot;$stateDir=FlightDir $root;if(!(Test-Path $stateDir) -and $Command -notin @('list','inspect')){New-Item -ItemType Directory -Force $stateDir|Out-Null};if(Test-Path $stateDir){$operationLock=[IO.File]::Open((Join-Path $stateDir 'task-lifecycle.lock'),[IO.FileMode]::OpenOrCreate,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None)};Recover-TaskState $root;$registry=ReadReg $root
 switch($Command){
  'register'{
   if(!$TaskId -or !$Owner -or !$Workspace -or !$WriteScope){Fail INVALID_INPUT 'TaskId Owner Workspace WriteScope required'}
   if(Find-Task $registry $TaskId){Fail TASK_EXISTS $TaskId}
   $task=[ordered]@{TaskId=$TaskId;Role=$Role;Owner=$Owner;Workspace=(NormWorkspace $Workspace);WriteScope=(Scopes $WriteScope $Workspace);ExpectedDependencies=@($ExpectedDependencies);Status='REGISTERED';BaselineHead=(Head $root);RegisteredAt=(Now);ActivatedAt=$null;ReleasedAt=$null;ClosedAt=$null;CloseReason=$null}
   $registry.tasks=@($registry.tasks)+[pscustomobject]$task;Save $root $registry $task 'REGISTERED';Write-Host "REGISTERED $TaskId"
  }
  'begin'{$task=Find-Task $registry $TaskId;if(!$task){Fail NOT_FOUND $TaskId};if($task.Status -ne 'REGISTERED'){Fail INVALID_TRANSITION "$($task.Status) -> ACTIVE"};$conflict=Conflict -Reg $registry -Wanted @($task.WriteScope) -TaskId $TaskId -WantedWorkspace ([string]$task.Workspace);if($conflict){Fail OWNERSHIP_CONFLICT "$TaskId overlaps $($conflict.TaskId)"};$task.Status='ACTIVE';$task.ActivatedAt=Now;Save $root $registry $task 'ACTIVE';Write-Host "ACTIVE $TaskId"}
  'list'{Write-Host "ACTIVE TASKS: $(@($registry.tasks|? Status -eq 'ACTIVE').Count)";@($registry.tasks|? Status -eq 'ACTIVE')|%{Write-Host "$($_.TaskId) $($_.Owner) $($_.Status)"}}
  'inspect'{if(!$TaskId){Fail INVALID_INPUT 'TaskId required'};$task=Find-Task $registry $TaskId;if($task){$task|ConvertTo-Json -Depth 12;break};$history=@(Get-Content (HistPath $root) -ErrorAction SilentlyContinue|%{try{$_|ConvertFrom-Json}catch{$null}}|?{[string]$_.TaskId -ceq $TaskId});if(!$history.Count){Fail NOT_FOUND $TaskId};$history|ConvertTo-Json -Depth 16}
  'amend'{if(!$TaskId -or !$AddWriteScope){Fail INVALID_INPUT 'TaskId AddWriteScope required'};$task=Find-Task $registry $TaskId;if(!$task){Fail NOT_FOUND $TaskId};if($task.Status -notin @('REGISTERED','ACTIVE')){Fail INVALID_TRANSITION "$($task.Status) -> AMEND"};$add=Scopes $AddWriteScope ([string]$task.Workspace);$conflict=Conflict -Reg $registry -Wanted $add -TaskId $TaskId -WantedWorkspace ([string]$task.Workspace);if($conflict){Fail OWNERSHIP_CONFLICT $conflict.TaskId};$task.WriteScope=@((@($task.WriteScope)+@($add))|sort -Unique);Save $root $registry $task 'AMEND APPROVED';Write-Host 'AMEND APPROVED'}
  'release'{ $task=Find-Task $registry $TaskId;if(!$task){Fail NOT_FOUND $TaskId};if($task.Status -eq 'RELEASED'){Write-Host "already RELEASED $TaskId";break};if($task.Status -ne 'ACTIVE'){Fail INVALID_TRANSITION "$($task.Status) -> RELEASED"};$task.Status='RELEASED';$task.ReleasedAt=Now;Save $root $registry $task 'RELEASED';Write-Host "RELEASED $TaskId"}
  'close'{if(!$TaskId -or !$CloseReason){Fail INVALID_INPUT 'TaskId and CloseReason required'};$task=Find-Task $registry $TaskId;if(!$task){Fail NOT_FOUND $TaskId};if($task.Status -ne 'RELEASED'){Fail INVALID_TRANSITION "$($task.Status) -> CLOSED"};Assert-TaskResourcesReleased $root $TaskId;$auth=Assert-OwnerAuthorization $root $AuthorizationEvidenceUrl 'task-close' $TaskId $CloseReason (Head $root);Consume-OwnerAuthorization $root $auth;$task.Status='CLOSED';$task.ClosedAt=Now;$task.CloseReason=$CloseReason;$registry.tasks=@($registry.tasks|? TaskId -ne $TaskId);Save $root $registry $task 'CLOSED' $CloseReason '' $auth;Write-Host "CLOSED $TaskId"}
  'reap'{if(!$TaskId -or !$Reason){Fail INVALID_INPUT 'TaskId and Reason required'};$task=Find-Task $registry $TaskId;if(!$task){Fail NOT_FOUND $TaskId};if($task.Status -ne 'ACTIVE'){Fail INVALID_TRANSITION "$($task.Status) -> CLOSED"};Assert-TaskResourcesReleased $root $TaskId;$auth=Assert-OwnerAuthorization $root $AuthorizationEvidenceUrl 'task-reap' $TaskId $Reason (Head $root);Consume-OwnerAuthorization $root $auth;$task.Status='CLOSED';$task.ClosedAt=Now;$task.CloseReason="ABANDONED/$Reason";$registry.tasks=@($registry.tasks|? TaskId -ne $TaskId);Save $root $registry $task 'REAP' $task.CloseReason '' $auth;Write-Host $task.CloseReason}
  'transfer'{if(!$TaskId -or !$NewOwner){Fail INVALID_INPUT 'TaskId and NewOwner required'};$task=Find-Task $registry $TaskId;if(!$task){Fail NOT_FOUND $TaskId};if($task.Status -ne 'ACTIVE'){Fail INVALID_TRANSITION "$($task.Status) -> TRANSFERRED"};Assert-TaskResourcesReleased $root $TaskId;$auth=Assert-OwnerAuthorization $root $AuthorizationEvidenceUrl 'task-transfer' $TaskId $NewOwner (Head $root);Consume-OwnerAuthorization $root $auth;$task|Add-Member NoteProperty PreviousOwner $task.Owner -Force;$task.Owner=$NewOwner;$task|Add-Member NoteProperty TransferredAt (Now) -Force;Save $root $registry $task 'TRANSFERRED' $NewOwner '' $auth;Write-Host "TRANSFERRED $TaskId"}
 }
}catch{$failed=$true;Write-Host $_.Exception.Message}finally{if($operationLock){$operationLock.Dispose()}}
if($failed){exit 1}
