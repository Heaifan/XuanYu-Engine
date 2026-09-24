using Avalonia.Input;
using XuanYu.Editor.Input;

namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    static readonly ViewportPointerSource AvaloniaSource = new("avalonia-viewport");

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (_hwnd != 0) return;
        if (DataContext is not UiVm vm) return;
        AvaloniaViewportInputForwarder.ForwardPressed(vm.ViewportInput.Sink, e, this, AvaloniaSource, GetDpiScale());
        e.Pointer.Capture(this); e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (_hwnd != 0) return;
        if (DataContext is not UiVm vm) return;
        AvaloniaViewportInputForwarder.ForwardMoved(vm.ViewportInput.Sink, e, this, AvaloniaSource, GetDpiScale());
        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_hwnd != 0) return;
        if (DataContext is not UiVm vm) return;
        AvaloniaViewportInputForwarder.ForwardReleased(vm.ViewportInput.Sink, e, this, AvaloniaSource, GetDpiScale());
        e.Pointer.Capture(null); e.Handled = true;
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        if (_hwnd != 0) { base.OnPointerCaptureLost(e); return; }
        if (DataContext is UiVm vm)
            AvaloniaViewportInputForwarder.ForwardLifecycle(vm.ViewportInput.Sink,
                EditorPointerEventKind.CaptureLost, AvaloniaSource);
        base.OnPointerCaptureLost(e);
    }
}
