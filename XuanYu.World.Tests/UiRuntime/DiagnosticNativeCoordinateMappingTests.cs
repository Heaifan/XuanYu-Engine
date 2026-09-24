using System.Reflection;
using Avalonia;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class DiagnosticNativeCoordinateMappingTests
{
    [Fact]
    public void Native_screen_point_maps_to_floating_layer_local_dip()
    {
        var result = Map(new PixelPoint(660, 465), new PixelPoint(0, 0), 1);

        Assert.Equal(new Point(660, 465), result);
    }

    [Fact]
    public void Native_viewport_origin_maps_to_window_offset()
    {
        var result = Map(new PixelPoint(260, 165), new PixelPoint(0, 0), 1);

        Assert.Equal(new Point(260, 165), result);
    }

    static Point Map(PixelPoint screen, PixelPoint layerOrigin, double scale)
    {
        var type = typeof(VulkanNativeHost).Assembly.GetType(
            "XuanYu.Editor.UI.DiagnosticNativeCoordinateMapping")!;
        var method = type.GetMethod("ToLayer", BindingFlags.Static |
            BindingFlags.Public | BindingFlags.NonPublic)!;
        return (Point)method.Invoke(null, [screen, layerOrigin, scale])!;
    }
}
