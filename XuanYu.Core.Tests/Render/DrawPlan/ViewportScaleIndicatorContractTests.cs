using System.IO;

namespace XuanYu.Core.Tests.Render;

public sealed class ViewportScaleIndicatorContractTests
{
    [Fact]
    public void Scale_indicator_is_a_vulkan_viewport_overlay()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var xaml = File.ReadAllText(Path.Combine(root, "XuanYu.Editor.UI", "Viewport", "Vulkan", "VulkanViewport.axaml"));
        var host = xaml.IndexOf("<local:VulkanNativeHost/>", StringComparison.Ordinal);
        Assert.Contains("<Grid>", xaml);
        Assert.DoesNotContain("RowDefinitions=\"*,Auto\"", xaml);
        Assert.DoesNotContain("ScaleIndicator", xaml);
        var drawPlan = File.ReadAllText(Path.Combine(root, "XuanYu.Render.Abstractions", "RenderDrawPlan.cs"));
        var pipeline = File.ReadAllText(Path.Combine(root, "XuanYu.Render.Vulkan", "Pipeline", "VulkanGraphicsPipelineOwner.Grid.cs"));
        var shader = File.ReadAllText(Path.Combine(root, "XuanYu.Render.Vulkan", "Shaders", "editor_scale_indicator.frag"));
        var nativeDir = Path.Combine(root, "XuanYu.Editor.UI", "Viewport", "Vulkan");
        Assert.Contains("RenderDrawKind.ScaleIndicatorOverlay", drawPlan);
        Assert.Contains("CreateScaleIndicator", pipeline);
        Assert.Contains("depthTest: false", pipeline);
        Assert.Contains("ScaleIndicatorGlyphLite", shader);
        Assert.DoesNotContain("sevenSegment", shader);
        Assert.Contains("0.973, 0.980, 0.984", shader);
        Assert.Contains("0.196, 0.435, 0.541", shader);
        Assert.Contains("CardWidthDip", File.ReadAllText(Path.Combine(root,
            "XuanYu.Render.Vulkan", "Render", "Grid", "VulkanClearFrameOwner.ScaleIndicator.cs")));
        Assert.False(File.Exists(Path.Combine(nativeDir, "Win32ViewportHost.ScaleIndicator.cs")));
        Assert.False(File.Exists(Path.Combine(nativeDir, "Win32ViewportHost.ScaleIndicator.Paint.cs")));
        Assert.False(File.Exists(Path.Combine(nativeDir, "VulkanNativeHost.ScaleIndicator.cs")));
        Assert.True(host >= 0, "视口必须保留 Native Host");
    }

    [Fact]
    public void Scale_indicator_draws_before_navigation_gizmo()
    {
        var projection = new XuanYu.Render.Abstractions.RenderProjection(
            default, [], false, default,
            ScaleIndicator: new(true, "104 m", 104));
        var plan = XuanYu.Render.Abstractions.RenderDrawPlan.GetFrameDrawPlan(projection);
        Assert.Equal(XuanYu.Render.Abstractions.RenderDrawKind.ScaleIndicatorOverlay, plan[^2].Kind);
        Assert.Equal(XuanYu.Render.Abstractions.RenderDrawKind.NavigationGizmo, plan[^1].Kind);
    }
}
