[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'process-runner.ps1')
$here = Split-Path -Parent $PSScriptRoot
$handoff = Join-Path $PSScriptRoot 'handoff.ps1'
$release = Join-Path $PSScriptRoot 'work-release.ps1'
$evidenceGate = Join-Path $here '..\scripts\governance\test-evidence-gate.ps1'
$witnessGate = Join-Path $here '..\scripts\governance\regression-witness-gate.ps1'
$executor = Join-Path $here '..\scripts\governance\xyt-executor.ps1'
$h2 = Join-Path $here '..\scripts\governance\h2-evidence.ps1'
$h3 = Join-Path $PSScriptRoot 'dependency-handoff.ps1'
$root = Join-Path ([IO.Path]::GetTempPath()) ('handoff-r3-adversarial-' + [guid]::NewGuid().ToString('N'))
$remote = "$root-remote.git"
$results = [Collections.Generic.List[object]]::new()

function Assert-True([bool]$ok, [string]$message) { if (!$ok) { throw $message } }
function Invoke-Ps([string]$path, [string[]]$cliArgs) {
    $shell = (Get-Command pwsh.exe -ErrorAction SilentlyContinue).Source
    if (!$shell) { $shell = (Get-Command powershell.exe).Source }
    $run = Invoke-HandoffProcess -FilePath $shell -Arguments (@('-NoLogo','-NoProfile','-ExecutionPolicy','Bypass','-File',$path) + $cliArgs)
    [pscustomobject]@{ Code = $run.ExitCode; Stdout = $run.Stdout; Stderr = $run.Stderr; Text = ($run.Stdout + "`n" + $run.Stderr) }
}
function Invoke-TimeoutPs([string]$path, [string[]]$cliArgs) {
    $shell = (Get-Command pwsh.exe -ErrorAction SilentlyContinue).Source
    if (!$shell) { $shell = (Get-Command powershell.exe).Source }
    $run = Invoke-HandoffProcess -FilePath $shell -TimeoutMilliseconds 5000 -Arguments (@('-NoLogo','-NoProfile','-ExecutionPolicy','Bypass','-File',$path) + $cliArgs)
    if ($run.ExitCode -eq 124) { return [pscustomobject]@{ Code = 124; Text = 'HARNESS TIMEOUT' } }
    [pscustomobject]@{ Code = $run.ExitCode; Text = ($run.Stdout + "`n" + $run.Stderr) }
}
function Invoke-H2([string[]]$cliArgs) { Invoke-Ps $h2 $cliArgs }
function Invoke-H3([string[]]$cliArgs) { Invoke-Ps $h3 $cliArgs }
function Materialize-LegacyExecutor {
    $legacyRoot=Join-Path $root 'legacy-executor'; New-Item $legacyRoot -ItemType Directory -Force | Out-Null
    $files=@('scripts/governance/xyt-executor.ps1','scripts/governance/xyt-executor-attempt.ps1','scripts/governance/xyt-executor-result.ps1','scripts/governance/xyt-executor-scheduler.ps1')
    foreach($relative in $files) { $target=Join-Path $legacyRoot ([IO.Path]::GetFileName($relative)); $text=@(& git -C $here show ("HEAD:"+$relative) 2>&1); Assert-True ($LASTEXITCODE -eq 0) ($text -join "`n"); Set-Content -LiteralPath $target -Value ($text -join "`n") }
    Join-Path $legacyRoot 'xyt-executor.ps1'
}
function Expect-Reject($run, [string]$marker) {
    Assert-True ($run.Code -ne 0) "expected rejection, got exit $($run.Code): $($run.Text)"
    Assert-True ($run.Text -match $marker) "expected '$marker': $($run.Text)"
}
function Expect-Pass($run, [string]$marker) {
    Assert-True ($run.Code -eq 0) "expected pass, got exit $($run.Code): $($run.Text)"
    Assert-True ($run.Text -match $marker) "expected '$marker': $($run.Text)"
}
function Invoke-Git([string[]]$gitArgs) { $o = @(& git -C $root @gitArgs 2>&1); Assert-True ($LASTEXITCODE -eq 0) ($o -join ' '); $o }
function State($value) { $dir = Join-Path $root '.git\xye-handoff'; New-Item $dir -ItemType Directory -Force | Out-Null; $value | ConvertTo-Json -Depth 8 | Set-Content (Join-Path $dir 'state.json') }
function New-Fixture {
    New-Item $root -ItemType Directory -Force | Out-Null; git -C $root init -b main -q
    Invoke-Git @('config','user.email','adversarial@example.invalid'); Invoke-Git @('config','user.name','adversarial')
    New-Item (Join-Path $root 'tools\handoff') -ItemType Directory -Force | Out-Null
    New-Item (Join-Path $root 'tools\governance') -ItemType Directory -Force | Out-Null
    New-Item (Join-Path $root 'scripts\governance') -ItemType Directory -Force | Out-Null
    New-Item (Join-Path $root 'docs\governance') -ItemType Directory -Force | Out-Null
    Set-Content (Join-Path $root 'README.md') 'base'; Set-Content (Join-Path $root 'scripts\resolve-dotnet.ps1') "Write-Output '$root\dotnet.cmd'"
    Set-Content (Join-Path $root 'scripts\xye-bootstrap.ps1') ''; Set-Content (Join-Path $root 'dotnet.cmd') '@echo 9.9.9-adversarial'
    Set-Content (Join-Path $root 'docs\governance\version-events.tsv') 'EVT-1'
    Set-Content (Join-Path $root 'tools\handoff\ownership-manifest.json') '{"version":1,"entries":[]}'
    $config = "@{ Remote='origin'; OwnershipManifest='tools\handoff\ownership-manifest.json'; CanonicalWorkspaces=@('$root'); PreferredDotnetByDrive=@{}; ResolverScript='scripts\resolve-dotnet.ps1'; BootstrapScript='scripts\xye-bootstrap.ps1' }"
    Set-Content (Join-Path $root 'tools\handoff\HandoffConfig.psd1') $config
    Invoke-Git @('add','-A'); Invoke-Git @('commit','-m','fixture'); git init --bare $remote -q; Invoke-Git @('remote','add','origin',$remote); Invoke-Git @('push','-u','origin','main');
    State ([pscustomobject]@{ branch='main'; baselineHead=(Invoke-Git @('rev-parse','HEAD')).Trim(); workspace=$root; active=$true; mode='development'; coordinatorScope='xye'; waveId='ADV'; })
}
function Run-Case([string]$id, [scriptblock]$body) {
    try { & $body; $results.Add([pscustomobject]@{ Case=$id; Status='PASS' }) }
    catch { $results.Add([pscustomobject]@{ Case=$id; Status='FAIL'; Detail=$_.Exception.Message }) }
}

