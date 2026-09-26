using System.Globalization;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
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
        using var source = new RenderTargetBitmap(size, new Vector(96 * dpi, 96 * dpi));
        using (var context = source.CreateDrawingContext()) context.DrawText(formatted, new(PaddingDip, PaddingDip));
        using var target = new WriteableBitmap(size, new Vector(96 * dpi, 96 * dpi), PixelFormats.Bgra8888, AlphaFormat.Unpremul);
        using var framebuffer = target.Lock();
        source.CopyPixels(framebuffer);
        var pixels = new byte[framebuffer.RowBytes * size.Height];
        Marshal.Copy(framebuffer.Address, pixels, 0, pixels.Length);
        if (!pixels.Any(value => value != 0)) throw new InvalidOperationException("地图标签栅格结果为空");
        return new(label.CacheKey, size.Width, size.Height, framebuffer.RowBytes,
            RenderLabelPixelFormat.Bgra8888Unpremultiplied, pixels);
    }
}
