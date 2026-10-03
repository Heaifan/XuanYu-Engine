using XuanYu.Core.Scene;
using XuanYu.Editor.Input;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Camera;

public sealed class CameraOrbitCaptureRegressionTests
{
    [Fact]
    public void Orbit_keeps_begin_pivot_when_pointer_crosses_hit_points()
    {
        var vm = new UiVm(null, () => true);
        vm.UpdateViewportFrame(800, 600);
        Assert.True(vm.BeginCameraNavigation(1, 300, 300, false, 800, 600));
        var begin = vm.OrbitProbeEvents.Single(e => e.Phase == OrbitProbePhase.Begin);
        var resolvedPivotAtBegin = begin.Pivot;
        Assert.True(vm.PreviewCameraNavigation(1, 220, 180));
        Assert.True(vm.PreviewCameraNavigation(1, 620, 420));

        Assert.True(vm.EndCameraNavigation(1));
        Assert.InRange(vm.ObservationCenter.DistanceTo(resolvedPivotAtBegin), 0, 1e-5);
    }

    [Fact]
    public void Orbit_hover_updates_do_not_change_camera_target()
    {
        var vm = new UiVm(null, () => true);
        vm.UpdateViewportFrame(800, 600);
        var target = vm.ObservationCenter;

        Assert.True(vm.BeginCameraNavigation(1, 300, 300, false, 800, 600));
        vm.SetNavigationGizmoHover(2);
        vm.SetNavigationGizmoCenterHover(true);

        Assert.Equal(target, vm.ObservationCenter);
        Assert.True(vm.EndCameraNavigation(1));
        Assert.False(vm.IsCameraNavigationActive);
    }

    [Fact]
    public void Capture_loss_cancels_orbit_without_terminal_pointer_preview()
    {
        var vm = new UiVm(null, () => true);
        vm.UpdateViewportFrame(800, 600);
        var published = new List<SceneRenderSnapshot>();
        vm.RenderSnapshotChanged += published.Add;

        vm.ViewportInput.Sink.Handle(Event(EditorPointerEventKind.Pressed,
            EditorPointerButtons.Middle, 300, 300));
        vm.ViewportInput.Sink.Handle(Event(EditorPointerEventKind.Move,
            EditorPointerButtons.Middle, 340, 260));
        vm.ViewportInput.Sink.Handle(Event(EditorPointerEventKind.CaptureLost,
            EditorPointerButtons.None, 0, 0));

        Assert.Equal(ViewportGestureState.Idle, vm.ViewportInput.Router.State);
        Assert.False(vm.IsCameraNavigationActive);
        Assert.Equal(2, published.Count);
    }

    static EditorPointerEvent Event(EditorPointerEventKind kind,
        EditorPointerButtons buttons, double x, double y) => new(
        kind, new(x, y), buttons, EditorPointerModifiers.None, 0, 1,
        new("native-hwnd"), 1);

}
