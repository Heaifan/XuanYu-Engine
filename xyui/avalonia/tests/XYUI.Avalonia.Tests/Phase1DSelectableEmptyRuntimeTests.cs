using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Typography;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class Phase1DSelectableEmptyRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public Phase1DSelectableEmptyRuntimeTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Selectable_disabled_downgrades_content_and_copy_mark() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var control = new XYSelectableText { Text = "只读值", IsEnabled = false }; var window = XyuiBatchTestHost.Show(control);
        var text = control.GetVisualDescendants().OfType<SelectableTextBlock>().Single(); var mark = control.GetVisualDescendants().OfType<VectorPath>().Single();
        Assert.False(control.IsEffectivelyEnabled); Assert.Equal(XyuiBatchTestHost.Token("XY.State.Disabled.Text"), XyuiBatchTestHost.ColorOf(text.Foreground));
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Disabled.Text"), XyuiBatchTestHost.ColorOf(mark.Stroke)); window.Close();
    });

    [Fact]
    public void Empty_text_is_lightweight_centered_caption() => _fx.Run(() =>
    {
        var app = XyuiBatchTestHost.Prepare(); app.RequestedThemeVariant = ThemeVariant.Light; var empty = new XYEmptyText { Text = "暂无搜索结果" }; var window = XyuiBatchTestHost.Show(empty);
        Assert.Equal("XYUI-1-22", empty.CanonicalId); Assert.Equal(TextAlignment.Center, empty.TextAlignment); Assert.Equal(XyuiTypographyTokens.FontSizeCaption, empty.FontSize);
        Assert.Equal(XyuiBatchTestHost.Token("XY.Text.Tertiary"), XyuiBatchTestHost.ColorOf(empty.Foreground)); Assert.Empty(empty.GetVisualDescendants().OfType<VectorPath>());
        app.RequestedThemeVariant = ThemeVariant.Dark; Assert.Equal(XyuiBatchTestHost.Token("XY.Text.Tertiary", true), XyuiBatchTestHost.ColorOf(empty.Foreground)); window.Close(); app.RequestedThemeVariant = ThemeVariant.Light;
    });

    [Fact]
    public void Empty_text_disabled_uses_shared_disabled_token() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var empty = new XYEmptyText { Text = "未选择对象", IsEnabled = false }; var window = XyuiBatchTestHost.Show(empty);
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Disabled.Text"), XyuiBatchTestHost.ColorOf(empty.Foreground)); window.Close();
    });
}
