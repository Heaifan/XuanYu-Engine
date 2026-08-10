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
    nint _hwnd;
    public VulkanNativeHost()
    {
        Focusable = false;
        FocusAdorner = null;
        _resizer = new NativeHostResizeCoalescer((snap, count) =>
        {
            _bridge?.Resize(snap.Width, snap.Height);
            ViewportNativeHostRoute.ReportMerged(DataContext as UiVm, snap, count);
        });
        DataContextChanged += (_, _) => { HookLayoutSync(); HookScaleIndicator(); };
    }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (!_createdReported)
        {
            Report(NativeHostLifecycleState.Created, 0, 0, 0, 1d, false);
            _createdReported = true;
        }
        var snap = Report(NativeHostLifecycleState.Attached, _hwnd, (int)Bounds.Width, (int)Bounds.Height, GetDpiScale(), _hwnd != 0);
        _bridge ??= CreateBridge();
        _bridge.Attach(NativeHostSurfaceContract.ToSurfaceHandle(snap));
    }
    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        _hwnd = Win32ViewportHost.CreateChild(parent.Handle);
        CreateNativeScaleIndicator();
        Win32ViewportHost.SetInputSink(_hwnd, OnNativePointerMessage);
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
            Win32ViewportHost.Resize(_hwnd, physicalW, physicalH);
            (DataContext as UiVm)?.UpdateViewportFrame(width, height);
            UpdateNativeScaleIndicator();
        }
        _resizer.OnResize(width, height, dpi, isValid, _hwnd);
    }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (DataContext is UiVm vm) CancelNativeInput(vm, "HostDetached");
        _resizer.Cancel();
        UnhookLayoutSync();
        UnhookScaleIndicator();
        Report(NativeHostLifecycleState.Detached, _hwnd, (int)Bounds.Width, (int)Bounds.Height, GetDpiScale(), _hwnd != 0);
        _bridge?.Detach();
        base.OnDetachedFromVisualTree(e);
    }
    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
        _resizer.Cancel();
        UnhookLayoutSync();
        UnhookScaleIndicator();
        Report(NativeHostLifecycleState.Disposed, _hwnd, (int)Bounds.Width, (int)Bounds.Height, GetDpiScale(), false);
        Report(NativeHostLifecycleState.Invalidated, _hwnd, (int)Bounds.Width, (int)Bounds.Height, GetDpiScale(), false);
        (_bridge as IDisposable)?.Dispose();
        _bridge = null;
        if (_hwnd != 0)
        {
            DestroyNativeScaleIndicator();
            Win32ViewportHost.SetInputSink(_hwnd, null);
            Win32ViewportHost.Destroy(_hwnd);
        }
        _hwnd = 0;
    }
    NativeHostHandleSnapshot Report(NativeHostLifecycleState state, nint hwnd, int width, int height, double dpiScale, bool isValid)
    {
        var snapshot = _probe.Capture(state, hwnd, width, height, dpiScale, isValid);
        ViewportNativeHostRoute.Report(DataContext as UiVm, snapshot);
        return snapshot;
    }
    double GetDpiScale() => TopLevel.GetTopLevel(this)?.RenderScaling ?? 1d;
}
