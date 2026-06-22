using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Input.Pointer;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Input.Keyboard;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Input.Focus;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Input.Arbitration;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Lifecycle;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Win32;
using FluidWarfare.Render.ViewportNavigation;

namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost;

public sealed class WindowsVulkanViewportHostControl : NativeControlHost
{
    [ThreadStatic]
    static WindowsVulkanViewportHostControl? _currentInstance;

    nint _windowHandle;
    nint _instanceHandle;
    readonly NativeViewportHostSync _hostSync = new();

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

    public WindowsVulkanViewportHostInfo GetHostInfo() => _hostSync.Current;
    public void RequestCapture() { if (_windowHandle != 0) _mouseCapture.Capture(_windowHandle); }
    public void RequestReleaseCapture() => _mouseCapture.Release();

    // ─── 生命周期 ────────────────────────────────────────────

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        if (!OperatingSystem.IsWindows())
        {
            _hostSync.SetFailed("当前平台不支持 Windows Vulkan 视口子窗口。", _instanceHandle);
            return new PlatformHandle(0, "HWND");
        }
        if (parent.Handle == 0)
        {
            _hostSync.SetFailed("Avalonia 未提供可嵌入原生子窗口的父级句柄。", _instanceHandle);
            return new PlatformHandle(0, "HWND");
        }
        try
        {
            var result = NativeViewportCreate.TryCreate(parent, CustomWndProc);
            if (!result.Success)
            {
                _hostSync.SetFailed(result.ErrorMessage ?? "", result.InstanceHandle);
                return new PlatformHandle(0, "HWND");
            }
            _windowHandle = result.WindowHandle;
            _instanceHandle = result.InstanceHandle;
            _currentInstance = this;
            SyncAndPublishHostInfo();
            return new PlatformHandle(_windowHandle, "HWND");
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            _hostSync.SetFailed($"Windows Vulkan 视口子窗口创建失败：{ex.Message}", _instanceHandle);
            return new PlatformHandle(0, "HWND");
        }
    }

    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
        NativeViewportDestroy.Destroy(_windowHandle, ref _windowHandle);
        _hostSync.Reset();
        if (_currentInstance == this) _currentInstance = null;
        base.DestroyNativeControlCore(control);
    }

    // ─── 自定义 WndProc ─────────────────────────────────────

    static nint CustomWndProc(nint hwnd, uint msg, nint wParam, nint lParam)
    {
        var instance = _currentInstance;
        if (instance is null || instance._windowHandle != hwnd)
            return Win32ViewportDefaultProc.DefWindowProc(hwnd, msg, wParam, lParam);

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
            else instance.RawKeyUp?.Invoke(key.VirtualKeyCode);
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
            return Win32ViewportDefaultProc.DefWindowProc(hwnd, msg, wParam, lParam);

        return Win32ViewportDefaultProc.DefWindowProc(hwnd, msg, wParam, lParam);
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
        _arbitration.HandleLeftDown(mx, my, _mouseCapture, _windowHandle,
            (x, y) => NavigationPointerPressed?.Invoke(x, y) ?? ViewportNavigationPressResult.NotHandled,
            (x, y) => SceneToolPointerPressed?.Invoke(x, y) ?? ViewportSceneToolPressResult.NotHandled,
            (x, y) => _pickInput.OnDown(x, y),
            (c, x, y) => RawPointerButtonDown?.Invoke(c, x, y));
        return 0;
    }

    nint HandleLeftButtonUp(int mx, int my)
    {
        _arbitration.HandleLeftUp(mx, my, _mouseCapture,
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
        if (_hostSync.Apply(_windowHandle, _instanceHandle, Bounds, out var info))
            HostInfoChanged?.Invoke(this, info);
    }

    // ─── 工具方法 ──────────────────────────────────────────

    void Trace(string msg) { if (_traceEnabled) System.Diagnostics.Debug.WriteLine(msg); }
}
