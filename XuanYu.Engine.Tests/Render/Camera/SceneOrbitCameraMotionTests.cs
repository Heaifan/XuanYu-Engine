using XuanYu.Engine.Core.Math;
using FluidWarfare.Render.Camera;

namespace FluidWarfare.Tests.Render.Camera;

public sealed class SceneOrbitCameraMotionTests
{
    [Fact]
    public void Default_IsNotNull()
    {
        var cam = SceneOrbitCameraMotion.CreateDefault();
        Assert.NotNull(cam);
    }

    [Fact]
    public void Orbit_ChangesYaw()
    {
        var cam = SceneOrbitCameraMotion.CreateDefault();
        var result = SceneOrbitCameraMotion.Orbit(cam, 45, 0);
        Assert.Equal(cam.Yaw + 45, result.Yaw, 5);
        Assert.Equal(cam.Pitch, result.Pitch);
    }

    [Fact]
    public void Orbit_ChangesPitch()
    {
        var cam = SceneOrbitCameraMotion.CreateDefault();
        var result = SceneOrbitCameraMotion.Orbit(cam, 0, 10);
        Assert.Equal(cam.Pitch + 10, result.Pitch, 5);
    }

    [Fact]
    public void Orbit_ClampsPitch()
    {
        var cam = SceneOrbitCameraMotion.CreateDefault();
        var result = SceneOrbitCameraMotion.Orbit(cam, 0, 200);
        Assert.True(result.Pitch < 90);
    }

    [Fact]
    public void Pan_MovesPivot()
    {
        var cam = SceneOrbitCameraMotion.CreateDefault();
        var result = SceneOrbitCameraMotion.Pan(cam, 100, 0, 720);
        Assert.NotEqual(cam.PivotX, result.PivotX);
    }

    [Fact]
    public void Dolly_ChangesDistance()
    {
        var cam = SceneOrbitCameraMotion.CreateDefault();
        var result = SceneOrbitCameraMotion.Dolly(cam, 50);
        Assert.NotEqual(cam.Distance, result.Distance);
    }

    [Fact]
    public void Zoom_ChangesDistance()
    {
        var cam = SceneOrbitCameraMotion.CreateDefault();
        var result = SceneOrbitCameraMotion.Zoom(cam, 1);
        Assert.True(result.Distance < cam.Distance);
    }

    [Fact]
    public void ComputePosition_ReturnsFiniteValues()
    {
        var cam = SceneOrbitCameraMotion.CreateDefault();
        var (x, y, z) = cam.ComputePosition();
        Assert.False(float.IsNaN(x));
        Assert.False(float.IsNaN(y));
        Assert.False(float.IsNaN(z));
        Assert.False(float.IsInfinity(x));
    }

    [Fact]
    public void FrameSelected_SetsPivotToCenter()
    {
        var cam = SceneOrbitCameraMotion.CreateDefault();
        var result = SceneOrbitCameraMotion.FrameSelected(cam, 10, 5, -3, 2);
        Assert.Equal(10, result.PivotX);
        Assert.Equal(5, result.PivotY);
        Assert.Equal(-3, result.PivotZ);
    }

    [Fact]
    public void FrameAll_ResetsToDefaults()
    {
        var cam = new SceneOrbitCameraState
        {
            PivotX = 100, PivotY = 50, PivotZ = 200,
            Yaw = 10, Pitch = 20, Distance = 5,
            FieldOfViewDegrees = 55, NearPlane = 0.1f, FarPlane = 1000
        };
        var result = SceneOrbitCameraMotion.FrameAll();
        Assert.NotEqual(cam.PivotX, result.PivotX);
    }

    [Fact]
    public void OrbitDirection_MatchesExpected()
    {
        // Pivot at origin, Yaw=135, Pitch=45, Distance=40 (Z-Up)
        var cam = new SceneOrbitCameraState
        {
            PivotX = 0, PivotY = 0, PivotZ = 0,
            Yaw = 135, Pitch = 45, Distance = 40,
            FieldOfViewDegrees = 55, NearPlane = 0.1f, FarPlane = 1000
        };
        var (px, py, pz) = cam.ComputePosition();

        // Expected (Z-Up): Yaw=135° → sin/cos produce +X/+Y direction.
        // offsetX = sin(135)*cos(45) = 0.707*0.707 = 0.5
        // offsetY = -cos(135)*cos(45) = -(-0.707)*0.707 = 0.5
        // offsetZ = sin(45) = 0.707
        // Position = (0.5*40, 0.5*40, 0.707*40) = (20, 20, 28.28)
        Assert.True(px > 10);  // +X
        Assert.True(py > 10);  // +Y (ground plane)
        Assert.True(pz > 20);  // +Z (up)
    }
}
