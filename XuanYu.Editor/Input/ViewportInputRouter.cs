namespace XuanYu.Editor.Input;

public sealed class ViewportInputRouter
{
    readonly IReadOnlyList<IViewportInputConsumer> _consumers;
    readonly IViewportPointerCaptureCoordinator _capture;
    public ViewportGestureState State { get; private set; } = ViewportGestureState.Idle;

    public ViewportInputRouter(IEnumerable<IViewportInputConsumer> consumers,
        IViewportPointerCaptureCoordinator capture)
    {
        _consumers = consumers.ToArray(); _capture = capture;
        if (_consumers.Any(x => x.Owner == GestureOwner.None) || _consumers.Select(x => x.Owner).Distinct().Count() != _consumers.Count)
            throw new ArgumentException("Each viewport consumer must have one unique GestureOwner.", nameof(consumers));
    }

    public ViewportInputDispatchResult Dispatch(EditorPointerEvent pointer) => State.IsActive
        ? DispatchActive(pointer) : pointer.Kind == EditorPointerEventKind.Pressed
            ? Begin(pointer) : DispatchIdle(pointer);

    ViewportInputDispatchResult Begin(EditorPointerEvent pointer)
    {
        foreach (var consumer in _consumers)
        {
            var result = consumer.Handle(pointer, State);
            if (!result.ClaimsGesture) continue;
            State = new(ViewportGesturePhase.Active, consumer.Owner, pointer.PointerId, result.Kind == ViewportInputDispatchKind.Captured);
            if (State.IsCaptured) _capture.Capture(pointer.PointerId, consumer.Owner);
            return result;
        }
        return ViewportInputDispatchResult.Ignored;
    }

    ViewportInputDispatchResult DispatchIdle(EditorPointerEvent pointer)
    {
        var observed = false;
        foreach (var consumer in _consumers)
        {
            var result = consumer.Handle(pointer, State);
            observed |= result.Kind == ViewportInputDispatchKind.Observed;
            if (result.ClaimsGesture) return result;
        }
        return observed ? ViewportInputDispatchResult.Observed : ViewportInputDispatchResult.Ignored;
    }

    ViewportInputDispatchResult DispatchActive(EditorPointerEvent pointer)
    {
        if (pointer.PointerId != State.PointerId && pointer.Kind != EditorPointerEventKind.CaptureLost) return ViewportInputDispatchResult.Ignored;
        var consumer = _consumers.Single(x => x.Owner == State.Owner);
        consumer.Handle(pointer, State);
        if (pointer.Kind is EditorPointerEventKind.Released) return End(ViewportInputDispatchKind.Released);
        if (pointer.Kind is EditorPointerEventKind.Cancel or EditorPointerEventKind.CaptureLost or EditorPointerEventKind.FocusLost or EditorPointerEventKind.WindowDeactivated)
            return End(ViewportInputDispatchKind.Cancelled);
        return ViewportInputDispatchResult.Handled;
    }

    ViewportInputDispatchResult End(ViewportInputDispatchKind kind)
    {
        if (State.IsCaptured) _capture.Release(State.PointerId, State.Owner);
        State = ViewportGestureState.Idle;
        return new(kind);
    }
}
