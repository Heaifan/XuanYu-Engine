using System.Collections.Concurrent;

namespace XuanYu.Editor.UI;

static partial class Win32ViewportHost
{
    const uint WM_MOUSEMOVE = 0x0200, WM_LBUTTONDOWN = 0x0201, WM_LBUTTONUP = 0x0202;
    const uint WM_CAPTURECHANGED = 0x0215, WM_KILLFOCUS = 0x0008;
    const int MK_LBUTTON = 0x0001;
    static readonly ConcurrentDictionary<nint, Action<NativePointerMessage>> InputSinks = new();

    public static void SetInputSink(nint hwnd, Action<NativePointerMessage>? sink)
    {
        if (hwnd == 0) return;
        if (sink is null) InputSinks.TryRemove(hwnd, out _);
        else InputSinks[hwnd] = sink;
    }

    static nint RouteWndProc(nint hWnd, uint msg, nint wParam, nint lParam)
    {
        if (InputSinks.TryGetValue(hWnd, out var sink) && IsPointerMessage(msg))
        {
            if (msg == WM_LBUTTONDOWN) SetCapture(hWnd);
            if (msg == WM_LBUTTONUP) ReleaseCapture();
            sink(new NativePointerMessage(msg, (int)wParam, LoWord(lParam), HiWord(lParam)));
        }
        return DefWindowProc(hWnd, msg, wParam, lParam);
    }

    static bool IsPointerMessage(uint msg) =>
        msg is WM_LBUTTONDOWN or WM_MOUSEMOVE or WM_LBUTTONUP or WM_CAPTURECHANGED or WM_KILLFOCUS;

    static int LoWord(nint value) => unchecked((short)((long)value & 0xffff));
    static int HiWord(nint value) => unchecked((short)(((long)value >> 16) & 0xffff));

    [System.Runtime.InteropServices.DllImport("user32")]
    static extern nint SetCapture(nint hWnd);
    [System.Runtime.InteropServices.DllImport("user32")]
    [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
    static extern bool ReleaseCapture();
}
