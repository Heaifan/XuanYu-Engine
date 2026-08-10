using Avalonia.Input;

namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        var point = e.GetCurrentPoint(this);
        if (TryBeginAvaloniaCamera(e, point)) return;
        if (!point.Properties.IsLeftButtonPressed) return;
        if (DataContext is not UiVm vm) return;
        if (TryNavGizmoPress(vm, point.Position.X, point.Position.Y))
        {
            e.Pointer.Capture(this); e.Handled = true; return;
        }
        if (ReportRegionDrawing(vm, point.Position.X, point.Position.Y))
        {
            e.Handled = true; return;
        }
        if (TryBeginGizmo(vm, e.Pointer.Id, point.Position.X, point.Position.Y))
        {
            e.Pointer.Capture(this); e.Handled = true; return;
        }
        ReportPointerPicking(vm, point.Position.X, point.Position.Y);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        var point = e.GetCurrentPoint(this);
        if (DataContext is UiVm vm && TryNavGizmoMove(vm, point.Position.X, point.Position.Y))
            e.Handled = true;
        else if (DataContext is UiVm vm2 && PreviewRegionDrawing(vm2, point.Position.X, point.Position.Y))
            e.Handled = true;
        else if (DataContext is UiVm vm3 && vm3.PreviewViewportPointer(
            e.Pointer.Id, point.Position.X, point.Position.Y))
            e.Handled = true;
        else if (DataContext is UiVm cameraVm && cameraVm.PreviewCameraNavigation(
            e.Pointer.Id, point.Position.X, point.Position.Y)) e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        var point = e.GetCurrentPoint(this);
        if (DataContext is UiVm vm && TryNavGizmoRelease(vm, point.Position.X, point.Position.Y))
            ReleaseAvaloniaCapture(e);
        else if (DataContext is UiVm vm2 && vm2.CommitViewportPointer(
            e.Pointer.Id, point.Position.X, point.Position.Y))
            ReleaseAvaloniaCapture(e);
        else if (DataContext is UiVm cameraVm && cameraVm.EndCameraNavigation(e.Pointer.Id))
            ReleaseAvaloniaCapture(e);
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        if (DataContext is UiVm vm)
        {
            CancelNavGizmo(vm);
            vm.CancelInteractionFromNativePointer("PointerCaptureLost");
        }
        base.OnPointerCaptureLost(e);
    }
}
