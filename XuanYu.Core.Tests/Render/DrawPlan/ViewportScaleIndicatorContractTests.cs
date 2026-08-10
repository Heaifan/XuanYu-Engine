using System.IO;

namespace XuanYu.Core.Tests.Render;

public sealed class ViewportScaleIndicatorContractTests
{
    [Fact]
    public void Scale_indicator_is_a_native_floating_overlay()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var xaml = File.ReadAllText(Path.Combine(root, "XuanYu.Editor.UI", "Viewport", "Vulkan", "VulkanViewport.axaml"));
        var host = xaml.IndexOf("<local:VulkanNativeHost/>", StringComparison.Ordinal);
        Assert.Contains("<Grid>", xaml);
        Assert.DoesNotContain("RowDefinitions=\"*,Auto\"", xaml);
        Assert.DoesNotContain("ScaleIndicator", xaml);
        var hostCode = File.ReadAllText(Path.Combine(root, "XuanYu.Editor.UI", "Viewport", "Vulkan", "VulkanNativeHost.cs"));
        Assert.Contains("CreateNativeScaleIndicator(parent.Handle)", hostCode);
        var native = File.ReadAllText(Path.Combine(root, "XuanYu.Editor.UI", "Viewport", "Vulkan", "Win32ViewportHost.ScaleIndicator.cs"));
        Assert.Contains("CreateScaleIndicator", native);
        Assert.Contains("IsWindowVisible", native);
        Assert.Contains("GetWindowRect", native);
        Assert.Contains("PaintCount", native);
        Assert.Contains("SetWindowPos(hwnd, 0", native);
        Assert.True(host >= 0, "视口必须保留 Native Host");
    }
}
