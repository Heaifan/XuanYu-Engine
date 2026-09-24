namespace XuanYu.Editor.Input.Consumers;

public sealed class GizmoViewportInputConsumer(IViewportD1ConsumerHandler handler)
    : ViewportD1InputConsumer(handler, ViewportInputDispatchKind.Captured)
{
    public override GestureOwner Owner => GestureOwner.Gizmo;
}
