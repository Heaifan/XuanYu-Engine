using System.Runtime.InteropServices;

namespace XuanYu.Editor.UI;

static partial class Win32ViewportHost
{
    const uint WM_PAINT = 0x000F, WM_ERASEBKGND = 0x0014, WM_NCHITTEST = 0x0084;
    const int HTTRANSPARENT = -1, TRANSPARENT = 1;
    const uint DT_LEFT = 0x0000, DT_VCENTER = 0x0004, DT_SINGLELINE = 0x0020;

    static nint RouteScaleWndProc(nint hWnd, uint msg, nint wParam, nint lParam)
    {
        if (msg == WM_NCHITTEST) return HTTRANSPARENT;
        if (msg == WM_PAINT) return PaintScaleIndicator(hWnd);
        if (msg == WM_ERASEBKGND) return 1;
        return DefWindowProc(hWnd, msg, wParam, lParam);
    }

    static nint PaintScaleIndicator(nint hwnd)
    {
        if (!ScaleStates.TryGetValue(hwnd, out var state)) return 0;
        state.PaintCount++;
        var paint = new PAINTSTRUCT { rgbReserved = new byte[32] };
        var hdc = BeginPaint(hwnd, ref paint);
        var area = new RECT { left = 0, top = 0, right = 400, bottom = 400 };
        var background = CreateSolidBrush(Rgb(246, 248, 250));
        FillRect(hdc, ref area, background);
        DeleteObject(background);
        var line = new RECT { left = Dip(6, state.Dpi), top = Dip(6, state.Dpi),
            right = Dip(6, state.Dpi) + state.BarWidth, bottom = Dip(8, state.Dpi) };
        var lineBrush = CreateSolidBrush(Rgb(36, 55, 68));
        FillRect(hdc, ref line, lineBrush);
        DeleteObject(lineBrush);
        SetBkMode(hdc, TRANSPARENT);
        SetTextColor(hdc, Rgb(36, 55, 68));
        var text = new RECT { left = Dip(6, state.Dpi), top = Dip(11, state.Dpi),
            right = 400, bottom = Dip(31, state.Dpi) };
        DrawTextW(hdc, state.Text, -1, ref text, DT_LEFT | DT_VCENTER | DT_SINGLELINE);
        EndPaint(hwnd, ref paint);
        if (ScaleProbeSinks.TryGetValue(hwnd, out var sink))
            sink(GetScaleIndicatorProbe(hwnd));
        return 0;
    }

    static int Dip(int value, double dpi) => Math.Max(1, (int)Math.Round(value * dpi));
    static uint Rgb(byte r, byte g, byte b) => (uint)(r | (g << 8) | (b << 16));

    [StructLayout(LayoutKind.Sequential)]
    struct PAINTSTRUCT
    {
        public nint hdc; public int fErase; public RECT rcPaint;
        public int fRestore; public int fIncUpdate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public byte[] rgbReserved;
    }

    [DllImport("user32")] static extern nint BeginPaint(nint hwnd, ref PAINTSTRUCT paint);
    [DllImport("user32")] [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool EndPaint(nint hwnd, ref PAINTSTRUCT paint);
    [DllImport("user32")] static extern int FillRect(nint hdc, ref RECT rect, nint brush);
    [DllImport("gdi32")] static extern nint CreateSolidBrush(uint color);
    [DllImport("gdi32")] [return: MarshalAs(UnmanagedType.Bool)] static extern bool DeleteObject(nint obj);
    [DllImport("gdi32")] static extern uint SetTextColor(nint hdc, uint color);
    [DllImport("gdi32")] static extern int SetBkMode(nint hdc, int mode);
    [DllImport("user32", CharSet = CharSet.Unicode)]
    static extern int DrawTextW(nint hdc, string text, int count, ref RECT rect, uint format);
}
