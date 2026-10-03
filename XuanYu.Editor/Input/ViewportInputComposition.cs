using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.Editor.Input;

public sealed class ViewportInputComposition
{
    readonly RouterSink _sink;
    bool _terminating;

    public ViewportInputComposition(
        IEnumerable<IViewportInputConsumer> consumers,
        IViewportPointerCaptureCoordinator captureCoordinator,
        Action<EditorPointerEventKind>? clearTransientState = null)
    {
        Consumers = consumers.ToArray();
        CaptureCoordinator = captureCoordinator;
        Router = new ViewportInputRouter(Consumers, CaptureCoordinator, clearTransientState);
        _sink = new(Router);
    }

    public IReadOnlyList<IViewportInputConsumer> Consumers { get; }
    public IViewportPointerCaptureCoordinator CaptureCoordinator { get; }
    public ViewportInputRouter Router { get; }
    public ViewportGestureLifecycle Lifecycle => Router.Lifecycle;
    public IViewportInputSink Sink => _sink;
    public ViewportInputDispatchResult Dispatch(EditorPointerEvent pointer) => Router.Dispatch(pointer);
    public ViewportInputDispatchResult Dispatch(EditorKeyEvent key) => Router.Dispatch(key);
    public void BeginInteractionEpoch() => Router.BeginInteractionEpoch();

    public ViewportInputDispatchResult CancelForModeChange() =>
        Terminate(EditorPointerEventKind.ModeChanged);

    public ViewportInputDispatchResult CancelForToolChange() =>
        Terminate(EditorPointerEventKind.ToolChanged);

    public ViewportInputDispatchResult Terminate(EditorPointerEventKind kind)
    {
        if (_terminating) return ViewportInputDispatchResult.Ignored;
        _terminating = true;
        try
        {
            return Router.Dispatch(new EditorPointerEvent(
            kind, new(0, 0), EditorPointerButtons.None,
                EditorPointerModifiers.None, 0, Router.State.PointerId,
                new("lifecycle"), 1));
        }
        finally { _terminating = false; }
    }

    sealed class RouterSink(ViewportInputRouter router) : IViewportInputSink
    {
        readonly ViewportInputRouter _router = router;
        public void Handle(EditorPointerEvent pointer) => _router.Dispatch(pointer);
        public void Handle(EditorKeyEvent key) => _router.Dispatch(key);
    }

}
