using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.Editor.Input;

public sealed class ViewportInputComposition
{
    readonly RouterSink _sink;

    public ViewportInputComposition(
        IEnumerable<IViewportInputConsumer> consumers,
        IViewportPointerCaptureCoordinator captureCoordinator)
    {
        Consumers = consumers.ToArray();
        CaptureCoordinator = captureCoordinator;
        Router = new ViewportInputRouter(Consumers, CaptureCoordinator);
        _sink = new(Router);
    }

    public IReadOnlyList<IViewportInputConsumer> Consumers { get; }
    public IViewportPointerCaptureCoordinator CaptureCoordinator { get; }
    public ViewportInputRouter Router { get; }
    public ViewportGestureLifecycle Lifecycle => Router.Lifecycle;
    public IViewportInputSink Sink => _sink;
    public ViewportInputDispatchResult Dispatch(EditorPointerEvent pointer) => Router.Dispatch(pointer);

    sealed class RouterSink(ViewportInputRouter router) : IViewportInputSink
    {
        readonly ViewportInputRouter _router = router;
        public void Handle(EditorPointerEvent pointer) => _router.Dispatch(pointer);
    }
}
