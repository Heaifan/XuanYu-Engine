using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using XYUI.Avalonia;
using XYUI.Avalonia.Spatial;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI08ShapeContractTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI08ShapeContractTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Shape_channels_compose_without_merging_radius_border_surface()
        => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var border = new Border { Width = 120, Height = 40, Classes = { "xyui-surface-panel" } };
        XY.SetRadius(border, "XY.Radius.Control");
        XY.SetBorder(border, "XY.Border.Strong");
        var window = XyuiBatchTestHost.Show(border);
        Assert.Equal(new CornerRadius(4), border.CornerRadius);
        Assert.Equal(new Thickness(2), border.BorderThickness);
        Assert.IsType<SolidColorBrush>(border.Background);
        window.Close();
    });

    [Fact]
    public void Panel_surface_does_not_add_a_second_outline()
        => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var panel = new Border { Classes = { "xyui-surface-panel" } };
        var window = XyuiBatchTestHost.Show(panel);
        Assert.Equal(new CornerRadius(0), panel.CornerRadius);
        Assert.Equal(new Thickness(0), panel.BorderThickness);
        Assert.IsType<SolidColorBrush>(panel.Background);
        window.Close();
    });

    [Fact]
    public void Shape_styles_do_not_resize_the_consumer()
        => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var control = new Border { Width = 160, Height = 48, Classes = { "xyui-border-default" } };
        var window = XyuiBatchTestHost.Show(control);
        Assert.Equal(160, control.Width);
        Assert.Equal(48, control.Height);
        window.Close();
    });
}
