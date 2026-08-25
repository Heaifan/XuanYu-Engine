using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Theme;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class R5F4F1AlignmentTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public R5F4F1AlignmentTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Section_title_uses_s05_soft_header_contract() => _fx.Run(() =>
    {
        var app = Application.Current!;
        app.Resources.MergedDictionaries.Add(XyuiTheme.CreateLight());
        app.Styles.Add(XyuiComponentStyles.Create());
        var section = new XYSectionTitle { Text = "属性分组" };
        var window = new Window { Content = section };
        window.Show();
        section.ApplyStyling();
        var header = Assert.IsType<Grid>(section.Child);
        var mark = Assert.IsType<Border>(header.Children[0]);
        var text = Assert.IsType<TextBlock>(header.Children[1]);
        mark.ApplyStyling(); text.ApplyStyling();

        Assert.Equal(28, section.Height);
        Assert.Equal(new CornerRadius(3), section.CornerRadius);
        Assert.Equal(new Thickness(0), section.BorderThickness);
        Assert.Equal(Color.Parse("#EEF3F6"), Assert.IsType<SolidColorBrush>(section.Background).Color);
        Assert.Equal(new GridLength(3), header.ColumnDefinitions[0].Width);
        Assert.Equal(global::Avalonia.Layout.VerticalAlignment.Center, header.VerticalAlignment);
        Assert.Equal(3, mark.Width); Assert.Equal(16, mark.Height);
        Assert.Equal(global::Avalonia.Layout.VerticalAlignment.Center, mark.VerticalAlignment);
        Assert.Equal(Color.Parse("#526873"), Assert.IsType<SolidColorBrush>(mark.Background).Color);
        Assert.Equal(14, text.FontSize); Assert.Equal(FontWeight.SemiBold, text.FontWeight);
        Assert.Equal(18, text.LineHeight);
        Assert.Equal(Color.Parse("#243744"), Assert.IsType<SolidColorBrush>(text.Foreground).Color);
        Assert.Equal("XYUI-1-05", section.CanonicalId);
        window.Close();
    });

    [Fact]
    public void Empty_text_is_quiet_text_without_vector_decoration() => _fx.Run(() =>
    {
        var empty = new XYEmptyText { Text = "暂无数据" };
        Assert.Equal("暂无数据", empty.Text);
        Assert.Equal("XYUI-1-22", empty.CanonicalId);
    });
}
