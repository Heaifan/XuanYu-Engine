function Assert-RenderProjectionBoundary {
    foreach ($file in Get-SourceFiles "XuanYu.Render.Abstractions") {
        Assert-NotContains $file.FullName @(
            "using XuanYu.Render.Vulkan",
            "using Silk.NET.Vulkan",
            "using Avalonia",
            "using XuanYu.World",
            "using XuanYu.Editor.UI",
            "using XuanYu.Core.Scene") "Render.Abstractions source reference"
    }

    foreach ($file in Get-SourceFiles "XuanYu.Render.Vulkan") {
        Assert-NotContains $file.FullName @(
            "using XuanYu.Core.Scene",
            "SceneRenderSnapshot",
            "ISceneRenderSnapshotSource",
            "DefaultEditorCamera") "Render.Vulkan render projection boundary"
    }
}

Assert-RenderProjectionBoundary
