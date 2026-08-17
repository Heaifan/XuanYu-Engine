using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using XYUI.Avalonia.Theme;
using XYUI.Avalonia.Typography;

namespace XYUI.Avalonia.Tests;

// Typography Runtime：基础资源 key/类型 + 语义样式类可被真实 TextBlock 消费
[Collection("XyuiHeadless")]
public class TypographyRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;

    public TypographyRuntimeTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Theme_Contains_Typography_Resources() => _fx.Run(() =>
    {
        var dict = XyuiTheme.CreateLight();
        Assert.IsType<FontFamily>(dict["XY.Font.UI"]);
        Assert.IsType<FontFamily>(dict["XY.Font.Mono"]);
        Assert.Equal(14.0, Assert.IsType<double>(dict["XY.FontSize.Body"]));
        Assert.Equal(FontWeight.SemiBold, dict["XY.FontWeight.Semibold"]);
        Assert.Equal(20.0, Assert.IsType<double>(dict["XY.LineHeight.Body"]));
        Assert.Equal(-0.10, Assert.IsType<double>(dict["XY.LetterSpacing.Label"]));
    });

    [Fact]
    public void TextStyles_Collection_Has_Nine_Semantic_Rules() => _fx.Run(() =>
    {
        var styles = XyuiTextStyles.Create();
        Assert.Equal(9, styles.Count);
    });

    [Fact]
    public void Body_Class_Applies_To_Real_TextBlock() => _fx.Run(() =>
    {
        var app = Application.Current!;
        app.Resources.MergedDictionaries.Add(XyuiTheme.CreateLight());
        app.Styles.Add(XyuiTextStyles.Create());
        var tb = new TextBlock { Classes = { "xyui-text-body" }, Text = "正文示例" };
        var win = new Window { Content = tb };
        win.Show();
        tb.ApplyStyling();
        Assert.Equal(14.0, tb.FontSize);
        Assert.Equal(FontWeight.Normal, tb.FontWeight);
        Assert.Equal(20.0, tb.LineHeight);
    });

    [Fact]
    public void PageTitle_Class_Uses_Bold_And_PageTitle_Size() => _fx.Run(() =>
    {
        var app = Application.Current!;
        var tb = new TextBlock { Classes = { "xyui-heading-page" }, Text = "页面标题" };
        var win = new Window { Content = tb };
        win.Show();
        tb.ApplyStyling();
        Assert.Equal(24.0, tb.FontSize);
        Assert.Equal(FontWeight.Bold, tb.FontWeight);
    });

    [Fact]
    public void Mono_Class_Uses_Mono_Family() => _fx.Run(() =>
    {
        var app = Application.Current!;
        var tb = new TextBlock { Classes = { "xyui-text-mono" }, Text = "X = 421.482" };
        var win = new Window { Content = tb };
        win.Show();
        tb.ApplyStyling();
        Assert.Equal(13.0, tb.FontSize);
        Assert.Equal("Source Code Pro", tb.FontFamily.ToString());
    });
}
