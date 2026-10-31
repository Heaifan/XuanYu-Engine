namespace XuanYu.Editor.Input;

public sealed partial class ViewportInputRouter
{
    public ViewportInputDispatchResult Dispatch(EditorPointerEvent pointer)
    {
        if (IsGlobalCancel(pointer.Kind) && _terminalDispatching)
            return ViewportInputDispatchResult.Ignored;
        ResetKeyboardState(pointer.Kind);
        var terminal = IsGlobalCancel(pointer.Kind);
        if (terminal) _terminalDispatching = true;
        try
        {
            var result = State.IsActive ? DispatchActive(pointer)
                : pointer.Kind == EditorPointerEventKind.Pressed
                    ? Begin(pointer) : DispatchIdle(pointer);
            if (terminal && !_terminalHandled)
            {
                _terminalHandled = true;
                _onTerminal?.Invoke(pointer.Kind);
            }
            return result;
        }
        finally { if (terminal) _terminalDispatching = false; }
    }
}
