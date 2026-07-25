# ARCH-WORLD-R4 Editor domain boundary guards, dot-sourced by arch-a-guard.ps1.
# Enforces the layering introduced by ARCH-WORLD-R4:
#   Editor -> Core + World (allowed)   Editor -/-> Editor.UI / Avalonia / Vulkan / Silk
#   Core / World -/-> Editor            Editor.UI -> Editor (allowed)
$editorCsproj = "XuanYu.Editor/XuanYu.Editor.csproj"
$coreCsproj = "XuanYu.Core/XuanYu.Core.csproj"
$worldCsproj = "XuanYu.World/XuanYu.World.csproj"
$editorUiCsproj = "XuanYu.Editor.UI/XuanYu.Editor.UI.csproj"

# Editor may ONLY depend on Core + World via ProjectReference.
foreach ($ref in (Get-ProjectReferences $editorCsproj)) {
    if ($ref -notmatch "XuanYu\.(Core|World)\\XuanYu\.(Core|World)\.csproj$") {
        Add-Failure "Editor project reference forbidden (only Core/World allowed): $ref ($editorCsproj)"
    }
}

# Editor production source must not import Editor.UI / Avalonia / Vulkan / Silk namespaces.
foreach ($file in Get-SourceFiles "XuanYu.Editor") {
    Assert-NotContains $file.FullName @(
        "using XuanYu.Editor.UI",
        "using Avalonia",
        "using XuanYu.Render.Vulkan",
        "using Silk.NET.Vulkan"
    ) "Editor source reference"
}

# Core / World production must NOT reference Editor (production layering).
foreach ($ref in (Get-ProjectReferences $coreCsproj)) {
    if ($ref -match "XuanYu\.Editor") { Add-Failure "Core project reference forbidden: $ref ($coreCsproj)" }
}
foreach ($ref in (Get-ProjectReferences $worldCsproj)) {
    if ($ref -match "XuanYu\.Editor") { Add-Failure "World project reference forbidden: $ref ($worldCsproj)" }
}

# Editor.UI is allowed to reference Editor (it consumes Editor types).
Assert-Contains $editorUiCsproj "XuanYu.Editor.csproj" "Editor.UI composes Editor"

# Solution must contain the new Editor assembly.
Assert-Contains "XuanYu.Engine.slnx" "XuanYu.Editor/XuanYu.Editor.csproj" "solution contains Editor"
