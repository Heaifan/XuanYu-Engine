using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Foundation;
using XYUI.Avalonia.Theme;
using XYUI.Avalonia.Vector;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class Phase1DSearchTruncatedRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public Phase1DSearchTruncatedRuntimeTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Search_highlight_presents_supplied_result_with_one_search_mark() => _fx.Run(() =>
    {
        Prepare(); var control = new XYSearchHighlight { Text = "命中结果" }; var window = new Window { Content = control }; window.Show();
        Assert.Equal("命中结果", control.GetVisualDescendants().OfType<TextBlock>().Single(x => x.Classes.Contains("xyui-search-highlight-text")).Text);
        var mark = control.GetVisualDescendants().OfType<VectorPath>().Single();
        Assert.Equal(8, mark.Width); Assert.Equal(8, mark.Height); Assert.Equal(6, Canvas.GetRight(mark));
        Assert.Equal(5, Canvas.GetTop(mark)); Assert.False(mark.IsHitTestVisible); window.Close();
    });

    [Fact]
    public void Truncated_text_keeps_end_contract_and_explicit_middle_gap() => _fx.Run(() =>
    {
        var end = new XYTruncatedText { Text = "region/terrain/very-long-identifier" };
        var middle = new XYTruncatedText { Text = "region/terrain/very-long-identifier", Mode = XyuiTruncatedTextMode.Middle };
        Assert.Equal(TextWrapping.NoWrap, end.TextWrapping); Assert.Equal(TextTrimming.CharacterEllipsis, end.TextTrimming);
        Assert.Equal(XyuiTruncatedTextMode.End, end.Mode); Assert.DoesNotContain("xyui-truncated-middle", end.Classes);
        Assert.Equal(TextTrimming.CharacterEllipsis, middle.TextTrimming); Assert.Contains("xyui-truncated-middle", middle.Classes);
        Assert.Equal(XyuiTruncatedTextMode.Middle, middle.Mode);
    });

    [Fact]
    public void Result_text_disabled_state_uses_shared_disabled_token() => _fx.Run(() =>
    {
        var app = Prepare(); var control = new XYSearchHighlight { Text = "结果", IsEnabled = false }; var window = new Window { Content = control }; window.Show();
        var expected = XyuiColorTokens.All.Single(x => x.TokenId == "XY.Text.Disabled").ToColor(app.ActualThemeVariant == ThemeVariant.Dark);
        var text = control.GetVisualDescendants().OfType<TextBlock>().Single(x => x.Classes.Contains("xyui-search-highlight-text"));
        Assert.Equal(expected, Assert.IsType<SolidColorBrush>(text.Foreground).Color); window.Close();
    });

    static Application Prepare() { var app = Application.Current!; app.Resources.MergedDictionaries.Add(XyuiTheme.CreateThemeDictionaries()); app.Styles.Add(XyuiComponentStyles.Create()); return app; }
}
