using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.Editor.Input.Consumers;

public interface INavigationViewportInputHandler : IViewportGestureConsumer
{
    bool CanClaim(EditorPointerEvent pointer);
    void Observe(EditorPointerEvent pointer);
}
