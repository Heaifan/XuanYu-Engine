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

    // Win32 message constants
    private const uint WmMButtonDown = 0x0207;
    private const uint WmMButtonUp = 0x0208;
    private const uint WmMouseMove = 0x0200;
    private const uint WmMouseWheel = 0x020A;
    private const uint WmKeyDown = 0x0100;
    private const uint WmKillFocus = 0x0008;
    private const uint WmLButtonDown = 0x0201;
    private const uint WmLButtonUp = 0x0202;
    private const uint WmNcHitTest = 0x0084;
    private const int VkHome = 0x24;
    private const int MkMbutton = 0x0010;

    private static bool _classRegistered;

    // Instance tracking for static WndProc
    [ThreadStatic]
    private static WindowsVulkanViewportHostControl? _currentInstance;

    private nint _windowHandle;
    private nint _instanceHandle;
    private int _width;
    private int _height;
    private WindowsVulkanViewportHostInfo _hostInfo = WindowsVulkanViewportHostInfo.NotCreated;

    // Input state
    private bool _isDragging;
    private int _lastMouseX;
    private int _lastMouseY;

    // ─── 输入事件 ────────────────────────────────────────────

    /// <summary>鼠标中键拖拽平移（deltaPixelX, deltaPixelY, viewportWidth, viewportHeight）。</summary>
    public event Action<int, int, int, int>? CameraPanRequested;

    /// <summary>鼠标滚轮缩放（wheelNotches）。</summary>
    public event Action<float>? CameraZoomRequested;

    /// <summary>Home 相机重置。</summary>
    public event Action? CameraResetRequested;

    /// <summary>左键点击拾取（pixelX, pixelY）。</summary>
    public event Action<int, int>? PickRequested;

    public event EventHandler<WindowsVulkanViewportHostInfo>? HostInfoChanged;

    // 左键点击跟踪
    private readonly WindowsVulkanViewportPickInput _pickInput = new();

    public WindowsVulkanViewportHostControl()
    {
        _currentInstance = this;

        PropertyChanged += (_, args) =>
        {
            if (args.Property == BoundsProperty)
                ResizeNativeWindowToControlBounds();
        };

        _pickInput.PickRequested += (x, y) => PickRequested?.Invoke(x, y);
    }

    public WindowsVulkanViewportHostInfo GetHostInfo() => _hostInfo;

    // ─── 生命周期 ────────────────────────────────────────────

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        if (!OperatingSystem.IsWindows())
        {
            _hostInfo = new WindowsVulkanViewportHostInfo(
                WindowsVulkanViewportHostState.UnsupportedPlatform,
                "当前平台不支持 Windows Vulkan 视口子窗口。",
                "非 Windows", false, 0, 0, 0, 0);
            return new PlatformHandle(0, "HWND");
        }

        if (parent.Handle == 0)
        {
            _hostInfo = new WindowsVulkanViewportHostInfo(
                WindowsVulkanViewportHostState.Failed,
                "Avalonia 未提供可嵌入原生子窗口的父级句柄。",
                "Windows", false, 0, 0, 0, 0);
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

            _currentInstance = this;
            RegisterWindowClass(_instanceHandle);

            _windowHandle = CreateWindowEx(0, WindowClassName, "FluidWarfare Vulkan Viewport",
                WindowStyle, 0, 0, 1, 1, parent.Handle, 0, _instanceHandle, 0);

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
        if (_currentInstance == this)
            _currentInstance = null;
        base.DestroyNativeControlCore(control);
    }

    // ─── 自定义 WndProc ─────────────────────────────────────

    private static nint CustomWndProc(nint hwnd, uint msg, nint wParam, nint lParam)
    {
        var instance = _currentInstance;
        if (instance is not null && instance._windowHandle == hwnd)
        {
            switch (msg)
            {
                case WmMButtonDown:
                    instance.HandleMButtonDown(lParam);
                    return 0;

                case WmMButtonUp:
                    instance.HandleMButtonUp();
                    return 0;

                case WmMouseMove:
                    instance.HandleMouseMove(lParam);
                    return 0;

                case WmMouseWheel:
                    instance.HandleMouseWheel(wParam);
                    return 0;

                case WmLButtonDown:
                    instance._pickInput.OnDown(
                        (short)(lParam.ToInt64() & 0xFFFF),
                        (short)((lParam.ToInt64() >> 16) & 0xFFFF));
                    return 0;

                case WmLButtonUp:
                    instance._pickInput.OnUp(
                        (short)(lParam.ToInt64() & 0xFFFF),
                        (short)((lParam.ToInt64() >> 16) & 0xFFFF));
                    return 0;

                case WmKeyDown when (int)wParam == VkHome:
                    instance.CameraResetRequested?.Invoke();
                    return 0;

                case WmKillFocus:
                    instance.HandleKillFocus();
                    return 0;

                case WmNcHitTest:
                    // 允许窗口拖动
                    return DefWindowProc(hwnd, msg, wParam, lParam);
            }
        }

        return DefWindowProc(hwnd, msg, wParam, lParam);
    }

    private void HandleMButtonDown(nint lParam)
    {
        _isDragging = true;
        _lastMouseX = (short)(lParam.ToInt64() & 0xFFFF);
        _lastMouseY = (short)((lParam.ToInt64() >> 16) & 0xFFFF);
        SetCapture(_windowHandle);
    }

    private void HandleMButtonUp()
    {
        _isDragging = false;
        ReleaseCapture();
    }

    private void HandleMouseMove(nint lParam)
    {
        if (!_isDragging) return;

        var x = (short)(lParam.ToInt64() & 0xFFFF);
        var y = (short)((lParam.ToInt64() >> 16) & 0xFFFF);

        var deltaX = x - _lastMouseX;
        var deltaY = y - _lastMouseY;

        _lastMouseX = x;
        _lastMouseY = y;

        CameraPanRequested?.Invoke(deltaX, deltaY, _width, _height);
    }

    private void HandleMouseWheel(nint wParam)
    {
        // HIWORD(wParam) = wheel delta (positive = up/away)
        var wheelDelta = (short)((wParam.ToInt64() >> 16) & 0xFFFF);
        var notches = wheelDelta / 120.0f;
        CameraZoomRequested?.Invoke(notches);
    }

    private void HandleKillFocus()
    {
        if (_isDragging)
        {
            _isDragging = false;
            ReleaseCapture();
        }
    }

    // ─── 窗口调整大小 ────────────────────────────────────────

    private void ResizeNativeWindowToControlBounds()
    {
        if (_windowHandle == 0) return;

        var width = Math.Max(1, (int)Math.Round(Bounds.Width));
        var height = Math.Max(1, (int)Math.Round(Bounds.Height));
        if (width < 1 || height < 1) return;

        var hasChanged = !_hostInfo.HasWindowHandle || _width != width || _height != height;

        SetWindowPos(_windowHandle, 0, 0, 0, width, height, SwpNoZOrder | SwpNoActivate);
        _width = width;
        _height = height;

        _hostInfo = new WindowsVulkanViewportHostInfo(
            WindowsVulkanViewportHostState.Created,
            $"Windows 原生子窗口已创建，HWND：{FormatHandle(_windowHandle)}，尺寸：{_width}x{_height}。",
            "Windows", true, _windowHandle, _instanceHandle, _width, _height);

        if (hasChanged)
            HostInfoChanged?.Invoke(this, _hostInfo);
    }

    // ─── 窗口类注册 ────────────────────────────────────────

    private static void RegisterWindowClass(nint instanceHandle)
    {
        if (_classRegistered) return;

        var wndProcPtr = Marshal.GetFunctionPointerForDelegate<WndProc>(CustomWndProc);

        var windowClass = new WndClass
        {
            Style = 0,
            LpfnWndProc = wndProcPtr,
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
                throw new Win32Exception(error, "注册 Windows Vulkan 视口子窗口类失败。");
        }

        _classRegistered = true;
    }

    private void SetFailedInfo(string message)
    {
        _hostInfo = new WindowsVulkanViewportHostInfo(
            WindowsVulkanViewportHostState.Failed, message,
            "Windows", false, 0, _instanceHandle, _width, _height);
    }

    private static string FormatHandle(nint handle) => $"0x{handle.ToInt64():X16}";

    // ─── P/Invoke ───────────────────────────────────────────

    [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern nint GetModuleHandle(string? moduleName);

    [DllImport("user32.dll", EntryPoint = "RegisterClassW", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern ushort RegisterClass(ref WndClass windowClass);

    [DllImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern nint CreateWindowEx(int exStyle, string className, string windowName,
        int style, int x, int y, int w, int h, nint parent, nint menu, nint instance, nint param);

    [DllImport("user32.dll", EntryPoint = "DestroyWindow", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyWindow(nint hwnd);

    private const uint SwpNoZOrder = 0x0004;
    private const uint SwpNoActivate = 0x0010;

    [DllImport("user32.dll", EntryPoint = "SetWindowPos", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(nint hwnd, nint after, int x, int y, int w, int h, uint flags);

    [DllImport("user32.dll", EntryPoint = "DefWindowProcW")]
    private static extern nint DefWindowProc(nint hwnd, uint msg, nint wParam, nint lParam);

    [DllImport("user32.dll", EntryPoint = "SetCapture")]
    private static extern nint SetCapture(nint hwnd);

    [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ReleaseCapture();

    private delegate nint WndProc(nint hwnd, uint msg, nint wParam, nint lParam);

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
        [MarshalAs(UnmanagedType.LPWStr)] public string? LpszMenuName;
        [MarshalAs(UnmanagedType.LPWStr)] public string LpszClassName;
    }
}
