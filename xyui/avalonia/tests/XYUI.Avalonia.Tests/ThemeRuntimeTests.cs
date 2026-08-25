using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Foundation;
using XYUI.Avalonia.Gallery;
using XYUI.Avalonia.Theme;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class ThemeRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public ThemeRuntimeTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Theme_variant_switch_updates_live_code_text_and_gallery_shell() => _fx.Run(() =>
    {
        var app = Prepare(); app.RequestedThemeVariant = ThemeVariant.Light;
        var code = new XYCodeText { Text = "terrain/main-heightfield" };
        var window = new Window { Content = code }; window.Show(); code.ApplyStyling();
        var text = code.GetVisualDescendants().OfType<TextBlock>().Single(x => x.Classes.Contains("xyui-code-text-text"));
        var mark = code.GetVisualDescendants().OfType<VectorPath>().Single(x => x.Classes.Contains("xyui-code-text-mark"));
        var lightText = ColorOf(text.Foreground); var lightMark = ColorOf(mark.Stroke);
        Assert.Equal(ThemeVariant.Light, window.ActualThemeVariant);
        app.RequestedThemeVariant = ThemeVariant.Dark;
        Assert.Equal(ThemeVariant.Dark, window.ActualThemeVariant);
        Assert.Equal(XyuiColorTokens.All.Single(x => x.TokenId == "XY.Text.Tertiary").ToColor(true), ColorOf(text.Foreground));
        Assert.Equal(XyuiColorTokens.All.Single(x => x.TokenId == "XY.Icon.Mark").ToColor(true), ColorOf(mark.Stroke));
        Assert.NotEqual(lightText, ColorOf(text.Foreground)); Assert.NotEqual(lightMark, ColorOf(mark.Stroke));
        app.RequestedThemeVariant = ThemeVariant.Light;
        Assert.Equal(lightText, ColorOf(text.Foreground)); Assert.Equal(lightMark, ColorOf(mark.Stroke));
        window.Close();
    });

    [Fact]
    public void Theme_switch_button_changes_application_variant_without_recreating_window() => _fx.Run(() =>
    {
        var app = Prepare(); app.RequestedThemeVariant = ThemeVariant.Light;
        var window = new MainWindow(); window.Show();
        var button = window.FindControl<Button>("ThemeSwitchButton")!;
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Assert.Equal(ThemeVariant.Dark, app.RequestedThemeVariant);
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Assert.Equal(ThemeVariant.Light, app.RequestedThemeVariant);
        window.Close();
    });

    [Fact]
    public void Section_title_resources_follow_light_and_dark_variants() => _fx.Run(() =>
    {
        var app = Prepare(); app.RequestedThemeVariant = ThemeVariant.Light;
        var section = new XYSectionTitle { Text = "属性分组" };
        var window = new Window { Content = section }; window.Show(); section.ApplyStyling();
        Assert.Equal(Color.Parse("#EEF3F6"), ColorOf(section.Background));
        app.RequestedThemeVariant = ThemeVariant.Dark;
        Assert.Equal(XyuiColorTokens.All.Single(x => x.TokenId == "XY.Surface.PanelAlt").ToColor(true), ColorOf(section.Background));
        window.Close(); app.RequestedThemeVariant = ThemeVariant.Light;
    });

    static Application Prepare()
    {
        var app = Application.Current!;
        app.Resources.MergedDictionaries.Add(XyuiTheme.CreateThemeDictionaries());
        app.Styles.Add(XyuiComponentStyles.Create());
        return app;
    }

    static Color ColorOf(IBrush? brush) => Assert.IsType<SolidColorBrush>(brush).Color;
}
