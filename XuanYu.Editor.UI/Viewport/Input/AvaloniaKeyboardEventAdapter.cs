using Avalonia.Input;
using XuanYu.Editor.Input;

namespace XuanYu.Editor.UI;

public enum AvaloniaKeyAction { Down, Up }

[Flags]
public enum AvaloniaKeyModifiers { None, Shift, Control, Alt, Meta }

public readonly record struct AvaloniaKeySample(
    int Key, AvaloniaKeyAction Action, AvaloniaKeyModifiers Modifiers, bool IsRepeat);

public static class AvaloniaKeyboardEventAdapter
{
    public static EditorKeyEvent Convert(AvaloniaKeySample sample, ViewportKeySource source) =>
        new(new(MapKey(sample.Key)), sample.Action == AvaloniaKeyAction.Down ? EditorKeyAction.Down : EditorKeyAction.Up,
            MapModifiers(sample.Modifiers), sample.IsRepeat, source);

    public static EditorKeyEvent Convert(KeyEventArgs args, ViewportKeySource source) =>
        Convert(new AvaloniaKeySample((int)args.Key, Action(args), MapModifiers(args.KeyModifiers), false), source);

    static AvaloniaKeyAction Action(KeyEventArgs args) => args.RoutedEvent == InputElement.KeyUpEvent
        ? AvaloniaKeyAction.Up : AvaloniaKeyAction.Down;

    static int MapKey(int key) => key is (int)Key.LeftAlt or (int)Key.RightAlt ? 0x12 : key;

    static AvaloniaKeyModifiers MapModifiers(KeyModifiers modifiers) =>
        (modifiers.HasFlag(KeyModifiers.Shift) ? AvaloniaKeyModifiers.Shift : 0) |
        (modifiers.HasFlag(KeyModifiers.Control) ? AvaloniaKeyModifiers.Control : 0) |
        (modifiers.HasFlag(KeyModifiers.Alt) ? AvaloniaKeyModifiers.Alt : 0) |
        (modifiers.HasFlag(KeyModifiers.Meta) ? AvaloniaKeyModifiers.Meta : 0);

    static EditorPointerModifiers MapModifiers(AvaloniaKeyModifiers modifiers) =>
        (modifiers.HasFlag(AvaloniaKeyModifiers.Shift) ? EditorPointerModifiers.Shift : 0) |
        (modifiers.HasFlag(AvaloniaKeyModifiers.Control) ? EditorPointerModifiers.Control : 0) |
        (modifiers.HasFlag(AvaloniaKeyModifiers.Alt) ? EditorPointerModifiers.Alt : 0) |
        (modifiers.HasFlag(AvaloniaKeyModifiers.Meta) ? EditorPointerModifiers.Meta : 0);
}
