$ErrorActionPreference = 'Stop'

$repo = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$workspaceModule = Join-Path $repo 'scripts/XyeWorkspace.psm1'
Import-Module $workspaceModule -Force -DisableNameChecking
$previousWorkspaceRoot = [Environment]::GetEnvironmentVariable('XYE_WORKSPACE_ROOT', 'Process')
[Environment]::SetEnvironmentVariable('XYE_WORKSPACE_ROOT', $null, 'Process')
$workspace = Get-XyeWorkspaceRoot
Assert-XyeWorkspaceBoundary $workspace $repo
$repoRoot = [IO.Path]::GetFullPath($repo).TrimEnd('\') + '\'
if (-not [IO.Path]::GetFullPath($workspace).StartsWith($repoRoot, [StringComparison]::OrdinalIgnoreCase)) {
    throw "Default workspace escaped repository root: $workspace"
}
[Environment]::SetEnvironmentVariable('XYE_WORKSPACE_ROOT', $workspace, 'Process')
$tempRoot = Join-Path $workspace ('Temp/arch-a-guard-selftest-' + [guid]::NewGuid().ToString('N'))
Assert-XyeChildPath $workspace $tempRoot
New-Item -ItemType Directory -Force -Path $tempRoot | Out-Null
$script:TempRoot = $tempRoot
$bootstrap = Join-Path $repo 'scripts/architecture/guard-bootstrap.ps1'
. $bootstrap
Initialize-GuardBootstrap -Root $repo -NewRun | Out-Null
$main = Join-Path $repo 'scripts/arch-a-guard.ps1'
$children = Get-ChildItem (Join-Path $repo 'scripts/arch-a-guard-*.ps1') -File |
    Where-Object { $_.Name -ne 'arch-a-guard-viewport-helpers.ps1' }

function Assert-True([bool]$condition, [string]$message) {
    if (!$condition) { throw $message }
}

function Invoke-GuardProcess([string]$exe, [string]$scriptPath, [switch]$CaptureOutput) {
    $out = Join-Path $script:TempRoot ('guard-out-' + [guid]::NewGuid().ToString('N'))
    $err = Join-Path $script:TempRoot ('guard-err-' + [guid]::NewGuid().ToString('N'))
    try {
        $process = Start-Process -FilePath $exe -ArgumentList @(
            '-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', $scriptPath) -Wait -PassThru -WindowStyle Hidden -RedirectStandardOutput $out -RedirectStandardError $err
        if ($CaptureOutput) {
            return [pscustomobject]@{
                ExitCode=$process.ExitCode
                Output=(Get-Content -LiteralPath $out -Raw -ErrorAction SilentlyContinue)
                Error=(Get-Content -LiteralPath $err -Raw -ErrorAction SilentlyContinue)
            }
        }
        return $process.ExitCode
    }
    finally {
        Remove-Item -LiteralPath $out, $err -Force -ErrorAction SilentlyContinue
    }
}

$fixtureProject = Join-Path $tempRoot 'project-references.csproj'
Set-Content -LiteralPath $fixtureProject -Encoding utf8 -Value @'
<Project><ItemGroup><InternalsVisibleTo Include="XuanYu.Render.Vulkan" /></ItemGroup></Project>
'@
$friendOnlyReferences = @(Get-ProjectReferences $fixtureProject)
Assert-True ($friendOnlyReferences.Count -eq 0) 'InternalsVisibleTo was classified as a ProjectReference.'
Set-Content -LiteralPath $fixtureProject -Encoding utf8 -Value @'
<Project><ItemGroup><InternalsVisibleTo Include="XuanYu.Render.Vulkan" /><ProjectReference Include="..\XuanYu.Render.Vulkan\XuanYu.Render.Vulkan.csproj" /></ItemGroup></Project>
'@
$withProjectReference = @(Get-ProjectReferences $fixtureProject)
Assert-True ($withProjectReference.Count -eq 1 -and $withProjectReference[0] -match 'XuanYu\.Render\.Vulkan') 'A real Vulkan ProjectReference was not classified.'

Assert-True (Test-Path $bootstrap) 'Shared Guard Bootstrap is missing.'
$bootstrapText = Get-Content -LiteralPath $bootstrap -Raw -Encoding utf8
foreach ($name in @('Get-SourceFiles', 'Read-Text', 'Get-ProjectReferences',
        'Assert-Contains', 'Assert-NotContains', 'Add-Failure')) {
    Assert-True ($bootstrapText -match "function\s+$name\b") "Bootstrap function missing: $name"
}

foreach ($guard in @($main) + @($children.FullName)) {
    $text = Get-Content -LiteralPath $guard -Raw -Encoding utf8
    Assert-True ($text.Contains('guard-bootstrap.ps1')) "Guard does not explicitly load Bootstrap: $guard"
}

$child = Join-Path $repo 'scripts/arch-a-guard-editor.ps1'
$ps51 = Get-Command powershell.exe -ErrorAction SilentlyContinue
$pwsh = Get-Command pwsh.exe -ErrorAction SilentlyContinue
Assert-True ($null -ne $ps51) 'PowerShell 5.1 executable is unavailable.'
Assert-True ($null -ne $pwsh) 'pwsh executable is unavailable.'

foreach ($shell in @($ps51.Source, $pwsh.Source)) {
    $exitCode = Invoke-GuardProcess $shell $child
    Assert-True ($exitCode -eq 0) "Child direct-run failed under $shell."
}

$mainRun = Invoke-GuardProcess $pwsh.Source $main -CaptureOutput
$mainText = Get-Content -LiteralPath $main -Raw -Encoding utf8
Assert-True ($mainText.Contains('Get-ProjectReferences $absCsproj')) 'Abstractions boundary check does not inspect ProjectReference entries.'
Assert-True (!(($mainRun.Output + $mainRun.Error) -match 'Render\.Abstractions project reference forbidden')) 'Arch-A rejected the legal Render.Abstractions friend assembly declaration.'

$fixture = Join-Path $tempRoot ('guard-propagation-' + [guid]::NewGuid().ToString('N'))
$fixtureChild = Join-Path $fixture 'child.ps1'
$fixtureMain = Join-Path $fixture 'main.ps1'
try {
    New-Item -ItemType Directory -Path $fixture -Force | Out-Null
    Set-Content -LiteralPath $fixtureChild -Encoding utf8 -Value @(
        ". '$bootstrap'"
        "Initialize-GuardBootstrap -Root '$repo' | Out-Null"
        "Add-Failure 'manufactured child failure'"
        "Complete-GuardRun 'child unexpectedly passed'"
    )
    Set-Content -LiteralPath $fixtureMain -Encoding utf8 -Value @(
        ". '$bootstrap'"
        "Initialize-GuardBootstrap -Root '$repo' -NewRun | Out-Null"
        ". '$fixtureChild'"
        "Complete-GuardRun 'main unexpectedly passed'"
    )
    foreach ($entry in @($fixtureChild, $fixtureMain)) {
        $exitCode = Invoke-GuardProcess $pwsh.Source $entry
        Assert-True ($exitCode -ne 0) "Manufactured failure was swallowed by $entry."
    }
}
finally {
    Remove-Item -LiteralPath $fixture -Recurse -Force -ErrorAction SilentlyContinue
}

$versionResolver = Join-Path $repo 'scripts/resolve-version.ps1'
$version = (& $pwsh.Source -NoProfile -ExecutionPolicy Bypass -File $versionResolver).Trim()
$mainText = Get-Content -LiteralPath $main -Raw -Encoding utf8
Assert-True ($mainText.Contains('resolve-version.ps1')) 'Main Guard does not consume formal Version Resolver.'
Assert-True ($version -match '^v[0-9]+(\.[0-9]+)+-[a-z0-9.-]+$') "Formal Version Resolver returned invalid value: $version"
Assert-True (!$mainText.Contains('UiWin.axaml') -and !$mainText.Contains('changelog.md')) 'Main Guard reverse-checks UI/changelog version sources.'

Write-Host 'ARCH-A Guard Bootstrap selftest passed.'
Remove-Item -LiteralPath $tempRoot -Recurse -Force
[Environment]::SetEnvironmentVariable('XYE_WORKSPACE_ROOT', $previousWorkspaceRoot, 'Process')
