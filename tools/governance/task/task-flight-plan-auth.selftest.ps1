[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
$root=Join-Path $env:TEMP ('xye-task-auth-'+[guid]::NewGuid().ToString('N'))
$script=Join-Path $PSScriptRoot 'task-flight-plan.ps1'
function Run([string[]]$a){$o=@(& pwsh -NoLogo -NoProfile -File $script @a 2>&1);[pscustomobject]@{Code=$LASTEXITCODE;Text=$o -join "`n"}}
function Deny($r){if($r.Code -eq 0 -or $r.Text -notmatch 'AUTHORIZATION_REQUIRED'){throw "TASK AUTH SELFTEST RED: expected authorization denial / $($r.Text)"}}
try{
 New-Item -ItemType Directory -Force $root|Out-Null;git -C $root init -q;git -C $root config user.email test@example.invalid;git -C $root config user.name test
 Set-Content (Join-Path $root seed) seed;git -C $root add seed;git -C $root commit -qm seed
 $base=@('-RepositoryRoot',$root,'-Role','Writer','-Workspace',$root,'-WriteScope','owned')
 $created=Run (@($base)+@('register','-TaskId','RELEASED-TASK','-Owner','owner'));if($created.Code){throw 'fixture registration failed'}
 $begun=Run @('-RepositoryRoot',$root,'begin','-TaskId','RELEASED-TASK');if($begun.Code){throw 'fixture activation failed'}
 $released=Run @('-RepositoryRoot',$root,'release','-TaskId','RELEASED-TASK');if($released.Code){throw "fixture release failed: $($released.Text)"}
 Deny (Run @('-RepositoryRoot',$root,'close','-TaskId','RELEASED-TASK','-CloseReason','done'))
 if((Get-Content (Join-Path $root '.git/xye-handoff/task-registry.json') -Raw|ConvertFrom-Json).tasks[0].Status -ne 'RELEASED'){throw 'denied Close changed Task Registry'}
 $created=Run (@($base)+@('register','-TaskId','ACTIVE-TASK','-Owner','owner'));if($created.Code){throw 'fixture registration failed'}
 $active=Run @('-RepositoryRoot',$root,'begin','-TaskId','ACTIVE-TASK');if($active.Code){throw 'fixture activation failed'}
 Deny (Run @('-RepositoryRoot',$root,'reap','-TaskId','ACTIVE-TASK','-Coordinator','fake-coordinator','-Reason','abandon'))
 if(@((Get-Content (Join-Path $root '.git/xye-handoff/task-registry.json') -Raw|ConvertFrom-Json).tasks|? TaskId -eq 'ACTIVE-TASK').Count -ne 1){throw 'denied Reap removed active Task'}
 $transfer=Run @('-RepositoryRoot',$root,'transfer','-TaskId','ACTIVE-TASK','-Coordinator','fake-coordinator','-NewOwner','new-owner')
 Deny $transfer
 if((Get-Content (Join-Path $root '.git/xye-handoff/task-registry.json') -Raw|ConvertFrom-Json).tasks[1].Owner -ne 'owner'){throw 'denied transfer changed Task Owner'}
 New-Item (Join-Path $root '.git/xye-handoff') -ItemType Directory -Force|Out-Null;'{"activeTasks":["STALE-TASK"]}'|Set-Content (Join-Path $root '.git/xye-handoff/state.json')
 $projection=Run @('-RepositoryRoot',$root,'list');if($projection.Code -eq 0 -or $projection.Text -notmatch 'TASK_STATE_PROJECTION_MISMATCH'){throw 'Task Registry/state projection mismatch was not blocked'}
 . (Join-Path $PSScriptRoot 'task-flight-plan.common.ps1')
 $taskRoot=Join-Path $root 'transfer-proof';New-Item -ItemType Directory -Force $taskRoot|Out-Null;git -C $taskRoot init -q;git -C $taskRoot config user.email test@example.invalid;git -C $taskRoot config user.name test;Set-Content (Join-Path $taskRoot seed) seed;git -C $taskRoot add seed;git -C $taskRoot commit -qm seed
 $task=[pscustomobject]@{TaskId='MOVE-ME';Owner='old-owner';Status='ACTIVE';ExpectedDependencies=@('DEP-A OPEN','DEP-B OPEN')};$registry=[pscustomobject]@{tasks=@($task)};Save $taskRoot $registry $task 'REGISTERED';$task|Add-Member NoteProperty PreviousOwner $task.Owner -Force;$task.Owner='new-owner';$task|Add-Member NoteProperty TransferredAt (Now) -Force;Save $taskRoot $registry $task 'TRANSFERRED' 'new-owner' '' ([pscustomobject]@{CommentId='55';Author='Heaifan'})
 $saved=ReadReg $taskRoot;$moved=$saved.tasks[0];if($moved.Owner -ne 'new-owner' -or $moved.ExpectedDependencies.Count -ne 2){throw 'transfer lost task dependencies'}
 $hist=Get-Content (HistPath $taskRoot)|%{$_|ConvertFrom-Json};if($hist[-1].Event -ne 'TRANSFERRED' -or $hist[-1].AuthorizationEvidence.Author -ne 'Heaifan'){throw 'transfer evidence missing from history'}
 $broken=Join-Path $root 'rollback';New-Item -ItemType Directory -Force $broken|Out-Null;git -C $broken init -q;git -C $broken config user.email test@example.invalid;git -C $broken config user.name test;Set-Content (Join-Path $broken seed) seed;git -C $broken add seed;git -C $broken commit -qm seed
 $brokenState=Join-Path $broken '.git/xye-handoff';New-Item (Join-Path $brokenState 'task-history.jsonl') -ItemType Directory -Force|Out-Null
 $failure=Run @('-RepositoryRoot',$broken,'register','-TaskId','ROLLBACK','-Role','Writer','-Owner','agent','-Workspace',$broken,'-WriteScope','owned');if($failure.Code -eq 0 -or (Test-Path (Join-Path $brokenState 'task-registry.json'))){throw 'failed registry/history commit left partial state'}
 if(!(Test-Path (Join-Path $brokenState 'task-transaction.json'))){throw 'failed state transaction was not recoverable'};Remove-Item (Join-Path $brokenState 'task-history.jsonl') -Recurse -Force
 $recovered=Run @('-RepositoryRoot',$broken,'list');if($recovered.Code -ne 0 -or (Test-Path (Join-Path $brokenState 'task-transaction.json'))){throw 'pending registry/history transaction did not recover'}
 if(!(Get-Content (Join-Path $brokenState 'task-history.jsonl') -Raw).Contains('TRANSACTION_ROLLED_BACK')){throw 'task transaction recovery was not audited'}
 Write-Output 'TASK FLIGHT PLAN AUTH SELFTEST PASS'
}finally{if(Test-Path $root){Remove-Item $root -Recurse -Force}}
