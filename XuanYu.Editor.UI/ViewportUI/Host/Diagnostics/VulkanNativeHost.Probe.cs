using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        IsRendererReady = false;
        if (!_createdReported)
        {
            Report(NativeHostLifecycleState.Created, 0, 0, 0, 1d, false);
            _createdReported = true;
        }
        var snap = Report(NativeHostLifecycleState.Attached, _hwnd, (int)Bounds.Width, (int)Bounds.Height, GetDpiScale(), _hwnd != 0);
        TryAttach(snap);
    }

    NativeHostHandleSnapshot Report(NativeHostLifecycleState state, nint hwnd, int width, int height, double dpiScale, bool isValid)
    {
        var snapshot = _probe.Capture(state, hwnd, width, height, dpiScale, isValid);
        ViewportNativeHostRoute.Report(DataContext as UiVm, snapshot);
        return snapshot;
    }

    void ApplyNativeSize(int width, int height)
    {
        if (_lastNativeWidth == width && _lastNativeHeight == height) return;
        Win32ViewportHost.Resize(_hwnd, width, height);
        _lastNativeWidth = width;
        _lastNativeHeight = height;
    }
}
