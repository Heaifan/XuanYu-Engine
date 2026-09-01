using Avalonia.Controls;
using XYUI.Avalonia;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class IconRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public IconRuntimeTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void XY_size_maps_to_icon_size_when_icon_has_no_override() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var icon = new XYIcon { Icon = XyuiVectorIcon.Search };
        var host = new Border { Child = icon }; XY.SetSize(host, XYSize.Touch); XyuiBatchTestHost.Show(host);
        Assert.Equal(24, icon.Bounds.Width); Assert.Equal(24, icon.Bounds.Height);
    });

    [Fact]
    public void Explicit_icon_size_overrides_inherited_XY_size() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var icon = new XYIcon { Icon = XyuiVectorIcon.Search, Size = XyuiIconSize.Compact };
        var host = new Border { Child = icon }; XY.SetSize(host, XYSize.Touch); XyuiBatchTestHost.Show(host);
        Assert.Equal(14, icon.Bounds.Width); Assert.Equal(14, icon.Bounds.Height);
    });

    [Fact]
    public void Toolbar_and_icon_button_keep_registry_icon_consumption() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var tool = new XYToolbarTool { Icon = XyuiVectorIcon.Locate };
        var toolbar = new XYToolbar(tool); XY.SetSize(toolbar, XYSize.Comfortable); XyuiBatchTestHost.Show(toolbar);
        Assert.Equal(XYSize.Comfortable, XY.GetSize(tool.Button));
        var button = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.Eye } };
        Assert.IsType<XYIcon>(button.Content); Assert.Equal(XyuiVectorIcon.Eye, ((XYIcon)button.Content).Icon);
    });

    [Fact]
    public void Registry_metrics_are_uniform_and_unknown_icon_fails_loudly() => _fx.Run(() =>
    {
        var metrics = XyuiVectorIcons.GetMetrics(XyuiVectorIcon.ChevronRight);
        Assert.Equal(24, metrics.LogicalViewport); Assert.Equal(0, metrics.OpticalOffset.X);
        Assert.False(metrics.HasOpticalCorrection); Assert.Equal(12, metrics.GeometryBounds.Center.X);
        Assert.Throws<KeyNotFoundException>(() => XyuiVectorIcons.Create((XyuiVectorIcon)999));
    });
}
