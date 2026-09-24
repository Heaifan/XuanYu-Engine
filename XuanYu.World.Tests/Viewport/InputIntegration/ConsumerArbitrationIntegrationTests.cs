using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Consumers;
using XuanYu.Editor.Input.Lifecycle;
using XuanYu.Editor.Input.Map;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed class ConsumerArbitrationIntegrationTests
{
    [Fact] public void Camera_beats_picking_when_only_camera_is_eligible() =>
        AssertOwner(GestureOwner.Camera, new CameraViewportInputConsumer(new CameraHandler(true)),
            new Probe(GestureOwner.Picking, 10, true));

    [Fact] public void Gizmo_beats_picking_when_gizmo_hit_is_eligible() =>
        AssertOwner(GestureOwner.Gizmo, new GizmoViewportInputConsumer(new GizmoHandler(true)), new CameraViewportInputConsumer(new CameraHandler(false)),
            new Probe(GestureOwner.Picking, 10, true));

    [Fact] public void Gizmo_beats_camera_when_both_hits_are_reported() =>
        AssertOwner(GestureOwner.Gizmo, new GizmoViewportInputConsumer(new GizmoHandler(true)), new CameraViewportInputConsumer(new CameraHandler(true)),
            new Probe(GestureOwner.Picking, 10, false));

    [Fact] public void Camera_beats_map_edit_when_map_mode_is_ineligible() =>
        AssertOwner(GestureOwner.Camera, new CameraViewportInputConsumer(new CameraHandler(true)),
            new MapGeometryInputConsumer(new Backend(false)));

    [Fact] public void Marker_beats_picking_when_marker_hit_is_eligible() =>
        AssertOwner(GestureOwner.Marker,
            new MarkerInputConsumer(new Backend(true)), new Probe(GestureOwner.Picking, 10, true));

    [Fact] public void Region_and_road_are_not_competed_by_generic_map_edit() =>
        AssertOwner(GestureOwner.Region, new MapGeometryInputConsumer(new Backend(false)),
            new RegionInputConsumer(new Backend(true)), new RoadInputConsumer(new Backend(false)));

    [Fact] public void Snap_helper_does_not_appear_as_an_owner() =>
        Assert.DoesNotContain(GestureOwner.SnapInteractionHelper,
            new IViewportInputConsumer[] { new MapGeometryInputConsumer(new Backend(true)) }
                .Select(x => x.Owner));

    static void AssertOwner(GestureOwner expected, params IViewportInputConsumer[] consumers)
    {
        var router = new ViewportInputRouter(consumers, new CaptureSpy());
        Assert.Equal(ViewportInputDispatchKind.Captured, router.Dispatch(Event()).Kind);
        Assert.Equal(expected, router.State.Owner);
        Assert.Single(consumers, x => x.Owner == expected);
    }

    static EditorPointerEvent Event() => new(EditorPointerEventKind.Pressed, new(1, 2),
        EditorPointerButtons.Left, EditorPointerModifiers.None, 0, 1, new("test"), 1);

    sealed class Probe(GestureOwner owner, int priority, bool eligible) : IViewportInputConsumer
    {
        public GestureOwner Owner => owner; public int BeginPriority => priority;
        public bool CanBegin(EditorPointerEvent p, ViewportGestureState s) => eligible;
        public ViewportInputDispatchResult Handle(EditorPointerEvent p, ViewportGestureState s) =>
            ViewportInputDispatchResult.Captured;
        public void Begin(ViewportGestureContext c) { } public void Update(ViewportGestureContext c) { }
        public void Commit(ViewportGestureContext c) { } public void Cancel(ViewportCancellationContext c) { }
    }

    sealed class CaptureSpy : IViewportPointerCaptureCoordinator
    { public void Capture(long id, GestureOwner owner) { } public void Release(long id, GestureOwner owner) { } }
    sealed class Backend(bool eligible) : IMapEditingInputBackend
    {
        public bool CanBegin(EditorPointerEvent p, ViewportGestureState s) => eligible;
        public void Begin(ViewportGestureContext c) { } public void Update(ViewportGestureContext c) { }
        public void Commit(ViewportGestureContext c) { } public void Cancel(ViewportCancellationContext c) { }
    }
    class CameraHandler(bool eligible) : IViewportD1ConsumerHandler
    {
        public bool CanClaim(EditorPointerEvent p) => eligible;
        public void Begin(ViewportGestureContext c) { } public void Update(ViewportGestureContext c) { }
        public void Commit(ViewportGestureContext c) { } public void Cancel(ViewportCancellationContext c) { }
    }
    sealed class GizmoHandler(bool eligible) : CameraHandler(eligible) { }
}
