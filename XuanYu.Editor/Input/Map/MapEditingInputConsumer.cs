using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.Editor.Input.Map;

public abstract class MapEditingInputConsumer(GestureOwner owner, IMapEditingInputBackend backend)
    : IViewportInputConsumer
{
    readonly IMapEditingInputBackend _backend = backend;
    public GestureOwner Owner { get; } = owner;
    public int BeginPriority => 200;
    public bool CanBegin(EditorPointerEvent pointer, ViewportGestureState state) =>
        pointer.Kind == EditorPointerEventKind.Pressed && _backend.CanBegin(pointer, state);
    public ViewportInputDispatchResult Handle(EditorPointerEvent pointer, ViewportGestureState state)
    {
        if (pointer.Kind == EditorPointerEventKind.Pressed) return ViewportInputDispatchResult.Captured;
        if (pointer.Kind == EditorPointerEventKind.Move) _backend.Update(Context(pointer, state));
        return ViewportInputDispatchResult.Handled;
    }
    public void Begin(ViewportGestureContext context) => _backend.Begin(context);
    public void Update(ViewportGestureContext context) => _backend.Update(context);
    public void Commit(ViewportGestureContext context) => _backend.Commit(context);
    public void Cancel(ViewportCancellationContext context) => _backend.Cancel(context);
    ViewportGestureContext Context(EditorPointerEvent input, ViewportGestureState state) =>
        new("MapEditing", state.Owner, state.PointerId, state.IsCaptured
            ? ViewportGestureCapture.Pointer : ViewportGestureCapture.None, input);
}
