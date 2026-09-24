using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Consumers;
using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed class D1ConsumerMigrationTests
{
    [Fact]
    public void Gizmo_claims_before_camera_and_picking()
    {
        var gizmo = new Probe(true);
        var camera = new Probe(true);
        var picking = new Probe(true);
        var router = NewRouter(gizmo, camera, picking);

        var result = router.Dispatch(Event(EditorPointerButtons.Left));

        Assert.Equal(ViewportInputDispatchKind.Captured, result.Kind);
        Assert.Equal(GestureOwner.Gizmo, router.State.Owner);
        Assert.Single(gizmo.BeginCalls);
        Assert.Empty(camera.BeginCalls);
        Assert.Empty(picking.BeginCalls);
    }

    [Fact]
    public void Camera_claims_middle_gesture_and_picking_cannot_repeat_consume()
    {
        var gizmo = new Probe(false);
        var camera = new Probe(true);
        var picking = new Probe(true);
        var router = NewRouter(gizmo, camera, picking);

        router.Dispatch(Event(EditorPointerButtons.Middle));
        var result = router.Dispatch(Event(EditorPointerButtons.Left, EditorPointerEventKind.Released));

        Assert.Equal(ViewportInputDispatchKind.Released, result.Kind);
        Assert.Equal(1, camera.CommitCalls);
        Assert.Empty(picking.BeginCalls);
    }

    [Fact]
    public void Picking_claims_left_click_when_gizmo_does_not_hit()
    {
        var picking = new Probe(true);
        var router = NewRouter(new Probe(false), new Probe(false), picking);

        router.Dispatch(Event(EditorPointerButtons.Left));
        Assert.Equal(GestureOwner.Picking, router.State.Owner);
        var result = router.Dispatch(Event(EditorPointerButtons.Left, EditorPointerEventKind.Released));

        Assert.Equal(ViewportInputDispatchKind.Released, result.Kind);
        Assert.Equal(1, picking.CommitCalls);
    }

    [Fact]
    public void Cancel_releases_owner_and_next_gesture_can_begin()
    {
        var camera = new Probe(true);
        var router = NewRouter(new Probe(false), camera, new Probe(false));

        router.Dispatch(Event(EditorPointerButtons.Middle));
        router.Dispatch(Event(EditorPointerButtons.Middle, EditorPointerEventKind.CaptureLost));
        router.Dispatch(Event(EditorPointerButtons.Middle, EditorPointerEventKind.Pressed, 2));

        Assert.Equal(1, camera.CancelCalls);
        Assert.Equal(2, camera.BeginCalls.Count);
        Assert.Equal(GestureOwner.Camera, router.State.Owner);
    }

    static ViewportInputRouter NewRouter(Probe gizmo, Probe camera, Probe picking) =>
        new(ViewportD1ConsumerSet.Create(
            new GizmoViewportInputConsumer(gizmo),
            new CameraViewportInputConsumer(camera),
            new PickingViewportInputConsumer(picking)), new CaptureSpy());

    static EditorPointerEvent Event(EditorPointerButtons buttons,
        EditorPointerEventKind kind = EditorPointerEventKind.Pressed, long id = 1) => new(
        kind, new(10, 20), buttons, EditorPointerModifiers.None, 0, id, new("test"), 1);

    sealed class Probe(bool claims) : IViewportD1ConsumerHandler
    {
        public List<ViewportGestureContext> BeginCalls { get; } = [];
        public int CommitCalls { get; private set; }
        public int CancelCalls { get; private set; }
        public bool CanClaim(EditorPointerEvent pointer) => claims && pointer.Kind == EditorPointerEventKind.Pressed;
        public void Begin(ViewportGestureContext context) => BeginCalls.Add(context);
        public void Update(ViewportGestureContext context) { }
        public void Commit(ViewportGestureContext context) => CommitCalls++;
        public void Cancel(ViewportCancellationContext context) => CancelCalls++;
    }

    sealed class CaptureSpy : IViewportPointerCaptureCoordinator
    {
        public void Capture(long pointerId, GestureOwner owner) { }
        public void Release(long pointerId, GestureOwner owner) { }
    }
}
