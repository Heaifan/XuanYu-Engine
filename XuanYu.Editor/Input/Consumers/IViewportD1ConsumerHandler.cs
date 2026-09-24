using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.Editor.Input.Consumers;

public interface IViewportD1ConsumerHandler : IViewportGestureConsumer
{
    bool CanClaim(EditorPointerEvent pointer);
}
