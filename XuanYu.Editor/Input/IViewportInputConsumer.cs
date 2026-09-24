namespace XuanYu.Editor.Input;

public interface IViewportInputConsumer
{
    GestureOwner Owner { get; }
    ViewportInputDispatchResult Handle(EditorPointerEvent pointer, ViewportGestureState state);
}
