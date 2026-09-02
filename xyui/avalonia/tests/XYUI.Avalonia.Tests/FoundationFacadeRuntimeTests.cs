using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using XYUI.Avalonia;
using XYUI.Avalonia.Foundation;
using XYUI.Avalonia.Spatial;
using XYUI.Avalonia.Typography;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class FoundationFacadeRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public FoundationFacadeRuntimeTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Text_facade_resolves_color_font_and_typography_roles() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var text = new TextBlock { Text = "Consumer" };
        XY.SetForeground(text, "XY.Text.Link");
        XY.SetFont(text, "XY.Font.Mono");
        XY.SetTypography(text, "XY.Type.Caption");
        var window = XyuiBatchTestHost.Show(text);
        Assert.Equal(XyuiTypographyTokens.FontUi, text.FontFamily.Name);
        Assert.Equal(XyuiTypographyTokens.FontSizeCaption, text.FontSize);
        Assert.Equal(XyuiTypographyTokens.LineHeightCaption, text.LineHeight);
        Assert.NotEqual(Brushes.Transparent, text.Foreground);
        window.Close();
    });

    [Fact]
    public void Geometry_and_spacing_facades_write_native_properties()
        => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var border = new Border();
        XY.SetSurface(border, "XY.Surface.Panel");
        XY.SetPadding(border, "XY.Panel.Padding");
        XY.SetRadius(border, "XY.Radius.Panel");
        XY.SetBorder(border, "XY.Border.Strong");
        XY.SetMargin(border, "XY.Space.2");
        var window = XyuiBatchTestHost.Show(border);
        Assert.Equal(new Thickness(XyuiSpatialTokens.PanelPadding), border.Padding);
        Assert.Equal(new CornerRadius(XyuiSpatialTokens.RadiusPanel), border.CornerRadius);
        Assert.Equal(new Thickness(XyuiSpatialTokens.BorderWidthStrong), border.BorderThickness);
        Assert.NotEqual(Brushes.Transparent, border.Background);
        Assert.Equal(new Thickness(XyuiSpatialTokens.Space2), border.Margin);
        window.Close();
    });

    [Fact]
    public void Gap_facade_maps_only_to_stack_panel()
        => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var panel = new StackPanel();
        XY.SetGap(panel, "XY.Space.3");
        var window = XyuiBatchTestHost.Show(panel);
        Assert.Equal(XyuiSpatialTokens.Space3, panel.Spacing);
        window.Close();
    });
}
