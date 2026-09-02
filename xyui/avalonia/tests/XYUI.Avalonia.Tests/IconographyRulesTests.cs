using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Tests;

 [Collection("XyuiHeadless")]
public sealed class IconographyRulesTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public IconographyRulesTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Theory]
    [InlineData(XyuiIconSize.Compact, 14, 1.25)]
    [InlineData(XyuiIconSize.Default, 16, 1.5)]
    [InlineData(XyuiIconSize.Comfortable, 20, 1.75)]
    [InlineData(XyuiIconSize.Touch, 24, 2)]
    public void Icon_size_contract_comes_from_one_runtime_mapping(XyuiIconSize size, double dip, double stroke)
    {
        var metrics = XyuiIconSizeMetrics.For(size);
        Assert.Equal(dip, metrics.SizeDip);
        Assert.Equal(stroke, metrics.StrokeWidth);
    }

    [Fact]
    public void Canonical_registry_contains_gallery_metrics_without_a_second_geometry_source() => _fx.Run(() =>
    {
        var required = new[] { XyuiVectorIcon.Search, XyuiVectorIcon.ChevronRight,
            XyuiVectorIcon.Eye, XyuiVectorIcon.Locate, XyuiVectorIcon.Code, XyuiVectorIcon.MoreHorizontal };
        Assert.All(required, icon => Assert.True(XyuiVectorIcons.PathData.ContainsKey(icon)));
        Assert.All(required, icon => Assert.Equal(24, XyuiVectorIcons.GetMetrics(icon).LogicalViewport));
        Assert.All(required, icon => Assert.Equal(new global::Avalonia.Vector(0, 0), XyuiVectorIcons.GetMetrics(icon).OpticalOffset));
    });

    [Fact]
    public void Registry_geometry_is_consumed_by_xyicon_and_not_control_owned() => _fx.Run(() =>
    {
        Assert.Equal(XyuiVectorIcons.Create(XyuiVectorIcon.Search).Bounds,
            XyuiVectorIcons.GetMetrics(XyuiVectorIcon.Search).GeometryBounds);
        Assert.Equal("XYUI-1-12", new XYIcon().CanonicalId);
    });
}
