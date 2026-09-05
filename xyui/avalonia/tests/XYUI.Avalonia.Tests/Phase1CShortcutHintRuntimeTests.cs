using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Sizing;
using XYUI.Avalonia.Spatial;
using XYUI.Avalonia.Typography;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class Phase1CShortcutHintRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public Phase1CShortcutHintRuntimeTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Separate_keycaps_consume_foundation_geometry_and_typography() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var hint = new XYShortcutHint { Shortcut = "Ctrl + Shift + S" };
        var window = XyuiBatchTestHost.Show(hint);
        var panel = Assert.IsType<StackPanel>(hint.Child);
        Assert.Equal(XyuiShortcutCombinationMode.SeparateKeycaps, hint.CombinationMode);
        Assert.Equal(5, panel.Children.Count); Assert.Equal(XyuiSpatialTokens.Space1, panel.Spacing);
        var key = Assert.IsType<Border>(panel.Children[0]); var text = Assert.IsType<TextBlock>(key.Child);
        Assert.Equal("Ctrl", text.Text); Assert.Equal(XyuiSizingMetrics.ControlExtraSmallHeight, key.Height);
        Assert.Equal(new Thickness(XyuiSpatialTokens.Space1 + XyuiSpatialTokens.BorderWidthDefault * 2, XyuiSpatialTokens.BorderWidthDefault * 2), key.Padding);
        Assert.Equal(new CornerRadius(XyuiSpatialTokens.RadiusControl), key.CornerRadius);
        Assert.Equal(new Thickness(XyuiSpatialTokens.BorderWidthDefault), key.BorderThickness);
        Assert.Equal(XyuiTypographyTokens.FontMono, text.FontFamily.Name); Assert.Equal(XyuiTypographyTokens.FontSizeCaption, text.FontSize);
        Assert.Equal(XyuiBatchTestHost.Token("XY.Surface.PanelAlt"), XyuiBatchTestHost.ColorOf(key.Background));
        window.Close();
    });

    [Fact]
    public void Disabled_keycaps_use_shared_disabled_text_and_border_tokens() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var hint = new XYShortcutHint { Shortcut = "Ctrl+S", IsEnabled = false }; var window = XyuiBatchTestHost.Show(hint);
        var key = Assert.IsType<Border>(((StackPanel)hint.Child!).Children[0]); var text = Assert.IsType<TextBlock>(key.Child);
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Disabled.Background"), XyuiBatchTestHost.ColorOf(key.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Disabled.Border"), XyuiBatchTestHost.ColorOf(key.BorderBrush));
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Disabled.Text"), XyuiBatchTestHost.ColorOf(text.Foreground)); window.Close();
    });

    [Fact]
    public void Shortcut_surface_keeps_light_dark_panel_semantics() => _fx.Run(() =>
    {
        var app = XyuiBatchTestHost.Prepare(); app.RequestedThemeVariant = ThemeVariant.Light; var hint = new XYShortcutHint { Shortcut = "Ctrl+S" }; var window = XyuiBatchTestHost.Show(hint);
        Assert.Equal(XyuiBatchTestHost.Token("XY.Surface.PanelAlt"), XyuiBatchTestHost.ColorOf(hint.Background)); app.RequestedThemeVariant = ThemeVariant.Dark;
        Assert.Equal(XyuiBatchTestHost.Token("XY.Surface.PanelAlt", true), XyuiBatchTestHost.ColorOf(hint.Background)); window.Close(); app.RequestedThemeVariant = ThemeVariant.Light;
    });
}
