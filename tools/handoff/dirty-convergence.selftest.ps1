[CmdletBinding()]
param()

$ErrorActionPreference = 'Continue'
$scriptPath = Join-Path $PSScriptRoot 'handoff.ps1'

function Assert-Case([bool]$condition, [string]$message) {
    if (-not $condition) { throw "DIRTY CONVERGENCE SELFTEST FAILED: $message" }
}

function New-Fixture {
    $root = Join-Path $env:TEMP ('xye-dirty-convergence-' + [guid]::NewGuid().ToString('N'))
    $remote = Join-Path $env:TEMP ((Split-Path $root -Leaf) + '-remote.git')
    New-Item -ItemType Directory -Path $root -Force | Out-Null
    git -C $root init -b main 2>&1 | Out-Null
    git -C $root config user.email 'handoff-selftest@example.invalid' 2>&1 | Out-Null
    git -C $root config user.name 'handoff-selftest' 2>&1 | Out-Null
    New-Item -ItemType Directory -Path (Join-Path $root 'XuanYu.Editor.UIInput') -Force | Out-Null
    Set-Content -LiteralPath (Join-Path $root 'XuanYu.Editor.UIInputUiVmMapBackend.cs') -Value 'baseline'
    Set-Content -LiteralPath (Join-Path $root 'bootstrap.ps1') -Value ''
    $dotnet = Join-Path $root 'dotnet.cmd'
    Set-Content -LiteralPath $dotnet -Value '@echo 9.9.9-selftest'
    @"
@{
    Remote = 'origin'
    OwnershipManifest = 'ownership-manifest.json'
    CanonicalWorkspaces = @('$root')
    PreferredDotnetByDrive = @{}
    ResolverScript = 'resolve-dotnet.ps1'
    BootstrapScript = 'bootstrap.ps1'
}
"@ | Set-Content -LiteralPath (Join-Path $root 'HandoffConfig.psd1')
    @{
        version = 1
        entries = @(@{ path = 'XuanYu.Editor.UI/Input/UiVmMapBackend.cs'; owner = 'B' })
    } | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $root 'ownership-manifest.json')
    Set-Content -LiteralPath (Join-Path $root 'resolve-dotnet.ps1') -Value "Write-Output '$dotnet'"
    git -C $root add HandoffConfig.psd1 bootstrap.ps1 dotnet.cmd ownership-manifest.json resolve-dotnet.ps1 2>&1 | Out-Null
    git -C $root add . 2>&1 | Out-Null
    git -C $root commit -m fixture 2>&1 | Out-Null
    git init --bare $remote 2>&1 | Out-Null
    git -C $root remote add origin $remote 2>&1 | Out-Null
    git -C $root push -u origin main 2>&1 | Out-Null
    [pscustomobject]@{ Root = $root; Remote = $remote }
}

function Invoke-Handoff($fixture, [string[]]$arguments) {
    $shell = (Get-Command pwsh.exe -ErrorAction SilentlyContinue).Source
    if ([string]::IsNullOrWhiteSpace($shell)) { $shell = 'powershell.exe' }
    $args = @('-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', $scriptPath) + $arguments + @('-RepositoryRoot', $fixture.Root, '-AllowTestWorkspace')
    $output = @(& $shell @args 2>&1)
    [pscustomobject]@{ ExitCode = $LASTEXITCODE; Text = ($output -join [Environment]::NewLine) }
}

function Run-Case([string]$name, [scriptblock]$setup, [string]$mode, [int]$exitCode, [string]$expected) {
    $fixture = @(New-Fixture)[-1]
    $root = ([string]$fixture.Root).Trim()
    try {
        & $setup $root
        $result = Invoke-Handoff $fixture @('-Mode', 'prepare', '-WaveMode', $mode, '-CoordinatorScope', 'xye')
        Assert-Case ($result.ExitCode -eq $exitCode) "$name exit=$($result.ExitCode): $($result.Text)"
        Assert-Case ($result.Text.Contains($expected)) "$name missing '$expected': $($result.Text)"
    } finally {
        if (Test-Path $fixture.Root) { Remove-Item -LiteralPath $fixture.Root -Recurse -Force }
        if (Test-Path $fixture.Remote) { Remove-Item -LiteralPath $fixture.Remote -Recurse -Force }
    }
}

$known = 'XuanYu.Editor.UI\Input\UiVmMapBackend.cs'
Run-Case 'clean prepare' { param($root) } 'development' 0 'HANDOFF PREPARE PASS'
Run-Case 'owned dirty convergence' { param($root) New-Item -ItemType Directory (Join-Path $root 'XuanYu.Editor.UI\Input') -Force | Out-Null; Set-Content (Join-Path $root $known) 'owned' } 'convergence' 0 'HANDOFF PREPARE PASS'
Run-Case 'unauthorized dirty' { param($root) New-Item -ItemType Directory (Join-Path $root 'XuanYu.Editor.UI\Input') -Force | Out-Null; Set-Content (Join-Path $root 'XuanYu.Editor.UI\Input\Unknown.cs') 'unknown' } 'convergence' 1 'UNAUTHORIZED_DIRTY'
Run-Case 'foreign dirty' { param($root) New-Item -ItemType Directory (Join-Path $root 'xyui') -Force | Out-Null; Set-Content (Join-Path $root 'xyui\foreign.cs') 'foreign' } 'convergence' 1 'FOREIGN_DIRTY'
Run-Case 'unowned dirty' { param($root) New-Item -ItemType Directory (Join-Path $root 'XuanYu.Editor.UI') -Force | Out-Null; Set-Content (Join-Path $root 'XuanYu.Editor.UI\Unowned.cs') 'unowned' } 'convergence' 1 'UNAUTHORIZED_DIRTY'
Run-Case 'staged dirty' { param($root) New-Item -ItemType Directory (Join-Path $root 'XuanYu.Editor.UI\Input') -Force | Out-Null; Set-Content (Join-Path $root $known) 'staged'; git -C $root add $known 2>&1 | Out-Null } 'convergence' 1 'STAGED_DIRTY'
Run-Case 'ahead dirty' { param($root) New-Item -ItemType Directory (Join-Path $root 'XuanYu.Editor.UI\Input') -Force | Out-Null; Set-Content (Join-Path $root $known) 'ahead'; git -C $root add $known 2>&1 | Out-Null; git -C $root commit -m ahead 2>&1 | Out-Null } 'convergence' 1 'LOCAL_COMMITS_EXIST'
Run-Case 'ordinary dirty wave' { param($root) New-Item -ItemType Directory (Join-Path $root 'XuanYu.Editor.UI\Input') -Force | Out-Null; Set-Content (Join-Path $root $known) 'ordinary' } 'development' 1 'DIRTY_AT_REMOTE_TIP'

Write-Host 'DIRTY CONVERGENCE SELFTEST PASS'
