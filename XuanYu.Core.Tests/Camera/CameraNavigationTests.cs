using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;

namespace XuanYu.Core.Tests.Camera;

public sealed class CameraNavigationTests
{
    [Fact]
    public void Orbit_keeps_center_distance_and_forward()
    {
        var camera = DefaultEditorCamera.Create(1);
        var center = Vector3d.Zero;
        var beforeDistance = camera.Position.DistanceTo(center);

        var result = CameraNavigation.Orbit(camera, center, 80, -30, 2);

        Near(beforeDistance, result.Camera.Position.DistanceTo(center));
        Near(center, result.ObservationCenter);
        Near((center - result.Camera.Position).Normalize(), result.Camera.Forward);
        Assert.True(result.Camera.Up.Dot(Vector3d.UnitZ) > 0.1);
    }

    [Fact]
    public void Orbit_clamps_pitch_without_nan_or_flip()
    {
        var camera = DefaultEditorCamera.Create(1);

        var result = CameraNavigation.Orbit(camera, Vector3d.Zero, 0, 100000, 2);

        AssertFinite(result.Camera.Position);
        AssertFinite(result.Camera.Forward);
        Assert.True(result.Camera.Up.Dot(Vector3d.UnitZ) > -0.2, "F3-F3：顶点奇异区回退稳定 Up，不翻转");
        Assert.True(System.Math.Abs(result.Camera.Forward.Cross(result.Camera.Up).Length - 1.0) < 1e-6);
    }

    [Fact]
    public void Pan_moves_position_and_center_together()
    {
        var camera = DefaultEditorCamera.Create(1);
        var center = Vector3d.Zero;

        var result = CameraNavigation.Pan(camera, center, 40, -25, 600, 2);
        var cameraMove = result.Camera.Position - camera.Position;

        Near(cameraMove, result.ObservationCenter - center);
        Near(camera.Position.DistanceTo(center), result.Camera.Position.DistanceTo(result.ObservationCenter));
        Near(camera.Forward, result.Camera.Forward);
        Near(camera.VerticalFovDegrees, result.Camera.VerticalFovDegrees);
    }

    [Fact]
    public void Dolly_changes_distance_without_changing_center_forward_or_fov()
    {
        var camera = DefaultEditorCamera.Create(1);
        var center = Vector3d.Zero;

        var zoomIn = CameraNavigation.Dolly(camera, center, 1, 2);
        var zoomOut = CameraNavigation.Dolly(camera, center, -1, 3);

        Assert.True(zoomIn.Camera.Position.DistanceTo(center) < camera.Position.DistanceTo(center));
        Assert.True(zoomOut.Camera.Position.DistanceTo(center) > camera.Position.DistanceTo(center));
        Near(center, zoomIn.ObservationCenter);
        Near(camera.Forward, zoomIn.Camera.Forward);
        Near(camera.VerticalFovDegrees, zoomIn.Camera.VerticalFovDegrees);
    }

    [Fact]
    public void Dolly_ignores_invalid_delta()
    {
        var camera = DefaultEditorCamera.Create(1);

        var result = CameraNavigation.Dolly(camera, Vector3d.Zero, double.NaN, 2);

        Near(camera.Position, result.Camera.Position);
        Near(Vector3d.Zero, result.ObservationCenter);
    }

    static void AssertFinite(Vector3d value)
    {
        Assert.True(double.IsFinite(value.X));
        Assert.True(double.IsFinite(value.Y));
        Assert.True(double.IsFinite(value.Z));
    }

    static void Near(double expected, double actual) => Assert.Equal(expected, actual, precision: 6);

    static void Near(Vector3d expected, Vector3d actual)
    {
        Near(expected.X, actual.X);
        Near(expected.Y, actual.Y);
        Near(expected.Z, actual.Z);
    }
}
