using System.Globalization;
using Avalonia.Media;

namespace XuanYu.Editor.UI;

static class InspectorColorValue
{
    public static Color Parse(string value)
    {
        var text = value.Trim().TrimStart('#');
        if (text.Length == 6 &&
            uint.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var rgb))
            return Color.FromRgb((byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        return Colors.Transparent;
    }

    public static uint ToRgb(Color color) =>
        ((uint)color.R << 16) | ((uint)color.G << 8) | color.B;
}
