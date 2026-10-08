[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$root = Join-Path ([IO.Path]::GetTempPath()) ('xye-task-lifecycle-' + [guid]::NewGuid().ToString('N'))
$flight = Join-Path $PSScriptRoot 'task-flight-plan.ps1'
$classifier = Join-Path $PSScriptRoot 'task-dirty-classifier.ps1'
function Run([string]$File, [string[]]$argv) { $old = $ErrorActionPreference; $ErrorActionPreference = 'Continue'; try { $cmd = @('register','begin','list','inspect','amend','release','close','reap') | ? { $argv -contains $_ } | select -First 1; if ($cmd) { $rest = @($argv | ? { $_ -ne $cmd }); $o = @(& powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $File $cmd @rest 2>&1) } else { $o = @(& powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $File @argv 2>&1) }; [pscustomobject]@{ Code = $LASTEXITCODE; Text = ($o -join "`n") } } finally { $ErrorActionPreference = $old } }
function Pass($r, [string]$Need) { if ($r.Code -ne 0 -or $r.Text -notmatch [regex]::Escape($Need)) { throw "REGRESSION FAILED: expected [$Need] / $($r.Text)" } }
function Deny($r) { if ($r.Code -eq 0) { throw "REGRESSION FAILED: forbidden operation was accepted / $($r.Text)" } }
function DenyAuthorization($r) { if ($r.Code -eq 0 -or $r.Text -notmatch 'AUTHORIZATION_REQUIRED') { throw "REGRESSION FAILED: owner authorization was not enforced / $($r.Text)" } }
function Json([string]$p) { Get-Content -Raw -LiteralPath $p | ConvertFrom-Json }
try {
    if (!(Test-Path -LiteralPath $flight -PathType Leaf)) { throw "PREREQUISITE MISSING: $flight" }
    New-Item -ItemType Directory -Force $root | Out-Null; git -C $root init -q
    git -C $root config user.email test@example.invalid; git -C $root config user.name test
    Set-Content (Join-Path $root 'seed.txt') seed; git -C $root add seed.txt; git -C $root commit -qm seed
    $a = @('-RepositoryRoot',$root,'register','-TaskId','GHOST-NORMAL','-Role','Writer','-Owner','agent-c','-Workspace',$root,'-WriteScope','candidate.txt')
    Pass (Run $flight $a) 'REGISTERED'; Pass (Run $flight @('-RepositoryRoot',$root,'begin','-TaskId','GHOST-NORMAL')) 'ACTIVE'
    Pass (Run $flight @('-RepositoryRoot',$root,'release','-TaskId','GHOST-NORMAL')) 'RELEASED'
    $list = Run $flight @('-RepositoryRoot',$root,'list'); Pass $list 'ACTIVE TASKS: 0'; if ($list.Text -match 'GHOST-NORMAL') { throw 'ACTIVE TASK LEAK: normal task remains listed' }
    Pass (Run $flight @('-RepositoryRoot',$root,'inspect','-TaskId','GHOST-NORMAL')) 'RELEASED'
    Set-Content (Join-Path $root 'candidate.txt') dirty
    $c = Run $classifier @('-RepositoryRoot',$root,'-CurrentTaskId','OTHER','-DirtyPath','candidate.txt')
    Pass $c 'FRIENDLY_COMPLETED'; Pass $c 'GHOST-NORMAL'
    DenyAuthorization (Run $flight @('-RepositoryRoot',$root,'close','-TaskId','GHOST-NORMAL','-CloseReason','absorbed'))
    Pass (Run $flight @('-RepositoryRoot',$root,'inspect','-TaskId','GHOST-NORMAL')) 'RELEASED'
    $live = Run $flight @('-RepositoryRoot',$root,'list'); Pass $live 'ACTIVE TASKS: 0'
    $history = Join-Path $root '.git/xye-handoff/task-history.jsonl'; if (!(Test-Path $history)) { throw 'history registry was not created' }
    Deny (Run $flight @('-RepositoryRoot',$root,'amend','-TaskId','GHOST-NORMAL','-AddWriteScope','forbidden.txt'))
    Deny (Run $flight @('-RepositoryRoot',$root,'write','-TaskId','GHOST-NORMAL','-Path','candidate.txt'))
    Pass (Run $flight @('-RepositoryRoot',$root,'register','-TaskId','GHOST-ABANDONED','-Role','Writer','-Owner','agent-i','-Workspace',$root,'-WriteScope','interrupted.txt')) 'REGISTERED'
    Pass (Run $flight @('-RepositoryRoot',$root,'begin','-TaskId','GHOST-ABANDONED')) 'ACTIVE'
    $before = Run $flight @('-RepositoryRoot',$root,'list'); Pass $before 'GHOST-ABANDONED'; if ($before.Text -match 'TTL|TIMEOUT') { throw 'ACTIVE ownership was automatically released' }
    Deny (Run $flight @('-RepositoryRoot',$root,'reap','-TaskId','GHOST-ABANDONED','-Reason','ABANDONED_SELFTEST'))
    DenyAuthorization (Run $flight @('-RepositoryRoot',$root,'reap','-TaskId','GHOST-ABANDONED','-Coordinator','coord','-Reason','ABANDONED_SELFTEST'))
    $after = Run $flight @('-RepositoryRoot',$root,'list'); Pass $after 'GHOST-ABANDONED'; Pass (Run $flight @('-RepositoryRoot',$root,'inspect','-TaskId','GHOST-ABANDONED')) 'ACTIVE'
    Pass (Run $flight @('-RepositoryRoot',$root,'release','-TaskId','GHOST-ABANDONED')) 'RELEASED'
    Pass (Run $flight @('-RepositoryRoot',$root,'register','-TaskId','SRP','-Role','Writer','-Owner','srp','-Workspace',$root,'-WriteScope','XuanYu.Render.Vulkan/**')) 'REGISTERED'
    Pass (Run $flight @('-RepositoryRoot',$root,'begin','-TaskId','SRP')) 'ACTIVE'
    Pass (Run $flight @('-RepositoryRoot',$root,'register','-TaskId','XYT','-Role','Writer','-Owner','xyt','-Workspace',$root,'-WriteScope','XuanYu.World/**','-ExpectedDependencies','SRP')) 'REGISTERED'
    Pass (Run $flight @('-RepositoryRoot',$root,'begin','-TaskId','XYT')) 'ACTIVE'
    $d = Run $classifier @('-RepositoryRoot',$root,'-CurrentTaskId','XYT','-DirtyPath','XuanYu.Render.Vulkan/shader.cs')
    Pass $d 'FRIENDLY_ACTIVE'; Pass $d 'SRP'
    Pass (Run $flight @('-RepositoryRoot',$root,'release','-TaskId','SRP')) 'RELEASED'
    $d = Run $classifier @('-RepositoryRoot',$root,'-CurrentTaskId','XYT','-DirtyPath','XuanYu.Render.Vulkan/shader.cs')
    Pass $d 'FRIENDLY_COMPLETED'; Pass $d 'SRP'
    $historyText = Get-Content -Raw $history; if ($historyText -notmatch 'GHOST-NORMAL') { throw 'released task history is not traceable' }; if ($historyText -match 'ABANDONED/ABANDONED_SELFTEST') { throw 'unauthorized reap was recorded as completed' }
    $files = Get-ChildItem (Join-Path $PSScriptRoot 'task-flight-plan*'); foreach ($f in $files) { if ((Get-Content $f.FullName).Count -gt 100) { throw "5+100 FAILED: $($f.Name)" } }
    $closeRoot=Join-Path $root 'close-auth';New-Item -ItemType Directory -Force $closeRoot|Out-Null;git -C $closeRoot init -q;git -C $closeRoot config user.email test@example.invalid;git -C $closeRoot config user.name test;Set-Content (Join-Path $closeRoot seed) seed;git -C $closeRoot add seed;git -C $closeRoot commit -qm seed
    $closeDir=Join-Path $closeRoot '.git/xye-handoff';New-Item $closeDir -ItemType Directory -Force|Out-Null
    [pscustomobject]@{active=$true;coordinatorScope='governance';waveId='CLOSE-AUTH';laneStates=[pscustomobject]@{governance=[pscustomobject]@{state='FROZEN'};xye=[pscustomobject]@{state='FROZEN'}}}|ConvertTo-Json -Depth 8|Set-Content (Join-Path $closeDir 'state.json')
    [pscustomobject]@{tasks=@()}|ConvertTo-Json|Set-Content (Join-Path $closeDir 'task-registry.json')
    $closeScript=Join-Path $PSScriptRoot '..\coordinator\close-authority.ps1';$old=$ErrorActionPreference;$ErrorActionPreference='Continue';$global=@(& powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $closeScript -Mode global-close -Scope governance -RepositoryRoot $closeRoot 2>&1);$globalCode=$LASTEXITCODE;$lane=@(& powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $closeScript -Mode lane-close -Scope integration -RepositoryRoot $closeRoot 2>&1);$laneCode=$LASTEXITCODE;$ErrorActionPreference=$old;if($globalCode -eq 0 -or ($global -join "`n") -notmatch 'AUTHORIZATION_REQUIRED'){throw 'global Wave close accepted without owner evidence'}
    if($laneCode -eq 0 -or ($lane -join "`n") -notmatch 'AUTHORIZATION_REQUIRED'){throw 'Lane close accepted without owner evidence'}
    if(!(Get-Content (Join-Path $closeDir 'state.json') -Raw|ConvertFrom-Json).active){throw 'denied close changed active Wave state'}
    Write-Host 'TASK FLIGHT PLAN LIFECYCLE SELFTEST PASS 10/10'
} finally { if (Test-Path -LiteralPath $root) { Remove-Item -LiteralPath $root -Recurse -Force } }
