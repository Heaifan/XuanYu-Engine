using Avalonia.Input;

namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        var point = e.GetCurrentPoint(this);
        if (!point.Properties.IsLeftButtonPressed) return;
        if (DataContext is not UiVm vm) return;
        var inViewport = point.Position.X >= 0 && point.Position.Y >= 0 &&
            point.Position.X <= Bounds.Width && point.Position.Y <= Bounds.Height;
        if (!vm.BeginViewportPointer(e.Pointer.Id, point.Position.X, point.Position.Y, inViewport, _hwnd != 0)) return;
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
        (DataContext as UiVm)?.CancelInteractionFromPointerCaptureLost();
        base.OnPointerCaptureLost(e);
    }
}
