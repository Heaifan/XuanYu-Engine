using System.Runtime.InteropServices;

namespace XuanYu.Editor.UI;

static partial class Win32ViewportHost
{
    const int WS_CHILD = 0x40000000, WS_VISIBLE = 0x10000000;
    const int SWP_NOZORDER = 0x0004, SWP_NOACTIVATE = 0x0010;
    static readonly WndProcDelegate WndProc = DefWindowProc;
    static readonly nint WndProcPtr = Marshal.GetFunctionPointerForDelegate(WndProc);

    public static nint CreateChild(nint parent)
    {
        var hinstance = GetModuleHandle(null);
        var name = "XuanYuVulkanViewport";
        RegisterClass(name, hinstance);
        return CreateWindowEx(0, name, "", WS_CHILD | WS_VISIBLE, 0, 0, 16, 16, parent, 0, hinstance, 0);
    }

    public static void Resize(nint hwnd, int width, int height) =>
        SetWindowPos(hwnd, 0, 0, 0, width, height, SWP_NOZORDER | SWP_NOACTIVATE);

    public static void Destroy(nint hwnd)
    {
        if (hwnd != 0) DestroyWindow(hwnd);
    }

    public static nint ModuleHandle => GetModuleHandle(null);

    static void RegisterClass(string name, nint hinstance)
    {
        var wc = new WndClass { lpfnWndProc = WndProcPtr, hInstance = hinstance, lpszClassName = name };
        RegisterClassW(ref wc);
    }

    delegate nint WndProcDelegate(nint hWnd, uint msg, nint wParam, nint lParam);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct WndClass
    {
        public uint style; public nint lpfnWndProc; public int cbClsExtra; public int cbWndExtra;
        public nint hInstance; public nint hIcon; public nint hCursor; public nint hbrBackground;
        public string? lpszMenuName; public string lpszClassName;
    }

    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    static extern nint GetModuleHandle(string? lpModuleName);
    [DllImport("user32", EntryPoint = "RegisterClassW", CharSet = CharSet.Unicode)]
    static extern ushort RegisterClassW(ref WndClass lpWndClass);
    [DllImport("user32", EntryPoint = "CreateWindowExW", CharSet = CharSet.Unicode)]
    static extern nint CreateWindowEx(int ex, string cls, string name, int style, int x, int y, int w, int h, nint parent, nint menu, nint inst, nint param);
    [DllImport("user32")] static extern nint DefWindowProc(nint hWnd, uint msg, nint wParam, nint lParam);
    [DllImport("user32")] [return: MarshalAs(UnmanagedType.Bool)] static extern bool DestroyWindow(nint hWnd);
    [DllImport("user32")] [return: MarshalAs(UnmanagedType.Bool)] static extern bool SetWindowPos(nint hWnd, nint after, int x, int y, int cx, int cy, uint flags);
}
