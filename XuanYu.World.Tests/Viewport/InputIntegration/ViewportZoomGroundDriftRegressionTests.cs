using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed class ViewportZoomGroundDriftRegressionTests
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);

    [Fact]
    public void Editor_wheel_result_is_independent_of_cursor_position()
    {
        var left = NewVm(); var right = NewVm();
        left.ViewportInput.Sink.Handle(Wheel(20, 300, 1));
        right.ViewportInput.Sink.Handle(Wheel(780, 300, 1));
        Assert.Equal(left.RenderSnapshot.CameraState, right.RenderSnapshot.CameraState);
        Assert.Equal(left.ObservationCenter, right.ObservationCenter);
    }

    [Fact]
    public void Zoom_round_trip_restores_camera_position_orientation_and_pivot()
    {
        var vm = NewVm();
        var before = vm.RenderSnapshot.CameraState;
        var center = vm.ObservationCenter;
        for (var i = 0; i < 20; i++)
        {
            Assert.True(vm.DollyCamera(1));
            Assert.True(vm.DollyCamera(-1));
        }

        var after = vm.RenderSnapshot.CameraState;
        Assert.InRange(Distance(before.Position, after.Position), 0, 1e-9);
        Assert.InRange(Distance(before.Forward, after.Forward), 0, 1e-9);
        Assert.InRange(Distance(before.Up, after.Up), 0, 1e-9);
        Assert.Equal(center, vm.ObservationCenter);
    }

    [Theory]
    [InlineData(1d)]
    [InlineData(5d)]
    [InlineData(20d)]
    [InlineData(100d)]
    [InlineData(500d)]
    public void Reference_plane_ray_intersection_remains_valid_at_camera_distance(double distance)
    {
        var direction = (Vector3d.Zero - DefaultEditorCamera.Position).Normalize();
        var camera = new CameraState(direction * -distance, direction, Vector3d.UnitZ,
            60, 0.1, Math.Max(1000, distance * 4), 1);
        var projection = ViewProjectionState.Create(camera, Viewport);
        var ray = WorldRayFactory.FromViewportPoint(projection, 400, 300);
        var hitDistance = -ray.Origin.Z / ray.Direction.Z;
        Assert.True(double.IsFinite(hitDistance) && hitDistance >= 0);
        Assert.True(double.IsFinite((ray.Origin + ray.Direction * hitDistance).Z));
    }

    static UiVm NewVm()
    {
        var vm = new UiVm(null, () => true);
        vm.UpdateViewportFrame(800, 600);
        return vm;
    }

    static double ScreenDistance(XuanYu.Core.Gizmo.ScreenPoint a,
        XuanYu.Core.Gizmo.ScreenPoint b) =>
        Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));

    static XuanYu.Editor.Input.EditorPointerEvent Wheel(double x, double y, double delta) => new(
        XuanYu.Editor.Input.EditorPointerEventKind.Wheel, new(x, y),
        XuanYu.Editor.Input.EditorPointerButtons.None,
        XuanYu.Editor.Input.EditorPointerModifiers.None, delta, 1,
        new("test"), 1);

    static double Distance(Vector3d a, Vector3d b) => (a - b).Length;
}
