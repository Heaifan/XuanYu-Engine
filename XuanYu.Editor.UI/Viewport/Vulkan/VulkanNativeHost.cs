using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using XuanYu.Render.Vulkan;

namespace XuanYu.Editor.UI;

public sealed class VulkanNativeHost : NativeControlHost
{
    readonly NativeHostLifecycleProbe _probe = new();
    readonly NativeHostResizeCoalescer _resizer;
    bool _createdReported;
    nint _hwnd;

    public VulkanNativeHost()
    {
        _resizer = new NativeHostResizeCoalescer(
            (snap, count) => ViewportNativeHostRoute.ReportMerged(DataContext as UiVm, snap, count));
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (!_createdReported)
        {
            Report(NativeHostLifecycleState.Created, 0, 0, 0, 1d, false);
            _createdReported = true;
        }
        Report(NativeHostLifecycleState.Attached, _hwnd, (int)Bounds.Width, (int)Bounds.Height, GetDpiScale(), _hwnd != 0);
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
        base.OnDetachedFromVisualTree(e);
    }

    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
        _resizer.Cancel();
        Report(NativeHostLifecycleState.Disposed, _hwnd, (int)Bounds.Width, (int)Bounds.Height, GetDpiScale(), false);
        Report(NativeHostLifecycleState.Invalidated, _hwnd, (int)Bounds.Width, (int)Bounds.Height, GetDpiScale(), false);
        if (_hwnd != 0) Win32ViewportHost.Destroy(_hwnd);
        _hwnd = 0;
    }

    void Report(NativeHostLifecycleState state, nint hwnd, int width, int height, double dpiScale, bool isValid)
    {
        var snapshot = _probe.Capture(state, hwnd, width, height, dpiScale, isValid);
        ViewportNativeHostRoute.Report(DataContext as UiVm, snapshot);
    }

    double GetDpiScale() => TopLevel.GetTopLevel(this)?.RenderScaling ?? 1d;
}
