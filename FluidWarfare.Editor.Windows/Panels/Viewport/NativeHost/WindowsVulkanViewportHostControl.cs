using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Input.Pointer;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Input.Keyboard;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Input.Focus;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Input.Arbitration;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Lifecycle;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Win32;

namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost;

public sealed partial class WindowsVulkanViewportHostControl : NativeControlHost
{
    [ThreadStatic] static WindowsVulkanViewportHostControl? _currentInstance;
    nint _windowHandle, _instanceHandle;
    readonly NativeViewportHostSync _hostSync = new();
    readonly NativeViewportPointerMessages _pointerMessages = new();
    readonly NativeViewportMouseCapture _mouseCapture = new();
    readonly NativeViewportMouseTrack _mouseTrack = new();
    readonly NativeViewportKeyboardMessages _keyboardMessages = new();
    readonly NativeViewportFocusMessages _focusMessages = new();
    readonly NativeViewportInputArbitration _arbitration = new();
    bool _rawPointerDragCaptured;
    readonly bool _traceEnabled;
    readonly WindowsVulkanViewportPickInput _pickInput = new();

    public WindowsVulkanViewportHostControl()
    {
        _currentInstance = this;
        _traceEnabled = Environment.GetEnvironmentVariable("FW_INPUT_TRACE") == "1";
        PropertyChanged += (_, args) => { if (args.Property == BoundsProperty) OnBoundsChanged(); };
        _pickInput.PickRequested += (x, y) => PickRequested?.Invoke(x, y);
    }

    public WindowsVulkanViewportHostInfo GetHostInfo() => _hostSync.Current;
    public void RequestCapture() { if (_windowHandle != 0) _mouseCapture.Capture(_windowHandle); }
    public void RequestReleaseCapture() => _mouseCapture.Release();

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        if (!OperatingSystem.IsWindows())
        { _hostSync.SetFailed("当前平台不支持 Windows Vulkan 视口子窗口。", _instanceHandle); return new PlatformHandle(0, "HWND"); }
        if (parent.Handle == 0)
        { _hostSync.SetFailed("Avalonia 未提供可嵌入原生子窗口的父级句柄。", _instanceHandle); return new PlatformHandle(0, "HWND"); }
        try
        {
            var r = NativeViewportCreate.TryCreate(parent, CustomWndProc);
            if (!r.Success) { _hostSync.SetFailed(r.ErrorMessage ?? "", r.InstanceHandle); return new PlatformHandle(0, "HWND"); }
            _windowHandle = r.WindowHandle; _instanceHandle = r.InstanceHandle; _currentInstance = this;
            SyncAndPublishHostInfo();
            return new PlatformHandle(_windowHandle, "HWND");
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        { _hostSync.SetFailed($"Windows Vulkan 视口子窗口创建失败：{ex.Message}", _instanceHandle); return new PlatformHandle(0, "HWND"); }
    }

    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
        NativeViewportDestroy.Destroy(_windowHandle, ref _windowHandle);
        _hostSync.Reset();
        if (_currentInstance == this) _currentInstance = null;
        base.DestroyNativeControlCore(control);
    }

    static nint CustomWndProc(nint hwnd, uint msg, nint wParam, nint lParam)
    {
        var inst = _currentInstance;
        if (inst is null || inst._windowHandle != hwnd)
            return Win32ViewportDefaultProc.DefWindowProc(hwnd, msg, wParam, lParam);
        return inst.DispatchWndProc(hwnd, msg, wParam, lParam);
    }

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

    void Trace(string msg) { if (_traceEnabled) System.Diagnostics.Debug.WriteLine(msg); }
}
