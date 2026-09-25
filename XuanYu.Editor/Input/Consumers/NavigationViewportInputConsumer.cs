using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.Editor.Input.Consumers;

public sealed class NavigationViewportInputConsumer(
    INavigationViewportInputHandler handler) : IViewportInputConsumer
{
    readonly INavigationViewportInputHandler _handler = handler;
    public GestureOwner Owner => GestureOwner.Navigation;
    public int BeginPriority => 500;
    public bool CanBegin(EditorPointerEvent pointer, ViewportGestureState state) =>
        pointer.Kind == EditorPointerEventKind.Pressed && _handler.CanClaim(pointer);
    public ViewportInputDispatchResult Handle(EditorPointerEvent pointer, ViewportGestureState state)
    {
        if (!state.IsActive) _handler.Observe(pointer);
        return CanBegin(pointer, state) ? ViewportInputDispatchResult.Captured :
            state.IsActive ? ViewportInputDispatchResult.Ignored : ViewportInputDispatchResult.Observed;
    }
    public void Begin(ViewportGestureContext context) => _handler.Begin(context);
    public void Update(ViewportGestureContext context) => _handler.Update(context);
    public void Commit(ViewportGestureContext context) => _handler.Commit(context);
    public void Cancel(ViewportCancellationContext context) => _handler.Cancel(context);
}
