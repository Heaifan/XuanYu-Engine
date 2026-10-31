namespace XuanYu.Editor.Input;

public sealed partial class ViewportInputRouter
{
    readonly HashSet<EditorKey> _pressedKeys = [];
    public IReadOnlySet<EditorKey> PressedKeys => _pressedKeys;
    public int KeyboardDispatchCount { get; private set; }

    public ViewportInputDispatchResult Dispatch(EditorKeyEvent key)
    {
        KeyboardDispatchCount++;
        if (key.Action == EditorKeyAction.Down) _pressedKeys.Add(key.Key);
        else _pressedKeys.Remove(key.Key);
        RefreshAltPreview(key);
        var observed = _consumers.Any(x => x.Handle(key, State).Kind != ViewportInputDispatchKind.Ignored);
        return observed ? ViewportInputDispatchResult.Observed : ViewportInputDispatchResult.Ignored;
    }

    void RefreshAltPreview(EditorKeyEvent key)
    {
        if (!State.IsActive || key.Key.VirtualKeyCode != 0x12 || _lifecycle.Current is not { } current) return;
        var modifiers = key.Action == EditorKeyAction.Down
            ? current.Input.Modifiers | EditorPointerModifiers.Alt
            : current.Input.Modifiers & ~EditorPointerModifiers.Alt;
        _lifecycle.Update(current.Input with { Kind = EditorPointerEventKind.Move, Modifiers = modifiers });
    }

    void ResetKeyboardState(EditorPointerEventKind kind)
    {
        if (kind is EditorPointerEventKind.FocusLost or EditorPointerEventKind.WindowDeactivated
            or EditorPointerEventKind.ViewportDisposed) _pressedKeys.Clear();
    }
}
