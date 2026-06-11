using System.ComponentModel;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;

namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost;

public sealed class WindowsVulkanViewportHostControl : NativeControlHost
{
    private const string WindowClassName = "FluidWarfareWindowsVulkanViewportHost";
    private const int ClassNameAlreadyExists = 1410;
    private const int WsChild = 0x40000000;
    private const int WsVisible = 0x10000000;
    private const int WsClipChildren = 0x02000000;
    private const int WsClipSiblings = 0x04000000;
    private const int WindowStyle = WsChild | WsVisible | WsClipChildren | WsClipSiblings;

    private static readonly WndProc WindowProcedure = DefWindowProc;
    private static bool _classRegistered;

    private nint _windowHandle;
    private nint _instanceHandle;
    private int _width;
    private int _height;
    private WindowsVulkanViewportHostInfo _hostInfo = WindowsVulkanViewportHostInfo.NotCreated;

    public WindowsVulkanViewportHostControl()
    {
        PropertyChanged += (_, args) =>
        {
            if (args.Property == BoundsProperty)
            {
                ResizeNativeWindowToControlBounds();
            }
        };
    }

    public WindowsVulkanViewportHostInfo GetHostInfo()
    {
        return _hostInfo;
    }

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        if (!OperatingSystem.IsWindows())
        {
            _hostInfo = new WindowsVulkanViewportHostInfo(
                WindowsVulkanViewportHostState.UnsupportedPlatform,
                "当前平台不支持 Windows Vulkan 视口子窗口。",
                "非 Windows",
                false,
                0,
                0,
                0,
                0);

            return new PlatformHandle(0, "HWND");
        }

        if (parent.Handle == 0)
        {
            _hostInfo = new WindowsVulkanViewportHostInfo(
                WindowsVulkanViewportHostState.Failed,
                "Avalonia 未提供可嵌入原生子窗口的父级句柄。",
                "Windows",
                false,
                0,
                0,
                0,
                0);

            return new PlatformHandle(0, "HWND");
        }

        try
        {
            _instanceHandle = GetModuleHandle(null);
            if (_instanceHandle == 0)
            {
                SetFailedInfo("无法获取当前进程模块句柄。");
                return new PlatformHandle(0, "HWND");
            }

            RegisterWindowClass(_instanceHandle);

            _windowHandle = CreateWindowEx(
                0,
                WindowClassName,
                "FluidWarfare Vulkan Viewport",
                WindowStyle,
                0,
                0,
                1,
                1,
                parent.Handle,
                0,
                _instanceHandle,
                0);

            if (_windowHandle == 0)
            {
                SetFailedInfo($"CreateWindowEx 失败：{new Win32Exception(Marshal.GetLastWin32Error()).Message}");
                return new PlatformHandle(0, "HWND");
            }

            ResizeNativeWindowToControlBounds();

            return new PlatformHandle(_windowHandle, "HWND");
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            SetFailedInfo($"Windows Vulkan 视口子窗口创建失败：{ex.Message}");
            return new PlatformHandle(0, "HWND");
        }
    }

    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
        if (_windowHandle != 0)
        {
            DestroyWindow(_windowHandle);
            _windowHandle = 0;
        }

        _hostInfo = WindowsVulkanViewportHostInfo.NotCreated;
        base.DestroyNativeControlCore(control);
    }

    private void ResizeNativeWindowToControlBounds()
    {
        if (_windowHandle == 0)
        {
            return;
        }

        var width = Math.Max(1, (int)Math.Round(Bounds.Width));
        var height = Math.Max(1, (int)Math.Round(Bounds.Height));

        if (width < 1 || height < 1)
        {
            return;
        }

        SetWindowPos(
            _windowHandle,
            0,
            0,
            0,
            width,
            height,
            SwpNoZOrder | SwpNoActivate);

        _width = width;
        _height = height;
        _hostInfo = new WindowsVulkanViewportHostInfo(
            WindowsVulkanViewportHostState.Created,
            $"Windows 原生子窗口已创建，HWND：{FormatHandle(_windowHandle)}，尺寸：{_width}x{_height}。",
            "Windows",
            true,
            _windowHandle,
            _instanceHandle,
            _width,
            _height);
    }

    private static void RegisterWindowClass(nint instanceHandle)
    {
        if (_classRegistered)
        {
            return;
        }

        var windowClass = new WndClass
        {
            Style = 0,
            LpfnWndProc = Marshal.GetFunctionPointerForDelegate(WindowProcedure),
            CbClsExtra = 0,
            CbWndExtra = 0,
            HInstance = instanceHandle,
            HIcon = 0,
            HCursor = 0,
            HbrBackground = 0,
            LpszMenuName = null,
            LpszClassName = WindowClassName
        };

        var atom = RegisterClass(ref windowClass);
        if (atom == 0)
        {
            var error = Marshal.GetLastWin32Error();
            if (error != ClassNameAlreadyExists)
            {
                throw new Win32Exception(error, "注册 Windows Vulkan 视口子窗口类失败。");
            }
        }

        _classRegistered = true;
    }

    private void SetFailedInfo(string message)
    {
        _hostInfo = new WindowsVulkanViewportHostInfo(
            WindowsVulkanViewportHostState.Failed,
            message,
            "Windows",
            false,
            0,
            _instanceHandle,
            _width,
            _height);
    }

    private static string FormatHandle(nint handle)
    {
        return $"0x{handle.ToInt64():X16}";
    }

    [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern nint GetModuleHandle(string? moduleName);

    [DllImport("user32.dll", EntryPoint = "RegisterClassW", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern ushort RegisterClass(ref WndClass windowClass);

    [DllImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern nint CreateWindowEx(
        int extendedStyle,
        string className,
        string windowName,
        int style,
        int x,
        int y,
        int width,
        int height,
        nint parentWindow,
        nint menu,
        nint instance,
        nint parameter);

    [DllImport("user32.dll", EntryPoint = "DestroyWindow", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyWindow(nint windowHandle);

    private const uint SwpNoZOrder = 0x0004;
    private const uint SwpNoActivate = 0x0010;

    [DllImport("user32.dll", EntryPoint = "SetWindowPos", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(
        nint windowHandle,
        nint insertAfter,
        int x,
        int y,
        int width,
        int height,
        uint flags);

    [DllImport("user32.dll", EntryPoint = "DefWindowProcW")]
    private static extern nint DefWindowProc(nint windowHandle, uint message, nint wParam, nint lParam);

    private delegate nint WndProc(nint windowHandle, uint message, nint wParam, nint lParam);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct WndClass
    {
        public uint Style;
        public nint LpfnWndProc;
        public int CbClsExtra;
        public int CbWndExtra;
        public nint HInstance;
        public nint HIcon;
        public nint HCursor;
        public nint HbrBackground;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string? LpszMenuName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string LpszClassName;
    }
}
