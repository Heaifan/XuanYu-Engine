namespace XuanYu.Editor.Input.Consumers;

public sealed class PickingViewportInputConsumer(IViewportD1ConsumerHandler handler)
    : ViewportD1InputConsumer(handler, ViewportInputDispatchKind.Handled)
{
    public override GestureOwner Owner => GestureOwner.Picking;
    public override int BeginPriority => 100;
}
