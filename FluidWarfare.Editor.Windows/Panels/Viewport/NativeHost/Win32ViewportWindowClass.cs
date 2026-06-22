using System.ComponentModel;
using System.Runtime.InteropServices;

namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost;

/// <summary>Win32 视口子窗口类注册管理。</summary>
internal static class Win32ViewportWindowClass
{
    public const string WindowClassName = "FluidWarfareWindowsVulkanViewportHost";
    const int ClassNameAlreadyExists = 1410;
    static bool _classRegistered;

    public delegate nint WndProc(nint hwnd, uint msg, nint wParam, nint lParam);

    public static void EnsureRegistered(nint instanceHandle, WndProc wndProc)
    {
        if (_classRegistered) return;

        var ptr = Marshal.GetFunctionPointerForDelegate(wndProc);
        var cls = new WndClass
        {
            Style = 0, LpfnWndProc = ptr,
            CbClsExtra = 0, CbWndExtra = 0,
            HInstance = instanceHandle,
            HIcon = 0, HCursor = 0, HbrBackground = 0,
            LpszMenuName = null, LpszClassName = WindowClassName
        };

        var atom = RegisterClass(ref cls);
        if (atom == 0)
        {
            var err = Marshal.GetLastWin32Error();
            if (err != ClassNameAlreadyExists)
                throw new Win32Exception(err, "注册 Windows Vulkan 视口子窗口类失败。");
        }
        _classRegistered = true;
    }

    [DllImport("user32.dll", EntryPoint = "RegisterClassW", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern ushort RegisterClass(ref WndClass windowClass);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct WndClass
    {
        public uint Style;
        public nint LpfnWndProc;
        public int CbClsExtra;
        public int CbWndExtra;
        public nint HInstance;
        public nint HIcon;
        public nint HCursor;
        public nint HbrBackground;
        [MarshalAs(UnmanagedType.LPWStr)] public string? LpszMenuName;
        [MarshalAs(UnmanagedType.LPWStr)] public string LpszClassName;
    }
}
