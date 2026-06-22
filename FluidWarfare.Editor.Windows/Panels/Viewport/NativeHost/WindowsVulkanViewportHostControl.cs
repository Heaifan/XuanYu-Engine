using System.ComponentModel;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Input;
using FluidWarfare.Render.ViewportNavigation;

namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost;

public sealed class WindowsVulkanViewportHostControl : NativeControlHost
{
    const int WsChild = 0x40000000;
    const int WsVisible = 0x10000000;
    const int WsClipChildren = 0x02000000;
    const int WsClipSiblings = 0x04000000;
    const int WindowStyle = WsChild | WsVisible | WsClipChildren | WsClipSiblings;

    // 非指针消息常量（指针常量移至 NativeViewportPointerMessages）
    const uint WmKeyDown = 0x0100;
    const uint WmKeyUp = 0x0101;
    const uint WmKillFocus = 0x0008;
    const uint WmNcHitTest = 0x0084;
    const int VkHome = 0x24;
    const int VkEscape = 0x1B;
    const int VkShift = 0x10;
    const int VkControl = 0x11;
    const int VkMenu = 0x12;
    const int VkDecimal = 0x6E;
    const int VkNumpad5 = 0x65;
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

    // ─── 提取的子组件 ─────────────────────────────────────────

    readonly NativeViewportPointerMessages _pointerMessages = new();
    readonly NativeViewportMouseCapture _mouseCapture = new();
    readonly NativeViewportMouseTrack _mouseTrack = new();

    // ─── 原始输入事件 ─────────────────────────────────────────

    public event Action<int, int, int>? RawPointerButtonDown;
    public event Action<int, int, int>? RawPointerButtonUp;
    public event Action<int, int>? RawPointerMoved;
    public event Action<int>? RawKeyDown;
    public event Action<int>? RawKeyUp;
    public event Action<int, int>? RawMouseWheel;
    public event Action? RawInputFocusLost;

