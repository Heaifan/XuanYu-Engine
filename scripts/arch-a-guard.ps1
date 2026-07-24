$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
Set-Location $root
$failures = New-Object System.Collections.Generic.List[string]

function Add-Failure([string]$message) { $failures.Add($message) }
function Read-Text([string]$path) { Get-Content -LiteralPath $path -Raw -Encoding utf8 }
function Assert-Contains([string]$path, [string]$needle, [string]$label) {
    if ((Read-Text $path).IndexOf($needle, [StringComparison]::OrdinalIgnoreCase) -lt 0) {
        Add-Failure "$label missing: $needle"
    }
}
function Assert-NotContains([string]$path, [string[]]$needles, [string]$label) {
    $text = Read-Text $path
    foreach ($needle in $needles) {
        if ($text.IndexOf($needle, [StringComparison]::OrdinalIgnoreCase) -ge 0) {
            Add-Failure "$label forbidden: $needle ($path)"
        }
    }
}
function Get-OutputType([string]$path) {
    $match = [regex]::Match((Read-Text $path), '<OutputType>(.*?)</OutputType>')
    if ($match.Success) { return $match.Groups[1].Value }
    return ""
}
function Get-SourceFiles([string]$dir) {
    git ls-files "$dir/*" | Where-Object { $_ -match '\.(cs|axaml|js)$' -and (Test-Path $_) } | ForEach-Object { Get-Item -LiteralPath $_ }
}
function Get-TrackedHandwrittenFiles {
    $tracked = git ls-files
    $untracked = git ls-files --others --exclude-standard
    @($tracked) + @($untracked) | Where-Object { $_ -match '\.(cs|axaml|js|ps1)$' -and (Test-Path $_) } | ForEach-Object { Get-Item -LiteralPath $_ }
}

$uiCsproj = "XuanYu.Editor.UI/XuanYu.Editor.UI.csproj"
$appCsproj = "XuanYu.Editor.App/XuanYu.Editor.App.csproj"
$absCsproj = "XuanYu.Render.Abstractions/XuanYu.Render.Abstractions.csproj"

Assert-NotContains $uiCsproj @("XuanYu.Render.Vulkan", "Silk.NET.Vulkan") "Editor.UI project reference"
foreach ($file in Get-SourceFiles "XuanYu.Editor.UI") {
    Assert-NotContains $file.FullName @("using XuanYu.Render.Vulkan", "using Silk.NET.Vulkan") "Editor.UI source reference"
}

Assert-NotContains $absCsproj @("XuanYu.Render.Vulkan", "Silk.NET.Vulkan", "Avalonia") "Render.Abstractions project reference"
foreach ($file in Get-SourceFiles "XuanYu.Render.Abstractions") {
    Assert-NotContains $file.FullName @("using XuanYu.Render.Vulkan", "using Silk.NET.Vulkan", "using Avalonia") "Render.Abstractions source reference"
}

Assert-Contains $appCsproj "XuanYu.Editor.UI.csproj" "Editor.App composes UI"
Assert-Contains $appCsproj "XuanYu.Render.Vulkan.csproj" "Editor.App composes Vulkan"
Assert-Contains "run.bat" "XuanYu.Editor.App\XuanYu.Editor.App.csproj" "run.bat startup project"

if ((Get-OutputType $appCsproj) -ne "WinExe") { Add-Failure "Editor.App must be WinExe" }

$projects = @(
    "XuanYu.Core/XuanYu.Core.csproj",
    "XuanYu.Core.Tests/XuanYu.Core.Tests.csproj",
    "XuanYu.World/XuanYu.World.csproj",
    "XuanYu.World.Tests/XuanYu.World.Tests.csproj",
    "XuanYu.Editor.App/XuanYu.Editor.App.csproj",
    "XuanYu.Editor.UI/XuanYu.Editor.UI.csproj",
    "XuanYu.Editor.Win/XuanYu.Editor.Win.csproj",
    "XuanYu.Render.Abstractions/XuanYu.Render.Abstractions.csproj",
    "XuanYu.Render.Vulkan/XuanYu.Render.Vulkan.csproj"
)
$slnx = Read-Text "XuanYu.Engine.slnx"
foreach ($project in $projects) {
    if ($slnx.IndexOf($project, [StringComparison]::OrdinalIgnoreCase) -lt 0) {
        Add-Failure "solution missing project: $project"
    }
    if ($project -ne "XuanYu.Editor.App/XuanYu.Editor.App.csproj") {
        $outputType = Get-OutputType $project
        if ($outputType -eq "Exe" -or $outputType -eq "WinExe") {
            Add-Failure "only Editor.App may be executable: $project is $outputType"
        }
    }
}

$changelog = Read-Text "changelog.md"
$versionPattern = 'v0\.[0-9]+\.[0-9]+\.[0-9]+-(rz|fix|vk)'
$versionMatch = [regex]::Match($changelog, "(?m)^##\s+($versionPattern)")
if (!$versionMatch.Success) { Add-Failure "changelog top version missing" }
else {
    $version = $versionMatch.Groups[1].Value
    if ($version -notmatch "^$versionPattern$") { Add-Failure "invalid development version: $version" }
    Assert-Contains "XuanYu.Editor.UI/Win/UiWin.axaml" $version "main window title version"
    Assert-Contains "run.bat" $version "run.bat title version"
}

# ARCH-WORLD red-line guards live in a separate file (5+100 split).
. "$PSScriptRoot/arch-a-guard-world.ps1"

foreach ($file in Get-TrackedHandwrittenFiles) {
    $lines = (Get-Content -LiteralPath $file.FullName -Encoding utf8 | Measure-Object -Line).Lines
    if ($lines -gt 100) { Add-Failure "5+100 exceeded: $lines lines $($file.FullName)" }
}

if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Error $_ }
    exit 1
}

Write-Host "ARCH-A guard passed."
