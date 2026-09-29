$ErrorActionPreference = 'Stop'
$common = Join-Path $PSScriptRoot 'work-release.common.ps1'
$root = Join-Path ([IO.Path]::GetTempPath()) ('h2f-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $root | Out-Null
. $common
. (Join-Path $PSScriptRoot 'work-release.issue.ps1')
. (Join-Path $PSScriptRoot 'work-release.assert.ps1')
function Assert([bool]$ok, [string]$message) { if (-not $ok) { throw $message } }
function Run-Git([string[]]$gitArgs) { $out = @(& git -C $root @gitArgs 2>&1); if ($LASTEXITCODE -ne 0) { throw ($out -join "`n") }; $out }
function Expect-Reject([scriptblock]$action) { try { & $action; throw 'expected rejection' } catch { if ($_.Exception.Message -notmatch 'Candidate Fingerprint changed') { throw } } }
Run-Git @('init','-q'); Run-Git @('config','user.email','h2f@example.test'); Run-Git @('config','user.name','H2F'); Run-Git @('config','core.autocrlf','false')
Set-Content (Join-Path $root 'owned.txt') 'base'; Set-Content (Join-Path $root 'foreign.txt') 'foreign'; New-Item (Join-Path $root 'tools\handoff') -ItemType Directory | Out-Null
Set-Content (Join-Path $root 'tools\handoff\ownership-manifest.json') '{"version":1,"entries":[{"path":"owned.txt","owner":"GOVERNANCE"},{"path":"foreign.txt","owner":"FOREIGN"}]}'
New-Item (Join-Path $root 'docs\governance') -ItemType Directory | Out-Null; Set-Content (Join-Path $root 'docs\governance\version-events.tsv') 'EVT-H2F'; Run-Git @('add','-A'); Run-Git @('commit','-qm','base')
$owned = Join-Path $root 'owned.txt'; $foreign = Join-Path $root 'foreign.txt'; $scope = @('owned.txt')
Set-Content $owned 'AAA'; $a = Fingerprint $root $scope; Set-Content $owned 'BBB'; $b = Fingerprint $root $scope
Assert ($a -ne $b) 'H2F-01 content change must change fingerprint'
Set-Content $owned 'AAA'; $restored = Fingerprint $root $scope; Assert ($a -eq $restored) 'H2F-02 restored content must restore equivalent identity'
Set-Content $foreign 'foreign-A'; $foreignA = Fingerprint $root $scope; Set-Content $foreign 'foreign-B'; $foreignB = Fingerprint $root $scope
Assert ($foreignA -eq $foreignB) 'H2F-03 unrelated ForeignDirty polluted candidate fingerprint'
Set-Content $foreign 'foreign-C'; $consumedA = Fingerprint $root @('foreign.txt'); Set-Content $foreign 'foreign-D'; $consumedB = Fingerprint $root @('foreign.txt')
Assert ($consumedA -ne $consumedB) 'H2F-04 consumed ForeignDirty content must change fingerprint'
$stateDir = Join-Path $root '.git\xye-handoff'; New-Item $stateDir -ItemType Directory -Force | Out-Null
$head = (Run-Git @('rev-parse','HEAD')).Trim(); [pscustomobject]@{active=$true;mode='development';coordinatorScope='xye';baselineHead=$head;waveId='H2F'} | ConvertTo-Json | Set-Content (Join-Path $stateDir 'state.json')
$release = Issue-Release $root ([pscustomobject]@{TaskId='H2F';LaneId='GOVERNANCE';OwnershipSet=@('owned.txt');VersionEventId='EVT-H2F'})
Set-Content $owned 'during-test'; Expect-Reject { Validate-Release $root $release.token 'GOVERNANCE' 'H2F' @('owned.txt') }
Write-Output 'H2F DIRTY CONTENT FINGERPRINT SELFTEST: PASS'
Write-Output 'Cases: 5'
