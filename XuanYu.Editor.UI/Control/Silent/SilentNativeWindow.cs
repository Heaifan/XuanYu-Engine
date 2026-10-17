using System.Runtime.InteropServices;

namespace XuanYu.Editor.UI;

internal sealed class SilentNativeWindow : IDisposable
{
    const int WsPopup = unchecked((int)0x80000000);
    const int WsVisible = 0x10000000;
    const int GwlStyle = -16;
    static readonly WindowProcedure Callback = DefWindowProc;
    static readonly nint CallbackPointer = Marshal.GetFunctionPointerForDelegate(Callback);
    readonly nint _hwnd;

    SilentNativeWindow(nint hwnd) => _hwnd = hwnd;
    internal nint NativeHandle => _hwnd;
    internal static nint ModuleHandle => GetModuleHandleW(null);
    public long Handle => _hwnd.ToInt64();
    public bool Exists => IsWindow(_hwnd);
    public bool Visible => IsWindowVisible(_hwnd);
    public bool HasVisibleStyle => (GetWindowLongPtr(_hwnd, GwlStyle).ToInt64() & WsVisible) != 0;
    public (int Width, int Height) ClientSize
    {
        get
        {
            var rect = default(Rect);
            if (_hwnd == 0 || !GetClientRect(_hwnd, ref rect)) return (0, 0);
            return (rect.Right - rect.Left, rect.Bottom - rect.Top);
        }
    }

    public static SilentNativeWindow Create(int width, int height)
    {
        if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException("Silent HWND requires Windows.");
        var instance = ModuleHandle;
        var name = $"XuanYuSilentVulkan-{Environment.ProcessId}";
        var windowClass = new WindowClass { Procedure = CallbackPointer, Instance = instance, ClassName = name };
        if (RegisterClassW(ref windowClass) == 0 && Marshal.GetLastWin32Error() != 1410)
            throw new InvalidOperationException($"RegisterClassW failed: {Marshal.GetLastWin32Error()}");
        var hwnd = CreateWindowExW(0, name, "XYC Silent Vulkan Host", WsPopup,
            0, 0, width, height, 0, 0, instance, 0);
        if (hwnd == 0) throw new InvalidOperationException($"CreateWindowExW failed: {Marshal.GetLastWin32Error()}");
        var result = new SilentNativeWindow(hwnd);
        if (result.Visible || result.HasVisibleStyle)
        {
            result.Dispose();
            throw new InvalidOperationException("Silent HWND unexpectedly became visible.");
        }
        return result;
    }

    public bool IsOwnProcessForeground()
    {
        var foreground = GetForegroundWindow();
        if (foreground == 0) return false;
        GetWindowThreadProcessId(foreground, out var processId);
        return processId == (uint)Environment.ProcessId;
    }

    public void Dispose() => DestroyWindow(_hwnd);
    static nint DefWindowProc(nint hwnd, uint msg, nint wParam, nint lParam) => DefWindowProcW(hwnd, msg, wParam, lParam);
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct WindowClass { public uint Style; public nint Procedure; public int ClassExtra, WindowExtra; public nint Instance, Icon, Cursor, Background; public string? MenuName; public string ClassName; }
    [StructLayout(LayoutKind.Sequential)] struct Rect { public int Left, Top, Right, Bottom; }
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] delegate nint WindowProcedure(nint hwnd, uint msg, nint wParam, nint lParam);
    [DllImport("kernel32", EntryPoint = "GetModuleHandleW", CharSet = CharSet.Unicode, SetLastError = true)] static extern nint GetModuleHandleW(string? name);
    [DllImport("user32", EntryPoint = "RegisterClassW", CharSet = CharSet.Unicode, SetLastError = true)] static extern ushort RegisterClassW(ref WindowClass windowClass);
    [DllImport("user32", EntryPoint = "CreateWindowExW", CharSet = CharSet.Unicode, SetLastError = true)] static extern nint CreateWindowExW(int ex, string cls, string title, int style, int x, int y, int width, int height, nint parent, nint menu, nint instance, nint parameter);
    [DllImport("user32", EntryPoint = "DefWindowProcW")] static extern nint DefWindowProcW(nint hwnd, uint msg, nint wParam, nint lParam);
    [DllImport("user32", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] static extern bool DestroyWindow(nint hwnd);
    [DllImport("user32", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] static extern bool GetClientRect(nint hwnd, ref Rect rect);
    [DllImport("user32")] [return: MarshalAs(UnmanagedType.Bool)] static extern bool IsWindow(nint hwnd);
    [DllImport("user32")] [return: MarshalAs(UnmanagedType.Bool)] static extern bool IsWindowVisible(nint hwnd);
    [DllImport("user32")] static extern nint GetForegroundWindow();
    [DllImport("user32")] static extern uint GetWindowThreadProcessId(nint hwnd, out uint processId);
    [DllImport("user32", EntryPoint = "GetWindowLongPtrW")] static extern nint GetWindowLongPtr(nint hwnd, int index);
}
