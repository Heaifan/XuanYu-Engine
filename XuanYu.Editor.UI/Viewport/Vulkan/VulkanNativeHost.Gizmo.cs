namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    bool TryBeginGizmo(UiVm vm, long pointerId, double x, double y)
    {
        var viewport = CaptureViewportState();
        var valid = _hwnd != 0;
        if (vm.ActiveTool == "旋转")
            return vm.TryBeginRotateGizmoCapture(pointerId, x, y, viewport, valid);
        if (vm.ActiveTool == "缩放")
            return vm.TryBeginScaleGizmoCapture(pointerId, x, y, viewport, valid);
        return vm.TryBeginMoveGizmoCapture(pointerId, x, y, viewport, valid);
    }
}
