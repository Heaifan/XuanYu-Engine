using System.Globalization;
using Avalonia;
using Avalonia.Media;
using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

public static class MapLabelRasterizer
{
    const string FontFallback = "Microsoft YaHei UI, Segoe UI, Noto Sans CJK SC";
    const double PaddingDip = 2;

    public static RenderLabelBitmap Rasterize(RenderVectorOverlayLabel label)
    {
        if (string.IsNullOrWhiteSpace(label.Text)) throw new InvalidOperationException("地图标签文本为空");
        var typeface = new Typeface(new FontFamily(FontFallback), FontStyle.Normal, FontWeight.SemiBold);
        if (!FontManager.Current.TryGetGlyphTypeface(typeface, out _))
            throw new InvalidOperationException("地图标签字体不可用：" + FontFallback);
        var formatted = new FormattedText(label.Text, CultureInfo.CurrentUICulture,
            FlowDirection.LeftToRight, typeface, label.FontSizeDip, Brushes.White);
        var widthDip = Math.Max(1, Math.Ceiling(formatted.Width + PaddingDip * 2));
        var heightDip = Math.Max(1, Math.Ceiling(formatted.Height + PaddingDip * 2));
        var dpi = Math.Max(.5, label.DpiScale);
        var size = new PixelSize((int)Math.Ceiling(widthDip * dpi), (int)Math.Ceiling(heightDip * dpi));
        var geometry = formatted.BuildGeometry(new(PaddingDip, PaddingDip)) ??
            throw new InvalidOperationException("地图标签字形几何为空");
        var pixels = RasterizeCoverage(geometry, size, dpi);
        if (!pixels.Any(value => value != 0)) throw new InvalidOperationException("地图标签栅格结果为空");
        return new(label.CacheKey, size.Width, size.Height, size.Width * 4,
            RenderLabelPixelFormat.Bgra8888Unpremultiplied, pixels);
    }

    static byte[] RasterizeCoverage(Geometry geometry, PixelSize size, double dpi)
    {
        const int samples = 4;
        var pixels = new byte[size.Width * size.Height * 4];
        for (var y = 0; y < size.Height; y++)
            for (var x = 0; x < size.Width; x++)
            {
                var hits = 0;
                for (var sy = 0; sy < samples; sy++)
                    for (var sx = 0; sx < samples; sx++)
                        if (geometry.FillContains(new((x + (sx + .5) / samples) / dpi,
                            (y + (sy + .5) / samples) / dpi))) hits++;
                var coverage = (byte)Math.Round(hits * 255d / (samples * samples));
                var index = (y * size.Width + x) * 4;
                pixels[index] = pixels[index + 1] = pixels[index + 2] = coverage;
                pixels[index + 3] = 255;
            }
        return pixels;
    }
}
