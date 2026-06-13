using System.ComponentModel;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using FluidWarfare.Render.ViewportNavigation;

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

    // Win32 消息常量
    private const uint WmMButtonDown = 0x0207;
    private const uint WmMButtonUp = 0x0208;
    private const uint WmMouseMove = 0x0200;
    private const uint WmMouseLeave = 0x02A3;
    private const uint WmMouseWheel = 0x020A;
    private const uint WmKeyDown = 0x0100;
    private const uint WmKeyUp = 0x0101;
    private const uint WmKillFocus = 0x0008;
    private const uint WmLButtonDown = 0x0201;
    private const uint WmLButtonUp = 0x0202;
    private const uint WmCaptureChanged = 0x0215;
    private const uint WmNcHitTest = 0x0084;
    // 虚拟键码
    private const int VkHome = 0x24;
    private const int VkEscape = 0x1B;
    private const int VkShift = 0x10;
    private const int VkControl = 0x11;
    private const int VkMenu = 0x12;
    private const int VkDecimal = 0x6E;
    private const int VkNumpad5 = 0x65;
    private const int VkMButton = 0x04;    // VK_MBUTTON (中键按钮码)
    // 鼠标按钮码 (WM 消息 MK_* 标志，用于 wParam 低位)
    private const int MkLButton = 0x0001;
    private const int MkRButton = 0x0002;
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

    // ─── 原始输入事件（Win32 层只转发原始消息，不含业务逻辑）──

    /// <summary>鼠标按钮按下（buttonCode: 1=Left, 2=Right, 4=Middle, x, y）。</summary>
    public event Action<int, int, int>? RawPointerButtonDown;

    /// <summary>鼠标按钮抬起（buttonCode, x, y）。</summary>
    public event Action<int, int, int>? RawPointerButtonUp;

    /// <summary>鼠标在视口内原始移动（x, y）。</summary>
    public event Action<int, int>? RawPointerMoved;

    /// <summary>键盘键按下（virtualKeyCode）。</summary>
    public event Action<int>? RawKeyDown;

    /// <summary>键盘键抬起（virtualKeyCode）。</summary>
    public event Action<int>? RawKeyUp;

    /// <summary>鼠标滚轮（HIWORD delta, modifiers-packed）。</summary>
    public event Action<int, int>? RawMouseWheel;

    // ─── Overlay 导航输入事件 ────────────────────────────────────

    /// <summary>Overlay 导航左键按下（pixelX, pixelY）。</summary>
    public event Func<int, int, ViewportNavigationPressResult>? NavigationPointerPressed;

    /// <summary>Overlay 导航鼠标移动（pixelX, pixelY）。</summary>
    public event Func<int, int, bool>? NavigationPointerMoved;

    /// <summary>Overlay 导航左键释放。</summary>
    public event Action? NavigationPointerReleased;

    /// <summary>Overlay 导航鼠标捕获丢失。</summary>
    public event Action? NavigationCaptureLost;

    // ─── 遗留事件 ──────────────────────────────────────────

    /// <summary>左键点击拾取（pixelX, pixelY）。遗留：未来迁移到 RawPointerButtonUp+Translator。</summary>
    public event Action<int, int>? PickRequested;

    /// <summary>鼠标在视口内移动（pixelX, pixelY，地面 hover 用）。</summary>
    public new event Action<int, int>? PointerMoved;

    /// <summary>鼠标离开视口。</summary>
    public event Action? PointerLeft;

    // ─── 状态 ──────────────────────────────────────────────

    private bool _trackingMouseLeave;
    private bool _leftButtonHandledByNavigation;
    private bool _navigationDragCaptured;

    public event EventHandler<WindowsVulkanViewportHostInfo>? HostInfoChanged;

    // 左键点击跟踪（遗留拾取）
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
                {
                    SetFocus(instance._windowHandle);
                    var mx = (short)(lParam.ToInt64() & 0xFFFF);
                    var my = (short)((lParam.ToInt64() >> 16) & 0xFFFF);
                    System.Diagnostics.Debug.WriteLine(
                        $"[InputTrace-NativeHost] WM_MBUTTONDOWN code=4(Middle) x={mx} y={my}");
                    SetCapture(instance._windowHandle);
                    instance.RawPointerButtonDown?.Invoke(VkMButton /* 4=Middle */, mx, my);
                    return 0;
                }

                case WmMButtonUp:
                {
                    var mx = (short)(lParam.ToInt64() & 0xFFFF);
                    var my = (short)((lParam.ToInt64() >> 16) & 0xFFFF);
                    System.Diagnostics.Debug.WriteLine(
                        $"[InputTrace-NativeHost] WM_MBUTTONUP code=4(Middle)");
                    ReleaseCapture();
                    instance.RawPointerButtonUp?.Invoke(VkMButton, mx, my);
                    return 0;
                }

                case WmMouseMove:
                {
                    var x = (short)(lParam.ToInt64() & 0xFFFF);
                    var y = (short)((lParam.ToInt64() >> 16) & 0xFFFF);

                    // Track WM_MOUSELEAVE on first move
                    if (!instance._trackingMouseLeave)
                    {
                        var tme = new TRACKMOUSEEVENT
                        {
                            cbSize = System.Runtime.InteropServices.Marshal.SizeOf<TRACKMOUSEEVENT>(),
                            dwFlags = 0x00000002u,
                            hwndTrack = instance._windowHandle
                        };
                        TrackMouseEvent(ref tme);
                        instance._trackingMouseLeave = true;
                    }

                    // Overlay navigation gets first chance to consume
                    var navConsumed = instance.NavigationPointerMoved?.Invoke(x, y) == true;
                    if (instance._navigationDragCaptured) navConsumed = true;

                    if (!navConsumed)
                    {
                        instance.RawPointerMoved?.Invoke(x, y);
                        instance.PointerMoved?.Invoke(x, y); // 遗留地面 hover
                    }
                    return 0;
                }

                case WmMouseLeave:
                    instance._trackingMouseLeave = false;
                    instance.PointerLeft?.Invoke();
                    return 0;

                case WmMouseWheel:
                {
                    var delta = (short)((wParam.ToInt64() >> 16) & 0xFFFF);
                    var mx = (short)(lParam.ToInt64() & 0xFFFF);
                    var my = (short)((lParam.ToInt64() >> 16) & 0xFFFF);
                    var mkFlags = (int)wParam & 0xFFFF;
                    System.Diagnostics.Debug.WriteLine(
                        $"[InputTrace-NativeHost] WM_MOUSEWHEEL delta={delta} mk=0x{mkFlags:X4}");
                    instance.RawMouseWheel?.Invoke(delta, mkFlags);
                    return 0;
                }

                case WmLButtonDown:
                {
                    var mx = (short)(lParam.ToInt64() & 0xFFFF);
                    var my = (short)((lParam.ToInt64() >> 16) & 0xFFFF);
                    SetFocus(instance._windowHandle);

                    var pressResult = instance.NavigationPointerPressed?.Invoke(mx, my)
                        ?? ViewportNavigationPressResult.NotHandled;
                    instance._leftButtonHandledByNavigation =
                        pressResult != ViewportNavigationPressResult.NotHandled;

                    if (pressResult == ViewportNavigationPressResult.BeginDrag)
                    {
                        SetCapture(instance._windowHandle);
                        instance._navigationDragCaptured = true;
                    }
                    else if (!instance._leftButtonHandledByNavigation)
                    {
                        // 遗留拾取
                        instance._pickInput.OnDown(mx, my);
                        // 同时转发原始按钮事件
                        instance.RawPointerButtonDown?.Invoke(1 /* Left */, mx, my);
                    }
                    return 0;
                }

                case WmLButtonUp:
                {
                    var mx = (short)(lParam.ToInt64() & 0xFFFF);
                    var my = (short)((lParam.ToInt64() >> 16) & 0xFFFF);

                    if (instance._leftButtonHandledByNavigation)
                    {
                        instance._leftButtonHandledByNavigation = false;
                        var hadCapture = instance._navigationDragCaptured;
                        instance._navigationDragCaptured = false;
                        instance.NavigationPointerReleased?.Invoke();
                        if (hadCapture) ReleaseCapture();
                    }
                    else
                    {
                        instance._pickInput.OnUp(mx, my);
                        instance.RawPointerButtonUp?.Invoke(1 /* Left */, mx, my);
                    }
                    return 0;
                }

                case WmKeyDown:
                    SetFocus(instance._windowHandle);
                    instance.RawKeyDown?.Invoke((int)wParam);
                    return 0;

                case WmKeyUp:
                    instance.RawKeyUp?.Invoke((int)wParam);
                    return 0;

                case WmKillFocus:
                    instance.HandleKillFocus();
                    return 0;

                case WmCaptureChanged:
                    instance.HandleCaptureChanged();
                    return 0;

                case WmNcHitTest:
                    return DefWindowProc(hwnd, msg, wParam, lParam);
            }
        }

        return DefWindowProc(hwnd, msg, wParam, lParam);
    }

    private void HandleKillFocus()
    {
        _pickInput.OnKillFocus();
        var hadNavigationCapture = _navigationDragCaptured || _leftButtonHandledByNavigation;
        EndNavigationCapture();
        if (hadNavigationCapture)
            NavigationCaptureLost?.Invoke();
    }

    private void HandleCaptureChanged()
    {
        if (_navigationDragCaptured || _leftButtonHandledByNavigation)
        {
            EndNavigationCapture();
            NavigationCaptureLost?.Invoke();
        }
    }

    private void EndNavigationCapture()
    {
        _leftButtonHandledByNavigation = false;
        _navigationDragCaptured = false;
        _pickInput.OnKillFocus();
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

    [DllImport("user32.dll")]
    private static extern nint SetFocus(nint hwnd);

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

    [DllImport("user32.dll", EntryPoint = "TrackMouseEvent", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT tme);

    [DllImport("user32.dll", EntryPoint = "GetKeyState")]
    private static extern short GetKeyState(int nVirtKey);

    [StructLayout(LayoutKind.Sequential)]
    private struct TRACKMOUSEEVENT
    {
        public int cbSize;
        public uint dwFlags;
        public nint hwndTrack;
        public uint dwHoverTime;
    }

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
