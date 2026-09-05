using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Foundation;
using XYUI.Avalonia.Theme;
using XYUI.Avalonia.Typography;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class CodeTextRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public CodeTextRuntimeTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void CodeText_body_and_mark_keep_separate_canonical_colors() => _fx.Run(() =>
    {
        var app = Application.Current!;
        app.Resources.MergedDictionaries.Add(XyuiTheme.CreateLight());
        app.Styles.Add(XyuiComponentStyles.Create());
        var code = new XYCodeText { Text = "terrain/main-heightfield" };
        var win = new Window { Content = code };
        win.Show(); code.ApplyStyling();

        var text = code.GetVisualDescendants().OfType<TextBlock>()
            .Single(x => x.Classes.Contains("xyui-code-text-text"));
        var mark = code.GetVisualDescendants().OfType<VectorPath>()
            .Single(x => x.Classes.Contains("xyui-code-text-mark"));
        Assert.True(XyuiColorTokens.TryFind("XY.Text.Tertiary", out var body));
        Assert.True(XyuiColorTokens.TryFind("XY.Icon.Mark", out var markToken));
        var textBrush = Assert.IsType<SolidColorBrush>(text.Foreground);
        var markBrush = Assert.IsType<SolidColorBrush>(mark.Stroke);
        Assert.Equal(body.ToColor(false), textBrush.Color);
        Assert.Equal(markToken.ToColor(false), markBrush.Color);
        Assert.NotEqual(textBrush.Color, markBrush.Color);
        Assert.Equal(XyuiTypographyTokens.FontMono, text.FontFamily.Name);
        Assert.Equal(XyuiTypographyTokens.FontSizeMono, text.FontSize);
        Assert.Equal(1.25, mark.StrokeThickness);
        Assert.Equal(8, mark.Width); Assert.Equal(8, mark.Height);
        Assert.Equal(6, Canvas.GetRight(mark)); Assert.Equal(5, Canvas.GetBottom(mark));
    });

    [Fact]
    public void CodeText_disabled_uses_disabled_text_token() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var code = new XYCodeText { Text = "World.RegionKey", IsEnabled = false };
        var win = XyuiBatchTestHost.Show(code);
        var text = code.GetVisualDescendants().OfType<TextBlock>().Single(x => x.Classes.Contains("xyui-code-text-text"));
        Assert.Equal(XyuiColorTokens.All.Single(x => x.TokenId == "XY.State.Disabled.Text").ToColor(false),
            Assert.IsType<SolidColorBrush>(text.Foreground).Color); win.Close();
    });
}
