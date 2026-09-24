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
        var observed = _consumers.Any(x => x.Handle(key, State).Kind != ViewportInputDispatchKind.Ignored);
        return observed ? ViewportInputDispatchResult.Observed : ViewportInputDispatchResult.Ignored;
    }

    void ResetKeyboardState(EditorPointerEventKind kind)
    {
        if (kind is EditorPointerEventKind.FocusLost or EditorPointerEventKind.WindowDeactivated
            or EditorPointerEventKind.ViewportDisposed) _pressedKeys.Clear();
    }
}
