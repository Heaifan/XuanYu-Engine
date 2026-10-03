namespace XuanYu.Editor.Input;

public sealed partial class ViewportInputRouter
{
    ViewportInputDispatchResult DispatchIdle(EditorPointerEvent pointer)
    {
        if (pointer.Kind is EditorPointerEventKind.Escape or EditorPointerEventKind.Cancel
            or EditorPointerEventKind.CaptureLost or EditorPointerEventKind.FocusLost
            or EditorPointerEventKind.WindowDeactivated)
            return ViewportInputDispatchResult.Ignored;
        var observed = false;
        foreach (var consumer in _consumers)
        {
            var result = consumer.Handle(pointer, State);
            observed |= result.Kind == ViewportInputDispatchKind.Observed;
            if (result.ClaimsGesture) return result;
        }
        return observed ? ViewportInputDispatchResult.Observed : ViewportInputDispatchResult.Ignored;
    }
}
