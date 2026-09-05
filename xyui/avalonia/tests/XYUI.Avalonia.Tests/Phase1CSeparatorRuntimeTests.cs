using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Spatial;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class Phase1CSeparatorRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public Phase1CSeparatorRuntimeTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Theory]
    [InlineData(XyuiSeparatorVariant.Default, false, 0)]
    [InlineData(XyuiSeparatorVariant.Header, false, 0)]
    [InlineData(XyuiSeparatorVariant.Panel, false, 0)]
    [InlineData(XyuiSeparatorVariant.Section, false, 8)]
    [InlineData(XyuiSeparatorVariant.ListRow, false, 16)]
    [InlineData(XyuiSeparatorVariant.VerticalSplit, true, 0)]
    public void Variant_maps_to_canonical_orientation_and_spacing(XyuiSeparatorVariant variant, bool vertical, double inset) => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var separator = new XYSeparator { Variant = variant };
        var window = XyuiBatchTestHost.Show(separator);
        if (vertical) { Assert.True(double.IsNaN(separator.Height)); Assert.Equal(XyuiSpatialTokens.BorderWidthDefault, separator.Width); }
        else { Assert.Equal(XyuiSpatialTokens.BorderWidthDefault, separator.Height); Assert.True(double.IsNaN(separator.Width)); }
        Assert.Equal(new Thickness(inset, 0, inset, 0), separator.Margin); window.Close();
    });

    [Fact]
    public void Divider_color_switches_with_light_and_dark_theme() => _fx.Run(() =>
    {
        var app = XyuiBatchTestHost.Prepare(); app.RequestedThemeVariant = ThemeVariant.Light;
        var separator = new XYSeparator(); var window = XyuiBatchTestHost.Show(separator);
        Assert.Equal(XyuiBatchTestHost.Token("XY.Divider.Default"), XyuiBatchTestHost.ColorOf(separator.Background));
        app.RequestedThemeVariant = ThemeVariant.Dark;
        Assert.Equal(XyuiBatchTestHost.Token("XY.Divider.Default", true), XyuiBatchTestHost.ColorOf(separator.Background));
        window.Close(); app.RequestedThemeVariant = ThemeVariant.Light;
    });
}
