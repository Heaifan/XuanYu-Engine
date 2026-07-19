namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    bool TryBeginGizmo(UiVm vm, long pointerId, double x, double y)
    {
        var viewport = CaptureViewportState();
        return vm.TryBeginMoveGizmoCapture(pointerId, x, y, viewport, _hwnd != 0);
    }
}
