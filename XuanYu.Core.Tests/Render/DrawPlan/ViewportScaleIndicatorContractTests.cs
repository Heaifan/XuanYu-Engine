using System.IO;

namespace XuanYu.Core.Tests.Render;

public sealed class ViewportScaleIndicatorContractTests
{
    [Fact]
    public void Scale_indicator_is_in_an_avalonia_row_outside_native_host()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var xaml = File.ReadAllText(Path.Combine(root, "XuanYu.Editor.UI", "Viewport", "Vulkan", "VulkanViewport.axaml"));
        var host = xaml.IndexOf("<local:VulkanNativeHost/>", StringComparison.Ordinal);
        var scale = xaml.IndexOf("Grid.Row=\"1\" IsVisible=\"{Binding IsScaleIndicatorVisible}", StringComparison.Ordinal);
        Assert.Contains("<Grid RowDefinitions=\"*,Auto\">", xaml);
        Assert.Contains("HorizontalAlignment=\"Right\"", xaml[scale..]);
        Assert.DoesNotContain("HorizontalAlignment=\"Stretch\"", xaml[scale..]);
        Assert.True(host >= 0 && scale > host, "比例尺必须位于 Native Host 所在行之外");
    }
}
