using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.Editor.Input;

public sealed partial class ViewportInputRouter
{
    ViewportInputDispatchResult Begin(EditorPointerEvent pointer)
    {
        var candidate = _consumers.Where(x => x.CanBegin(pointer, State))
            .OrderByDescending(x => x.BeginPriority).ThenBy(x => x.Owner).FirstOrDefault();
        if (candidate is null) return ViewportInputDispatchResult.Ignored;
        var result = candidate.Handle(pointer, State);
        if (!result.ClaimsGesture) return ViewportInputDispatchResult.Ignored;
        BeginInteractionEpoch();
        _activeConsumer.Set(candidate);
        _lifecycle.Begin(new("ViewportGesture", candidate.Owner, pointer.PointerId,
            result.Kind == ViewportInputDispatchKind.Captured
                ? ViewportGestureCapture.Pointer : ViewportGestureCapture.None, pointer));
        return result;
    }

    ViewportInputDispatchResult DispatchActive(EditorPointerEvent pointer)
    {
        if (pointer.PointerId != State.PointerId && !IsGlobalCancel(pointer.Kind))
            return ViewportInputDispatchResult.Ignored;
        if (IsGlobalCancel(pointer.Kind)) return Cancel(pointer.Kind);
        _lifecycle.Update(pointer);
        if (pointer.Kind == EditorPointerEventKind.Released)
            return End(ViewportInputDispatchKind.Released);
        return ViewportInputDispatchResult.Handled;
    }

    ViewportInputDispatchResult End(ViewportInputDispatchKind kind)
    {
        _lifecycle.Commit();
        _activeConsumer.Clear();
        return new(kind);
    }
}
