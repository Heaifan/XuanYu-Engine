using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using Avalonia.Styling;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Foundation;
using XYUI.Avalonia.Typography;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class Phase1DTooltipRichTextRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public Phase1DTooltipRichTextRuntimeTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Tooltip_keeps_content_contract_and_foundation_surface() => _fx.Run(() =>
    {
        var app = Prepare(); app.RequestedThemeVariant = ThemeVariant.Light;
        var tooltip = new XYTooltip { Content = new XYCaption { Text = "说明" } };
        var window = XyuiBatchTestHost.Show(tooltip);
        Assert.IsAssignableFrom<ContentControl>(tooltip); Assert.Equal("XYUI-1-19", tooltip.CanonicalId);
        Assert.Equal(280, tooltip.MaxWidth); Assert.Equal(400, tooltip.ShowDelay);
        Assert.Equal(new Thickness(2, 0, 0, 0), tooltip.BorderThickness); Assert.Equal(new CornerRadius(6), tooltip.CornerRadius);
        Assert.Equal(new Thickness(8, 4), tooltip.Padding); Assert.Equal(XyuiTypographyTokens.FontUi, tooltip.FontFamily.Name);
        Assert.Equal(XyuiTypographyTokens.FontSizeCaption, tooltip.FontSize);
        Assert.Equal(XyuiBatchTestHost.Token("XY.Surface.Overlay"), ColorOf(tooltip.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.Border.Color.Subtle"), ColorOf(tooltip.BorderBrush));
        app.RequestedThemeVariant = ThemeVariant.Dark;
        Assert.Equal(XyuiBatchTestHost.Token("XY.Surface.Overlay", true), ColorOf(tooltip.Background));
        window.Close(); app.RequestedThemeVariant = ThemeVariant.Light;
    });

    [Fact]
    public void Rich_text_has_only_stable_text_strong_mono_run_order() => _fx.Run(() =>
    {
        Prepare(); var rich = new XYRichText { Text = "Compiled", StrongText = "18 shaders", MonoText = "2.4 s" };
        var runs = rich.Inlines!.OfType<Run>().ToArray();
        Assert.Equal(3, runs.Length); Assert.Equal("Compiled", runs[0].Text); Assert.Equal("  18 shaders", runs[1].Text);
        Assert.Equal("  2.4 s", runs[2].Text); Assert.Equal(FontWeight.SemiBold, runs[1].FontWeight);
        Assert.Equal(XyuiTypographyTokens.FontMono, runs[2].FontFamily.Name); Assert.Equal(XyuiTypographyTokens.FontSizeMono, runs[2].FontSize);
        Assert.Equal(TextWrapping.Wrap, rich.TextWrapping); Assert.Equal("XYUI-1-20", rich.CanonicalId);
    });

    [Fact]
    public void Rich_text_disabled_uses_shared_disabled_text_token() => _fx.Run(() =>
    {
        Prepare(); var rich = new XYRichText { Text = "普通", StrongText = "强调", MonoText = "代码", IsEnabled = false }; var window = XyuiBatchTestHost.Show(rich);
        Assert.Equal(3, rich.Inlines!.OfType<Run>().Count());
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Disabled.Text"), XyuiBatchTestHost.ColorOf(rich.Foreground)); window.Close();
    });

    static Application Prepare() => XyuiBatchTestHost.Prepare();
    static Color ColorOf(IBrush? brush) => Assert.IsType<SolidColorBrush>(brush).Color;
}
