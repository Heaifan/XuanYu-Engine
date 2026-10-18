using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace XuanYu.Editor.UI.XycControl;

internal sealed partial class XycUiRuntime
{
    XycUiCaptureState CaptureAvaloniaWindowTop(string fileName, Top top,
        XycUiRegionState region, XycUiWindowState client, CancellationToken cancellationToken)
    {
        window.UpdateLayout();
        if (!top.IsEffectivelyVisible || top.Bounds.Width <= 0 || top.Bounds.Height <= 0)
            throw new InvalidOperationException("生产 TOP 未完成可见布局。");
        var scale = window.RenderScaling;
        var size = new PixelSize((int)Math.Ceiling(window.Bounds.Width * scale),
            (int)Math.Ceiling(window.Bounds.Height * scale));
        var left = (int)Math.Round(region.X * scale);
        var topPx = (int)Math.Round(region.Y * scale);
        if (left < 0 || topPx < 0 || left + region.WidthPx > size.Width ||
            topPx + region.HeightPx > size.Height)
            throw new InvalidOperationException("TOP 区域超出 Avalonia Window 渲染范围。");
        var image = Path.Combine(evidenceDirectory!, fileName);
        var metadata = Path.ChangeExtension(image, ".json");
        var hwnd = GetHwnd();
        var foreground = XycUiWindowNative.GetForegroundWindow();
        var previous = XycUiWindowNative.GetWindow(hwnd, XycUiWindowNative.PreviousWindow);
        using var source = new RenderTargetBitmap(size, new Vector(96 * scale, 96 * scale));
        source.Render(window);
        cancellationToken.ThrowIfCancellationRequested();
        using var full = new WriteableBitmap(size, new Vector(96 * scale, 96 * scale),
            PixelFormats.Bgra8888, AlphaFormat.Opaque);
        using var sourceFrame = full.Lock();
        source.CopyPixels(sourceFrame);
        using var crop = new WriteableBitmap(new PixelSize(region.WidthPx, region.HeightPx),
            new Vector(96 * scale, 96 * scale), PixelFormats.Bgra8888, AlphaFormat.Opaque);
        using (var cropFrame = crop.Lock())
            for (var row = 0; row < region.HeightPx; row++)
            {
                var bytes = new byte[region.WidthPx * 4];
                Marshal.Copy(sourceFrame.Address + (topPx + row) * sourceFrame.RowBytes +
                    left * 4, bytes, 0, bytes.Length);
                Marshal.Copy(bytes, 0, cropFrame.Address + row * cropFrame.RowBytes, bytes.Length);
            }
        var foregroundUnchanged = XycUiWindowNative.GetForegroundWindow() == foreground;
        var zOrderUnchanged = XycUiWindowNative.GetWindow(hwnd,
            XycUiWindowNative.PreviousWindow) == previous;
        if (!foregroundUnchanged || !zOrderUnchanged)
            throw new InvalidOperationException("Avalonia 采集期间窗口前台状态或 Z 序发生变化。");
        crop.Save(image);
        var state = new XycUiCaptureState(image, metadata,
            Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(image))),
            region.WidthPx, region.HeightPx, region, client, InspectThemeCore(), BuildIdentity(),
            "Avalonia.RenderTargetBitmap;software-render;source=production-Window;crop=TOP;z-order-unchanged",
            DateTimeOffset.UtcNow, foregroundUnchanged, zOrderUnchanged,
            "avaloniaWindow", false, false, true, true, "EXTERNAL_COMPARISON_REQUIRED");
        File.WriteAllText(metadata, JsonSerializer.Serialize(state,
            new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true }));
        return state;
    }
}
