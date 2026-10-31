using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.Editor.Input;

public sealed partial class ViewportInputRouter
{
    readonly IReadOnlyList<IViewportInputConsumer> _consumers;
    readonly RouterLifecycleConsumer _activeConsumer;
    readonly ViewportGestureLifecycle _lifecycle;
    readonly Action<EditorPointerEventKind>? _onTerminal;
    bool _terminalHandled, _terminalDispatching;
    public ViewportGestureState State => ViewportGestureState.From(_lifecycle.Current);
    public ViewportGestureLifecycle Lifecycle => _lifecycle;
    public IViewportPointerCaptureCoordinator CaptureCoordinator { get; }

    public ViewportInputRouter(IEnumerable<IViewportInputConsumer> consumers,
        IViewportPointerCaptureCoordinator capture,
        Action<EditorPointerEventKind>? onTerminal = null)
    {
        _consumers = consumers.ToArray();
        if (_consumers.Any(x => x.Owner == GestureOwner.None) ||
            _consumers.Select(x => x.Owner).Distinct().Count() != _consumers.Count)
            throw new ArgumentException("Each viewport consumer must have one unique GestureOwner.", nameof(consumers));
        _activeConsumer = new();
        CaptureCoordinator = capture;
        _onTerminal = onTerminal;
        _lifecycle = new(_activeConsumer, capture, _ => { });
    }

    public void BeginInteractionEpoch()
    {
        if (!_terminalDispatching) _terminalHandled = false;
    }

    sealed class RouterLifecycleConsumer : IViewportGestureConsumer
    {
        IViewportInputConsumer? _current;
        public void Set(IViewportInputConsumer consumer) => _current = consumer;
        public void Clear() => _current = null;
        public void Begin(ViewportGestureContext context) => _current?.Begin(context);
        public void Update(ViewportGestureContext context) => _current?.Handle(
            context.Input, ViewportGestureState.From(context));
        public void Commit(ViewportGestureContext context) => _current?.Commit(context);
        public void Cancel(ViewportCancellationContext context) => _current?.Cancel(context);
    }
}
