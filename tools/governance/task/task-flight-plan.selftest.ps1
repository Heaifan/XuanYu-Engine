[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
$root=Join-Path $env:TEMP ('xye-flight-'+[guid]::NewGuid().ToString('N'))
$script=Join-Path $PSScriptRoot 'task-flight-plan.ps1'
function Run([string[]]$a){$o=@(& pwsh -NoLogo -NoProfile -File $script @a 2>&1);[pscustomobject]@{Code=$LASTEXITCODE;Text=$o -join "`n"}}
function Ok($r,[string]$s){if($r.Code -ne 0 -or !$r.Text.Contains($s)){throw "SELFTEST FAILED: $s / $($r.Text)"}}
function Bad($r,[string]$s){if($r.Code -eq 0 -or !$r.Text.Contains($s)){throw "SELFTEST FAILED: blocked $s / $($r.Text)"}}
try {
 New-Item -ItemType Directory -Force $root|Out-Null; git -C $root init -q; git -C $root config user.email test@example.invalid; git -C $root config user.name test
 Set-Content (Join-Path $root 'seed') seed; git -C $root add seed; git -C $root commit -qm seed
 $base=(git -C $root rev-parse HEAD).Trim()
 Ok (Run @('-RepositoryRoot',$root,'register','-TaskId','A','-Role','Writer','-Owner','one','-Workspace',$root,'-WriteScope','src/a')) 'REGISTERED'
 Ok (Run @('-RepositoryRoot',$root,'begin','-TaskId','A')) 'ACTIVE'
 Ok (Run @('-RepositoryRoot',$root,'list')) 'A'
 Ok (Run @('-RepositoryRoot',$root,'release','-TaskId','A')) 'RELEASED'
 Ok (Run @('-RepositoryRoot',$root,'list')) 'ACTIVE TASKS: 0'; Ok (Run @('-RepositoryRoot',$root,'inspect','-TaskId','A')) 'RELEASED'
 Ok (Run @('-RepositoryRoot',$root,'close','-TaskId','A','-CloseReason','done')) 'CLOSED'; if((Test-Path (Join-Path $root '.git/xye-handoff/task-registry.json')) -and (Get-Content (Join-Path $root '.git/xye-handoff/task-registry.json') -Raw).Contains('"A"')){throw 'live registry retained closed task'}
 Ok (Run @('-RepositoryRoot',$root,'register','-TaskId','B','-Role','Writer','-Owner','two','-Workspace',$root,'-WriteScope','same')) 'REGISTERED'; Ok (Run @('-RepositoryRoot',$root,'begin','-TaskId','B')) 'ACTIVE'
 Ok (Run @('-RepositoryRoot',$root,'register','-TaskId','C','-Role','Writer','-Owner','three','-Workspace',$root,'-WriteScope','same')) 'REGISTERED'; Bad (Run @('-RepositoryRoot',$root,'begin','-TaskId','C')) 'OWNERSHIP_CONFLICT'
 Ok (Run @('-RepositoryRoot',$root,'register','-TaskId','D','-Role','Writer','-Owner','four','-Workspace',$root,'-WriteScope','free')) 'REGISTERED'; Bad (Run @('-RepositoryRoot',$root,'amend','-TaskId','D','-AddWriteScope','same')) 'AMEND BLOCKED'
 Ok (Run @('-RepositoryRoot',$root,'register','-TaskId','E','-Role','Writer','-Owner','five','-Workspace',$root,'-WriteScope','ghost')) 'REGISTERED'; Ok (Run @('-RepositoryRoot',$root,'begin','-TaskId','E')) 'ACTIVE'; Ok (Run @('-RepositoryRoot',$root,'reap','-TaskId','E','-Coordinator','coord','-Reason','ABANDONED_SELFTEST')) 'ABANDONED/ABANDONED_SELFTEST'; $reap=Get-Content (Join-Path $root '.git/xye-handoff/task-history.jsonl')|%{$_|ConvertFrom-Json}|?{$_.TaskId -eq 'E' -and $_.Event -eq 'REAP'}|select -Last 1; if($reap.CloseReason -ne 'ABANDONED/ABANDONED_SELFTEST' -or $reap.Coordinator -ne 'coord'){throw "SELFTEST FAILED: reap history contract"}
 Ok (Run @('-RepositoryRoot',$root,'release','-TaskId','B')) 'RELEASED'; Ok (Run @('-RepositoryRoot',$root,'release','-TaskId','B')) 'already RELEASED'
 $files=Get-ChildItem (Join-Path $PSScriptRoot 'task-flight-plan*'); foreach($f in $files){if((Get-Content $f.FullName).Count -gt 100){throw "5+100: $($f.Name)"}}
 $reg=Get-Content (Join-Path $root '.git/xye-handoff/task-registry.json') -Raw|ConvertFrom-Json; if(!$reg.tasks){throw 'atomic registry unreadable'}
 Write-Host 'TASK FLIGHT PLAN SELFTEST PASS'
} finally {if(Test-Path $root){Remove-Item $root -Recurse -Force}}
