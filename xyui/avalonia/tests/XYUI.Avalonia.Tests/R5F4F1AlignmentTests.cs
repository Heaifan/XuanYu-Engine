using Avalonia.Controls.Shapes;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class R5F4F1AlignmentTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public R5F4F1AlignmentTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Section_title_has_divider_but_no_default_vector_mark() => _fx.Run(() =>
    {
        var section = new XYSectionTitle { Text = "属性分组" };
        Assert.Empty(section.GetVisualDescendants().OfType<Path>());
        Assert.Equal("XYUI-1-05", section.CanonicalId);
    });

    [Fact]
    public void Empty_text_is_quiet_text_without_vector_decoration() => _fx.Run(() =>
    {
        var empty = new XYEmptyText { Text = "暂无数据" };
        Assert.Empty(empty.GetVisualDescendants().OfType<Path>());
        Assert.Equal("暂无数据", empty.Text);
        Assert.Equal("XYUI-1-22", empty.CanonicalId);
    });
}
