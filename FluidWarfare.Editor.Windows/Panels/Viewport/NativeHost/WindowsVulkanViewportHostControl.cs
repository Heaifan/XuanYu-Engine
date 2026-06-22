using System.ComponentModel;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using FluidWarfare.Render.ViewportNavigation;

namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost;

public sealed class WindowsVulkanViewportHostControl : NativeControlHost
{
    const int WsChild = 0x40000000;
    const int WsVisible = 0x10000000;
    const int WsClipChildren = 0x02000000;
    const int WsClipSiblings = 0x04000000;
    const int WindowStyle = WsChild | WsVisible | WsClipChildren | WsClipSiblings;

    // Win32 消息常量
    const uint WmMButtonDown = 0x0207;
    const uint WmMButtonUp = 0x0208;
    const uint WmMouseMove = 0x0200;
    const uint WmMouseLeave = 0x02A3;
    const uint WmMouseWheel = 0x020A;
    const uint WmKeyDown = 0x0100;
    const uint WmKeyUp = 0x0101;
    const uint WmKillFocus = 0x0008;
    const uint WmLButtonDown = 0x0201;
    const uint WmLButtonUp = 0x0202;
    const uint WmCaptureChanged = 0x0215;
    const uint WmNcHitTest = 0x0084;
    const int VkHome = 0x24;
    const int VkEscape = 0x1B;
    const int VkShift = 0x10;
    const int VkControl = 0x11;
    const int VkMenu = 0x12;
    const int VkDecimal = 0x6E;
    const int VkNumpad5 = 0x65;
    const int VkMButton = 0x04;
    const int MkLButton = 0x0001;
    const int MkRButton = 0x0002;
    const int MkMbutton = 0x0010;

    [ThreadStatic]
    static WindowsVulkanViewportHostControl? _currentInstance;

    nint _windowHandle;
    nint _instanceHandle;
    int _width;
    int _height;
    WindowsVulkanViewportHostInfo _hostInfo = WindowsVulkanViewportHostInfo.NotCreated;

    // ─── 原始输入事件（Win32 层只转发原始消息，不含业务逻辑）──

    public event Action<int, int, int>? RawPointerButtonDown;
    public event Action<int, int, int>? RawPointerButtonUp;
    public event Action<int, int>? RawPointerMoved;
    public event Action<int>? RawKeyDown;
    public event Action<int>? RawKeyUp;
    public event Action<int, int>? RawMouseWheel;
    public event Action? RawInputFocusLost;

    // ─── Overlay 导航输入事件 ────────────────────────────────────

    public event Func<int, int, ViewportNavigationPressResult>? NavigationPointerPressed;
    public event Func<int, int, bool>? NavigationPointerMoved;
    public event Action? NavigationPointerReleased;
    public event Action? NavigationCaptureLost;

    // ─── 遗留事件 ──────────────────────────────────────────

    public event Action<int, int>? PickRequested;
    public new event Action<int, int>? PointerMoved;
    public event Action? PointerLeft;

    // ─── 场景工具输入事件 ────────────────────────────────────

    public event Func<int, int, ViewportSceneToolPressResult>? SceneToolPointerPressed;
    public event Action<int, int>? SceneToolPointerReleased;
    public event EventHandler<WindowsVulkanViewportHostInfo>? HostInfoChanged;

    // ─── 状态 ──────────────────────────────────────────────

    bool _trackingMouseLeave;
    bool _leftButtonHandledByNavigation;
    bool _navigationDragCaptured;
    bool _leftButtonHandledBySceneTool;
    bool _sceneToolDragCaptured;
    bool _rawPointerDragCaptured;
    readonly bool _traceEnabled;
    readonly WindowsVulkanViewportPickInput _pickInput = new();

    public WindowsVulkanViewportHostControl()
    {
        _currentInstance = this;
        _traceEnabled = Environment.GetEnvironmentVariable("FW_INPUT_TRACE") == "1";
        PropertyChanged += (_, args) =>
        {
            if (args.Property == BoundsProperty)
                OnBoundsChanged();
        };
        _pickInput.PickRequested += (x, y) => PickRequested?.Invoke(x, y);
    }

    public WindowsVulkanViewportHostInfo GetHostInfo() => _hostInfo;

    public void RequestCapture()
    {
        if (_windowHandle != 0) SetCapture(_windowHandle);
    }

    public void RequestReleaseCapture() => ReleaseCapture();

