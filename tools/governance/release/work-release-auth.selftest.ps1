[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
$root=Join-Path $env:TEMP ('xye-release-auth-'+[guid]::NewGuid().ToString('N'))
$script=Join-Path $PSScriptRoot 'work-release.ps1'
function Run([string[]]$a){$old=$ErrorActionPreference;$ErrorActionPreference='Continue';try{$o=@(& powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $script @a 2>&1);[pscustomobject]@{Code=$LASTEXITCODE;Text=$o -join "`n"}}finally{$ErrorActionPreference=$old}}
function Deny($r,[string]$needle){if($r.Code -eq 0 -or $r.Text -notmatch $needle){throw "AUTH SELFTEST RED: expected denial $needle / $($r.Text)"}}
try{
 New-Item -ItemType Directory -Force $root|Out-Null;git -C $root init -q;git -C $root config user.email test@example.invalid;git -C $root config user.name test
 New-Item (Join-Path $root 'docs/governance') -ItemType Directory -Force|Out-Null;New-Item (Join-Path $root 'tools/governance/ownership') -ItemType Directory -Force|Out-Null
 Set-Content (Join-Path $root 'owned.txt') base;Set-Content (Join-Path $root 'tools/governance/ownership/ownership-manifest.json') '{"version":1,"entries":[{"path":"owned.txt","owner":"GOVERNANCE"}]}'
 Set-Content (Join-Path $root 'docs/governance/version-events.tsv') "EVT-EXACT`tTASK`tGOVERNANCE`tName`t-`t-`tAPPLIED`t-`t-`t-`t-`t-`t-`t-`t-`t-`t-`t-`t-"
 git -C $root add -A;git -C $root commit -qm seed;$head=(git -C $root rev-parse HEAD).Trim();$dir=Join-Path $root '.git/xye-handoff';New-Item $dir -ItemType Directory -Force|Out-Null
 [pscustomobject]@{active=$true;mode='development';coordinatorScope='xye';baselineHead=$head;waveId='AUTH-TEST'}|ConvertTo-Json|Set-Content (Join-Path $dir 'state.json')
 [pscustomobject]@{tasks=@([pscustomobject]@{TaskId='TASK-A';Status='ACTIVE'})}|ConvertTo-Json|Set-Content (Join-Path $dir 'task-registry.json')
 $denied=Run @('-RepositoryRoot',$root,'-Mode','issue','-CentralIssuer','-TaskId','TASK-A','-LaneId','GOVERNANCE','-OwnershipSet','owned.txt','-VersionEventId','EVT-EXACT')
 if($denied.Code -eq 0 -or $denied.Text -notmatch 'CentralIssuer|AUTHORIZATION_REQUIRED'){throw "AUTH SELFTEST RED: legacy switch authorized issue / $($denied.Text)"}
 Deny (Run @('-RepositoryRoot',$root,'-Mode','issue','-TaskId','TASK-A','-LaneId','GOVERNANCE','-OwnershipSet','owned.txt','-VersionEventId','EVT-EXACT')) 'AUTHORIZATION_REQUIRED'
 $releasePath=Join-Path $dir 'work-release.json';[pscustomobject]@{status='ACTIVE';token='fixture';taskId='TASK-A';laneId='GOVERNANCE'}|ConvertTo-Json|Set-Content $releasePath
 Deny (Run @('-RepositoryRoot',$root,'-Mode','revoke')) 'AUTHORIZATION_REQUIRED';if((Get-Content $releasePath -Raw|ConvertFrom-Json).status -ne 'ACTIVE'){throw 'denied revoke changed Work Release state'}
 . (Join-Path $PSScriptRoot 'work-release.common.ps1')
 if((VersionEvent $root '') -or (VersionEvent $root 'EVT') -or (VersionEvent $root 'EventId') -or !(VersionEvent $root 'EVT-EXACT')){throw 'VERSION EVENT exact/nonempty contract failed'}
 . (Join-Path $PSScriptRoot '..\coordinator\authority-auth.ps1')
 $created=[datetimeoffset]::UtcNow;$script:authBody="XYE AUTHORIZATION V1`nAction: work-release-issue`nTarget: TASK-A`nDetail: EVT-EXACT`nBaseline: $head`nExpiresAt: $($created.AddHours(2).ToString('o'))"
 $script:authComment=[pscustomobject]@{id=101;user=[pscustomobject]@{login='Heaifan';id=86356120};author_association='OWNER';body=$script:authBody;created_at=$created.ToString('o');updated_at=$created.ToString('o')}
 function Get-AuthorityComment([string]$Root,[string]$Url){$script:authComment}
 $grant=Assert-OwnerAuthorization $root 'https://github.com/Heaifan/XuanYu-Engine/issues/8#issuecomment-101' 'work-release-issue' 'TASK-A' 'EVT-EXACT' $head
 if($grant.Author -ne 'Heaifan'){throw 'OWNER AUTHORIZATION positive fixture failed'}
 $script:authComment.author_association='CONTRIBUTOR';try{Assert-OwnerAuthorization $root 'https://github.com/Heaifan/XuanYu-Engine/issues/8#issuecomment-101' 'work-release-issue' 'TASK-A' 'EVT-EXACT' $head|Out-Null;throw 'non-owner authorization was accepted'}catch{if($_.Exception.Message -notmatch 'AUTH_OWNER_REQUIRED'){throw}}
 $script:authComment.author_association='OWNER';try{Assert-OwnerAuthorization $root 'https://github.com/Heaifan/XuanYu-Engine/issues/8#issuecomment-101' 'work-release-revoke' 'TASK-A' 'REVOKE' $head|Out-Null;throw 'wrong action was accepted'}catch{if($_.Exception.Message -notmatch 'AUTH_SCOPE_MISMATCH'){throw}}
 Consume-OwnerAuthorization $root $grant;try{Consume-OwnerAuthorization $root $grant;throw 'authorization replay was accepted'}catch{if($_.Exception.Message -notmatch 'AUTH_EVIDENCE_REPLAYED'){throw}}
 if(@(Get-Content (Join-Path $root '.git/xye-handoff/authorization-events.jsonl')|%{$_|ConvertFrom-Json}|? Event -eq 'AUTHORIZATION_CONSUMED').Count -ne 1){throw 'authorization consumption audit is missing'}
 if((Get-Content $releasePath -Raw|ConvertFrom-Json).status -ne 'ACTIVE'){throw 'denied issue changed Work Release state'}
 Write-Output 'WORK RELEASE AUTH SELFTEST PASS'
}finally{if(Test-Path $root){Remove-Item $root -Recurse -Force}}
