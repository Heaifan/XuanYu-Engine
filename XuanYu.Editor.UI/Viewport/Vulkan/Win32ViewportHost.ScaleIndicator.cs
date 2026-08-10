using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace XuanYu.Editor.UI;

static partial class Win32ViewportHost
{
    const int WS_EX_NOACTIVATE = 134217728, WS_EX_TOOLWINDOW = 128;
    const int WS_POPUP = unchecked((int)0x80000000);
    const int WS_CLIPSIBLINGS = 67108864;
    const uint SWP_SHOWWINDOW = 0x0040, SWP_HIDEWINDOW = 0x0080;
    static readonly ScaleWndProcDelegate ScaleWndProc = RouteScaleWndProc;
    static readonly nint ScaleWndProcPtr = Marshal.GetFunctionPointerForDelegate(ScaleWndProc);
    static readonly Dictionary<nint, ScaleIndicatorState> ScaleStates = new();
    static readonly Dictionary<nint, Action<ScaleIndicatorProbe>> ScaleProbeSinks = new();

    public static nint CreateScaleIndicator(nint parent)
    {
        var hinstance = GetModuleHandle(null);
        var name = "XuanYuScaleIndicator";
        var wc = new WndClass { lpfnWndProc = ScaleWndProcPtr, hInstance = hinstance,
            lpszClassName = name };
        RegisterClassW(ref wc);
        return CreateWindowEx(WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW, name, "",
            WS_POPUP | WS_VISIBLE, 0, 0, 80, 34, parent, 0, hinstance, 0);
    }

    public static void SetScaleIndicatorProbeSink(nint hwnd, Action<ScaleIndicatorProbe> sink)
    {
        if (hwnd != 0) ScaleProbeSinks[hwnd] = sink;
    }

    public static void UpdateScaleIndicator(nint hwnd, bool visible, string text,
        double widthDip, double dpi, int viewportLeft, int viewportTop,
        int viewportRight, int viewportBottom)
    {
        if (hwnd == 0) return;
        dpi = double.IsFinite(dpi) && dpi > 0.0 ? dpi : 1.0;
        var barWidth = Math.Min((int)Math.Round(220 * dpi),
            Math.Max(40, (int)Math.Round(Math.Max(0.0, widthDip) * dpi)));
        var width = Math.Max(barWidth + (int)Math.Round(12 * dpi), (int)Math.Round(78 * dpi));
        var height = Math.Max(28, (int)Math.Round(34 * dpi));
        var margin = (int)Math.Round(12 * dpi);
        if (ScaleStates.TryGetValue(hwnd, out var current))
            ScaleStates[hwnd] = current with { Text = text, BarWidth = barWidth, Dpi = dpi, WindowWidth = width };
        else ScaleStates[hwnd] = new ScaleIndicatorState(text, barWidth, dpi, width);
        var x = Math.Max(viewportLeft, viewportRight - width - margin);
        var y = Math.Max(viewportTop, viewportBottom - height - margin);
        var flags = SWP_NOACTIVATE | (visible ? SWP_SHOWWINDOW : SWP_HIDEWINDOW);
        SetWindowPos(hwnd, 0, x, y, width, height, flags);
        InvalidateRect(hwnd, 0, true);
    }

    public static ScaleIndicatorProbe GetScaleIndicatorProbe(nint hwnd)
    {
        var rect = new RECT();
        var state = ScaleStates.GetValueOrDefault(hwnd);
        return new ScaleIndicatorProbe(hwnd, IsWindow(hwnd), IsWindowVisible(hwnd),
            GetWindowRect(hwnd, ref rect), rect.left, rect.top, rect.right, rect.bottom,
            state?.Text ?? "", state?.WindowWidth ?? 0,
            state?.PaintCount ?? 0);
    }

    public static void DestroyScaleIndicator(nint hwnd)
    {
        if (hwnd == 0) return;
        ScaleStates.Remove(hwnd);
        ScaleProbeSinks.Remove(hwnd);
        DestroyWindow(hwnd);
    }

    sealed record ScaleIndicatorState(string Text, int BarWidth, double Dpi, int WindowWidth)
    {
        public int PaintCount { get; set; }
    }
    internal readonly record struct ScaleIndicatorProbe(
        nint Hwnd, bool IsWindow, bool IsVisible, bool HasRect,
        int Left, int Top, int Right, int Bottom, string Text,
        int WindowWidth, int PaintCount);
    delegate nint ScaleWndProcDelegate(nint hWnd, uint msg, nint wParam, nint lParam);

    [DllImport("user32")] [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool InvalidateRect(nint hWnd, nint rect, [MarshalAs(UnmanagedType.Bool)] bool erase);
    [DllImport("user32")] [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool IsWindow(nint hWnd);
    [DllImport("user32")] [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool IsWindowVisible(nint hWnd);
    [DllImport("user32")] [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetWindowRect(nint hWnd, ref RECT rect);
}
