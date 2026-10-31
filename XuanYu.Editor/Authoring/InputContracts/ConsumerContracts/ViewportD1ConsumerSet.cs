namespace XuanYu.Editor.Input.Consumers;

public static class ViewportD1ConsumerSet
{
    public static IReadOnlyList<IViewportInputConsumer> Create(
        GizmoViewportInputConsumer gizmo,
        CameraViewportInputConsumer camera,
        PickingViewportInputConsumer picking) => [gizmo, camera, picking];
}
