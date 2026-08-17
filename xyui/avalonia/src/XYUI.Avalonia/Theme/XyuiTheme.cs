using Avalonia.Controls;
using Avalonia.Media;
using XYUI.Avalonia.Foundation;
using XYUI.Avalonia.Spatial;
using XYUI.Avalonia.Typography;

namespace XYUI.Avalonia.Theme;

// 双主题 ResourceDictionary 构建器：Light/Dark 均来自 Canonical 成对值（86 对，非伪造）
public static class XyuiTheme
{
    public const string LightName = "Light";
    public const string DarkName = "Dark";

    public static ResourceDictionary CreateLight() => Create(dark: false);

    public static ResourceDictionary CreateDark() => Create(dark: true);

    private static ResourceDictionary Create(bool dark)
    {
        var dict = new ResourceDictionary();
        foreach (var token in XyuiColorTokens.All)
        {
            dict[XyuiColorTokens.BrushKey(token.TokenId)] =
                new SolidColorBrush(token.ToColor(dark));
        }
        Merge(dict, XyuiTypography.CreateResources());
        Merge(dict, XyuiSpatial.CreateResources());
        return dict;
    }

    private static void Merge(ResourceDictionary target, ResourceDictionary source)
    {
        foreach (var key in source.Keys.Cast<string>().ToArray())
        {
            target[key] = source[key];
        }
    }
}
