using Avalonia.Input;

namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    const long NativePointerId = 1;

    void OnNativePointerMessage(NativePointerMessage message)
    {
        var dpi = GetDpiScale();
        var x = message.PhysicalX / dpi;
        var y = message.PhysicalY / dpi;
        if (DataContext is not UiVm vm) return;
        if (message.Message == NativePointerMessage.LeftDown)
            vm.BeginViewportPointer(NativePointerId, x, y, IsInBounds(x, y), _hwnd != 0);
        else if (message.Message == NativePointerMessage.Move && message.IsLeftButtonDown)
            vm.PreviewViewportPointer(NativePointerId, x, y);
        else if (message.Message == NativePointerMessage.LeftUp)
            vm.CommitViewportPointer(NativePointerId, x, y);
        else if (message.Message is NativePointerMessage.CaptureChanged or NativePointerMessage.KillFocus)
            vm.CancelInteractionFromPointerCaptureLost();
    }

    bool IsInBounds(double x, double y) =>
        x >= 0 && y >= 0 && x <= Bounds.Width && y <= Bounds.Height;

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        var point = e.GetCurrentPoint(this);
        if (!point.Properties.IsLeftButtonPressed) return;
        if (DataContext is not UiVm vm) return;
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
        (DataContext as UiVm)?.CancelInteractionFromPointerCaptureLost();
        base.OnPointerCaptureLost(e);
    }
}