    // ─── Overlay 导航输入事件 ─────────────────────────────────

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
            if (args.Property == BoundsProperty) OnBoundsChanged();
        };
        _pickInput.PickRequested += (x, y) => PickRequested?.Invoke(x, y);
    }

    public WindowsVulkanViewportHostInfo GetHostInfo() => _hostInfo;

    public void RequestCapture()
    {
        if (_windowHandle != 0) _mouseCapture.Capture(_windowHandle);
    }

    public void RequestReleaseCapture() => _mouseCapture.Release();

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
        if (instance is null || instance._windowHandle != hwnd)
            return DefWindowProc(hwnd, msg, wParam, lParam);

        // 指针消息解析（不包含编辑器业务仲裁）
        var parsed = instance._pointerMessages.Parse(msg, wParam, lParam);
        if (parsed is not null)
        {
            switch (parsed.Action)
            {
                case NativeViewportPointerAction.LeftDown:
                    return instance.HandleLeftButtonDown(parsed.X, parsed.Y);

                case NativeViewportPointerAction.LeftUp:
                    return instance.HandleLeftButtonUp(parsed.X, parsed.Y);

                case NativeViewportPointerAction.MiddleDown:
                    return instance.HandleMiddleButtonDown(parsed.X, parsed.Y);

                case NativeViewportPointerAction.MiddleUp:
                    return instance.HandleMiddleButtonUp(parsed.X, parsed.Y);

                case NativeViewportPointerAction.Move:
                    return instance.HandlePointerMove(parsed.X, parsed.Y);

                case NativeViewportPointerAction.Leave:
                    instance._mouseTrack.Reset();
                    instance.PointerLeft?.Invoke();
                    return 0;

                case NativeViewportPointerAction.Wheel:
                    return instance.HandleMouseWheel(parsed.X, parsed.Y, parsed.WheelDelta, parsed.ModifierFlags);

                case NativeViewportPointerAction.CaptureChanged:
                    instance.HandleCaptureChanged();
                    return 0;
            }
        }

        // 非指针消息
        switch (msg)
        {
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
            case WmNcHitTest:
                return DefWindowProc(hwnd, msg, wParam, lParam);
        }
        return DefWindowProc(hwnd, msg, wParam, lParam);
    }

    // ─── 指针消息处理（原始翻译，不含编辑器业务）───────────────

    nint HandleMiddleButtonDown(int x, int y)
    {
        SetFocus(_windowHandle);
        if (_traceEnabled)
            System.Diagnostics.Debug.WriteLine($"[InputTrace-NativeHost] WM_MBUTTONDOWN code=4(Middle) x={x} y={y}");
        _mouseCapture.Capture(_windowHandle);
        _rawPointerDragCaptured = true;
        RawPointerButtonDown?.Invoke(NativeViewportPointerMessages.VkMButton, x, y);
        return 0;
    }

    nint HandleMiddleButtonUp(int x, int y)
    {
        if (_traceEnabled)
            System.Diagnostics.Debug.WriteLine($"[InputTrace-NativeHost] WM_MBUTTONUP code=4(Middle)");
        _mouseCapture.ClearState();
        RawPointerButtonUp?.Invoke(NativeViewportPointerMessages.VkMButton, x, y);
        return 0;
    }

    nint HandlePointerMove(int x, int y)
    {
        _mouseTrack.Begin(_windowHandle);
        var navConsumed = NavigationPointerMoved?.Invoke(x, y) == true;
        if (_navigationDragCaptured) navConsumed = true;
        if (!navConsumed) { RawPointerMoved?.Invoke(x, y); PointerMoved?.Invoke(x, y); }
        return 0;
    }

    nint HandleMouseWheel(int x, int y, int delta, int modifiers)
    {
        if (_traceEnabled)
            System.Diagnostics.Debug.WriteLine($"[InputTrace-NativeHost] WM_MOUSEWHEEL delta={delta} mk=0x{modifiers:X4}");
        RawMouseWheel?.Invoke(delta, modifiers);
        return 0;
    }

    // ─── 左键按下/抬起（含导航/场景工具仲裁）─────────────────

    nint HandleLeftButtonDown(int mx, int my)
    {
        SetFocus(_windowHandle);
        var pressResult = NavigationPointerPressed?.Invoke(mx, my)
            ?? ViewportNavigationPressResult.NotHandled;
        _leftButtonHandledByNavigation = pressResult != ViewportNavigationPressResult.NotHandled;

        if (pressResult == ViewportNavigationPressResult.BeginDrag)
        {
            _mouseCapture.Capture(_windowHandle);
            _navigationDragCaptured = true;
        }
        else if (!_leftButtonHandledByNavigation)
        {
            var toolResult = SceneToolPointerPressed?.Invoke(mx, my)
                ?? ViewportSceneToolPressResult.NotHandled;
            if (toolResult == ViewportSceneToolPressResult.BeginDrag)
            {
                _leftButtonHandledBySceneTool = true;
                _sceneToolDragCaptured = true;
                _mouseCapture.Capture(_windowHandle);
            }
            else
            {
                _pickInput.OnDown(mx, my);
                RawPointerButtonDown?.Invoke(1, mx, my);
            }
        }
        return 0;
    }

    nint HandleLeftButtonUp(int mx, int my)
    {
        if (_leftButtonHandledByNavigation)
        {
            _leftButtonHandledByNavigation = false;
            var hadCapture = _navigationDragCaptured;
            _navigationDragCaptured = false;
            NavigationPointerReleased?.Invoke();
            if (hadCapture) _mouseCapture.Release();
        }
        else if (_leftButtonHandledBySceneTool)
        {
            _leftButtonHandledBySceneTool = false;
            var hadCapture = _sceneToolDragCaptured;
            _sceneToolDragCaptured = false;
            SceneToolPointerReleased?.Invoke(mx, my);
            if (hadCapture) _mouseCapture.Release();
        }
        else
        {
            _pickInput.OnUp(mx, my);
            RawPointerButtonUp?.Invoke(1, mx, my);
        }
        return 0;
    }

    // ─── Focus / Capture 变更 ────────────────────────────────

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
        _width = w; _height = h;
        _hostInfo = NativeViewportHostInfoStatics.CreateHostInfo(_windowHandle, _instanceHandle, _width, _height);
        if (changed) HostInfoChanged?.Invoke(this, _hostInfo);
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
}
