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
 $two=Join-Path $root 'rollback';$identity=Make-Repo $two;$rollbackDir=Join-Path $two '.git/xye-handoff';New-Item (Join-Path $rollbackDir 'ownership-locks.json') -ItemType Directory -Force|Out-Null
 Set-Grant 'WAVE-ROLLBACK' $identity.Head;try{Initialize-Wave $two 'WAVE-ROLLBACK' governance $identity.Head $identity.Branch 'https://github.com/Heaifan/XuanYu-Engine/issues/8#issuecomment-55'|Out-Null;throw 'invalid install path unexpectedly succeeded'}catch{if($_.Exception.Message -notmatch 'STATE_PATH_INVALID'){throw}}
 if((Test-Path (Join-Path $rollbackDir 'state.json')) -or (Test-Path (Join-Path $rollbackDir 'work-release.json'))){throw 'Wave rollback left partial state'}
 if(!(Get-Content (Join-Path $rollbackDir 'wave-history.jsonl') -Raw).Contains('WAVE_INIT_ROLLED_BACK')){throw 'Wave rollback is not audited'}
}finally{if(Test-Path $root){Remove-Item $root -Recurse -Force}}
Write-Output 'WAVE INIT SELFTEST PASS'
