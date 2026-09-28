. (Join-Path $PSScriptRoot "architecture/guard-bootstrap.ps1")
$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
Initialize-GuardBootstrap -Root $root | Out-Null

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

Complete-GuardRun "ARCH-A WarCore guard completed." -Child
