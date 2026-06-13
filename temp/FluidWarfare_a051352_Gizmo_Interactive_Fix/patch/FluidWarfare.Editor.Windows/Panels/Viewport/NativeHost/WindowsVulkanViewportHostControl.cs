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

    // Win32 message constants
    private const uint WmMButtonDown = 0x0207;
    private const uint WmMButtonUp = 0x0208;
    private const uint WmMouseMove = 0x0200;
    private const uint WmMouseLeave = 0x02A3;
    private const uint WmMouseWheel = 0x020A;
    private const uint WmKeyDown = 0x0100;
    private const uint WmKillFocus = 0x0008;
    private const uint WmLButtonDown = 0x0201;
    private const uint WmLButtonUp = 0x0202;
    private const uint WmCaptureChanged = 0x0215;
    private const uint WmNcHitTest = 0x0084;
    private const int VkHome = 0x24;
    private const int VkEscape = 0x1B;
    private const int VkShift = 0x10;
    private const int VkControl = 0x11;
    private const int VkDecimal = 0x6E; // Numpad period
    private const int VkNumpad5 = 0x65; // Projection toggle (Blender numpad 5)
    private const int MkMbutton = 0x0010;

    private enum MouseDragMode { None, Orbit, Pan, Dolly }

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
    private MouseDragMode _mouseDragMode = MouseDragMode.None;
    private int _lastMouseX;
    private int _lastMouseY;
    private bool _trackingMouseLeave;

    // ─── 输入事件 ────────────────────────────────────────────

    /// <summary>鼠标中键拖拽环绕旋转（deltaYaw, deltaPitch）。</summary>
    public event Action<float, float>? CameraOrbitRequested;

    /// <summary>Shift + 中键拖拽平移（deltaPixelX, deltaPixelY, viewportWidth, viewportHeight）。</summary>
    public event Action<int, int, int, int>? CameraPanRequested;

    /// <summary>Ctrl + 中键拖拽推拉（deltaPixels）。</summary>
    public event Action<float>? CameraDollyRequested;

    /// <summary>鼠标滚轮缩放（wheelNotches）。</summary>
    public event Action<float>? CameraZoomRequested;

    /// <summary>Home 相机重置。</summary>
    public event Action? CameraResetRequested;

    /// <summary>小键盘句点聚焦选中实体。</summary>
    public event Action? NumpadPeriodRequested;

    /// <summary>小键盘 5 切换投影模式（透视/正交）。</summary>
    public event Action? CameraProjectionToggleRequested;

    /// <summary>Esc 键按下。</summary>
    public event Action? EscapeRequested;

    /// <summary>左键点击拾取（pixelX, pixelY）。</summary>
    public event Action<int, int>? PickRequested;

    // ─── Overlay 导航输入事件 ────────────────────────────────────

    /// <summary>Overlay 导航左键按下（pixelX, pixelY）。</summary>
    public event Func<int, int, ViewportNavigationPressResult>? NavigationPointerPressed;

    /// <summary>Overlay 导航鼠标移动（pixelX, pixelY）。</summary>
    public event Func<int, int, bool>? NavigationPointerMoved;

    /// <summary>Overlay 导航左键释放。</summary>
    public event Action? NavigationPointerReleased;

    /// <summary>Overlay 导航鼠标捕获丢失。</summary>
    public event Action? NavigationCaptureLost;

    /// <summary>鼠标在视口内移动（pixelX, pixelY）。</summary>
    public new event Action<int, int>? PointerMoved;

    /// <summary>鼠标离开视口。</summary>
    public event Action? PointerLeft;

    private bool _leftButtonHandledByNavigation;
    private bool _navigationDragCaptured;

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

                case WmMouseLeave:
                    instance._trackingMouseLeave = false;
                    instance.PointerLeft?.Invoke();
                    return 0;

                case WmMouseWheel:
                    instance.HandleMouseWheel(wParam);
                    return 0;

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
                        instance._pickInput.OnDown(mx, my);
                    }

                    return 0;
                }

                case WmLButtonUp:
                {
                    var mx = (short)(lParam.ToInt64() & 0xFFFF);
                    var my = (short)((lParam.ToInt64() >> 16) & 0xFFFF);

                    if (instance._leftButtonHandledByNavigation)
                    {
                        // 先清理本地标志，再 ReleaseCapture，避免 WM_CAPTURECHANGED 重复派发。
                        instance._leftButtonHandledByNavigation = false;
                        var hadCapture = instance._navigationDragCaptured;
                        instance._navigationDragCaptured = false;
                        instance.NavigationPointerReleased?.Invoke();
                        if (hadCapture) ReleaseCapture();
                    }
                    else
                    {
                        instance._pickInput.OnUp(mx, my);
                    }

                    return 0;
                }

                case WmKeyDown when (int)wParam == VkHome:
                    instance.CameraResetRequested?.Invoke();
                    return 0;

                case WmKeyDown when (int)wParam == VkEscape:
                    instance.EscapeRequested?.Invoke();
                    return 0;

                case WmKeyDown when (int)wParam == VkDecimal:
                    instance.NumpadPeriodRequested?.Invoke();
                    return 0;

                case WmKeyDown when (int)wParam == VkNumpad5:
                    instance.CameraProjectionToggleRequested?.Invoke();
                    return 0;

                case WmKillFocus:
                    instance.HandleKillFocus();
                    return 0;

                case WmCaptureChanged:
                    instance.HandleCaptureChanged();
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
        _lastMouseX = (short)(lParam.ToInt64() & 0xFFFF);
        _lastMouseY = (short)((lParam.ToInt64() >> 16) & 0xFFFF);

        var shiftDown = (GetKeyState(VkShift) & 0x8000) != 0;
        var ctrlDown = (GetKeyState(VkControl) & 0x8000) != 0;

        if (ctrlDown)
            _mouseDragMode = MouseDragMode.Dolly;
        else if (shiftDown)
            _mouseDragMode = MouseDragMode.Pan;
        else
            _mouseDragMode = MouseDragMode.Orbit;

        SetCapture(_windowHandle);
    }

    private void HandleMButtonUp()
    {
        _mouseDragMode = MouseDragMode.None;
        ReleaseCapture();
    }

    private void HandleMouseMove(nint lParam)
    {
        var x = (short)(lParam.ToInt64() & 0xFFFF);
        var y = (short)((lParam.ToInt64() >> 16) & 0xFFFF);
        var deltaX = x - _lastMouseX;
        var deltaY = y - _lastMouseY;
        _lastMouseX = x;
        _lastMouseY = y;

        // Track WM_MOUSELEAVE on first mouse move
        if (!_trackingMouseLeave && _windowHandle != 0)
        {
            var tme = new TRACKMOUSEEVENT
            {
                cbSize = System.Runtime.InteropServices.Marshal.SizeOf<TRACKMOUSEEVENT>(),
                dwFlags = 0x00000002u, // TME_LEAVE
                hwndTrack = _windowHandle
            };
            TrackMouseEvent(ref tme);
            _trackingMouseLeave = true;
        }

        var navigationConsumed = NavigationPointerMoved?.Invoke(x, y) == true;
        if (_navigationDragCaptured) navigationConsumed = true;

        if (!navigationConsumed)
            PointerMoved?.Invoke(x, y);

        // Handle camera drag modes
        switch (_mouseDragMode)
        {
            case MouseDragMode.Orbit:
                CameraOrbitRequested?.Invoke(-deltaX, -deltaY); // invert for natural feel
                break;

            case MouseDragMode.Pan:
                CameraPanRequested?.Invoke(deltaX, deltaY, _width, _height);
                break;

            case MouseDragMode.Dolly:
                CameraDollyRequested?.Invoke(-deltaY);
                break;
        }
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
        if (_mouseDragMode != MouseDragMode.None)
        {
            _mouseDragMode = MouseDragMode.None;
            ReleaseCapture();
        }

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