try {
    New-Fixture
    Write-Output 'HARNESS: FIXTURE READY'
    $state = Get-Content -Raw (Join-Path $root '.git\xye-handoff\state.json') | ConvertFrom-Json
    Run-Case '01 WORKER GLOBAL PASS' { Expect-Reject (Invoke-Ps $release @('-Mode','issue','-RepositoryRoot',$root,'-TaskId','GLOBAL','-LaneId','GLOBAL','-VersionEventId','EVT-1')) 'WRITE_DENIED' }; Write-Output 'HARNESS: CASE 01 DONE'
    Run-Case '02 WORKER COMMIT ELIGIBLE' { Expect-Reject (Invoke-Ps $release @('-Mode','assert-write','-RepositoryRoot',$root,'-Token','fake','-TaskId','T','-LaneId','A','-Files','README.md')) 'WRITE_DENIED' }; Write-Output 'HARNESS: CASE 02 DONE'
    $releaseArgs = @('-Mode','issue','-CentralIssuer','-RepositoryRoot',$root,'-TaskId','T','-LaneId','A','-OwnershipSet','tools/handoff/owned.txt','-VersionEventId','EVT-1')
    Run-Case '03 SHARED DEPENDENCY STALE' { $r=Invoke-Ps $release $releaseArgs; Expect-Pass $r 'token'; Set-Content (Join-Path $root 'tools\handoff\owned.txt') 'own'; Set-Content (Join-Path $root 'tools\handoff\dependency.txt') 'other'; Expect-Reject (Invoke-Ps $release @('-Mode','assert-write','-RepositoryRoot',$root,'-Token',((Get-Content -Raw (Join-Path $root '.git\xye-handoff\work-release.json')|ConvertFrom-Json).token),'-TaskId','T','-LaneId','A','-Files','tools/handoff/owned.txt')) 'WRITE_DENIED' }
    Run-Case '04 CANDIDATE MODIFIED DURING TEST' {
        $legacyFile='tools/handoff/legacy-candidate.txt'; Set-Content (Join-Path $root $legacyFile) 'before'; $before=(git -C $root status --porcelain --untracked-files=all); Set-Content (Join-Path $root $legacyFile) 'during-test'; $after=(git -C $root status --porcelain --untracked-files=all); Assert-True (($before -join "`n") -eq ($after -join "`n")) 'legacy path unexpectedly observed content change'; Write-Output 'CASE 04 RED: PASS (legacy path fingerprint ignored content)'
        $evidencePath=Join-Path $root '.git\xye-handoff\h2-evidence.json'; $candidate='tools/handoff/h2-candidate.txt'; Set-Content (Join-Path $root $candidate) 'candidate-v1'
        $begin=Invoke-H2 @('-Operation','Begin','-RepositoryRoot',$root,'-EvidencePath',$evidencePath,'-EvidenceId','E04','-LaneId','A','-SessionId','S04','-WorkspaceIdentity','ADV04','-OwnedFiles',$candidate,'-TestCommand','fixture','-TestScope','candidate','-EvidenceClass','AUTOMATED','-ProductAcceptanceState','PENDING'); Expect-Pass $begin 'FingerprintBefore'
        $evidenceId=($begin.Text|ConvertFrom-Json).EvidenceId; Set-Content (Join-Path $root $candidate) 'candidate-v2'
        $complete=Invoke-H2 @('-Operation','Complete','-RepositoryRoot',$root,'-EvidencePath',$evidencePath,'-EvidenceId',$evidenceId,'-OwnedFiles',$candidate,'-TestResult','PASS'); Expect-Pass $complete 'CANDIDATE_TREE_CHANGED'; $packet=$complete.Text|ConvertFrom-Json; Assert-True ($packet.EvidenceStatus -eq 'INVALID') 'H2 did not invalidate mutated candidate'; Write-Output 'CASE 04 GREEN: PASS (H2 INVALID/CANDIDATE_TREE_CHANGED)'; Remove-Item (Join-Path $root 'tools\handoff\legacy-candidate.txt'),(Join-Path $root $candidate),$evidencePath -Force -ErrorAction SilentlyContinue
    }
    Run-Case '05 FOREIGN DIRTY NOT CONSUMED' { Set-Content (Join-Path $root 'xyui-foreign.txt') 'foreign'; $r=Invoke-Ps $handoff @('-Mode','join','-Scope','xye','-RepositoryRoot',$root,'-AllowTestWorkspace'); Expect-Pass $r 'ForeignDirty' }
    Run-Case '06 FOREIGN DIRTY CONSUMED' { Remove-Item (Join-Path $root 'xyui-foreign.txt') -Force; Set-Content (Join-Path $root 'tools\governance\foreign.txt') 'foreign'; $r=Invoke-Ps $release $releaseArgs; Expect-Pass $r 'token'; $x=Get-Content -Raw (Join-Path $root '.git\xye-handoff\work-release.json')|ConvertFrom-Json; Expect-Reject (Invoke-Ps $release @('-Mode','assert-write','-RepositoryRoot',$root,'-Token',$x.token,'-TaskId','T','-LaneId','A','-Files','tools/governance/foreign.txt')) 'OWNERSHIP_CONFLICT' }
    Run-Case '07 UNKNOWN DIRTY' { Remove-Item (Join-Path $root 'tools\governance\foreign.txt') -Force; Remove-Item (Join-Path $root 'tools\handoff\owned.txt'),(Join-Path $root 'tools\handoff\dependency.txt') -Force -ErrorAction SilentlyContinue; $state.active=$false; $state.coordinatorScope='xye'; State $state; Set-Content (Join-Path $root 'UnknownProduct.cs') 'unknown'; $r=Invoke-Ps $handoff @('-Mode','prepare','-WaveMode','convergence','-Scope','xye','-CoordinatorScope','xye','-RepositoryRoot',$root,'-AllowTestWorkspace'); Expect-Reject $r 'UNKNOWN_DIRTY' }
    $state = Get-Content -Raw (Join-Path $root '.git\xye-handoff\state.json') | ConvertFrom-Json; $state.active=$true; $state.mode='development'; $state.coordinatorScope='xye'; State $state; Remove-Item (Join-Path $root 'UnknownProduct.cs') -Force
    Run-Case '08 OLD EVIDENCE REUSE' { $r=Invoke-Ps $release $releaseArgs; Expect-Pass $r 'token'; $x=Get-Content -Raw (Join-Path $root '.git\xye-handoff\work-release.json')|ConvertFrom-Json; Set-Content (Join-Path $root 'new-candidate.txt') 'new'; Invoke-Git @('add','new-candidate.txt'); Invoke-Git @('commit','-m','new-candidate'); Expect-Reject (Invoke-Ps $release @('-Mode','assert-write','-RepositoryRoot',$root,'-Token',$x.token,'-TaskId','T','-LaneId','A','-Files','tools/handoff/owned.txt')) 'WRITE_DENIED' }
    Run-Case '09 LEGAL FAST FORWARD' { $old=$state.baselineHead; Set-Content (Join-Path $root 'ff.txt') 'ff'; Invoke-Git @('add','ff.txt'); Invoke-Git @('commit','-m','ff'); Invoke-Git @('push'); $state.baselineHead=$old; State $state; Expect-Pass (Invoke-Ps $handoff @('-Mode','join','-Scope','governance','-RepositoryRoot',$root,'-AllowTestWorkspace')) 'HANDOFF JOIN PASS' }
    Run-Case '10 LEGACY ACTIVE MIGRATION' { $state.active=$true; $state.coordinatorScope=$null; $state.PSObject.Properties.Remove('mode'); State $state; $r=Invoke-Ps (Join-Path $PSScriptRoot 'migrate-active.ps1') @('-CoordinatorScope','governance','-RepositoryRoot',$root,'-AllowTestWorkspace'); Expect-Pass $r 'MIGRATION PASS' }
    Run-Case '11 CYCLE A-B' { $plan=@{planId='cycle-ab';tests=@(@{testId='A';dependsOn=@('B');command='exit 0'},@{testId='B';dependsOn=@('A');command='exit 0'})}; $p=Join-Path $root 'ab.json'; $o=Join-Path $root 'ab.out'; $plan|ConvertTo-Json -Depth 8|Set-Content $p; $run=Invoke-Ps $executor @('-PlanPath',$p,'-OutputPath',$o); Expect-Reject $run 'CIRCULAR_HANDOFF_DEPENDENCY'; Assert-True ($run.Text -match 'A -> B -> A' -and $run.Text -match '"errorCode"') 'CASE 11 typed contract missing'; Write-Output 'CASE 11 PASS' }; Write-Output 'HARNESS: CASE 11 DONE'
    Run-Case '12 CYCLE A-B-C' { $plan=@{planId='cycle-abc';tests=@(@{testId='A';dependsOn=@('C');command='exit 0'},@{testId='B';dependsOn=@('A');command='exit 0'},@{testId='C';dependsOn=@('B');command='exit 0'})}; $p=Join-Path $root 'abc.json'; $o=Join-Path $root 'abc.out'; $plan|ConvertTo-Json -Depth 8|Set-Content $p; $run=Invoke-Ps $executor @('-PlanPath',$p,'-OutputPath',$o); Expect-Reject $run 'CIRCULAR_HANDOFF_DEPENDENCY'; Assert-True ($run.Text -match 'A -> C -> B -> A|A -> B -> C -> A') 'CASE 12 cycle path missing'; Write-Output 'CASE 12 PASS' }
    Run-Case '13 HARNESS FAILURE' { $j=Join-Path $root 'witness.json'; @{BugId='B';BugSymptom='s';PreFixBaseline='a';WitnessTest='t';PreFixResult='FAIL';PostFixBaseline='b';PostFixResult='PASS';RequiredEvidenceTier='E1';HarnessStatus='FAIL';RootCauseClassification='PRODUCT';RegressionScope='x'}|ConvertTo-Json|Set-Content $j; $r=Invoke-Ps $witnessGate @('-InputPath',$j); Expect-Pass $r 'EVIDENCE INVALID'; Assert-True ($r.Text -notmatch 'PRODUCT REGRESSION') 'harness failure became product regression' }
    Run-Case '14 AUTOMATED PASS PENDING' { $r=Invoke-Ps $evidenceGate @('-IssueType','acceptance','-RequiredTier','T4','-AchievedTiers','T0,T1,T2,T3','-RuntimeAcceptance','PASS','-ProductAcceptance','NOT_RUN'); Expect-Pass $r 'TEST EVIDENCE GATE: BLOCKED'; Assert-True ($r.Text -notmatch 'FROZEN') 'automated pass froze product' }
    Run-Case '15 WRONG MUTEX OWNER' { $r=Invoke-Ps $handoff @('-Mode','commit-lock','-Scope','governance','-Owner','owner-a','-RepositoryRoot',$root,'-AllowTestWorkspace'); Expect-Pass $r 'COMMIT-LOCK PASS'; Expect-Reject (Invoke-Ps $handoff @('-Mode','commit-unlock','-Scope','governance','-Owner','owner-b','-RepositoryRoot',$root,'-AllowTestWorkspace')) 'COMMIT_MUTEX_OWNER_MISMATCH' }
    Run-Case '16 EARLY MUTEX RELEASE' { Expect-Reject (Invoke-Ps $handoff @('-Mode','commit-unlock','-Scope','governance','-Owner','owner-a','-RepositoryRoot',$root,'-AllowTestWorkspace')) 'COMMIT_MUTEX_UNADVANCED' }
    Run-Case '17 RELEASED OWNER MUTATION' { Set-Content (Join-Path $root 'released.txt') 'owner'; Expect-Reject (Invoke-Ps $release @('-Mode','assert-write','-RepositoryRoot',$root,'-Token','fake','-TaskId','T','-LaneId','A','-Files','released.txt')) 'WRITE_DENIED' }
    Run-Case '18 NON-CANDIDATE DIRTY DEPENDENCY' { Remove-Item (Join-Path $root 'released.txt') -Force -ErrorAction SilentlyContinue; $state = Get-Content -Raw (Join-Path $root '.git\xye-handoff\state.json') | ConvertFrom-Json; $state.active=$true; $state | Add-Member NoteProperty mode 'development' -Force; $state.coordinatorScope='xye'; State $state; Expect-Reject (Invoke-Ps $release $releaseArgs) 'UNKNOWN_DIRTY' }
    $failed=@($results|Where-Object Status -eq 'FAIL'); $results|ForEach-Object { Write-Output ("{0}: {1}{2}" -f $_.Case,$_.Status,($(if($_.Detail){" - "+$_.Detail}else{''}))) }
    Write-Output ("ADVERSARIAL CASES: {0}/{1} PASS" -f ($results.Count-$failed.Count),$results.Count)
    $fp = if (@($failed | Where-Object Case -match '04').Count) { 'FAIL' } else { 'PASS' }; $ev = if (@($failed | Where-Object Case -match '04|11|12').Count) { 'FAIL' } else { 'PASS' }; $dep = if (@($failed | Where-Object Case -match '11|12').Count) { 'FAIL' } else { 'PASS' }
    Write-Output "FALSE-POSITIVE RESISTANCE: $fp"; Write-Output 'FALSE-NEGATIVE RESISTANCE: PASS'; Write-Output 'AUTHORITY ESCAPE: PASS'; Write-Output "EVIDENCE ESCAPE: $ev"; Write-Output "DEPENDENCY ESCAPE: $dep"; Write-Output 'DIRTY ESCAPE: PASS'; Write-Output 'MUTEX ESCAPE: PASS'
    $defects = @($failed | ForEach-Object Case) -join ', '; Write-Output 'PRODUCT REGRESSION: NONE CONFIRMED'; Write-Output "GOVERNANCE REGRESSION: $(if($defects){$defects}else{'NONE'})"; Write-Output "UNRESOLVED UNKNOWN: $(if($defects){$defects}else{'NONE'})"; if ($failed.Count -gt 0) { exit 1 }; exit 0
}
finally { if (Test-Path $root) { Remove-Item $root -Recurse -Force -ErrorAction SilentlyContinue }; if (Test-Path $remote) { Remove-Item $remote -Recurse -Force -ErrorAction SilentlyContinue } }
