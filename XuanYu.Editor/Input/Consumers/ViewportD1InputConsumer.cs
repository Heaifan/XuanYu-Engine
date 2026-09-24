using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.Editor.Input.Consumers;

public abstract class ViewportD1InputConsumer(
    IViewportD1ConsumerHandler handler,
    ViewportInputDispatchKind claimKind) : IViewportInputConsumer
{
    readonly IViewportD1ConsumerHandler _handler = handler;
    readonly ViewportInputDispatchKind _claimKind = claimKind;
    public abstract GestureOwner Owner { get; }

    public ViewportInputDispatchResult Handle(EditorPointerEvent pointer, ViewportGestureState state)
    {
        if (state.IsActive) _handler.Update(new ViewportGestureContext(
            "ViewportGesture", Owner, state.PointerId, state.IsCaptured
                ? ViewportGestureCapture.Pointer : ViewportGestureCapture.None, pointer));
        if (pointer.Kind != EditorPointerEventKind.Pressed || state.IsActive)
            return ViewportInputDispatchResult.Ignored;
        return _handler.CanClaim(pointer) ? new(_claimKind) : ViewportInputDispatchResult.Ignored;
    }

    public void Begin(ViewportGestureContext context) => _handler.Begin(context);
    public void Update(ViewportGestureContext context) => _handler.Update(context);
    public void Commit(ViewportGestureContext context) => _handler.Commit(context);
    public void Cancel(ViewportCancellationContext context) => _handler.Cancel(context);
}
