using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.Editor.Input;

public sealed partial class ViewportInputRouter
{
    readonly IReadOnlyList<IViewportInputConsumer> _consumers;
    readonly RouterLifecycleConsumer _activeConsumer;
    readonly ViewportGestureLifecycle _lifecycle;
    public ViewportGestureState State => ViewportGestureState.From(_lifecycle.Current);
    public ViewportGestureLifecycle Lifecycle => _lifecycle;
    public IViewportPointerCaptureCoordinator CaptureCoordinator { get; }

    public ViewportInputRouter(IEnumerable<IViewportInputConsumer> consumers,
        IViewportPointerCaptureCoordinator capture)
    {
        _consumers = consumers.ToArray();
        if (_consumers.Any(x => x.Owner == GestureOwner.None) || _consumers.Select(x => x.Owner).Distinct().Count() != _consumers.Count)
            throw new ArgumentException("Each viewport consumer must have one unique GestureOwner.", nameof(consumers));
        _activeConsumer = new();
        CaptureCoordinator = capture;
        _lifecycle = new(_activeConsumer, capture, _ => { });
    }

    public ViewportInputDispatchResult Dispatch(EditorPointerEvent pointer)
    {
        ResetKeyboardState(pointer.Kind);
        return State.IsActive ? DispatchActive(pointer) : pointer.Kind == EditorPointerEventKind.Pressed
            ? Begin(pointer) : DispatchIdle(pointer);
    }

    ViewportInputDispatchResult Begin(EditorPointerEvent pointer)
    {
        var candidate = _consumers.Where(x => x.CanBegin(pointer, State))
            .OrderByDescending(x => x.BeginPriority)
            .ThenBy(x => x.Owner)
            .FirstOrDefault();
        if (candidate is null) return ViewportInputDispatchResult.Ignored;
        var result = candidate.Handle(pointer, State);
        if (!result.ClaimsGesture) return ViewportInputDispatchResult.Ignored;
        _activeConsumer.Set(candidate);
        _lifecycle.Begin(new("ViewportGesture", candidate.Owner, pointer.PointerId,
            result.Kind == ViewportInputDispatchKind.Captured
                ? ViewportGestureCapture.Pointer : ViewportGestureCapture.None, pointer));
        return result;
    }

    ViewportInputDispatchResult DispatchIdle(EditorPointerEvent pointer)
    {
        if (pointer.Kind is EditorPointerEventKind.Escape or EditorPointerEventKind.Cancel or EditorPointerEventKind.CaptureLost
            or EditorPointerEventKind.FocusLost or EditorPointerEventKind.WindowDeactivated)
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

    ViewportInputDispatchResult DispatchActive(EditorPointerEvent pointer)
    {
        if (pointer.PointerId != State.PointerId && !IsGlobalCancel(pointer.Kind)) return ViewportInputDispatchResult.Ignored;
        _lifecycle.Update(pointer);
        if (pointer.Kind is EditorPointerEventKind.Released) return End(ViewportInputDispatchKind.Released);
        if (IsGlobalCancel(pointer.Kind))
            return Cancel(pointer.Kind);
        return ViewportInputDispatchResult.Handled;
    }

    ViewportInputDispatchResult End(ViewportInputDispatchKind kind)
    {
        _lifecycle.Commit(); _activeConsumer.Clear();
        return new(kind);
    }

    sealed class RouterLifecycleConsumer : IViewportGestureConsumer
    {
        IViewportInputConsumer? _current;
        public void Set(IViewportInputConsumer consumer) => _current = consumer;
        public void Clear() => _current = null;
        public void Begin(ViewportGestureContext context) => _current?.Begin(context);
        public void Update(ViewportGestureContext context) => _current?.Handle(context.Input, ViewportGestureState.From(context));
        public void Commit(ViewportGestureContext context) => _current?.Commit(context);
        public void Cancel(ViewportCancellationContext context) => _current?.Cancel(context);
    }
}
