using Avalonia;

namespace XuanYu.Editor.UI.XycControl;

internal sealed partial class XycUiRuntime
{
    XycUiWindowState InspectWindowCore()
    {
        var hwnd = GetHwnd();
        if (!XycUiWindowNative.GetClientRect(hwnd, out var client) ||
            !XycUiWindowNative.GetWindowRect(hwnd, out var outer))
            throw new InvalidOperationException("Win32 窗口几何读取失败。");
        var dpi = checked((int)XycUiWindowNative.GetDpiForWindow(hwnd));
        if (dpi <= 0) throw new InvalidOperationException("窗口 DPI 不可用。");
        return new(Environment.ProcessId, $"0x{hwnd.ToInt64():X}", window.IsVisible,
            XycUiWindowNative.GetForegroundWindow() == hwnd, window.WindowState.ToString(),
            client.Right * 96d / dpi, client.Bottom * 96d / dpi,
            client.Right, client.Bottom, dpi, dpi / 96d);
    }

    XycUiWindowState ResizeWindowCore(double widthDip, double heightDip)
    {
        var hwnd = GetHwnd();
        if (!XycUiWindowNative.GetWindowRect(hwnd, out var outer) ||
            !XycUiWindowNative.GetClientRect(hwnd, out var client))
            throw new InvalidOperationException("窗口尺寸基线读取失败。");
        var dpi = XycUiWindowNative.GetDpiForWindow(hwnd);
        var width = checked((int)Math.Round(widthDip * dpi / 96d));
        var height = checked((int)Math.Round(heightDip * dpi / 96d));
        var frameWidth = outer.Right - outer.Left - client.Right;
        var frameHeight = outer.Bottom - outer.Top - client.Bottom;
        window.Width = widthDip;
        window.Height = heightDip;
        window.UpdateLayout();
        var flags = XycUiWindowNative.NoZOrder | XycUiWindowNative.NoActivate;
        if (!XycUiWindowNative.SetWindowPos(hwnd, 0, 0, 0,
                width + frameWidth, height + frameHeight, flags))
            throw new InvalidOperationException("SetWindowPos 未能调整生产窗口。");
        return InspectWindowCore();
    }
}
