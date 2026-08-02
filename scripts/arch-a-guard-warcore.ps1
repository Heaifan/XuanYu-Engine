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
function Get-SourceFiles([string]$dir) {
    @($(git ls-files "$dir/*") + $(git ls-files --others --exclude-standard "$dir/*")) |
        Where-Object { $_ -match '\.(cs|axaml|js)$' -and (Test-Path $_) } |
        ForEach-Object { Get-Item -LiteralPath $_ }
}

$wcCsproj = "XuanYu.WarCore/XuanYu.WarCore.csproj"
$wcTestsCsproj = "XuanYu.WarCore.Tests/XuanYu.WarCore.Tests.csproj"

# 依赖禁区：WarCore → Editor / Vulkan / Avalonia 禁止。
Assert-NotContains $wcCsproj @("XuanYu.Editor", "XuanYu.Render.Vulkan", "Silk.NET.Vulkan", "Avalonia") "WarCore project reference"
foreach ($file in Get-SourceFiles "XuanYu.WarCore") {
    Assert-NotContains $file.FullName @("using XuanYu.Editor", "using XuanYu.Render.Vulkan", "using Silk.NET.Vulkan", "using Avalonia") "WarCore source reference"
}

# 依赖禁区：Core → WarCore、World → WarCore 禁止。
Assert-NotContains "XuanYu.Core/XuanYu.Core.csproj" @("XuanYu.WarCore") "Core must not reference WarCore"
Assert-NotContains "XuanYu.World/XuanYu.World.csproj" @("XuanYu.WarCore") "World must not reference WarCore"

# WarCore 必须引用 Core（EntityId 关联），测试项目必须引用 WarCore。
Assert-Contains $wcCsproj "XuanYu.Core.csproj" "WarCore references Core"
Assert-Contains $wcTestsCsproj "XuanYu.WarCore.csproj" "WarCore.Tests references WarCore"

# 解决方案必须包含两个新项目。
$slnx = Read-Text "XuanYu.Engine.slnx"
if ($slnx.IndexOf("XuanYu.WarCore/XuanYu.WarCore.csproj", [StringComparison]::OrdinalIgnoreCase) -lt 0) {
    Add-Failure "solution missing project: XuanYu.WarCore/XuanYu.WarCore.csproj"
}
if ($slnx.IndexOf("XuanYu.WarCore.Tests/XuanYu.WarCore.Tests.csproj", [StringComparison]::OrdinalIgnoreCase) -lt 0) {
    Add-Failure "solution missing project: XuanYu.WarCore.Tests/XuanYu.WarCore.Tests.csproj"
}

if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Error $_ }
    exit 1
}

Write-Host "ARCH-A WarCore guard passed."
