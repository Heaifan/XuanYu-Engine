namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    void HandleNativeCaptureChanged(UiVm vm, NativePointerMessage message)
    {
        if ((!_nativeDragActive && !_mapGeometryDragActive && !_nativeCameraActive) ||
            _expectedCaptureRelease || message.CaptureTarget == _hwnd) return;
        CancelNativeInput(vm, "PointerCaptureLost");
    }

    void CancelNativeInput(UiVm vm, string reason)
    {
        CancelNavGizmo(vm);
        if (_mapGeometryDragActive)
        {
            _mapGeometryDragActive = false;
            vm.CancelMapGeometryPointer(reason);
            ReleaseExpectedCapture();
        }
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
}
