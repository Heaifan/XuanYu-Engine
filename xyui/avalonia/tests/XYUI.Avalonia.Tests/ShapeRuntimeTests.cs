using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using XYUI.Avalonia.Spatial;
using XYUI.Avalonia.Theme;

namespace XYUI.Avalonia.Tests;

// Spatial Runtime：基础资源 key/类型 + 语义形状类可被真实 Border 消费
[Collection("XyuiHeadless")]
public class ShapeRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;

    public ShapeRuntimeTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Theme_Contains_Spatial_Resources() => _fx.Run(() =>
    {
        var dict = XyuiTheme.CreateLight();
        Assert.Equal(8.0, Assert.IsType<double>(dict["XY.Space.2"]));
        Assert.Equal(new Thickness(8), Assert.IsType<Thickness>(dict["XY.Panel.Padding"]));
        Assert.Equal(new CornerRadius(4), Assert.IsType<CornerRadius>(dict["XY.Radius.Control"]));
        Assert.Equal(new Thickness(1), Assert.IsType<Thickness>(dict["XY.Border.Width.Default"]));
        Assert.IsType<BoxShadows>(dict["XY.Shadow.Tooltip"]);
    });

    [Fact]
    public void Shadow_Parse_Produces_NonEmpty_BoxShadows() => _fx.Run(() =>
    {
        var tooltip = XyuiSpatial.ParseShadow(XyuiSpatialTokens.ShadowTooltip);
        Assert.True(tooltip.Count == 1);
        Assert.Equal(3.0, tooltip[0].OffsetY);
        Assert.Equal(10.0, tooltip[0].Blur);
        var none = XyuiSpatial.ParseShadow(XyuiSpatialTokens.ShadowNone);
        Assert.True(none.Count == 0);
    });

    [Fact]
    public void ShapeStyles_Collection_Has_Nine_Rules() => _fx.Run(() =>
    {
        Assert.Equal(9, XyuiShapeStyles.Create().Count);
    });

    [Fact]
    public void Border_Class_Applies_To_Real_Border() => _fx.Run(() =>
    {
        var app = Application.Current!;
        app.Resources.MergedDictionaries.Add(XyuiTheme.CreateLight());
        app.Styles.Add(XyuiShapeStyles.Create());
        var border = new Border { Classes = { "xyui-border-default" } };
        var win = new Window { Content = border };
        win.Show();
        border.ApplyStyling();
        Assert.Equal(new Thickness(1), border.BorderThickness);
        Assert.Equal(new CornerRadius(4), border.CornerRadius);
        Assert.IsType<SolidColorBrush>(border.BorderBrush);
    });

    [Fact]
    public void Panel_Class_Applies_Padding_And_Surface() => _fx.Run(() =>
    {
        var app = Application.Current!;
        var border = new Border { Classes = { "xyui-surface-panel" } };
        var win = new Window { Content = border };
        win.Show();
        border.ApplyStyling();
        Assert.Equal(new Thickness(8), border.Padding);
        Assert.IsType<SolidColorBrush>(border.Background);
    });
}
