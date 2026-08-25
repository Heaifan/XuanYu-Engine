using Avalonia.Controls;
using Avalonia.Media;
using XYUI.Avalonia.Foundation;

namespace XYUI.Avalonia.Theme;

public static class XyuiSectionTitleResources
{
    public const string HeaderBackgroundKey = "XY.SectionTitle.Brush.HeaderBackground";
    public const string LeftMarkKey = "XY.SectionTitle.Brush.LeftMark";
    public const string TextKey = "XY.SectionTitle.Brush.Text";

    public static IReadOnlyList<string> CanonicalLightHexValues =>
        ["#EEF3F6", "#526873", "#243744"];

    public static ResourceDictionary Create(bool dark)
    {
        var resources = new ResourceDictionary
        {
            [HeaderBackgroundKey] = Brush(dark, "#EEF3F6", "XY.Surface.PanelAlt"),
            [LeftMarkKey] = Brush(dark, "#526873", "XY.Text.Secondary"),
            [TextKey] = Brush(dark, "#243744", "XY.Text.Primary"),
        };
        return resources;
    }

    static SolidColorBrush Brush(bool dark, string lightHex, string fallbackToken)
    {
        if (dark && XyuiColorTokens.TryFind(fallbackToken, out var token))
            return new SolidColorBrush(token.ToColor(true));
        return new SolidColorBrush(Color.Parse(lightHex));
    }
}
