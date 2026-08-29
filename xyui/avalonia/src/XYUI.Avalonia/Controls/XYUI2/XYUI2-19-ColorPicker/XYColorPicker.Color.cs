using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public partial class XYColorPicker
{
    void UpdateHsv(Color color)
    {
        var r = color.R / 255d; var g = color.G / 255d; var b = color.B / 255d;
        var max = Math.Max(r, Math.Max(g, b)); var min = Math.Min(r, Math.Min(g, b)); var delta = max - min;
        Value = max; Saturation = max == 0 ? 0 : delta / max;
        Hue = delta == 0 ? 0 : max == r ? 60 * ((g - b) / delta % 6) : max == g ? 60 * ((b - r) / delta + 2) : 60 * ((r - g) / delta + 4);
        if (Hue < 0) Hue += 360;
    }
    internal void SetFromHsv(double hue, double saturation, double value, byte? alpha = null)
    {
        var c = HsvToColor(hue, saturation, value, alpha ?? Color.A); SetColor(c);
    }
    internal static Color HsvToColor(double hue, double saturation, double value, byte alpha)
    {
        var chroma = value * saturation; var x = chroma * (1 - Math.Abs(hue / 60 % 2 - 1)); var m = value - chroma;
        var rgb = hue switch { < 60 => (chroma, x, 0d), < 120 => (x, chroma, 0d), < 180 => (0d, chroma, x), < 240 => (0d, x, chroma), < 300 => (x, 0d, chroma), _ => (chroma, 0d, x) };
        return Color.FromArgb(alpha, ToByte(rgb.Item1 + m), ToByte(rgb.Item2 + m), ToByte(rgb.Item3 + m));
    }
    static byte ToByte(double value) => (byte)Math.Clamp(Math.Round(value * 255), 0, 255);
}
