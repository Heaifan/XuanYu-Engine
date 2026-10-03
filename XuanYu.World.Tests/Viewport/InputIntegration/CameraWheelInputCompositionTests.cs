using XuanYu.Core.Space;
using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Consumers;
using XuanYu.Editor.Input.Lifecycle;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed class CameraWheelInputCompositionTests
{
    [Fact]
    public void Camera_consumer_claims_wheel_without_capture()
    {
        var consumer = new CameraViewportInputConsumer(new Probe());

        var result = consumer.Handle(Wheel(1), ViewportGestureState.Idle);

        Assert.Equal(ViewportInputDispatchKind.Handled, result.Kind);
    }

    [Fact]
    public void Perspective_wheel_preserves_camera_pivot_and_orientation()
    {
        var vm = new UiVm(null, () => true);
        vm.UpdateViewportFrame(800, 600);
        const double cursorX = 620;
        const double cursorY = 180;
        var viewport = vm.CurrentViewport;
        var before = vm.RenderSnapshot.CameraState;
        var center = vm.ObservationCenter;

        vm.ViewportInput.Sink.Handle(Wheel(cursorX, cursorY, 1));

        var after = vm.RenderSnapshot.CameraState;
        Assert.Equal(center, vm.ObservationCenter);
        AssertVector(before.Forward, after.Forward);
        AssertVector(before.Up, after.Up);
        Assert.Equal(before.VerticalFovDegrees, after.VerticalFovDegrees);
        Assert.False(vm.ViewportInput.Router.State.IsActive);
    }

    [Fact]
    public void Orthographic_wheel_changes_scale_without_changing_mode()
    {
        var vm = new UiVm(null, () => true);
        vm.ApplyNavigationGizmoEndpoint("+Z");
        var before = vm.RenderSnapshot.CameraState;

        vm.ViewportInput.Sink.Handle(Wheel(1));

        var after = vm.RenderSnapshot.CameraState;
        Assert.Equal(ProjectionMode.Orthographic, after.Mode);
        Assert.NotEqual(before.OrthographicScale, after.OrthographicScale);
    }

    [Fact]
    public void Opposite_wheel_deltas_have_opposite_zoom_direction()
    {
        var positive = new UiVm(null, () => true);
        var negative = new UiVm(null, () => true);
        var positiveBefore = positive.RenderSnapshot.CameraState.Position.DistanceTo(positive.ObservationCenter);
        var negativeBefore = negative.RenderSnapshot.CameraState.Position.DistanceTo(negative.ObservationCenter);

        positive.ViewportInput.Sink.Handle(Wheel(1));
        negative.ViewportInput.Sink.Handle(Wheel(-1));

        Assert.True(positive.RenderSnapshot.CameraState.Position.DistanceTo(positive.ObservationCenter) < positiveBefore);
        Assert.True(negative.RenderSnapshot.CameraState.Position.DistanceTo(negative.ObservationCenter) > negativeBefore);
    }

    static EditorPointerEvent Wheel(double delta) => Wheel(10, 20, delta);

    static EditorPointerEvent Wheel(double x, double y, double delta) => new(EditorPointerEventKind.Wheel,
        new(x, y), EditorPointerButtons.None, EditorPointerModifiers.None, delta, 1,
        new("test"), 1);

    static void AssertVector(XuanYu.Core.Math.Vector3d expected, XuanYu.Core.Math.Vector3d actual)
    {
        Assert.Equal(expected.X, actual.X, 12);
        Assert.Equal(expected.Y, actual.Y, 12);
        Assert.Equal(expected.Z, actual.Z, 12);
    }

    sealed class Probe : IViewportD1ConsumerHandler
    {
        public bool CanClaim(EditorPointerEvent pointer) => pointer.Kind == EditorPointerEventKind.Wheel;
        public bool HandleWheel(EditorPointerEvent pointer) => CanClaim(pointer);
        public void Begin(ViewportGestureContext context) { }
        public void Update(ViewportGestureContext context) { }
        public void Commit(ViewportGestureContext context) { }
        public void Cancel(ViewportCancellationContext context) { }
    }
}
