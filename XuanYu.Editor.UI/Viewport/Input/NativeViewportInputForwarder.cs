using XuanYu.Editor.Input;

namespace XuanYu.Editor.UI;

public sealed class NativeViewportInputForwarder(
    IViewportInputSink sink, ViewportPointerSource source)
{
    readonly IViewportInputSink _sink = sink;
    readonly ViewportPointerSource _source = source;

    public void Forward(NativePointerMessage message, double dpiScale = 1)
    {
        Forward(_sink, message, _source, dpiScale);
    }

    public static void Forward(
        IViewportInputSink sink, NativePointerMessage message,
        ViewportPointerSource source, double dpiScale = 1)
    {
        if (message.Message == NativePointerMessage.MouseLeave) return;
        sink.Handle(NativePointerEventAdapter.Convert(message, source, dpiScale));
    }

    public static void ForwardLifecycle(
        IViewportInputSink sink, EditorPointerEventKind kind, ViewportPointerSource source)
    {
        sink.Handle(new(kind, new(0, 0), EditorPointerButtons.None,
            EditorPointerModifiers.None, 0, 1, source, 1));
    }
}
