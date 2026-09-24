using XuanYu.Editor.Input;

namespace XuanYu.Editor.UI;

public static class NativeViewportKeyboardInputForwarder
{
    public static void Forward(IViewportInputSink sink, NativeKeyMessage message, ViewportKeySource source) =>
        sink.Handle(NativeKeyboardEventAdapter.Convert(message, source));
}
