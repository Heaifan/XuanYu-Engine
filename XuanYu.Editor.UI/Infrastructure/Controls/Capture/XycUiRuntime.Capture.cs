using System.Security.Cryptography;
using System.Text.Json;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace XuanYu.Editor.UI.XycControl;

internal sealed partial class XycUiRuntime
{
    XycUiCaptureState CaptureTopCore(string fileName, string backend,
        bool temporaryTopmost, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(evidenceDirectory))
            throw new InvalidOperationException("XYC UI evidence directory is not configured.");
        var top = FindTop();
        top.UpdateLayout();
        if (!top.IsEffectivelyVisible || top.Bounds.Width <= 0 || top.Bounds.Height <= 0)
            throw new InvalidOperationException("生产 TOP 未完成可见布局。");
        var region = InspectTopCore();
        var client = InspectWindowCore();
        cancellationToken.ThrowIfCancellationRequested();
        var image = Path.Combine(evidenceDirectory, fileName);
        var metadata = Path.ChangeExtension(image, ".json");
        if (File.Exists(image) || File.Exists(metadata))
            throw new XycUiCommandException("EVIDENCE_ALREADY_EXISTS");
        if (backend == "avaloniaWindow")
            return CaptureAvaloniaWindowTop(fileName, top, region, client, cancellationToken);
        if (backend != "gdiCompatibility")
            throw new XycUiCommandException("UNSUPPORTED_BACKEND");
        var width = region.WidthPx;
        var height = region.HeightPx;
        var capturedAt = DateTimeOffset.UtcNow;
        var hwnd = GetHwnd();
        var previous = XycUiWindowNative.GetWindow(hwnd, XycUiWindowNative.PreviousWindow);
        var foreground = XycUiWindowNative.GetForegroundWindow();
        var flags = XycUiWindowNative.NoMove | XycUiWindowNative.NoSize |
            XycUiWindowNative.NoActivate | XycUiWindowNative.ShowWindow;
        var insertAfter = temporaryTopmost ? XycUiWindowNative.Topmost : 0;
        if (!XycUiWindowNative.SetWindowPos(hwnd, insertAfter, 0, 0, 0, 0, flags))
            throw new InvalidOperationException("窗口无法在不激活的情况下进入桌面采集层。");
        byte[] crop;
        try
        {
            if (XycUiWindowNative.DwmFlush() < 0)
                throw new InvalidOperationException("DWM 未完成窗口合成。");
            crop = XycUiNativeCapture.ReadScreen(region.Xpx, region.Ypx, width, height);
            cancellationToken.ThrowIfCancellationRequested();
        }
        finally
        {
            if (previous != 0)
                XycUiWindowNative.SetWindowPos(hwnd, previous, 0, 0, 0, 0,
                    XycUiWindowNative.NoMove | XycUiWindowNative.NoSize |
                    XycUiWindowNative.NoActivate);
        }
        if (XycUiWindowNative.GetForegroundWindow() != foreground)
            throw new InvalidOperationException("截图期间前台窗口发生变化。");
        for (var alpha = 3; alpha < crop.Length; alpha += 4) crop[alpha] = 255;
        using (var bitmap = new WriteableBitmap(new PixelSize(width, height), new Vector(96, 96),
                   PixelFormats.Bgra8888, AlphaFormat.Opaque))
        {
            using var framebuffer = bitmap.Lock();
            for (var row = 0; row < height; row++)
                Marshal.Copy(crop, row * width * 4, framebuffer.Address + row * framebuffer.RowBytes,
                    width * 4);
            bitmap.Save(image);
        }
        var state = new XycUiCaptureState(image, metadata,
            Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(image))), width, height,
            region, client, InspectThemeCore(), BuildIdentity(),
            CaptureMethod(temporaryTopmost), capturedAt,
            XycUiWindowNative.GetForegroundWindow() == foreground,
            XycUiWindowNative.GetWindow(hwnd, XycUiWindowNative.PreviousWindow) == previous,
            "gdiCompatibility", true, false, true, false, "REFERENCE_ONLY");
        File.WriteAllText(metadata, JsonSerializer.Serialize(state,
            new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true }));
        return state;
    }

    static string BuildIdentity() => Environment.GetEnvironmentVariable("XYC_BUILD_IDENTITY")
        ?? $"Editor.UI-MVID={typeof(App).Assembly.ManifestModule.ModuleVersionId:D}";

    static string CaptureMethod(bool topmost) => topmost
        ? "GDI.BitBlt;source=DWM-desktop-composition;temporary-HWND_TOPMOST;SWP_NOACTIVATE;restore-z-order;crop=TOP"
        : "GDI.BitBlt;source=DWM-desktop-composition;temporary-HWND_TOP;SWP_NOACTIVATE;crop=TOP";

}
