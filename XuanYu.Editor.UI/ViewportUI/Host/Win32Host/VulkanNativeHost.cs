using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using XuanYu.Render.Abstractions;
namespace XuanYu.Editor.UI;
public sealed partial class VulkanNativeHost : NativeControlHost
{
    readonly NativeHostLifecycleProbe _probe = new();
    readonly NativeHostResizeCoalescer _resizer;
    INativeHostSurfaceBridge? _bridge;
    bool _createdReported;
    bool _layoutSyncHooked;
    int _lastNativeWidth;
    int _lastNativeHeight;
    int _lastBridgeWidth;
    int _lastBridgeHeight;
    nint _hwnd;
    internal event EventHandler? RendererReady; internal bool IsRendererReady { get; private set; }
    public VulkanNativeHost()
    {
        Focusable = false;
        FocusAdorner = null;
        _resizer = new NativeHostResizeCoalescer((snap, count) =>
        {
            TryAttach(snap);
            var physical = NativeHostSurfaceContract.ToSurfaceHandle(snap);
            if (_bridge is not null && (physical.Width != _lastBridgeWidth || physical.Height != _lastBridgeHeight))
            {
                _bridge.Resize(physical.Width, physical.Height);
                _lastBridgeWidth = physical.Width;
                _lastBridgeHeight = physical.Height;
            }
            ViewportNativeHostRoute.ReportMerged(DataContext as UiVm, snap, count);
        });
        DataContextChanged += (_, _) => HookLayoutSync(); DataContextChanged += OnMapContextDataContextChanged; LostFocus += OnAvaloniaLostFocus;
    }
    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        _lastNativeWidth = 0;
        _lastNativeHeight = 0;
        _hwnd = Win32ViewportHost.CreateChild(parent.Handle);
        Win32ViewportHost.SetInputSinks(_hwnd, OnNativePointerMessage, OnNativeKeyMessage);
        Report(NativeHostLifecycleState.HandleAvailable, _hwnd, (int)Bounds.Width, (int)Bounds.Height, GetDpiScale(), true);
        return new PlatformHandle(_hwnd, "HWND");
    }
    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        var width = (int)e.NewSize.Width;
        var height = (int)e.NewSize.Height;
        var isValid = _hwnd != 0 && width > 0 && height > 0;
        var dpi = GetDpiScale();
        (DataContext as UiVm)?.UpdateViewportDpi(dpi);
        if (isValid)
        {
            var (physicalW, physicalH) = ToPhysicalSize(width, height, dpi);
            ApplyNativeSize(physicalW, physicalH);
            (DataContext as UiVm)?.UpdateViewportFrame(width, height);
        }
        _resizer.OnResize(width, height, dpi, isValid, _hwnd);
    }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ExitDiagnosticPointer();
        ForwardViewportDisposed();
        _resizer.Cancel();
        UnhookLayoutSync();
        Report(NativeHostLifecycleState.Detached, _hwnd, (int)Bounds.Width, (int)Bounds.Height, GetDpiScale(), _hwnd != 0);
        _bridge?.Detach();
        IsRendererReady = false;
        base.OnDetachedFromVisualTree(e);
    }
    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
        ExitDiagnosticPointer();
        ForwardViewportDisposed();
        _resizer.Cancel();
        UnhookLayoutSync();
        Report(NativeHostLifecycleState.Disposed, _hwnd, (int)Bounds.Width, (int)Bounds.Height, GetDpiScale(), false);
        Report(NativeHostLifecycleState.Invalidated, _hwnd, (int)Bounds.Width, (int)Bounds.Height, GetDpiScale(), false);
        (_bridge as IDisposable)?.Dispose();
        _bridge = null;
        IsRendererReady = false;
        if (_hwnd != 0)
        {
            Win32ViewportHost.ClearInputSinks(_hwnd);
            Win32ViewportHost.Destroy(_hwnd);
        }
        _hwnd = 0;
    }
}
