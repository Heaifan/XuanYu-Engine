namespace XuanYu.Editor.Input.Lifecycle;

public sealed class ViewportGestureLifecycle
{
    readonly IViewportGestureConsumer _consumer;
    readonly Action<ViewportGestureContext> _releaseCapture;
    readonly Action<ViewportGestureContext> _clearTemporaryState;
    ViewportGestureContext? _current;

    public ViewportGestureLifecycle(
        IViewportGestureConsumer consumer,
        Action<ViewportGestureContext> releaseCapture,
        Action<ViewportGestureContext> clearTemporaryState)
    {
        _consumer = consumer;
        _releaseCapture = releaseCapture;
        _clearTemporaryState = clearTemporaryState;
    }

    public ViewportGestureLifecycleState State =>
        _current is null ? ViewportGestureLifecycleState.Idle :
        ViewportGestureLifecycleState.Active;

    public ViewportGestureContext? Current => _current;

    public bool Begin(ViewportGestureContext context)
    {
        if (_current is not null) return false;
        _current = context;
        _consumer.Begin(context);
        return true;
    }

    public bool Update(EditorPointerEvent input)
    {
        if (_current is null) return false;
        _current = _current with { Input = input };
        _consumer.Update(_current);
        return true;
    }

    public ViewportGestureLifecycleResult Commit() =>
        Finish(ViewportGestureTerminalKind.Committed, null);

    public ViewportGestureLifecycleResult Cancel(ViewportCancellationReason reason) =>
        Finish(ViewportGestureTerminalKind.Canceled, reason);

    ViewportGestureLifecycleResult Finish(
        ViewportGestureTerminalKind terminal,
        ViewportCancellationReason? reason)
    {
        if (_current is null) return ViewportGestureLifecycleResult.NoOp;
        var context = _current;
        if (terminal == ViewportGestureTerminalKind.Canceled)
            _consumer.Cancel(new ViewportCancellationContext(context, reason!.Value));
        else _consumer.Commit(context);
        _releaseCapture(context);
        _clearTemporaryState(context);
        _current = null;
        return new(ViewportGestureLifecycleState.Idle, terminal, true, true, true);
    }
}
