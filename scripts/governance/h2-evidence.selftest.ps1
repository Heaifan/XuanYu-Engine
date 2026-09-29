$ErrorActionPreference = 'Stop'
$module = Join-Path $PSScriptRoot 'h2-evidence.ps1'
$root = Join-Path ([IO.Path]::GetTempPath()) ('h2-evidence-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $root | Out-Null
function Invoke-TestGit([string[]]$a) { & git -C $root @a 2>&1; if ($LASTEXITCODE -ne 0) { throw "git failed: $($a -join ' ')" } }
function Run([string[]]$a) { $named = @{}; for ($i = 0; $i -lt $a.Count; $i += 2) { $named[$a[$i].TrimStart('-')] = $a[$i + 1] }; $raw = @(& $module @named 2>&1); if ($LASTEXITCODE -ne 0) { throw ($raw -join "`n") }; $parsed = ($raw -join "`n") | ConvertFrom-Json; if ($parsed -is [array]) { return ($parsed | Where-Object EvidenceId | Select-Object -Last 1) }; return $parsed }
function Assert([bool]$condition, [string]$message) { if (-not $condition) { throw $message } }
Invoke-TestGit @('init','-q'); Invoke-TestGit @('config','user.email','h2@example.test'); Invoke-TestGit @('config','user.name','H2'); Invoke-TestGit @('config','core.autocrlf','false')
Set-Content (Join-Path $root 'owned.txt') 'one'; Set-Content (Join-Path $root 'dependency.txt') 'dep'; Set-Content (Join-Path $root 'foreign.txt') 'foreign'
Invoke-TestGit @('add','owned.txt','dependency.txt','foreign.txt'); Invoke-TestGit @('commit','-qm','base')
$owned = Join-Path $root 'owned.txt'; $dependency = Join-Path $root 'dependency.txt'; $store = Join-Path $root 'evidence.json'; Set-Content $owned 'dirty'
$begin = Run @('-Operation','Begin','-RepositoryRoot',$root,'-EvidencePath',$store,'-LaneId','GOVERNANCE','-SessionId','H2','-WorkspaceIdentity','temp','-OwnedFiles','owned.txt','-ConsumedDependencies','dependency.txt','-TestCommand','selftest','-TestScope','H2','-EvidenceClass','AUTOMATED','-ProductAcceptanceState','NOT_REQUIRED')
Set-Content $owned 'dirty'; $complete = Run @('-Operation','Complete','-RepositoryRoot',$root,'-EvidencePath',$store,'-EvidenceId',$begin.EvidenceId,'-TestResult','PASS')
Assert ($complete.EvidenceStatus -eq 'VALID') 'same candidate must be VALID'
Set-Content $owned 'two'; $stale = Run @('-Operation','Validate','-RepositoryRoot',$root,'-EvidencePath',$store,'-EvidenceId',$begin.EvidenceId)
Assert ($stale.EvidenceStatus -eq 'STALE') 'dirty content must be STALE'; Assert ($stale.Reason -eq 'DIRTY_CONTENT_CHANGED') 'dirty reason mismatch'
Set-Content $owned 'one'; $begin2 = Run @('-Operation','Begin','-RepositoryRoot',$root,'-EvidencePath',$store,'-LaneId','GOVERNANCE','-SessionId','H2B','-WorkspaceIdentity','temp','-OwnedFiles','owned.txt','-ConsumedDependencies','dependency.txt','-TestCommand','selftest','-TestScope','H2','-EvidenceClass','AUTOMATED','-ProductAcceptanceState','NOT_REQUIRED')
Set-Content $owned 'during'; $during = Run @('-Operation','Complete','-RepositoryRoot',$root,'-EvidencePath',$store,'-EvidenceId',$begin2.EvidenceId,'-TestResult','PASS')
Assert ($during.EvidenceStatus -eq 'INVALID') 'test-time candidate change must be INVALID'; Assert ($during.Reason -eq 'CANDIDATE_TREE_CHANGED') 'candidate change reason mismatch'
$foreignBegin = Run @('-Operation','Begin','-RepositoryRoot',$root,'-EvidencePath',$store,'-LaneId','GOVERNANCE','-SessionId','H2C','-WorkspaceIdentity','temp','-OwnedFiles','owned.txt','-ConsumedDependencies','dependency.txt','-TestCommand','selftest','-TestScope','H2','-EvidenceClass','AUTOMATED','-ProductAcceptanceState','NOT_REQUIRED')
Set-Content (Join-Path $root 'foreign.txt') 'foreign-2'; $foreign = Run @('-Operation','Validate','-RepositoryRoot',$root,'-EvidencePath',$store,'-EvidenceId',$foreignBegin.EvidenceId)
Assert ($foreign.EvidenceStatus -eq 'VALID') ("irrelevant ForeignDirty must not invalidate evidence: " + ($foreign | ConvertTo-Json -Depth 5 -Compress))
Set-Content $dependency 'dep-2'; $dep = Run @('-Operation','Validate','-RepositoryRoot',$root,'-EvidencePath',$store,'-EvidenceId',$foreignBegin.EvidenceId)
Assert ($dep.EvidenceStatus -eq 'STALE') 'dependency content must be STALE'; Assert ($dep.Reason -eq 'DEPENDENCY_CHANGED') 'dependency reason mismatch'
$ff = Run @('-Operation','Begin','-RepositoryRoot',$root,'-EvidencePath',$store,'-LaneId','GOVERNANCE','-SessionId','H2FF','-WorkspaceIdentity','temp','-OwnedFiles','owned.txt','-ConsumedDependencies','dependency.txt','-TestCommand','selftest','-TestScope','H2','-EvidenceClass','AUTOMATED','-ProductAcceptanceState','NOT_REQUIRED')
Set-Content (Join-Path $root 'fast-forward.txt') 'ff'; Invoke-TestGit @('add','fast-forward.txt'); Invoke-TestGit @('commit','-qm','fast-forward')
$ffResult = Run @('-Operation','Validate','-RepositoryRoot',$root,'-EvidencePath',$store,'-EvidenceId',$ff.EvidenceId)
Assert ($ffResult.EvidenceStatus -eq 'STALE') 'HEAD fast-forward must stale evidence'; Assert ($ffResult.Reason -eq 'HEAD_CHANGED') 'HEAD fast-forward reason mismatch'
$unknown = Join-Path $root 'unknown.txt'; Set-Content $unknown 'unknown'; $unknownPacket = Run @('-Operation','Begin','-RepositoryRoot',$root,'-EvidencePath',$store,'-LaneId','GOVERNANCE','-SessionId','H2D','-WorkspaceIdentity','temp','-OwnedFiles','owned.txt','-ConsumedDependencies','unknown.txt','-TestCommand','selftest','-TestScope','H2','-EvidenceClass','AUTOMATED','-ProductAcceptanceState','NOT_REQUIRED')
Set-Content $unknown 'unknown-2'; $unknownResult = Run @('-Operation','Complete','-RepositoryRoot',$root,'-EvidencePath',$store,'-EvidenceId',$unknownPacket.EvidenceId,'-TestResult','PASS')
Assert ($unknownResult.EvidenceStatus -eq 'INVALID') 'consumed UnknownDirty must invalidate candidate'
$manual = Run @('-Operation','ManualInvalidate','-RepositoryRoot',$root,'-EvidencePath',$store,'-EvidenceId',$foreignBegin.EvidenceId)
Assert ($manual.EvidenceStatus -eq 'INVALID') 'manual invalidation must be INVALID'
$savedPreference = $ErrorActionPreference; $ErrorActionPreference = 'Continue'; $reuse = @(& powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $module -Operation Reuse -RepositoryRoot $root -EvidencePath $store -EvidenceId $foreignBegin.EvidenceId 2>&1); $reuseCode = $LASTEXITCODE; $ErrorActionPreference = $savedPreference
Assert ($reuseCode -ne 0) 'old evidence reuse must be rejected'
Write-Output 'H2 EVIDENCE SELFTEST: PASS'
Write-Output 'Cases: 9'
