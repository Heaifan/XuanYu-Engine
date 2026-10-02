[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$root = Join-Path ([IO.Path]::GetTempPath()) ('candidate-gate-' + [guid]::NewGuid().ToString('N'))
$gate = Join-Path $PSScriptRoot 'candidate-gate.ps1'
function Assert([bool]$ok,[string]$message){if(!$ok){throw "SELFTEST FAILED: $message"}}
function Run($candidate,$repo,$before=$null,$after=$null){
    $candidate | ConvertTo-Json -Depth 10 | Set-Content (Join-Path $root 'candidate.json') -Encoding UTF8
    $repo | ConvertTo-Json -Depth 10 | Set-Content (Join-Path $root 'repo.json') -Encoding UTF8
    $args=@('-RepositoryRoot',$root,'-CandidatePath',(Join-Path $root 'candidate.json'),'-RepositoryFactsPath',(Join-Path $root 'repo.json'))
    if($null -ne $before){$before|ConvertTo-Json|Set-Content (Join-Path $root 'before.json');$args+=@('-BeforeFingerprintPath',(Join-Path $root 'before.json'))}
    if($null -ne $after){$after|ConvertTo-Json|Set-Content (Join-Path $root 'after.json');$args+=@('-AfterFingerprintPath',(Join-Path $root 'after.json'))}
    @(& pwsh -NoLogo -NoProfile -File $gate @args 2>&1) -join "`n" | ConvertFrom-Json
}
try {
    New-Item -ItemType Directory -Force $root | Out-Null;git -C $root init -q
    . (Join-Path $PSScriptRoot 'candidate-fingerprint.ps1');New-Item -ItemType Directory -Force (Join-Path $root 'src') | Out-Null;Set-Content (Join-Path $root 'src/a.cs') 'v1';$fp1=Get-CandidateFingerprint $root @('src/a.cs');Set-Content (Join-Path $root 'docs.txt') 'outside';$fp2=Get-CandidateFingerprint $root @('src/a.cs');Set-Content (Join-Path $root 'src/a.cs') 'v2';$fp3=Get-CandidateFingerprint $root @('src/a.cs');Assert ($fp1.Digest -eq $fp2.Digest -and $fp1.Digest -ne $fp3.Digest) 'freeze fingerprint closure scope'
    $a=[pscustomobject]@{Id='A';OwnerTaskId='A';CandidateFiles=@('src/a.cs');BuildDependencies=@('core/**');TestDependencies=@('tests/a/**');RuntimeDependencies=@('runtime/a/**');TruthFiles=@('truth/a.json')}
    $unrelated=[pscustomobject]@{Path='docs/other.md';Classification='FRIENDLY_ACTIVE';OwnerTaskId='B'}
    $base=[pscustomobject]@{ActiveTasks=@([pscustomobject]@{TaskId='A';Status='ACTIVE';WriteScope=@('src/**')},[pscustomobject]@{TaskId='B';Status='ACTIVE';WriteScope=@('docs/**')});DirtyRecords=@($unrelated);ReleasedRecords=@();RepositoryUnknownTemp=1}
    $r=Run $a $base; Assert ($r.RepositoryActiveTasks -eq 2 -and $r.CandidateScopedActiveWriters -eq 0 -and $r.CertificationAllowed -eq 'YES') 'case 1'
    $base.DirtyRecords=@([pscustomobject]@{Path='core/x.cs';Classification='FOREIGN_KNOWN';OwnerTaskId='B'});$base.ActiveTasks[1].WriteScope=@('core/**')
    $r=Run $a $base; Assert ($r.ConsumedActiveForeignTemp -eq 1 -and $r.CertificationAllowed -eq 'NO') 'case 2'
    $base.DirtyRecords=@([pscustomobject]@{Path='unknown/x.tmp';Classification='UNAUTHORIZED_DIRTY'});$base.ActiveTasks[1].WriteScope=@('unknown/**')
    $r=Run $a $base; Assert ($r.RepositoryUnknownTemp -eq 1 -and $r.CandidateScopedUnknownTemp -eq 0 -and $r.CertificationAllowed -eq 'YES') 'case 3'
    $base.DirtyRecords=@([pscustomobject]@{Path='core/x.tmp';Classification='UNAUTHORIZED_DIRTY'});$r=Run $a $base; Assert ($r.CandidateScopedUnknownTemp -eq 1 -and $r.CertificationAllowed -eq 'NO') 'case 4'
    $base.DirtyRecords=@([pscustomobject]@{Path='docs/x.tmp';Classification='OWNERSHIP_CONFLICT'});$r=Run $a $base; Assert ($r.CandidateScopedOwnershipConflict -eq 0) 'case 5'
    $base.DirtyRecords=@([pscustomobject]@{Path='core/x.tmp';Classification='OWNERSHIP_CONFLICT'});$r=Run $a $base; Assert ($r.CandidateScopedOwnershipConflict -eq 1 -and $r.CertificationAllowed -eq 'NO') 'case 6'
    $fp=[pscustomobject]@{Digest='same'};$changed=[pscustomobject]@{Digest='changed'};$r=Run $a ([pscustomobject]@{ActiveTasks=@();DirtyRecords=@();ReleasedRecords=@()}) $fp $changed; Assert ($r.CandidateChangedDuringCertification -eq 'YES' -and $r.FinalEvidenceEligibility -eq 'NO') 'case 7'
    $r=Run $a ([pscustomobject]@{ActiveTasks=@();DirtyRecords=@();ReleasedRecords=@()}) $fp $fp; Assert ($r.CandidateChangedDuringCertification -eq 'NO' -and $r.CertificationAllowed -eq 'YES') 'case 8'
    $base.DirtyRecords=@([pscustomobject]@{Path='legacy/x.tmp';Classification='FRIENDLY_COMPLETED'});$r=Run $a $base; Assert ($r.CertificationAllowed -eq 'YES') 'case 9'
    $a.BuildDependencies=@('legacy/**');$r=Run $a $base; Assert ($r.FinalEvidenceEligibility -eq 'NO') 'case 10'
    Assert ((Get-Content $gate).Count -le 100) '5+100 gate'
    'CANDIDATE-SCOPED GATE SELFTEST PASS 10/10'
} finally {if(Test-Path $root){Remove-Item $root -Recurse -Force}}
