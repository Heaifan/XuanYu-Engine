using Avalonia.Input;

namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    const long NativePointerId = 1;
    bool _nativeDragActive;
    bool _expectedCaptureRelease;
    void OnNativePointerMessage(NativePointerMessage message)
    {
        var dpi = GetDpiScale();
        var x = message.PhysicalX / dpi;
        var y = message.PhysicalY / dpi;
        if (DataContext is not UiVm vm) return;
        if (message.Message == NativePointerMessage.LeftDown)
        {
            ReportPointerPicking(vm, x, y);
            BeginNativePointer(vm, x, y);
        }
        else if (message.Message == NativePointerMessage.Move && message.IsLeftButtonDown) PreviewNativePointer(vm, x, y);
        else if (message.Message == NativePointerMessage.LeftUp) CommitNativePointer(vm, x, y);
        else if (message.Message == NativePointerMessage.CaptureChanged) HandleNativeCaptureChanged(vm, message);
        else if (message.Message == NativePointerMessage.KillFocus) CancelNativePointer(vm, "WindowFocusLost");
        else if (message.Message == NativePointerMessage.CancelMode) CancelNativePointer(vm, "WM_CANCELMODE");
    }
    void BeginNativePointer(UiVm vm, double x, double y)
    {
        if (!Win32ViewportHost.HasMouseCapture(_hwnd)) return;
        if (vm.BeginViewportPointer(NativePointerId, x, y, IsInBounds(x, y), _hwnd != 0)) _nativeDragActive = true;
        else ReleaseExpectedCapture();
    }
    void PreviewNativePointer(UiVm vm, double x, double y)
    {
        if (_nativeDragActive) vm.PreviewViewportPointer(NativePointerId, x, y);
    }
    void CommitNativePointer(UiVm vm, double x, double y)
    {
        if (!_nativeDragActive) return;
        vm.CommitViewportPointer(NativePointerId, x, y);
        _nativeDragActive = false;
        ReleaseExpectedCapture();
    }
    void HandleNativeCaptureChanged(UiVm vm, NativePointerMessage message)
    {
        if (!_nativeDragActive || _expectedCaptureRelease || message.CaptureTarget == _hwnd) return;
        CancelNativePointer(vm, "PointerCaptureLost");
    }
    void CancelNativePointer(UiVm vm, string reason)
    {
        if (!_nativeDragActive) return;
        _nativeDragActive = false;
        vm.CancelInteractionFromNativePointer(reason);
        ReleaseExpectedCapture();
    }
    void ReleaseExpectedCapture()
    {
        _expectedCaptureRelease = true;
        Win32ViewportHost.ReleaseMouseCapture(_hwnd);
        _expectedCaptureRelease = false;
    }
    bool IsInBounds(double x, double y) =>
        x >= 0 && y >= 0 && x <= Bounds.Width && y <= Bounds.Height;
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        var point = e.GetCurrentPoint(this);
        if (!point.Properties.IsLeftButtonPressed) return;
        if (DataContext is not UiVm vm) return;
        ReportPointerPicking(vm, point.Position.X, point.Position.Y);
        if (!vm.BeginViewportPointer(e.Pointer.Id, point.Position.X, point.Position.Y,
            IsInBounds(point.Position.X, point.Position.Y), _hwnd != 0)) return;
        e.Pointer.Capture(this);
        e.Handled = true;
    }
    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        var point = e.GetCurrentPoint(this);
        if (DataContext is UiVm vm && vm.PreviewViewportPointer(
            e.Pointer.Id, point.Position.X, point.Position.Y))
            e.Handled = true;
    }
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        var point = e.GetCurrentPoint(this);
        if (DataContext is UiVm vm && vm.CommitViewportPointer(
            e.Pointer.Id, point.Position.X, point.Position.Y))
        {
            e.Pointer.Capture(null);
            e.Handled = true;
        }
    }
    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        (DataContext as UiVm)?.CancelInteractionFromNativePointer("PointerCaptureLost");
        base.OnPointerCaptureLost(e);
    }
}
