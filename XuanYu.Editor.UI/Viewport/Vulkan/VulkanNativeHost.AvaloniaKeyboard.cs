using Avalonia.Input;
using XuanYu.Editor.Input;

namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    static readonly ViewportKeySource AvaloniaKeySource = new("avalonia-viewport");

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (DataContext is UiVm vm)
            AvaloniaViewportKeyboardInputForwarder.Forward(vm.ViewportInput.Sink, e, AvaloniaKeySource);
        e.Handled = true;
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);
        if (DataContext is UiVm vm)
            AvaloniaViewportKeyboardInputForwarder.Forward(vm.ViewportInput.Sink, e, AvaloniaKeySource);
        e.Handled = true;
    }
}
