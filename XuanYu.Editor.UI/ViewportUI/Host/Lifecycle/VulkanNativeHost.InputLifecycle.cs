using Avalonia;
using Avalonia.Controls;
using XuanYu.Editor.Input;

namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    double GetDpiScale() => TopLevel.GetTopLevel(this)?.RenderScaling ?? 1d;

    void ForwardViewportDisposed()
    {
        if (DataContext is UiVm vm)
            NativeViewportInputForwarder.ForwardLifecycle(vm.ViewportInput.Sink, EditorPointerEventKind.ViewportDisposed, new("native-hwnd"));
    }
}
