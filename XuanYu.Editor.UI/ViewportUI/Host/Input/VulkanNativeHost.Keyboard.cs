using XuanYu.Editor.Input;

namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    void OnNativeKeyMessage(NativeKeyMessage message)
    {
        if (DataContext is UiVm vm)
            NativeViewportKeyboardInputForwarder.Forward(vm.ViewportInput.Sink, message, new("native-hwnd"));
    }
}
