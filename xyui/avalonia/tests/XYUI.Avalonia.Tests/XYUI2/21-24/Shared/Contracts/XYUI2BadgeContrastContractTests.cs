using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2BadgeContrastContractTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2BadgeContrastContractTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Badge_accent_text_has_readable_light_theme_contrast()
    {
        _fx.Run(() =>
        {
            XyuiBatchTestHost.Prepare();
            var window = XyuiBatchTestHost.Show(new XYBadge { Text = "区域面", Variant = XyuiBadgeVariant.Accent });
            var badge = (XYBadge)window.Content!;
            var text = badge.GetVisualDescendants().OfType<TextBlock>().Single();
            var path = badge.GetVisualDescendants().OfType<VectorPath>().Single();
            var foreground = XyuiBatchTestHost.ColorOf(text.Foreground);
            var background = XyuiBatchTestHost.ColorOf(path.Fill);
            Assert.True(Contrast(foreground, background) >= 4.5,
                $"foreground={foreground}, background={background}");
            window.Close();
        });
    }

    static double Contrast(Color foreground, Color background)
    {
        var a = Luminance(foreground); var b = Luminance(background);
        return (Math.Max(a, b) + 0.05) / (Math.Min(a, b) + 0.05);
    }

    static double Luminance(Color color)
    {
        static double Channel(byte value)
        {
            var channel = value / 255d;
            return channel <= 0.03928 ? channel / 12.92 : Math.Pow((channel + 0.055) / 1.055, 2.4);
        }

        return 0.2126 * Channel(color.R) + 0.7152 * Channel(color.G) + 0.0722 * Channel(color.B);
    }
}
