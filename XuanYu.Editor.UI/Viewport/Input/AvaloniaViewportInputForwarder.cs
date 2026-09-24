using XuanYu.Editor.Input;
using Avalonia;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

public static class AvaloniaViewportInputForwarder
{
    public static void Forward(IViewportInputSink sink, AvaloniaPointerSample sample,
        ViewportPointerSource source, double dpiScale) =>
        sink.Handle(AvaloniaPointerEventAdapter.Convert(sample, source, dpiScale));

    public static void ForwardPressed(IViewportInputSink sink, PointerPressedEventArgs args,
        Visual relativeTo, ViewportPointerSource source, double dpiScale) =>
        sink.Handle(AvaloniaPointerEventAdapter.FromPressed(args, relativeTo, source, dpiScale));

    public static void ForwardMoved(IViewportInputSink sink, PointerEventArgs args,
        Visual relativeTo, ViewportPointerSource source, double dpiScale) =>
        sink.Handle(AvaloniaPointerEventAdapter.FromMove(args, relativeTo, source, dpiScale));

    public static void ForwardReleased(IViewportInputSink sink, PointerReleasedEventArgs args,
        Visual relativeTo, ViewportPointerSource source, double dpiScale) =>
        sink.Handle(AvaloniaPointerEventAdapter.FromReleased(args, relativeTo, source, dpiScale));

    public static void ForwardWheel(IViewportInputSink sink, PointerWheelEventArgs args,
        Visual relativeTo, ViewportPointerSource source, double dpiScale) =>
        sink.Handle(AvaloniaPointerEventAdapter.FromWheel(args, relativeTo, source, dpiScale));

    public static void ForwardLifecycle(IViewportInputSink sink, EditorPointerEventKind kind,
        ViewportPointerSource source) => sink.Handle(new EditorPointerEvent(kind, new(0, 0),
            EditorPointerButtons.None, EditorPointerModifiers.None, 0, 1, source, 1));
}
