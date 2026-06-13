using FluidWarfare.Core.Math;
using FluidWarfare.Render.Camera;
using FluidWarfare.Render.Camera.Navigation;

namespace FluidWarfare.Tests.Render.Camera.Navigation;

public sealed class SceneNavigationCameraMotionTests
{
    [Fact]
    public void SnapToPositiveX_PlacesCameraOnPositiveX()
    {
        var cam = SceneOrbitCameraMotion.CreateDefault();
        var result = SceneNavigationCameraMotion.SnapToDirection(
            cam, new Vector3d(1, 0, 0));

        // Looking toward origin from +X: Yaw should ~90°, Pitch ~0°
        Assert.True(result.Yaw > 85 && result.Yaw < 95);
        Assert.True(result.Pitch < 10);
    }

    [Fact]
    public void SnapToPositiveY_PlacesCameraOnPositiveY()
    {
        var cam = SceneOrbitCameraMotion.CreateDefault();
        var result = SceneNavigationCameraMotion.SnapToDirection(
            cam, new Vector3d(0, 1, 0));

        // Looking toward origin from +Y: Yaw should ~180°
        Assert.True(result.Yaw > 175 && result.Yaw < 185);
        Assert.True(result.Pitch < 10);
    }

    [Fact]
    public void SnapToPositiveZ_PlacesCameraAboveScene()
    {
        var cam = SceneOrbitCameraMotion.CreateDefault();
        var result = SceneNavigationCameraMotion.SnapToDirection(
            cam, new Vector3d(0, 0, 1));

        // Looking down from above: Pitch should be ~90° (clamped to 89°)
        Assert.True(result.Pitch >= 85);
    }

    [Fact]
    public void SnapToNegativeZ_PlacesCameraBelowScene()
    {
        var cam = SceneOrbitCameraMotion.CreateDefault();
        var result = SceneNavigationCameraMotion.SnapToDirection(
            cam, new Vector3d(0, 0, -1));

        // Looking up from below: Pitch should be ~-90° (clamped to 5° = MinPitch)
        Assert.True(result.Pitch >= 5);
    }

    [Fact]
    public void SnapToView_CorrectlySetsProjectionMode()
    {
        var cam = SceneOrbitCameraMotion.CreateDefault();
        var result = SceneNavigationCameraMotion.SnapToView(
            cam, SceneNavigationView.PositiveX);

        // Axis snap should auto-enter orthographic
        Assert.Equal(SceneProjectionMode.Orthographic, result.ProjectionMode);
    }

    [Fact]
    public void SnapToView_Free_DoesNotChange()
    {
        var cam = SceneOrbitCameraMotion.CreateDefault();
        var result = SceneNavigationCameraMotion.SnapToView(
            cam, SceneNavigationView.Free);

        Assert.Equal(cam, result);
    }

    [Fact]
    public void ToggleProjection_PerspectiveToOrthographic()
    {
        var cam = SceneOrbitCameraMotion.CreateDefault();
        Assert.Equal(SceneProjectionMode.Perspective, cam.ProjectionMode);

        var result = SceneNavigationCameraMotion.ToggleProjection(cam);
        Assert.Equal(SceneProjectionMode.Orthographic, result.ProjectionMode);
    }

    [Fact]
    public void ToggleProjection_OrthographicToPerspective()
    {
        var cam = SceneOrbitCameraMotion.CreateDefault() with
        {
            ProjectionMode = SceneProjectionMode.Orthographic
        };

        var result = SceneNavigationCameraMotion.ToggleProjection(cam);
        Assert.Equal(SceneProjectionMode.Perspective, result.ProjectionMode);
    }

    [Fact]
    public void ToggleProjection_PreservesOtherProperties()
    {
        var cam = SceneOrbitCameraMotion.CreateDefault() with
        {
            PivotX = 10, PivotY = 20, PivotZ = 30,
            Yaw = 45, Pitch = 30, Distance = 50
        };

        var result = SceneNavigationCameraMotion.ToggleProjection(cam);
        Assert.Equal(cam.PivotX, result.PivotX);
        Assert.Equal(cam.PivotY, result.PivotY);
        Assert.Equal(cam.PivotZ, result.PivotZ);
        Assert.Equal(cam.Yaw, result.Yaw);
        Assert.Equal(cam.Pitch, result.Pitch);
        Assert.Equal(cam.Distance, result.Distance);
    }

    [Fact]
    public void DirectionToYawPitch_AxisAligned_ReturnsCorrectValues()
    {
        var (yaw, pitch) = SceneNavigationCameraMotion.DirectionToYawPitch(
            new Vector3d(0, 0, 1));

        // +Z direction: pitch = +90° (looking straight up), yaw = 0
        Assert.True(yaw >= 0 && yaw < 360);
        Assert.True(Math.Abs(pitch - 90) < 1);
    }

    [Fact]
    public void DirectionToYawPitch_XYPlane_ReturnsZeroPitch()
    {
        var (yaw, pitch) = SceneNavigationCameraMotion.DirectionToYawPitch(
            new Vector3d(1, 0, 0));

        // +X direction on XY plane: pitch = 0°
        Assert.True(Math.Abs(pitch) < 1);
        Assert.True(yaw > 85 && yaw < 95); // atan2(1, 0) = 90°
    }

    [Fact]
    public void DetectView_ReturnsCorrectView()
    {
        var cam = SceneOrbitCameraMotion.CreateDefault() with
        {
            Yaw = 0, Pitch = 5
        };

        var view = SceneNavigationCameraMotion.DetectView(cam);
        // Yaw=0 looking toward -Y (offsetY = -cos(0) = -1)
        Assert.Equal(SceneNavigationView.NegativeY, view);
    }
}
