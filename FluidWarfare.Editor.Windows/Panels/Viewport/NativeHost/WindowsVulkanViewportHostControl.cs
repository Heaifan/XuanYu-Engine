using System.ComponentModel;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Input.Pointer;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Input.Keyboard;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Input.Focus;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Input.Arbitration;
using FluidWarfare.Render.ViewportNavigation;

namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost;

public sealed class WindowsVulkanViewportHostControl : NativeControlHost
{
    const int WsChild = 0x40000000;
    const int WsVisible = 0x10000000;
    const int WsClipChildren = 0x02000000;
    const int WsClipSiblings = 0x04000000;
    const int WindowStyle = WsChild | WsVisible | WsClipChildren | WsClipSiblings;

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
    readonly NativeViewportKeyboardMessages _keyboardMessages = new();
    readonly NativeViewportFocusMessages _focusMessages = new();
    readonly NativeViewportInputArbitration _arbitration = new();

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

        var key = instance._keyboardMessages.Parse(msg, wParam);
        if (key is not null)
        {
            instance._focusMessages.SetFocusTo(instance._windowHandle);
            if (key.Action == NativeViewportKeyboardAction.Down)
                instance.RawKeyDown?.Invoke(key.VirtualKeyCode);
            else
                instance.RawKeyUp?.Invoke(key.VirtualKeyCode);
            return 0;
        }

        if (instance._focusMessages.IsKillFocus(msg))
        {
            instance._arbitration.HandleKillFocus(
                () => instance._pickInput.OnKillFocus(),
                instance._rawPointerDragCaptured,
                () => instance.RawInputFocusLost?.Invoke(),
                () => instance.NavigationCaptureLost?.Invoke(),
                () => instance.RawInputFocusLost?.Invoke());
            instance._rawPointerDragCaptured = false;
            return 0;
        }
        if (NativeViewportHitTestMessages.IsHitTest(msg))
            return DefWindowProc(hwnd, msg, wParam, lParam);

        return DefWindowProc(hwnd, msg, wParam, lParam);
    }

    // ─── 指针消息处理（原始翻译，不含编辑器业务）───────────────

    nint HandleMiddleButtonDown(int x, int y)
    {
        _focusMessages.SetFocusTo(_windowHandle);
        Trace($"[InputTrace-NativeHost] WM_MBUTTONDOWN code=4(Middle) x={x} y={y}");
        _mouseCapture.Capture(_windowHandle);
        _rawPointerDragCaptured = true;
        RawPointerButtonDown?.Invoke(NativeViewportPointerMessages.VkMButton, x, y);
        return 0;
    }

    nint HandleMiddleButtonUp(int x, int y)
    {
        Trace($"[InputTrace-NativeHost] WM_MBUTTONUP code=4(Middle)");
        _mouseCapture.ClearState();
        RawPointerButtonUp?.Invoke(NativeViewportPointerMessages.VkMButton, x, y);
        return 0;
    }

    nint HandlePointerMove(int x, int y)
    {
        _mouseTrack.Begin(_windowHandle);
        var navConsumed = NavigationPointerMoved?.Invoke(x, y) == true;
        if (_arbitration.NavCapture.DragCaptured) navConsumed = true;
        if (!navConsumed) { RawPointerMoved?.Invoke(x, y); PointerMoved?.Invoke(x, y); }
        return 0;
    }

    nint HandleMouseWheel(int x, int y, int delta, int modifiers)
    {
        Trace($"[InputTrace-NativeHost] WM_MOUSEWHEEL delta={delta} mk=0x{modifiers:X4}");
        RawMouseWheel?.Invoke(delta, modifiers);
        return 0;
    }

    // ─── 左键按下/抬起（委托至 NativeViewportInputArbitration）─

    nint HandleLeftButtonDown(int mx, int my)
    {
        _focusMessages.SetFocusTo(_windowHandle);
        _arbitration.HandleLeftDown(mx, my,
            _mouseCapture, _windowHandle,
            (x, y) => NavigationPointerPressed?.Invoke(x, y) ?? ViewportNavigationPressResult.NotHandled,
            (x, y) => SceneToolPointerPressed?.Invoke(x, y) ?? ViewportSceneToolPressResult.NotHandled,
            (x, y) => _pickInput.OnDown(x, y),
            (c, x, y) => RawPointerButtonDown?.Invoke(c, x, y));
        return 0;
    }

    nint HandleLeftButtonUp(int mx, int my)
    {
        _arbitration.HandleLeftUp(mx, my,
            _mouseCapture,
            () => NavigationPointerReleased?.Invoke(),
            (x, y) => SceneToolPointerReleased?.Invoke(x, y),
            (x, y) => _pickInput.OnUp(x, y),
            (c, x, y) => RawPointerButtonUp?.Invoke(c, x, y));
        return 0;
    }

    // ─── Capture 变更 ────────────────────────────────────────

    void HandleCaptureChanged()
    {
        if (_rawPointerDragCaptured) { _rawPointerDragCaptured = false; RawInputFocusLost?.Invoke(); }
        _arbitration.HandleCaptureChanged(
            _mouseCapture,
            () => NavigationCaptureLost?.Invoke(),
            () => RawInputFocusLost?.Invoke(),
            false);
    }

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

    // ─── 工具方法 ──────────────────────────────────────────

    void Trace(string msg)
    {
        if (_traceEnabled) System.Diagnostics.Debug.WriteLine(msg);
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

    [DllImport("user32.dll", EntryPoint = "DefWindowProcW")]
    static extern nint DefWindowProc(nint hwnd, uint msg, nint wParam, nint lParam);
}
