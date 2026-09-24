namespace XuanYu.Editor.Input;

public readonly record struct EditorKey(int VirtualKeyCode);

public enum EditorKeyAction
{
    Down,
    Up,
}

public readonly record struct ViewportKeySource(string Id);

public readonly record struct EditorKeyEvent(
    EditorKey Key,
    EditorKeyAction Action,
    EditorPointerModifiers Modifiers,
    bool IsRepeat,
    ViewportKeySource Source);
