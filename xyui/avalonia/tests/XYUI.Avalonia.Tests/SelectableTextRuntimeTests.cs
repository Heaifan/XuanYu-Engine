using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Foundation;
using XYUI.Avalonia.Gallery;
using XYUI.Avalonia.Theme;
using XYUI.Avalonia.Typography;
using XYUI.Avalonia.Vector;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class SelectableTextRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public SelectableTextRuntimeTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Copy_mark_is_an_eight_dip_scaled_vector() => _fx.Run(() =>
    {
        Prepare(); var control = new XYSelectableText { Text = "region-7ad21c", Variant = XyuiSelectableTextVariant.Technical };
        var window = new Window { Content = control }; window.Show();
        var grid = Assert.IsType<Grid>(control.Child); var text = Assert.Single(grid.Children.OfType<SelectableTextBlock>());
        var mark = Assert.Single(grid.Children.OfType<VectorPath>());
        Assert.Equal(XyuiVectorIcon.Copy, control.CopyIcon); Assert.NotNull(mark.Data);
        Assert.Equal(8, mark.Width); Assert.Equal(8, mark.Height); Assert.Equal(1, mark.StrokeThickness);
        Assert.Equal(Stretch.Uniform, mark.Stretch);
        Assert.False(mark.IsHitTestVisible); Assert.Equal(3, grid.ColumnDefinitions.Count);
        Assert.Equal(XYSelectableText.CopyMarkGap, grid.ColumnDefinitions[1].Width.Value);
        Assert.Equal(XYSelectableText.CopyMarkSize, grid.ColumnDefinitions[2].Width.Value);
        Assert.Equal(0, Grid.GetColumn(text)); Assert.Equal(2, Grid.GetColumn(mark)); window.Close();
    });

    [Fact]
    public void Selection_and_copy_result_contain_text_only() => _fx.Run(() =>
    {
        Prepare(); var control = new XYSelectableText { Text = "region-7ad21c" };
        var window = new Window { Content = control }; window.Show();
        var text = control.GetVisualDescendants().OfType<SelectableTextBlock>().Single();
        control.SelectionStart = 0; control.SelectionEnd = control.Text.Length;
        Assert.Equal(control.Text, text.SelectedText); Assert.True(text.CanCopy);
        Assert.DoesNotContain("Copy", text.SelectedText); Assert.DoesNotContain("□", text.SelectedText);
        window.Close();
    });

    [Fact]
    public void Variants_and_light_dark_semantics_resolve() => _fx.Run(() =>
    {
        var app = Prepare(); var preview = Assert.IsType<StackPanel>(XYUI1GalleryCatalog.CreatePreview("XYUI-1-21"));
        var controls = preview.Children.OfType<XYSelectableText>().ToArray(); Assert.Equal(2, controls.Length);
        var window = new Window { Content = preview }; window.Show(); var technical = controls[1];
        var text = technical.GetVisualDescendants().OfType<SelectableTextBlock>().Single(); var mark = technical.GetVisualDescendants().OfType<VectorPath>().Single();
        Assert.Equal(XyuiTypographyTokens.FontMono, text.FontFamily.Name);
        foreach (var dark in new[] { false, true })
        {
            app.RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light;
            Assert.Equal(Token("XY.Text.Disabled", dark), ColorOf(mark.Stroke));
            Assert.Equal(Token("XY.Surface.Selected", dark), ColorOf(text.SelectionBrush));
            Assert.Equal(Token("XY.Text.Primary", dark), ColorOf(text.SelectionForegroundBrush));
        }
        window.Close();
    });

    static Application Prepare() { var app = Application.Current!; app.Resources.MergedDictionaries.Add(XyuiTheme.CreateThemeDictionaries()); app.Styles.Add(XyuiComponentStyles.Create()); return app; }
    static Color Token(string id, bool dark) => XyuiColorTokens.All.Single(x => x.TokenId == id).ToColor(dark);
    static Color ColorOf(IBrush? brush) => Assert.IsType<SolidColorBrush>(brush).Color;
}
