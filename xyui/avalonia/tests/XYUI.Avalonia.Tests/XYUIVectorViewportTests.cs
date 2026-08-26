using Avalonia.Controls;
using Avalonia.Media;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUIVectorViewportTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUIVectorViewportTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void XYIcon_is_a_control_with_a_24_DIP_logical_viewport() => _fx.Run(() =>
    {
        Assert.True(typeof(Control).IsAssignableFrom(typeof(XYIcon)));
        Assert.False(typeof(global::Avalonia.Controls.Shapes.Path).IsAssignableFrom(typeof(XYIcon)));
        Assert.Equal(24d, XyuiVectorIcons.LogicalIconSize);
        Assert.Equal("M6 9 L12 15 L18 9", XyuiVectorIcons.PathData[XyuiVectorIcon.ChevronDown]);
    });

    [Fact]
    public void ChevronDown_geometry_is_centered_in_logical_space() => _fx.Run(() =>
    {
        var bounds = XyuiVectorIcons.Create(XyuiVectorIcon.ChevronDown).Bounds;
        Assert.Equal(12d, bounds.Center.X);
        Assert.Equal(12d, bounds.Center.Y);
    });

    [Fact]
    public void Different_geometry_bounds_share_the_same_visual_box() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var icons = new[] { XyuiVectorIcon.Info, XyuiVectorIcon.Section, XyuiVectorIcon.Empty }
            .Select(icon => Show(icon, XyuiIconSize.Medium)).ToArray();
        Assert.All(icons, icon => { Assert.Equal(16d, icon.Bounds.Width); Assert.Equal(16d, icon.Bounds.Height); });
    });

    [Fact]
    public void Size_and_stroke_width_contract_are_preserved() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var expected = new[] { (XyuiIconSize.Tiny, 12d, 1d), (XyuiIconSize.Small, 14d, 1.25d),
            (XyuiIconSize.Medium, 16d, 1.5d), (XyuiIconSize.Large, 20d, 1.75d) };
        foreach (var item in expected)
        {
            var icon = Show(XyuiVectorIcon.ChevronDown, item.Item1);
            Assert.Equal(item.Item2, icon.Bounds.Width);
            Assert.Equal(item.Item2, icon.Bounds.Height);
            Assert.Equal(item.Item3, icon.StrokeWidth);
            Assert.Equal(item.Item3, icon.StrokeThickness);
        }
    });

    static XYIcon Show(XyuiVectorIcon icon, XyuiIconSize size)
    {
        var control = new XYIcon { Icon = icon, Size = size };
        XyuiBatchTestHost.Show(control);
        return control;
    }
}
