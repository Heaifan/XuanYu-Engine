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
            if (TryBeginGizmo(vm, NativePointerId, x, y)) { _nativeDragActive = true; return; }
            ReportPointerPicking(vm, x, y);
            ReleaseExpectedCapture();
        }
        else if (message.Message == NativePointerMessage.MiddleDown)
            TryBeginNativeCamera(vm, NativePointerId, x, y, message.IsShiftDown);
        else if (message.Message == NativePointerMessage.Move && message.IsLeftButtonDown) PreviewNativePointer(vm, x, y);
        else if (message.Message == NativePointerMessage.Move && message.IsMiddleButtonDown) PreviewNativeCamera(vm, x, y);
        else if (message.Message == NativePointerMessage.LeftUp) CommitNativePointer(vm, x, y);
        else if (message.Message == NativePointerMessage.MiddleUp) EndNativeCamera(vm);
        else if (message.Message == NativePointerMessage.Wheel) vm.DollyCamera(message.WheelDelta / 120.0);
        else if (message.Message == NativePointerMessage.CaptureChanged) HandleNativeCaptureChanged(vm, message);
        else if (message.Message == NativePointerMessage.KillFocus) CancelNativeInput(vm, "WindowFocusLost");
        else if (message.Message == NativePointerMessage.CancelMode) CancelNativeInput(vm, "WM_CANCELMODE");
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
        if ((!_nativeDragActive && !_nativeCameraActive) || _expectedCaptureRelease || message.CaptureTarget == _hwnd) return;
        CancelNativeInput(vm, "PointerCaptureLost");
    }
    void CancelNativeInput(UiVm vm, string reason)
    {
        CancelNativePointer(vm, reason);
        CancelNativeCamera(vm, reason);
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
}
