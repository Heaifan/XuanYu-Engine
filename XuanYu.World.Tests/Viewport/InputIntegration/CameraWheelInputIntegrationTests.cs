using XuanYu.Core.Space;
using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Consumers;
using XuanYu.Editor.Input.Lifecycle;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed class CameraWheelInputIntegrationTests
{
    [Fact]
    public void Camera_consumer_claims_wheel_without_capture()
    {
        var consumer = new CameraViewportInputConsumer(new Probe());

        var result = consumer.Handle(Wheel(1), ViewportGestureState.Idle);

        Assert.Equal(ViewportInputDispatchKind.Handled, result.Kind);
    }

    [Fact]
    public void Perspective_wheel_changes_position_and_preserves_center()
    {
        var vm = new UiVm(null, () => true);
        var before = vm.RenderSnapshot.CameraState;
        var center = vm.ObservationCenter;

        vm.ViewportInput.Sink.Handle(Wheel(1));

        var after = vm.RenderSnapshot.CameraState;
        Assert.NotEqual(before.Position, after.Position);
        Assert.Equal(center, vm.ObservationCenter);
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

    static EditorPointerEvent Wheel(double delta) => new(EditorPointerEventKind.Wheel,
        new(10, 20), EditorPointerButtons.None, EditorPointerModifiers.None, delta, 1,
        new("test"), 1);

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
