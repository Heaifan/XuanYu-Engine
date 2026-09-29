[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$scriptPath = Join-Path $PSScriptRoot 'handoff.ps1'
$root = Join-Path $env:TEMP ('xye-handoff-lifecycle-' + [guid]::NewGuid().ToString('N'))
$remote = Join-Path (Split-Path $root) ((Split-Path $root -Leaf) + '-remote.git')
$peer = Join-Path (Split-Path $root) ((Split-Path $root -Leaf) + '-peer')

function Assert-True([bool]$Condition, [string]$Message) {
    if (-not $Condition) { throw "STATE LIFECYCLE SELF TEST FAILED: $Message" }
}

function Invoke-Handoff([string[]]$Arguments) {
    $shellName = if ($PSVersionTable.PSEdition -eq 'Core') { 'pwsh.exe' } else { 'powershell.exe' }
    $shell = (Get-Command $shellName -ErrorAction Stop).Source
    $output = @(& $shell -NoLogo -NoProfile -ExecutionPolicy Bypass -File $scriptPath @Arguments 2>&1)
    [pscustomobject]@{ ExitCode = $LASTEXITCODE; Text = ($output -join [Environment]::NewLine) }
}

function Assert-Output($Result, [int]$ExitCode, [string]$Text) {
    Assert-True ($Result.ExitCode -eq $ExitCode) "expected exit $ExitCode, got $($Result.ExitCode): $($Result.Text)"
    Assert-True $Result.Text.Contains($Text) "expected '$Text' in: $($Result.Text)"
}

function Write-State($State) {
    $dir = Join-Path $root '.git\xye-handoff'
    New-Item -ItemType Directory -Path $dir -Force | Out-Null
    $State | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath (Join-Path $dir 'state.json')
}

function New-State([bool]$Active, [string]$Mode, [string]$Branch, [string]$Baseline, [string]$Source, [string]$Target) {
    [pscustomobject]@{
        branch = $Branch
        baselineHead = $Baseline
        workspace = $root
        active = $Active
        mode = $Mode
        sourceBranch = $Source
        targetBranch = $Target
        createdAt = '2026-09-29T00:00:00Z'
        expiryCondition = 'CurrentBranch==targetBranch && HEAD==origin/targetBranch && Ahead/Behind==0/0'
        coordinatorScope = $null
    }
}

try {
    New-Item -ItemType Directory -Path $root -Force | Out-Null
    git -C $root init -b main | Out-Null
    git -C $root config user.email 'handoff-lifecycle@example.invalid'
    git -C $root config user.name 'handoff-lifecycle'
    Set-Content -LiteralPath (Join-Path $root 'README.md') -Value 'A'
    git -C $root add README.md
    git -C $root commit -m A | Out-Null
    git init --bare $remote | Out-Null
    git -C $root remote add origin $remote
    git -C $root push -u origin main | Out-Null

    $dotnet = Join-Path $root 'dotnet.cmd'
    Set-Content -LiteralPath $dotnet -Value '@echo 9.9.9-lifecycle'
    Set-Content -LiteralPath (Join-Path $root 'resolve-dotnet.ps1') -Value "Write-Output '$dotnet'"
    Set-Content -LiteralPath (Join-Path $root 'bootstrap.ps1') -Value ''
    @"
@{
    Remote = 'origin'
    CanonicalWorkspaces = @('$root')
    PreferredDotnetByDrive = @{}
    ResolverScript = 'resolve-dotnet.ps1'
    BootstrapScript = 'bootstrap.ps1'
    RemoteWinsWhenBehind = `$true
    LegacyWorktreesBlockHandoff = `$false
}
"@ | Set-Content -LiteralPath (Join-Path $root 'HandoffConfig.psd1')
    git -C $root add HandoffConfig.psd1 bootstrap.ps1 resolve-dotnet.ps1 dotnet.cmd
    git -C $root commit -m fixture-config | Out-Null
    git -C $root push | Out-Null
    git -C $root branch wip
    git -C $root push -u origin wip | Out-Null

    Set-Content -LiteralPath (Join-Path $root 'README.md') -Value 'B'
    git -C $root add README.md
    git -C $root commit -m B | Out-Null
    git -C $root push | Out-Null
    git -C $root fetch origin | Out-Null
    $headB = (git -C $root rev-parse HEAD).Trim()
    $headA = (git -C $root rev-parse HEAD~1).Trim()

    Write-State (New-State $true 'WIP_RESUME' 'wip' $headA 'wip' 'main')
    Assert-Output (Invoke-Handoff @('-Mode', 'join', '-Scope', 'governance', '-RepositoryRoot', $root, '-AllowTestWorkspace')) 0 'STATELESS / READY'
    Assert-Output (Invoke-Handoff @('-Mode', 'prepare', '-Scope', 'governance', '-RepositoryRoot', $root, '-AllowTestWorkspace')) 0 'HANDOFF PREPARE PASS'
    foreach ($scope in @('xye', 'xyui', 'governance')) {
        Assert-Output (Invoke-Handoff @('-Mode', 'join', '-Scope', $scope, '-RepositoryRoot', $root, '-AllowTestWorkspace')) 0 'HANDOFF JOIN PASS'
    }

    Write-State (New-State $false 'development' 'wip' $headA 'wip' 'main')
    Assert-Output (Invoke-Handoff @('-Mode', 'join', '-RepositoryRoot', $root, '-AllowTestWorkspace')) 0 'STATELESS / READY'

    Set-Content -LiteralPath (Join-Path $root 'ahead.txt') -Value 'local'
    git -C $root add ahead.txt
    git -C $root commit -m local-ahead | Out-Null
    Assert-Output (Invoke-Handoff @('-Mode', 'prepare', '-RepositoryRoot', $root, '-AllowTestWorkspace')) 1 'LOCAL_COMMITS_EXIST'
    git -C $root push | Out-Null
    $headAhead = (git -C $root rev-parse HEAD).Trim()

    git clone -b main $remote $peer | Out-Null
    git -C $peer config user.email 'handoff-peer@example.invalid'
    git -C $peer config user.name 'handoff-peer'
    Set-Content -LiteralPath (Join-Path $peer 'peer.txt') -Value 'remote'
    git -C $peer add peer.txt
    git -C $peer commit -m remote-only | Out-Null
    git -C $peer push | Out-Null
    git -C $root fetch origin | Out-Null
    Write-State (New-State $false 'development' 'main' $headAhead 'wip' 'main')
    Assert-Output (Invoke-Handoff @('-Mode', 'prepare', '-RepositoryRoot', $root, '-AllowTestWorkspace')) 0 'HANDOFF PREPARE PASS'
    $remoteWinsCount = ((git -C $root rev-list --left-right --count 'HEAD...origin/main') -join ' ').Trim()
    Assert-True ($remoteWinsCount -replace '\s+', ' ' -eq '0 0') "remote-wins prepare did not converge: $remoteWinsCount"

    git -C $root switch wip | Out-Null
    $wipHead = (git -C $root rev-parse HEAD).Trim()
    Write-State (New-State $true 'WIP_RESUME' 'wip' $wipHead 'wip' 'main')
    Assert-Output (Invoke-Handoff @('-Mode', 'join', '-RepositoryRoot', $root, '-AllowTestWorkspace')) 0 'HandoffMode: WIP_RESUME'
    git -C $root switch main | Out-Null
    Write-State (New-State $true 'WIP_RESUME' 'wip' $wipHead 'wip' 'main')
    Assert-Output (Invoke-Handoff @('-Mode', 'join', '-RepositoryRoot', $root, '-AllowTestWorkspace')) 0 'STATELESS / READY'

    Set-Content -LiteralPath (Join-Path $root 'dirty.txt') -Value 'dirty'
    Assert-Output (Invoke-Handoff @('-Mode', 'maintenance', '-Scope', 'governance', '-RepositoryRoot', $root, '-AllowTestWorkspace')) 1 'MAINTENANCE_DIRTY'
    Remove-Item -LiteralPath (Join-Path $root 'dirty.txt') -Force
    Assert-Output (Invoke-Handoff @('-Mode', 'maintenance', '-Scope', 'governance', '-RepositoryRoot', $root, '-AllowTestWorkspace')) 0 'HANDOFF MAINTENANCE PASS'
    Write-Host 'HANDOFF STATE LIFECYCLE SELF TEST PASS'
}
finally {
    if (Test-Path -LiteralPath $root) { Remove-Item -LiteralPath $root -Recurse -Force }
    if (Test-Path -LiteralPath $remote) { Remove-Item -LiteralPath $remote -Recurse -Force }
    if (Test-Path -LiteralPath $peer) { Remove-Item -LiteralPath $peer -Recurse -Force }
}

exit 0
