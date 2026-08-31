using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Foundation;
using XYUI.Avalonia.Gallery;
using XYUI.Avalonia.Theme;
using XYUI.Avalonia.Typography;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class MonoTextRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public MonoTextRuntimeTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Runtime_owns_three_data_columns_with_token_gap_columns() => _fx.Run(() =>
    {
        var mono = Preview();
        Assert.Equal(5, mono.ColumnDefinitions.Count); Assert.Equal(6, mono.RowDefinitions.Count);
        Assert.Equal(GridLength.Auto, mono.ColumnDefinitions[0].Width);
        Assert.Equal(new GridLength(XYMonoText.LabelValueGap), mono.ColumnDefinitions[1].Width);
        Assert.Equal(GridLength.Auto, mono.ColumnDefinitions[2].Width);
        Assert.Equal(new GridLength(XYMonoText.ValueUnitGap), mono.ColumnDefinitions[3].Width);
        Assert.Equal(GridLength.Auto, mono.ColumnDefinitions[4].Width);
        var labels = Cells(mono, "label"); var values = Cells(mono, "value"); var units = Cells(mono, "unit");
        Assert.All(labels, x => { Assert.Equal(0, Grid.GetColumn(x)); Assert.Equal(TextAlignment.Left, x.TextAlignment); Assert.Equal(HorizontalAlignment.Left, x.HorizontalAlignment); });
        Assert.All(values, x => { Assert.Equal(2, Grid.GetColumn(x)); Assert.Equal(TextAlignment.Right, x.TextAlignment); Assert.Equal(HorizontalAlignment.Right, x.HorizontalAlignment); });
        Assert.All(units, x => { Assert.Equal(4, Grid.GetColumn(x)); Assert.Equal(TextAlignment.Left, x.TextAlignment); Assert.Equal(HorizontalAlignment.Left, x.HorizontalAlignment); });
        Assert.All(labels.Concat(values).Concat(units), x => Assert.Equal(default, x.Margin));
        Assert.All(labels, x => Assert.Equal(TextTrimming.None, x.TextTrimming));
        Assert.Equal("", units[4].Text); Assert.All(mono.Rows, x => { Assert.DoesNotContain("  ", x.Label + x.Value + x.Unit); Assert.DoesNotContain('\t', x.Label + x.Value + x.Unit); });
    });

    [Fact]
    public void Roles_use_ui_mono_ui_fonts_and_semantic_brushes() => _fx.Run(() =>
    {
        var app = Prepare(); app.RequestedThemeVariant = ThemeVariant.Light;
        var mono = Preview(); var window = Show(mono);
        var label = Cells(mono, "label")[0]; var value = Cells(mono, "value")[0]; var unit = Cells(mono, "unit")[0];
        Assert.Contains(XyuiTypographyTokens.FontUi, label.FontFamily.ToString());
        Assert.Contains(XyuiTypographyTokens.FontMono, value.FontFamily.ToString());
        Assert.Contains(XyuiTypographyTokens.FontUi, unit.FontFamily.ToString());
        Assert.Equal(FontWeight.SemiBold, label.FontWeight); Assert.Equal(FontWeight.Normal, value.FontWeight);
        Assert.Equal(FontWeight.SemiBold, unit.FontWeight);
        Assert.Equal(Token("XY.Text.Secondary", false), ColorOf(label.Foreground));
        Assert.Equal(Token("XY.Text.Secondary", false), ColorOf(value.Foreground));
        Assert.Equal(Token("XY.Text.Secondary", false), ColorOf(unit.Foreground)); window.Close();
    });

    [Fact]
    public void Values_share_right_edge_and_units_share_start_with_empty_unit() => _fx.Run(() =>
    {
        Prepare(); var mono = Preview(); var window = Show(mono);
        var valueEdges = Cells(mono, "value").Select(x => x.Bounds.Right).Distinct().ToArray();
        var unitStarts = Cells(mono, "unit").Select(x => x.Bounds.Left).Distinct().ToArray();
        Assert.Single(valueEdges); Assert.Single(unitStarts); Assert.Equal(0, Cells(mono, "unit")[4].Bounds.Width); window.Close();
    });

    [Fact]
    public void Dark_switch_changes_only_role_colors() => _fx.Run(() =>
    {
        var app = Prepare(); app.RequestedThemeVariant = ThemeVariant.Light;
        var mono = Preview(); var window = Show(mono); var widths = mono.ColumnDefinitions.Select(x => x.Width).ToArray();
        app.RequestedThemeVariant = ThemeVariant.Dark;
        Assert.Equal(widths, mono.ColumnDefinitions.Select(x => x.Width));
        Assert.Equal(Token("XY.Text.Secondary", true), ColorOf(Cells(mono, "label")[0].Foreground));
        Assert.Equal(Token("XY.Text.Secondary", true), ColorOf(Cells(mono, "value")[0].Foreground));
        Assert.Equal(Token("XY.Text.Secondary", true), ColorOf(Cells(mono, "unit")[0].Foreground));
        Assert.All(Cells(mono, "value"), x => Assert.Equal(TextAlignment.Right, x.TextAlignment)); window.Close();
    });

    static XYMonoText Preview() => Assert.IsType<XYMonoText>(XYUI1GalleryCatalog.CreatePreview("XYUI-1-08"));
    static TextBlock[] Cells(XYMonoText mono, string role) => mono.Children.OfType<TextBlock>().Where(x => x.Classes.Contains($"xyui-mono-data-{role}")).ToArray();
    static Window Show(XYMonoText mono) { var window = new Window { Width = 420, Height = 260, Content = mono }; window.Show(); return window; }
    static Application Prepare() { var app = Application.Current!; app.Resources.MergedDictionaries.Add(XyuiTheme.CreateThemeDictionaries()); app.Styles.Add(XyuiComponentStyles.Create()); return app; }
    static Color Token(string id, bool dark) => XyuiColorTokens.All.Single(x => x.TokenId == id).ToColor(dark);
    static Color ColorOf(IBrush? brush) => Assert.IsType<SolidColorBrush>(brush).Color;
}
