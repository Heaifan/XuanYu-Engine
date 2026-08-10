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
        var route = NativePointerRoutePolicy.Resolve(
            message, _nativeCameraActive, vm.IsRegionDrawingTool && vm.IsRegionDrawingDraftActive);
        if (route == NativePointerRoute.MiddleDown)
            TryBeginNativeCamera(vm, NativePointerId, x, y, message.IsShiftDown);
        else if (route == NativePointerRoute.CameraPreview)
        {
            PreviewNativeCamera(vm, x, y);
            return;
        }
        else if (route == NativePointerRoute.RegionPreview)
        {
            PreviewRegionDrawing(vm, x, y);
            return;
        }
        else if (route == NativePointerRoute.LeftDown)
        {
            if (ReportRegionDrawing(vm, x, y)) { ReleaseExpectedCapture(); return; }
            // F3-F1：导航 Gizmo 优先（右上角区域）；否则进入变换 Gizmo / Picking。
            if (TryNavGizmoPress(vm, x, y)) return;
            if (TryBeginGizmo(vm, NativePointerId, x, y)) { _nativeDragActive = true; return; }
            ReportPointerPicking(vm, x, y);
            ReleaseExpectedCapture();
        }
        else if (route == NativePointerRoute.LeftPreview)
        {
            if (TryNavGizmoMove(vm, x, y)) return;
            PreviewNativePointer(vm, x, y);
        }
        else if (route == NativePointerRoute.LeftUp)
        {
            if (TryNavGizmoRelease(vm, x, y)) return;
            CommitNativePointer(vm, x, y);
        }
        else if (route == NativePointerRoute.MiddleUp) EndNativeCamera(vm);
        else if (route == NativePointerRoute.Wheel) vm.DollyCamera(message.WheelDelta / 120.0);
        else if (route == NativePointerRoute.CaptureChanged) HandleNativeCaptureChanged(vm, message);
        else if (route == NativePointerRoute.KillFocus) CancelNativeInput(vm, "WindowFocusLost");
        else if (route == NativePointerRoute.CancelMode) CancelNativeInput(vm, "WM_CANCELMODE");
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
        CancelNavGizmo(vm);
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
