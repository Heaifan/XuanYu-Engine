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
    @($(git ls-files "$dir/*") + $(git ls-files --others --exclude-standard "$dir/*")) |
        Where-Object { $_ -match '\.(cs|axaml|js)$' -and (Test-Path $_) } |
        ForEach-Object { Get-Item -LiteralPath $_ }
}
function Get-TrackedHandwrittenFiles {
    # 5+100 检查范围与宪法第十三条一致：.cs / .axaml / .js（ps1 不在红线内，SHR-2026-08-D2）
    $tracked = git ls-files
    $untracked = git ls-files --others --exclude-standard
    @($tracked) + @($untracked) | Where-Object { $_ -match '\.(cs|axaml|js)$' -and (Test-Path $_) } | ForEach-Object { Get-Item -LiteralPath $_ }
}

# 5+100 行数统计（SHR-2026-08-D2）：逻辑物理行数。
# 空白行计入；无末尾换行时最后一行仍计入；CRLF/LF/CR 均正确识别；中文 UTF-8 正确。
# 禁止改回 Get-Content | Measure-Object -Line：PS 5.1 实测行数失真（109 行数成 96），导致门禁漏检。
function Get-PhysicalLineCount([string]$path) {
    return [System.IO.File]::ReadAllLines($path).Count
}

function Assert-LineCounter([string]$label, [string]$content, [int]$expected) {
    $tmp = Join-Path ([System.IO.Path]::GetTempPath()) ("arch-guard-linecheck-" + [guid]::NewGuid().ToString("N") + ".txt")
    try {
        [System.IO.File]::WriteAllBytes($tmp, [System.Text.Encoding]::UTF8.GetBytes($content))
        $actual = Get-PhysicalLineCount $tmp
        if ($actual -ne $expected) { Add-Failure "5+100 self-check FAIL [$label]: expected $expected, got $actual" }
    }
    finally {
        Remove-Item -LiteralPath $tmp -Force -ErrorAction SilentlyContinue
    }
}

# 门禁自验证（SHR-2026-08-D2）：统计函数必须通过这些样本；任一失败即门禁 FAIL。
Assert-LineCounter "99行PASS" (("x`n") * 99) 99
Assert-LineCounter "100行PASS" (("x`n") * 100) 100
Assert-LineCounter "101行须检出" (("x`n") * 101) 101
Assert-LineCounter "连续空白行计入" ("a`n`n`nb`n") 3
Assert-LineCounter "CRLF正确" ("a`r`nb`r`n") 2
Assert-LineCounter "LF正确" ("a`nb`n") 2
Assert-LineCounter "无末尾换行末行计入" ("a`nb") 2
Assert-LineCounter "中文UTF-8正确" ("中文第一行`n中文第二行") 2

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
    "XuanYu.Editor/XuanYu.Editor.csproj",
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

# ARCH-WORLD-R4 Editor boundary guards live in a separate file (5+100 split).
. "$PSScriptRoot/arch-a-guard-editor.ps1"

# ARCH-WORLD-R5 Render projection boundary guards live in a separate file.
. "$PSScriptRoot/arch-a-guard-render.ps1"
. "$PSScriptRoot/arch-a-guard-warcore.ps1"

foreach ($file in Get-TrackedHandwrittenFiles) {
    $lines = Get-PhysicalLineCount $file.FullName
    if ($lines -gt 100) { Add-Failure "5+100 exceeded: $lines lines $($file.FullName)" }
}

if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Error $_ }
    exit 1
}

Write-Host "ARCH-A guard passed."
