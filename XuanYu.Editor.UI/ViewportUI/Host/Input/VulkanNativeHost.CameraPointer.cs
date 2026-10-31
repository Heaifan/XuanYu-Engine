using Avalonia.Input;

namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        if (_hwnd != 0) return;
        if (DataContext is UiVm vm)
            AvaloniaViewportInputForwarder.ForwardWheel(vm.ViewportInput.Sink, e, this,
                AvaloniaSource, GetDpiScale());
        e.Handled = true;
    }
}
