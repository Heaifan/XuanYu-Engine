$ErrorActionPreference='Stop'
$root=Join-Path ([IO.Path]::GetTempPath())('xyt-wr-'+[guid]::NewGuid().ToString('N'));$d=Join-Path $PSScriptRoot '..\..\tools\handoff';New-Item -ItemType Directory $root|Out-Null
. (Join-Path $d 'work-release.common.ps1');. (Join-Path $d 'work-release.issue.ps1');. (Join-Path $d 'work-release.assert.ps1')
function Denied([scriptblock]$Action,[string]$Code){try{&$Action;throw "Expected $Code"}catch{if($_.Exception.Message -notmatch $Code){throw}}}
function State([string]$R){$script:fakeState}
function Current([string]$R){[pscustomobject]@{Head='HEAD';Branch='main';Dirty=@()}}
function Fingerprint([string]$R){$script:fakeFingerprint}
function UnknownDirty([string]$R){if($script:unknownDirty){@('UNKNOWN')}else{@()}}
function VersionEvent([string]$R,[string]$Id){$Id -ne 'MISSING'}
try{
 $script:fakeState=[pscustomobject]@{active=$true;mode='development';coordinatorScope='xye';waveId='BASE';baselineHead='HEAD';remoteHead='REMOTE'};$script:fakeFingerprint='fp-1';$script:unknownDirty=$false
 $p=[pscustomobject]@{TaskId='T';LaneId='A';OwnershipSet=@('ViewProjectionState.cs','TerrainFrustumCuller.cs','TerrainLodSelector.cs');VersionEventId='EVT-1'};$x=Issue-Release -RepoRoot $root -Payload $p
 Claim-Ownership $root $x.token 'A' 'T' @('ViewProjectionState.cs')|Out-Null
 $q=Issue-Release $root ([pscustomobject]@{TaskId='T2';LaneId='B';OwnershipSet=@('ViewProjectionState.cs');VersionEventId='EVT-1'})
 Denied {Claim-Ownership $root $q.token 'B' 'T2' @('ViewProjectionState.cs')} 'OWNERSHIP_CONFLICT'
 Denied {Validate-Release $root $x.token 'B' 'T' @('ViewProjectionState.cs')} 'WRITE_DENIED'
 $script:fakeFingerprint='fp-1';$x=Issue-Release -RepoRoot $root -Payload $p;$script:fakeFingerprint='fp-2';Denied {Validate-Release $root $x.token 'A' 'T' @('ViewProjectionState.cs')} 'WRITE_DENIED'
 $script:fakeFingerprint='fp-1';$x=Issue-Release $root $p;$script:fakeState.mode='blocked';Denied {Validate-Release $root $x.token 'A' 'T' @('ViewProjectionState.cs')} 'WRITE_DENIED';$script:fakeState.mode='development'
 $script:fakeState.coordinatorScope=$null;Denied {Issue-Release $root $p} 'COORDINATOR_BLOCKED';$script:fakeState.coordinatorScope='xye'
 $script:unknownDirty=$true;Denied {Issue-Release $root $p} 'UNKNOWN_DIRTY';$script:unknownDirty=$false
 Issue-Release $root $p|Out-Null
 Denied {Issue-Release $root ([pscustomobject]@{TaskId='T';LaneId='A';OwnershipSet=@('ViewProjectionState.cs');VersionEventId='MISSING'})} 'VERSION_EVENT_MISSING'
 'WORK RELEASE SELFTEST PASS'
}finally{if(Test-Path $root){Remove-Item $root -Recurse -Force}}
