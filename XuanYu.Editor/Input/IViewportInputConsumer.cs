using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.Editor.Input;

public interface IViewportInputConsumer : IViewportGestureConsumer
{
    GestureOwner Owner { get; }
    ViewportInputDispatchResult Handle(EditorPointerEvent pointer, ViewportGestureState state);
}
