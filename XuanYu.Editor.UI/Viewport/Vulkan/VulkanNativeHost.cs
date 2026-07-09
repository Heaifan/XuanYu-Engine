using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Threading;
using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

public sealed class VulkanNativeHost : NativeControlHost
{
    readonly NativeHostLifecycleProbe _probe = new();
    readonly NativeHostResizeCoalescer _resizer;
    INativeHostSurfaceBridge? _bridge;
    bool _createdReported;
    nint _hwnd;

    public VulkanNativeHost()
    {
        _resizer = new NativeHostResizeCoalescer((snap, count) =>
        {
            _bridge?.Resize(snap.Width, snap.Height);
            ViewportNativeHostRoute.ReportMerged(DataContext as UiVm, snap, count);
        });
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
        _bridge ??= VulkanSurfaceBridgeProvider.Create(ReportVulkanMessage);
        _bridge.Attach(NativeHostSurfaceContract.ToSurfaceHandle(snap));
    }

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        _hwnd = Win32ViewportHost.CreateChild(parent.Handle);
        Report(NativeHostLifecycleState.HandleAvailable, _hwnd, (int)Bounds.Width, (int)Bounds.Height, GetDpiScale(), true);
        return new PlatformHandle(_hwnd, "HWND");
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        var width = (int)e.NewSize.Width;
        var height = (int)e.NewSize.Height;
        var isValid = _hwnd != 0 && width > 0 && height > 0;
        if (isValid) Win32ViewportHost.Resize(_hwnd, width, height);
        _resizer.OnResize(width, height, GetDpiScale(), isValid, _hwnd);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _resizer.Cancel();
        Report(NativeHostLifecycleState.Detached, _hwnd, (int)Bounds.Width, (int)Bounds.Height, GetDpiScale(), _hwnd != 0);
        _bridge?.Detach();
        base.OnDetachedFromVisualTree(e);
    }

    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
        _resizer.Cancel();
        Report(NativeHostLifecycleState.Disposed, _hwnd, (int)Bounds.Width, (int)Bounds.Height, GetDpiScale(), false);
        Report(NativeHostLifecycleState.Invalidated, _hwnd, (int)Bounds.Width, (int)Bounds.Height, GetDpiScale(), false);
        (_bridge as IDisposable)?.Dispose();
        _bridge = null;
        if (_hwnd != 0) Win32ViewportHost.Destroy(_hwnd);
        _hwnd = 0;
    }

    NativeHostHandleSnapshot Report(NativeHostLifecycleState state, nint hwnd, int width, int height, double dpiScale, bool isValid)
    {
        var snapshot = _probe.Capture(state, hwnd, width, height, dpiScale, isValid);
        ViewportNativeHostRoute.Report(DataContext as UiVm, snapshot);
        return snapshot;
    }

    double GetDpiScale() => TopLevel.GetTopLevel(this)?.RenderScaling ?? 1d;

    // VK4-D-R2：Vulkan 日志回调线程安全入口。Present 泵在后台线程打日志，
    // 访问 DataContext / UiVm / 日志集合必须回 UI 线程，否则 Avalonia 抛 InvalidOperationException。
    void ReportVulkanMessage(string msg)
    {
        if (Dispatcher.UIThread.CheckAccess()) ReportVulkanMessageOnUiThread(msg);
        else Dispatcher.UIThread.Post(() => ReportVulkanMessageOnUiThread(msg));
    }

    void ReportVulkanMessageOnUiThread(string msg) =>
        ViewportNativeHostRoute.ReportVulkanBridge(DataContext as UiVm, msg);
}
