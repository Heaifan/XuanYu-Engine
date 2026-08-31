using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Foundation;
using XYUI.Avalonia.Theme;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class SearchHighlightRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public SearchHighlightRuntimeTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Search_mark_is_an_eight_dip_uniform_vector() => _fx.Run(() =>
    {
        Prepare(); var control = new XYSearchHighlight { Text = "命中：区域数据集" };
        var window = new Window { Content = control }; window.Show();
        var mark = control.GetVisualDescendants().OfType<VectorPath>().Single();
        Assert.NotNull(mark.Data); Assert.Equal(8, mark.Width); Assert.Equal(8, mark.Height);
        Assert.Equal(1, mark.StrokeThickness); Assert.Equal(Stretch.Uniform, mark.Stretch);
        Assert.False(mark.IsHitTestVisible); Assert.Equal(6, Canvas.GetRight(mark)); Assert.Equal(5, Canvas.GetTop(mark));
        Assert.Equal(22, control.GetVisualDescendants().OfType<TextBlock>().Single().Margin.Right);
        window.Close();
    });

    [Fact]
    public void Search_mark_uses_light_gray_in_light_and_dark() => _fx.Run(() =>
    {
        var app = Prepare(); var control = new XYSearchHighlight { Text = "命中：区域数据集" };
        var window = new Window { Content = control }; window.Show();
        var mark = control.GetVisualDescendants().OfType<VectorPath>().Single();
        foreach (var dark in new[] { false, true })
        {
            app.RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light;
            Assert.Equal(Token(dark), Assert.IsType<SolidColorBrush>(mark.Stroke).Color);
        }
        window.Close();
    });

    static Application Prepare() { var app = Application.Current!; app.Resources.MergedDictionaries.Add(XyuiTheme.CreateThemeDictionaries()); app.Styles.Add(XyuiComponentStyles.Create()); return app; }
    static Color Token(bool dark) => XyuiColorTokens.All.Single(x => x.TokenId == "XY.Text.Disabled").ToColor(dark);
}
