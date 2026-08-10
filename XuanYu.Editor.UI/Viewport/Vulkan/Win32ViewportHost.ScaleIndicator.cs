using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace XuanYu.Editor.UI;

static partial class Win32ViewportHost
{
    const int WS_EX_TRANSPARENT = 32, WS_EX_NOACTIVATE = 134217728;
    const int WS_CLIPSIBLINGS = 67108864;
    const uint SWP_SHOWWINDOW = 0x0040, SWP_HIDEWINDOW = 0x0080;
    static readonly ScaleWndProcDelegate ScaleWndProc = RouteScaleWndProc;
    static readonly nint ScaleWndProcPtr = Marshal.GetFunctionPointerForDelegate(ScaleWndProc);
    static readonly Dictionary<nint, ScaleIndicatorState> ScaleStates = new();

    public static nint CreateScaleIndicator(nint parent)
    {
        var hinstance = GetModuleHandle(null);
        var name = "XuanYuScaleIndicator";
        var wc = new WndClass { lpfnWndProc = ScaleWndProcPtr, hInstance = hinstance,
            lpszClassName = name };
        RegisterClassW(ref wc);
        return CreateWindowEx(WS_EX_TRANSPARENT | WS_EX_NOACTIVATE, name, "",
            WS_CHILD | WS_VISIBLE | WS_CLIPSIBLINGS, 0, 0, 80, 34, parent, 0, hinstance, 0);
    }

    public static void UpdateScaleIndicator(nint hwnd, bool visible, string text,
        double widthDip, double dpi, int viewportWidth, int viewportHeight)
    {
        if (hwnd == 0) return;
        dpi = double.IsFinite(dpi) && dpi > 0.0 ? dpi : 1.0;
        var barWidth = Math.Max(40, (int)Math.Round(Math.Max(0.0, widthDip) * dpi));
        var width = Math.Max(barWidth + (int)Math.Round(12 * dpi), (int)Math.Round(78 * dpi));
        var height = Math.Max(28, (int)Math.Round(34 * dpi));
        var margin = (int)Math.Round(12 * dpi);
        ScaleStates[hwnd] = new ScaleIndicatorState(text, barWidth, dpi);
        var x = Math.Max(0, viewportWidth - width - margin);
        var y = Math.Max(0, viewportHeight - height - margin);
        var flags = SWP_NOACTIVATE | (visible ? SWP_SHOWWINDOW : SWP_HIDEWINDOW);
        SetWindowPos(hwnd, 0, x, y, width, height, flags);
        InvalidateRect(hwnd, 0, true);
    }

    public static void DestroyScaleIndicator(nint hwnd)
    {
        if (hwnd == 0) return;
        ScaleStates.Remove(hwnd);
        DestroyWindow(hwnd);
    }

    sealed record ScaleIndicatorState(string Text, int BarWidth, double Dpi);
    delegate nint ScaleWndProcDelegate(nint hWnd, uint msg, nint wParam, nint lParam);

    [DllImport("user32")] [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool InvalidateRect(nint hWnd, nint rect, [MarshalAs(UnmanagedType.Bool)] bool erase);
}
