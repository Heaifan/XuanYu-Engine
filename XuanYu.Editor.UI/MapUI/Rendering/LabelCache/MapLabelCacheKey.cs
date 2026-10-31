using System.Globalization;

namespace XuanYu.Editor.UI;

static class MapLabelCacheKey
{
    public static string Region(string text, double dpiScale) => string.Join('|',
        "map-region", text, "editor-map-label", "13", dpiScale.ToString("0.###", CultureInfo.InvariantCulture));
}
