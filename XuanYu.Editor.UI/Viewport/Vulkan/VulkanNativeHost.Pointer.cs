namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    const long NativePointerId = 1;
    bool _nativeDragActive;
    bool _mapGeometryDragActive;
    bool _expectedCaptureRelease;
    void OnNativePointerMessage(NativePointerMessage message)
    {
        var dpi = GetDpiScale();
        var x = message.PhysicalX / dpi;
        var y = message.PhysicalY / dpi;
        if (DataContext is not UiVm vm) return;
        var route = NativePointerRoutePolicy.Resolve(
            message, _nativeCameraActive, vm.IsRegionDrawingTool || (vm.IsRoadDrawingTool && vm.IsRoadDrawingDraftActive),
            _navGizmoPressed);
        if (route == NativePointerRoute.MiddleDown)
            TryBeginNativeCamera(vm, NativePointerId, x, y, message.IsShiftDown);
        else if (route == NativePointerRoute.CameraPreview)
        {
            PreviewNativeCamera(vm, x, y);
            return;
        }
        else if (route == NativePointerRoute.RegionPreview)
        {
            PreviewDrawing(vm, x, y);
            return;
        }
        else if (route == NativePointerRoute.LeftDown)
        {
            // F3-F1：导航 Gizmo 优先（右上角区域）；否则进入变换 Gizmo / Picking。
            if (TryNavGizmoPress(vm, x, y)) return;
            if (vm.TryBeginMapGeometryVertexPointer(x, y, CaptureViewportState()))
            {
                _mapGeometryDragActive = true; return;
            }
            if (ReportDrawing(vm, x, y)) { ReleaseExpectedCapture(); return; }
            if (vm.TryBeginMapGeometryPointer(x, y, CaptureViewportState()))
            {
                _mapGeometryDragActive = vm.IsMapGeometryDragActive;
                if (_mapGeometryDragActive) return;
                ReleaseExpectedCapture(); return;
            }
            if (TryBeginGizmo(vm, NativePointerId, x, y)) { _nativeDragActive = true; return; }
            ReportPointerPicking(vm, x, y);
            ReleaseExpectedCapture();
        }
        else if (route == NativePointerRoute.LeftPreview)
        {
            if (TryNavGizmoMove(vm, x, y)) return;
            if (_mapGeometryDragActive) { vm.PreviewMapGeometryPointer(x, y, CaptureViewportState()); return; }
            PreviewNativePointer(vm, x, y);
        }
        else if (route == NativePointerRoute.LeftUp)
        {
            if (TryNavGizmoRelease(vm, x, y)) return;
            if (_mapGeometryDragActive)
            {
                vm.CommitMapGeometryPointer(x, y, CaptureViewportState());
                _mapGeometryDragActive = false; ReleaseExpectedCapture(); return;
            }
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
    void ReleaseExpectedCapture()
    {
        _expectedCaptureRelease = true;
        Win32ViewportHost.ReleaseMouseCapture(_hwnd);
        _expectedCaptureRelease = false;
    }
    bool IsInBounds(double x, double y) =>
        x >= 0 && y >= 0 && x <= Bounds.Width && y <= Bounds.Height;
}
