[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
$root=Join-Path $env:TEMP ('xye-wave-init-'+[guid]::NewGuid().ToString('N'))
$script=Join-Path $PSScriptRoot 'wave-init.ps1'
function Run([string]$R,[string]$Head,[string]$Branch,[string]$Id,[string]$Evidence=''){ $args=@('-RepositoryRoot',$R,'-WaveId',$Id,'-Scope','governance','-BaselineHead',$Head,'-ExpectedBranch',$Branch);if($Evidence){$args+=@('-AuthorizationEvidenceUrl',$Evidence)};$old=$ErrorActionPreference;$ErrorActionPreference='Continue';try{$o=@(& powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $script @args 2>&1);[pscustomobject]@{Code=$LASTEXITCODE;Text=$o -join "`n"}}finally{$ErrorActionPreference=$old} }
function Make-Repo([string]$Path){New-Item -ItemType Directory -Force $Path|Out-Null;git -C $Path init -q;git -C $Path config user.email test@example.invalid;git -C $Path config user.name test;Set-Content (Join-Path $Path seed) seed;git -C $Path add seed;git -C $Path commit -qm seed;[pscustomobject]@{Head=(git -C $Path rev-parse HEAD).Trim();Branch=(git -C $Path branch --show-current).Trim()}}
function Deny($r,[string]$code){if($r.Code -eq 0 -or $r.Text -notmatch $code){throw "WAVE INIT RED: expected $code / $($r.Text)"}}
try{
 $one=Make-Repo $root;Deny (Run $root $one.Head $one.Branch 'WAVE-NO-AUTH') 'AUTHORIZATION_REQUIRED';if(Test-Path (Join-Path $root '.git/xye-handoff')){throw 'denied init created production state'}
 Deny (Run $root ('0'*40) $one.Branch 'WAVE-STALE') 'BASELINE_MISMATCH';if(Test-Path (Join-Path $root '.git/xye-handoff')){throw 'stale baseline created Wave state'}
 . $script
 function Get-AuthorityComment([string]$Root,[string]$Url){$script:authComment}
 function Set-Grant([string]$Target,[string]$Baseline){$created=[datetimeoffset]::UtcNow;$script:authComment=[pscustomobject]@{id=55;user=[pscustomobject]@{login='Heaifan';id=86356120};author_association='OWNER';created_at=$created.ToString('o');updated_at=$created.ToString('o');body="XYE AUTHORIZATION V1`nAction: wave-init`nTarget: $Target`nDetail: governance`nBaseline: $Baseline`nExpiresAt: $($created.AddHours(2).ToString('o'))"}}
 Set-Grant 'WAVE-READY' $one.Head;$ready=Initialize-Wave $root 'WAVE-READY' governance $one.Head $one.Branch 'https://github.com/Heaifan/XuanYu-Engine/issues/8#issuecomment-55'
 if($ready.Status -ne 'INITIALIZED'){throw 'valid Wave initialization did not complete'}
 $dir=Join-Path $root '.git/xye-handoff';$state=Get-Content (Join-Path $dir 'state.json') -Raw|ConvertFrom-Json;$release=Get-Content (Join-Path $dir 'work-release.json') -Raw|ConvertFrom-Json
 if($state.waveId -ne 'WAVE-READY' -or $release.status -ne 'IDLE'){throw 'new Wave state contract failed'}
 Set-Grant 'WAVE-OVERWRITE' $one.Head;try{Initialize-Wave $root 'WAVE-OVERWRITE' governance $one.Head $one.Branch 'https://github.com/Heaifan/XuanYu-Engine/issues/8#issuecomment-55'|Out-Null;throw 'active Wave overwrite unexpectedly succeeded'}catch{if($_.Exception.Message -notmatch 'ACTIVE_WAVE_EXISTS'){throw}}
 $deps=Join-Path $root 'open-dependencies';$depIdentity=Make-Repo $deps;$depState=Join-Path $deps '.git/xye-handoff';New-Item $depState -ItemType Directory -Force|Out-Null;[pscustomobject]@{tasks=@([pscustomobject]@{TaskId='OWNER-C';Status='ACTIVE';ExpectedDependencies=@('Render Projection OPEN')})}|ConvertTo-Json -Depth 8|Set-Content (Join-Path $depState 'task-registry.json')
 Set-Grant 'WAVE-OPEN-DEPS' $depIdentity.Head;try{Initialize-Wave $deps 'WAVE-OPEN-DEPS' governance $depIdentity.Head $depIdentity.Branch 'https://github.com/Heaifan/XuanYu-Engine/issues/8#issuecomment-55'|Out-Null;throw 'OPEN dependencies were overwritten'}catch{if($_.Exception.Message -notmatch 'OPEN_DEPENDENCIES'){throw}};if(Test-Path (Join-Path $depState 'state.json')){throw 'blocked OPEN dependencies created Wave state'}
 $candidate=Join-Path $root 'old-candidate';$candidateIdentity=Make-Repo $candidate;$candidateState=Join-Path $candidate '.git/xye-handoff';New-Item $candidateState -ItemType Directory -Force|Out-Null;Set-Content (Join-Path $candidateState 'candidate-freeze.json') '{"status":"FROZEN_FOR_CERTIFICATION","productAcceptanceState":"P4_PENDING"}'
 Set-Grant 'WAVE-OLD-CANDIDATE' $candidateIdentity.Head;try{Initialize-Wave $candidate 'WAVE-OLD-CANDIDATE' governance $candidateIdentity.Head $candidateIdentity.Branch 'https://github.com/Heaifan/XuanYu-Engine/issues/8#issuecomment-55'|Out-Null;throw 'old Candidate was overwritten'}catch{if($_.Exception.Message -notmatch 'CANDIDATE_EXISTS'){throw}};if(!(Get-Content (Join-Path $candidateState 'candidate-freeze.json') -Raw).Contains('P4_PENDING')){throw 'old Candidate acceptance state changed'}
 $stale=Join-Path $root 'stale-recovery';$staleIdentity=Make-Repo $stale;$staleDir=Join-Path $stale '.git/xye-handoff';New-Item $staleDir -ItemType Directory -Force|Out-Null
 $oldState=[pscustomobject]@{waveId='OLD-WAVE';active=$true;coordinatorScope='governance';baselineHead=$staleIdentity.Head;branch='retired/governance';laneStates=[pscustomobject]@{}}
 $oldState|ConvertTo-Json -Depth 8|Set-Content (Join-Path $staleDir 'state.json');'prior-wave-event'|Set-Content (Join-Path $staleDir 'wave-history.jsonl')
 $oldTask=[pscustomobject]@{TaskId='OLD-C';Lane='xye';Owner='owner-c';Status='ACTIVE';WriteScope=@('src/**');ExpectedDependencies=@('RenderProjection OPEN')}
 $taskJson=[pscustomobject]@{tasks=@($oldTask)}|ConvertTo-Json -Depth 8;Set-Content (Join-Path $staleDir 'task-registry.json') $taskJson
 $freeze='{"candidateId":"OLD-CANDIDATE","ownerTaskId":"OLD-C","CandidateFiles":["src/old.cs"],"status":"FROZEN_FOR_CERTIFICATION","productAcceptanceState":"P4_PENDING"}';Set-Content (Join-Path $staleDir 'candidate-freeze.json') $freeze
 $candidateBefore=Get-Content (Join-Path $staleDir 'candidate-freeze.json') -Raw;$tasksBefore=Get-Content (Join-Path $staleDir 'task-registry.json') -Raw
 Set-Grant 'WAVE-AFTER-STALE' $staleIdentity.Head;$recovered=Initialize-Wave $stale 'WAVE-AFTER-STALE' governance $staleIdentity.Head $staleIdentity.Branch 'https://github.com/Heaifan/XuanYu-Engine/issues/8#issuecomment-55'
 if($recovered.Status -ne 'INITIALIZED'){throw 'ancestor-history stale Wave did not permit a disjoint governance Wave'}
 $history=Get-Content (Join-Path $staleDir 'wave-history.jsonl') -Raw;if(!$history.Contains('prior-wave-event') -or !$history.Contains('OLD-WAVE')){throw 'stale Wave history was not preserved'}
 if((Get-Content (Join-Path $staleDir 'candidate-freeze.json') -Raw) -cne $candidateBefore -or !(Get-Content (Join-Path $staleDir 'candidate-freeze.json') -Raw).Contains('P4_PENDING')){throw 'stale Candidate or P4 state changed'}
 if((Get-Content (Join-Path $staleDir 'task-registry.json') -Raw) -cne $tasksBefore -or !(Get-Content (Join-Path $staleDir 'task-registry.json') -Raw).Contains('RenderProjection OPEN')){throw 'unrelated Task or OPEN dependency changed'}
 $conflict=Join-Path $root 'stale-conflict';$conflictIdentity=Make-Repo $conflict;$conflictDir=Join-Path $conflict '.git/xye-handoff';New-Item $conflictDir -ItemType Directory -Force|Out-Null
 [pscustomobject]@{waveId='CONFLICT-WAVE';active=$true;coordinatorScope='governance';baselineHead=$conflictIdentity.Head;branch='retired/governance'}|ConvertTo-Json|Set-Content (Join-Path $conflictDir 'state.json')
 [pscustomobject]@{tasks=@([pscustomobject]@{TaskId='GOV-OWNER';Status='ACTIVE';WriteScope=@('tools/governance/**');ExpectedDependencies=@('Gate OPEN')})}|ConvertTo-Json -Depth 8|Set-Content (Join-Path $conflictDir 'task-registry.json')
 [IO.File]::WriteAllText((Join-Path $conflictDir 'wave-history.jsonl'),'conflict-history'+"`n");$conflictState=Get-Content (Join-Path $conflictDir 'state.json') -Raw;$conflictHistory=Get-Content (Join-Path $conflictDir 'wave-history.jsonl') -Raw
 Set-Grant 'WAVE-REAL-CONFLICT' $conflictIdentity.Head;try{Initialize-Wave $conflict 'WAVE-REAL-CONFLICT' governance $conflictIdentity.Head $conflictIdentity.Branch 'https://github.com/Heaifan/XuanYu-Engine/issues/8#issuecomment-55'|Out-Null;throw 'overlapping live Task was allowed to replace stale Wave'}catch{if($_.Exception.Message -notmatch 'LIVE_SCOPE_CONFLICT'){throw}}
 if((Get-Content (Join-Path $conflictDir 'state.json') -Raw) -cne $conflictState -or (Get-Content (Join-Path $conflictDir 'wave-history.jsonl') -Raw) -cne $conflictHistory){throw 'denied stale recovery changed Wave/history'}
 $two=Join-Path $root 'rollback';$identity=Make-Repo $two;$rollbackDir=Join-Path $two '.git/xye-handoff';New-Item (Join-Path $rollbackDir 'ownership-locks.json') -ItemType Directory -Force|Out-Null
 Set-Grant 'WAVE-ROLLBACK' $identity.Head;try{Initialize-Wave $two 'WAVE-ROLLBACK' governance $identity.Head $identity.Branch 'https://github.com/Heaifan/XuanYu-Engine/issues/8#issuecomment-55'|Out-Null;throw 'invalid install path unexpectedly succeeded'}catch{if($_.Exception.Message -notmatch 'STATE_PATH_INVALID'){throw}}
 if((Test-Path (Join-Path $rollbackDir 'state.json')) -or (Test-Path (Join-Path $rollbackDir 'work-release.json'))){throw 'Wave rollback left partial state'}
 if(!(Get-Content (Join-Path $rollbackDir 'wave-history.jsonl') -Raw).Contains('WAVE_INIT_ROLLED_BACK')){throw 'Wave rollback is not audited'}
}finally{if(Test-Path $root){Remove-Item $root -Recurse -Force}}
Write-Output 'WAVE INIT SELFTEST PASS'
