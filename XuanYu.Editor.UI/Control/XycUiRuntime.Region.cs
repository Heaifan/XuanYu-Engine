using Avalonia;

namespace XuanYu.Editor.UI.XycControl;

internal sealed partial class XycUiRuntime
{
    XycUiRegionState InspectTopCore()
    {
        var top = FindTop();
        top.UpdateLayout();
        var origin = top.TranslatePoint(default, window)
            ?? throw new InvalidOperationException("TOP 到窗口的坐标转换失败。");
        var hwnd = GetHwnd();
        var clientOrigin = new XycUiWindowNative.NativePoint();
        if (!XycUiWindowNative.ClientToScreen(hwnd, ref clientOrigin))
            throw new InvalidOperationException("窗口客户区屏幕原点读取失败。");
        var scale = window.RenderScaling;
        return new("TOP", origin.X, origin.Y, top.Bounds.Width, top.Bounds.Height,
            clientOrigin.X + (int)Math.Round(origin.X * scale),
            clientOrigin.Y + (int)Math.Round(origin.Y * scale),
            (int)Math.Ceiling(top.Bounds.Width * scale),
            (int)Math.Ceiling(top.Bounds.Height * scale));
    }
}
