using Avalonia.Input;
using XuanYu.Editor.Input;

namespace XuanYu.Editor.UI;

public static class AvaloniaViewportKeyboardInputForwarder
{
    public static void Forward(IViewportInputSink sink, AvaloniaKeySample sample, ViewportKeySource source) =>
        sink.Handle(AvaloniaKeyboardEventAdapter.Convert(sample, source));

    public static void Forward(IViewportInputSink sink, KeyEventArgs args, ViewportKeySource source) =>
        sink.Handle(AvaloniaKeyboardEventAdapter.Convert(args, source));
}
