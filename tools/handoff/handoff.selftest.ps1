[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$scriptPath = Join-Path $PSScriptRoot 'handoff.ps1'
$root = Join-Path $env:TEMP ('xye-handoff-selftest-' + [guid]::NewGuid().ToString('N'))
$remote = Join-Path (Split-Path $root) ((Split-Path $root -Leaf) + '-remote.git')

function Assert-True([bool]$Condition, [string]$Message) {
    if (-not $Condition) { throw "SELF TEST FAILED: $Message" }
}

function Invoke-Handoff([string[]]$Arguments) {
    $shell = (Get-Command pwsh.exe -ErrorAction SilentlyContinue).Source
    if ([string]::IsNullOrWhiteSpace($shell)) { $shell = 'powershell.exe' }
    $output = @(& $shell -NoProfile -ExecutionPolicy Bypass -File $scriptPath @Arguments 2>&1)
    [pscustomobject]@{ ExitCode = $LASTEXITCODE; Text = ($output -join [Environment]::NewLine) }
}

function Assert-Output($Result, [int]$ExitCode, [string]$Text) {
    Assert-True ($Result.ExitCode -eq $ExitCode) "expected exit $ExitCode, got $($Result.ExitCode): $($Result.Text)"
    Assert-True $Result.Text.Contains($Text) "expected '$Text' in: $($Result.Text)"
}

try {
    New-Item -ItemType Directory -Path $root -Force | Out-Null
    git -C $root init -b main | Out-Null
    git -C $root config user.email 'handoff-selftest@example.invalid'
    git -C $root config user.name 'handoff-selftest'
    Set-Content -LiteralPath (Join-Path $root 'README.md') -Value 'fixture'
    git -C $root add README.md
    git -C $root commit -m fixture | Out-Null
    git init --bare $remote | Out-Null
    git -C $root remote add origin $remote
    git -C $root push -u origin main | Out-Null

    $dotnet = Join-Path $root 'dotnet.cmd'
    Set-Content -LiteralPath $dotnet -Value '@echo 9.9.9-selftest'
    Set-Content -LiteralPath (Join-Path $root 'resolve-dotnet.ps1') -Value "Write-Output '$dotnet'"
    Set-Content -LiteralPath (Join-Path $root 'bootstrap.ps1') -Value ''
    @"
@{
    Remote = 'origin'
    OwnershipManifest = 'tools\handoff\ownership-manifest.json'
    CanonicalWorkspaces = @('$root')
    PreferredDotnetByDrive = @{}
    ResolverScript = 'resolve-dotnet.ps1'
    BootstrapScript = 'bootstrap.ps1'
}
"@ | Set-Content -LiteralPath (Join-Path $root 'HandoffConfig.psd1')

    git -C $root add HandoffConfig.psd1 bootstrap.ps1 resolve-dotnet.ps1 dotnet.cmd
    git -C $root commit -m fixture-config | Out-Null
    git -C $root push | Out-Null
    $head = (git -C $root rev-parse HEAD).Trim()
    $stateDir = Join-Path $root '.git\xye-handoff'
    $result = Invoke-Handoff @('-Mode', 'prepare', '-WaveMode', 'development', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 0 'HANDOFF PREPARE PASS'
    $stateBefore = Get-Content -Raw -LiteralPath (Join-Path $stateDir 'state.json')
    Assert-True $stateBefore.Contains('"coordinatorScope": null') 'prepare must write coordinatorScope null'
    $state = Get-Content -Raw -LiteralPath (Join-Path $stateDir 'state.json') | ConvertFrom-Json
    Assert-True ($null -eq $state.coordinatorScope) 'prepare coordinatorScope must be null'
    Assert-True ($state.mode -eq 'development') 'prepare must create ordinary development mode'
    Assert-True ($null -eq $state.PSObject.Properties['convergenceTargetBranch']) 'prepare must not persist stale convergence target'

    $result = Invoke-Handoff @('-Mode', 'advance', '-Scope', 'xye', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 0 'ADVANCE NOOP: BASELINE_ALREADY_CURRENT'
    Assert-True ((Get-Content -Raw -LiteralPath (Join-Path $stateDir 'state.json')) -eq $stateBefore) 'advance noop changed state.json'

    New-Item -ItemType Directory -Path (Join-Path $root 'xyui') -Force | Out-Null
    New-Item -ItemType Directory -Path (Join-Path $root 'XuanYu.World') -Force | Out-Null
    Set-Content -LiteralPath (Join-Path $root 'xyui\dirty.txt') -Value 'xyui'
    Set-Content -LiteralPath (Join-Path $root 'XuanYu.World\dirty.txt') -Value 'xye'
    $statusBefore = @(git -C $root status --porcelain=v1 --untracked-files=all)

    $result = Invoke-Handoff @('-Mode', 'join', '-Scope', 'xyui', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 0 'HANDOFF JOIN PASS'
    Assert-Output $result 0 'OwnDirty'
    Assert-Output $result 0 'ForeignDirty'
    Assert-Output $result 0 'Scope       : xyui'
    Assert-True ((Get-Content -Raw -LiteralPath (Join-Path $stateDir 'state.json')) -eq $stateBefore) 'join changed state.json'

    $result = Invoke-Handoff @('-Mode', 'join', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 0 'Scope       : xye'
    Assert-True (@(git -C $root status --porcelain=v1 --untracked-files=all) -join "`n" -eq ($statusBefore -join "`n")) 'join changed dirty files'
    Assert-True ((git -C $root rev-parse HEAD).Trim() -eq $head) 'join changed HEAD'

    $result = Invoke-Handoff @('-Mode', 'commit-lock', '-Scope', 'xye', '-Owner', 'agent-a', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 0 'HANDOFF COMMIT-LOCK PASS'
    $result = Invoke-Handoff @('-Mode', 'commit-unlock', '-Scope', 'xye', '-Owner', 'agent-a', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 0 'HANDOFF COMMIT-UNLOCK PASS'

    $state.coordinatorScope = ''
    $state | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $stateDir 'state.json')
    Set-Content -LiteralPath (Join-Path $root 'committed.txt') -Value 'B'
    git -C $root add committed.txt
    git -C $root commit -m B | Out-Null
    git -C $root push | Out-Null
    $headB = (git -C $root rev-parse HEAD).Trim()
    $result = Invoke-Handoff @('-Mode', 'commit-lock', '-Scope', 'xye', '-Owner', 'agent-a', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 0 'HANDOFF COMMIT-LOCK PASS'
    $result = Invoke-Handoff @('-Mode', 'commit-lock', '-Scope', 'xye', '-Owner', 'agent-b', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 1 'COMMIT_MUTEX_HELD'
    $result = Invoke-Handoff @('-Mode', 'join', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 0 'HANDOFF JOIN PASS'
    $result = Invoke-Handoff @('-Mode', 'prepare', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 1 'ACTIVE_WAVE_EXISTS'
    Assert-Output $result 1 'Current wave does not require a new prepare.'
    $result = Invoke-Handoff @('-Mode', 'advance', '-Scope', 'xye', '-Owner', 'agent-b', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 1 'COMMIT_MUTEX_OWNER_MISMATCH'
    $result = Invoke-Handoff @('-Mode', 'advance', '-Scope', 'xye', '-Owner', 'agent-a', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 0 'HANDOFF ADVANCE PASS'
    $stateAfterB = Get-Content -Raw -LiteralPath (Join-Path $stateDir 'state.json') | ConvertFrom-Json
    Assert-True ($stateAfterB.baselineHead -eq $headB) 'advance did not move baseline to B'
    Assert-True ($null -ne $stateAfterB.baselineAdvance) 'advance audit missing'
    Assert-True ($null -eq $stateAfterB.coordinatorScope) 'legacy empty coordinatorScope must normalize to null'
    Assert-True ((Get-Content -Raw -LiteralPath (Join-Path $stateDir 'state.json')) -match '"coordinatorScope"\s*:\s*null') 'advance must write coordinatorScope null'
    Assert-Output (Invoke-Handoff @('-Mode', 'join', '-RepositoryRoot', $root, '-AllowTestWorkspace')) 0 'HANDOFF JOIN PASS'

    Set-Content -LiteralPath (Join-Path $root 'committed.txt') -Value 'C'
    git -C $root add committed.txt
    git -C $root commit -m C | Out-Null
    git -C $root push | Out-Null
    $headC = (git -C $root rev-parse HEAD).Trim()
    $result = Invoke-Handoff @('-Mode', 'join', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 0 'HANDOFF JOIN PASS'
    $result = Invoke-Handoff @('-Mode', 'commit-lock', '-Scope', 'xye', '-Owner', 'agent-a', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 0 'HANDOFF COMMIT-LOCK PASS'
    $result = Invoke-Handoff @('-Mode', 'advance', '-Scope', 'xye', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 1 'COMMIT_MUTEX_REQUIRED'
    $result = Invoke-Handoff @('-Mode', 'advance', '-Scope', 'xye', '-Owner', 'agent-a', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 0 "New Baseline: $headC"
    Assert-Output (Invoke-Handoff @('-Mode', 'join', '-RepositoryRoot', $root, '-AllowTestWorkspace')) 0 'HANDOFF JOIN PASS'

    Set-Content -LiteralPath (Join-Path $root 'un pushed.txt') -Value 'ahead'
    git -C $root add 'un pushed.txt'
    git -C $root commit -m local-ahead | Out-Null
    $result = Invoke-Handoff @('-Mode', 'commit-lock', '-Scope', 'xye', '-Owner', 'agent-a', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 0 'HANDOFF COMMIT-LOCK PASS'
    $result = Invoke-Handoff @('-Mode', 'commit-unlock', '-Scope', 'xye', '-Owner', 'agent-a', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 1 'COMMIT_MUTEX_UNADVANCED'
    $result = Invoke-Handoff @('-Mode', 'advance', '-Scope', 'xye', '-Owner', 'agent-a', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 1 'REMOTE_DIVERGED'
    git -C $root push | Out-Null

    $stateAfterC = Get-Content -Raw -LiteralPath (Join-Path $stateDir 'state.json') | ConvertFrom-Json
    $stateAfterC.branch = 'wrong-branch'
    $stateAfterC | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $stateDir 'state.json')
    $result = Invoke-Handoff @('-Mode', 'advance', '-Scope', 'xye', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 1 'BRANCH_MISMATCH'
    $stateAfterC.branch = 'main'
    $stateAfterC.baselineHead = (git -C $root rev-list --max-parents=0 HEAD).Trim()
    $stateAfterC.coordinatorScope = '   '
    $stateAfterC | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $stateDir 'state.json')
    $result = Invoke-Handoff @('-Mode', 'advance', '-Scope', 'xye', '-Owner', 'agent-a', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 0 'HANDOFF ADVANCE PASS'
    Assert-True ((Get-Content -Raw -LiteralPath (Join-Path $stateDir 'state.json')) -match '"coordinatorScope"\s*:\s*null') 'whitespace coordinatorScope must write null'

    $statusHead = (git -C $root rev-parse HEAD).Trim()
    $statusDirty = @(git -C $root status --porcelain=v1 --untracked-files=all)
    $statusState = Get-Content -Raw -LiteralPath (Join-Path $stateDir 'state.json')
    $result = Invoke-Handoff @('-Mode', 'status', '-Scope', 'xye', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 0 'HANDOFF STATUS PASS'
    Assert-True ((git -C $root rev-parse HEAD).Trim() -eq $statusHead) 'status changed HEAD'
    Assert-True (@(git -C $root status --porcelain=v1 --untracked-files=all) -join "`n" -eq ($statusDirty -join "`n")) 'status changed dirty files'
    Assert-True ((Get-Content -Raw -LiteralPath (Join-Path $stateDir 'state.json')) -eq $statusState) 'status changed state.json'

    $state = Get-Content -Raw -LiteralPath (Join-Path $stateDir 'state.json') | ConvertFrom-Json
    $state.mode = 'convergence'; $state.coordinatorScope = 'xye'; $state.baselineHead = $statusHead
    $state | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $stateDir 'state.json')
    $result = Invoke-Handoff @('-Mode', 'advance', '-Scope', 'integration', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 1 'COORDINATOR_MISMATCH'
    $result = Invoke-Handoff @('-Mode', 'join', '-Scope', 'xyui', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 1 'CONVERGENCE_EXCLUSIVE'

    $state.mode = 'development'; $state.coordinatorScope = $null
    $state | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $stateDir 'state.json')
    $result = Invoke-Handoff @('-Mode', 'prepare', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 1 'ACTIVE_WAVE_EXISTS'
    $result = Invoke-Handoff @('-Mode', 'close', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 1 'DIRTY_ON_CLOSE'
    git -C $root clean -fd | Out-Null
    Assert-True (@(git -C $root status --porcelain=v1 --untracked-files=all).Count -eq 0) 'fixture must be clean before close regression'
    $result = Invoke-Handoff @('-Mode', 'close', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 0 'HANDOFF CLOSE PASS'
    $closedState = Get-Content -Raw -LiteralPath (Join-Path $stateDir 'state.json') | ConvertFrom-Json
    Assert-True (-not [bool]$closedState.active) 'close must deactivate the wave'
    Assert-True ($null -ne $closedState.closedAt) 'close must persist closedAt for legacy state'
    Write-Host 'HANDOFF SELF TEST PASS'
}
finally {
    if (Test-Path -LiteralPath $root) { Remove-Item -LiteralPath $root -Recurse -Force }
    if (Test-Path -LiteralPath $remote) { Remove-Item -LiteralPath $remote -Recurse -Force }
}

exit 0
