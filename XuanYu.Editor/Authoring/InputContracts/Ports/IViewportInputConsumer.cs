using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.Editor.Input;

public interface IViewportInputConsumer : IViewportGestureConsumer
{
    GestureOwner Owner { get; }
    int BeginPriority => 0;
    bool CanBegin(EditorPointerEvent pointer, ViewportGestureState state) => true;
    ViewportInputDispatchResult Handle(EditorPointerEvent pointer, ViewportGestureState state);
    ViewportInputDispatchResult Handle(EditorKeyEvent key, ViewportGestureState state) =>
        ViewportInputDispatchResult.Ignored;
}
