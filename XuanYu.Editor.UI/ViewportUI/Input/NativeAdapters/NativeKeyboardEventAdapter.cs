using XuanYu.Editor.Input;

namespace XuanYu.Editor.UI;

public static class NativeKeyboardEventAdapter
{
    public static EditorKeyEvent Convert(NativeKeyMessage message, ViewportKeySource source) =>
        new(new(message.VirtualKeyCode), Action(message.Message), Modifiers(message), message.IsRepeat, source);

    static EditorKeyAction Action(uint message) => message is NativeKeyMessage.KeyUp or NativeKeyMessage.SystemKeyUp
        ? EditorKeyAction.Up : EditorKeyAction.Down;

    static EditorPointerModifiers Modifiers(NativeKeyMessage message) =>
        (message.ShiftDown ? EditorPointerModifiers.Shift : EditorPointerModifiers.None) |
        (message.ControlDown ? EditorPointerModifiers.Control : EditorPointerModifiers.None) |
        (message.AltDown ? EditorPointerModifiers.Alt : EditorPointerModifiers.None) |
        (message.MetaDown ? EditorPointerModifiers.Meta : EditorPointerModifiers.None);
}