    // ─── 生命周期 ────────────────────────────────────────────

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        if (!OperatingSystem.IsWindows())
        {
            _hostInfo = NativeViewportHostInfoStatics.CreateUnsupportedPlatformInfo();
            return new PlatformHandle(0, "HWND");
        }
        if (parent.Handle == 0)
        {
            _hostInfo = NativeViewportHostInfoStatics.CreateNoParentHandleInfo();
            return new PlatformHandle(0, "HWND");
        }

        try
        {
            _instanceHandle = GetModuleHandle(null);
            if (_instanceHandle == 0)
            {
                _hostInfo = NativeViewportHostInfoStatics.CreateFailedHostInfo(
                    "无法获取当前进程模块句柄。", _instanceHandle, _width, _height);
                return new PlatformHandle(0, "HWND");
            }

            _currentInstance = this;
            Win32ViewportWindowClass.EnsureRegistered(_instanceHandle, CustomWndProc);

            _windowHandle = CreateWindowEx(0, Win32ViewportWindowClass.WindowClassName,
                "FluidWarfare Vulkan Viewport", WindowStyle,
                0, 0, 1, 1, parent.Handle, 0, _instanceHandle, 0);

            if (_windowHandle == 0)
            {
                _hostInfo = NativeViewportHostInfoStatics.CreateFailedHostInfo(
                    $"CreateWindowEx 失败：{new Win32Exception(Marshal.GetLastWin32Error()).Message}",
                    _instanceHandle, _width, _height);
                return new PlatformHandle(0, "HWND");
            }

            SyncAndPublishHostInfo();
            return new PlatformHandle(_windowHandle, "HWND");
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            _hostInfo = NativeViewportHostInfoStatics.CreateFailedHostInfo(
                $"Windows Vulkan 视口子窗口创建失败：{ex.Message}",
                _instanceHandle, _width, _height);
            return new PlatformHandle(0, "HWND");
        }
    }

    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
        if (_windowHandle != 0) { DestroyWindow(_windowHandle); _windowHandle = 0; }
        _hostInfo = WindowsVulkanViewportHostInfo.NotCreated;
        if (_currentInstance == this) _currentInstance = null;
        base.DestroyNativeControlCore(control);
    }

    // ─── 自定义 WndProc ─────────────────────────────────────

    static nint CustomWndProc(nint hwnd, uint msg, nint wParam, nint lParam)
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
                    if (instance._traceEnabled)
                        System.Diagnostics.Debug.WriteLine($"[InputTrace-NativeHost] WM_MBUTTONDOWN code=4(Middle) x={mx} y={my}");
                    SetCapture(instance._windowHandle);
                    instance._rawPointerDragCaptured = true;
                    instance.RawPointerButtonDown?.Invoke(VkMButton, mx, my);
                    return 0;
                }

                case WmMButtonUp:
                {
                    var mx = (short)(lParam.ToInt64() & 0xFFFF);
                    var my = (short)((lParam.ToInt64() >> 16) & 0xFFFF);
                    if (instance._traceEnabled)
                        System.Diagnostics.Debug.WriteLine($"[InputTrace-NativeHost] WM_MBUTTONUP code=4(Middle)");
                    instance._rawPointerDragCaptured = false;
                    ReleaseCapture();
                    instance.RawPointerButtonUp?.Invoke(VkMButton, mx, my);
                    return 0;
                }

                case WmMouseMove:
                {
                    var x = (short)(lParam.ToInt64() & 0xFFFF);
                    var y = (short)((lParam.ToInt64() >> 16) & 0xFFFF);
                    if (!instance._trackingMouseLeave)
                    {
                        var tme = new TRACKMOUSEEVENT
                        {
                            cbSize = Marshal.SizeOf<TRACKMOUSEEVENT>(),
                            dwFlags = 0x00000002u,
                            hwndTrack = instance._windowHandle
                        };
                        TrackMouseEvent(ref tme);
                        instance._trackingMouseLeave = true;
                    }
                    var navConsumed = instance.NavigationPointerMoved?.Invoke(x, y) == true;
                    if (instance._navigationDragCaptured) navConsumed = true;
                    if (!navConsumed) { instance.RawPointerMoved?.Invoke(x, y); instance.PointerMoved?.Invoke(x, y); }
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
                    if (instance._traceEnabled)
                        System.Diagnostics.Debug.WriteLine($"[InputTrace-NativeHost] WM_MOUSEWHEEL delta={delta} mk=0x{mkFlags:X4}");
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
                    instance._leftButtonHandledByNavigation = pressResult != ViewportNavigationPressResult.NotHandled;
                    if (pressResult == ViewportNavigationPressResult.BeginDrag)
                    { SetCapture(instance._windowHandle); instance._navigationDragCaptured = true; }
                    else if (!instance._leftButtonHandledByNavigation)
                    {
                        var toolResult = instance.SceneToolPointerPressed?.Invoke(mx, my)
                            ?? ViewportSceneToolPressResult.NotHandled;
                        if (toolResult == ViewportSceneToolPressResult.BeginDrag)
                        { instance._leftButtonHandledBySceneTool = true; instance._sceneToolDragCaptured = true; SetCapture(instance._windowHandle); }
                        else { instance._pickInput.OnDown(mx, my); instance.RawPointerButtonDown?.Invoke(1, mx, my); }
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
                    else if (instance._leftButtonHandledBySceneTool)
                    {
                        instance._leftButtonHandledBySceneTool = false;
                        var hadCapture = instance._sceneToolDragCaptured;
                        instance._sceneToolDragCaptured = false;
                        instance.SceneToolPointerReleased?.Invoke(mx, my);
                        if (hadCapture) ReleaseCapture();
                    }
                    else { instance._pickInput.OnUp(mx, my); instance.RawPointerButtonUp?.Invoke(1, mx, my); }
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

    void HandleKillFocus()
    {
        _pickInput.OnKillFocus();
        if (_rawPointerDragCaptured) { _rawPointerDragCaptured = false; RawInputFocusLost?.Invoke(); }
        var hadNav = _navigationDragCaptured || _leftButtonHandledByNavigation;
        EndNavigationCapture();
        var hadTool = _sceneToolDragCaptured || _leftButtonHandledBySceneTool;
        EndSceneToolCapture();
        if (hadNav) NavigationCaptureLost?.Invoke();
        if (hadTool) RawInputFocusLost?.Invoke();
    }

    void HandleCaptureChanged()
    {
        if (_rawPointerDragCaptured) { _rawPointerDragCaptured = false; RawInputFocusLost?.Invoke(); }
        if (_navigationDragCaptured || _leftButtonHandledByNavigation) { EndNavigationCapture(); NavigationCaptureLost?.Invoke(); }
        if (_sceneToolDragCaptured || _leftButtonHandledBySceneTool) { EndSceneToolCapture(); RawInputFocusLost?.Invoke(); }
    }

    void EndNavigationCapture() { _leftButtonHandledByNavigation = false; _navigationDragCaptured = false; _pickInput.OnKillFocus(); }
    void EndSceneToolCapture() { _leftButtonHandledBySceneTool = false; _sceneToolDragCaptured = false; }

    // ─── 窗口调整大小 ────────────────────────────────────────

    void OnBoundsChanged()
    {
        if (_windowHandle == 0) return;
        SyncAndPublishHostInfo();
    }

    void SyncAndPublishHostInfo()
    {
        var w = Math.Max(1, (int)Math.Round(Bounds.Width));
        var h = Math.Max(1, (int)Math.Round(Bounds.Height));
        if (w < 1 || h < 1) return;

        var changed = !_hostInfo.HasWindowHandle || _width != w || _height != h;

        NativeViewportHostInfoStatics.SyncWindowSize(_windowHandle, w, h);
        _width = w;
        _height = h;
        _hostInfo = NativeViewportHostInfoStatics.CreateHostInfo(_windowHandle, _instanceHandle, _width, _height);

        if (changed)
            HostInfoChanged?.Invoke(this, _hostInfo);
    }

    // ─── P/Invoke ───────────────────────────────────────────

    [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern nint GetModuleHandle(string? moduleName);

    [DllImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern nint CreateWindowEx(int exStyle, string className, string windowName,
        int style, int x, int y, int w, int h, nint parent, nint menu, nint instance, nint param);

    [DllImport("user32.dll", EntryPoint = "DestroyWindow", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool DestroyWindow(nint hwnd);

    [DllImport("user32.dll")]
    static extern nint SetFocus(nint hwnd);

    [DllImport("user32.dll", EntryPoint = "DefWindowProcW")]
    static extern nint DefWindowProc(nint hwnd, uint msg, nint wParam, nint lParam);

    [DllImport("user32.dll", EntryPoint = "SetCapture")]
    static extern nint SetCapture(nint hwnd);

    [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool ReleaseCapture();

    [DllImport("user32.dll", EntryPoint = "TrackMouseEvent", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT tme);

    [DllImport("user32.dll", EntryPoint = "GetKeyState")]
    static extern short GetKeyState(int nVirtKey);

    [StructLayout(LayoutKind.Sequential)]
    struct TRACKMOUSEEVENT
    {
        public int cbSize;
        public uint dwFlags;
        public nint hwndTrack;
        public uint dwHoverTime;
    }
}
