using Avalonia.Input;
using Avalonia.Interactivity;
using XuanYu.Editor.Input;

namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    void OnAvaloniaLostFocus(object? sender, RoutedEventArgs e)
    {
        if (_hwnd != 0) return;
        if (DataContext is UiVm vm)
            AvaloniaViewportInputForwarder.ForwardLifecycle(vm.ViewportInput.Sink,
                EditorPointerEventKind.FocusLost, AvaloniaSource);
    }
}
