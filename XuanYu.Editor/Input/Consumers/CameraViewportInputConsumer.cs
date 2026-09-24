namespace XuanYu.Editor.Input.Consumers;

public sealed class CameraViewportInputConsumer(IViewportD1ConsumerHandler handler)
    : ViewportD1InputConsumer(handler, ViewportInputDispatchKind.Captured)
{
    public override GestureOwner Owner => GestureOwner.Camera;
    public override int BeginPriority => 300;
}
