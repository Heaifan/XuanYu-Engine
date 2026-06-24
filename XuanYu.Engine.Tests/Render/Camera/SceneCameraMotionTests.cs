using XuanYu.Engine.Render.Camera;

namespace XuanYu.Engine.Tests.Render.Camera;

public sealed class SceneCameraMotionTests
{
    [Fact]
    public void Pan_ShouldChangeTarget()
    {
        var state = SceneCameraDefaults.CreateDefault();
        var panned = SceneCameraMotion.Pan(state, 10, 0, 600);

        Assert.NotEqual(state.TargetX, panned.TargetX);
        Assert.Equal(state.TargetZ, panned.TargetZ);
    }

    [Fact]
    public void Pan_ShouldNotChangeDistance()
    {
        var state = SceneCameraDefaults.CreateDefault();
        var panned = SceneCameraMotion.Pan(state, 10, 0, 600);

        Assert.Equal(state.Distance, panned.Distance);
    }

    [Fact]
    public void Pan_ShouldNotChangeFov()
    {
        var state = SceneCameraDefaults.CreateDefault();
        var panned = SceneCameraMotion.Pan(state, 10, 0, 600);

        Assert.Equal(state.FieldOfViewDegrees, panned.FieldOfViewDegrees);
    }

    [Fact]
    public void Zoom_In_ShouldDecreaseDistance()
    {
        var state = SceneCameraDefaults.CreateDefault();
        var zoomed = SceneCameraMotion.Zoom(state, 1);

        Assert.True(zoomed.Distance < state.Distance);
    }

    [Fact]
    public void Zoom_Out_ShouldIncreaseDistance()
    {
        var state = SceneCameraDefaults.CreateDefault();
        var zoomed = SceneCameraMotion.Zoom(state, -1);

        Assert.True(zoomed.Distance > state.Distance);
    }

    [Fact]
    public void Zoom_ShouldNotChangeTarget()
    {
        var state = SceneCameraDefaults.CreateDefault();
        var zoomed = SceneCameraMotion.Zoom(state, 1);

        Assert.Equal(state.TargetX, zoomed.TargetX);
        Assert.Equal(state.TargetZ, zoomed.TargetZ);
    }

    [Fact]
    public void Zoom_ClampMinDistance()
    {
        var state = new SceneCameraState
        {
            TargetX = 0, TargetZ = 0,
            Distance = 100,
            FieldOfViewDegrees = 55,
            NearPlane = 0.1f, FarPlane = 1000f
        };
        var zoomed = SceneCameraMotion.Zoom(state, 100); // heavy zoom in

        Assert.True(zoomed.Distance >= SceneCameraLimits.MinDistance);
    }

    [Fact]
    public void Zoom_ClampMaxDistance()
    {
        var state = new SceneCameraState
        {
            TargetX = 0, TargetZ = 0,
            Distance = 50,
            FieldOfViewDegrees = 55,
            NearPlane = 0.1f, FarPlane = 1000f
        };
        var zoomed = SceneCameraMotion.Zoom(state, -100); // heavy zoom out

        Assert.True(zoomed.Distance <= SceneCameraLimits.MaxDistance);
    }

    [Fact]
    public void Reset_ShouldRestoreDefaults()
    {
        var panned = SceneCameraMotion.Pan(SceneCameraDefaults.CreateDefault(), 100, 50, 600);
        Assert.NotEqual(0, panned.TargetX);

        var reset = SceneCameraMotion.Reset();
        Assert.Equal(0, reset.TargetX);
        Assert.Equal(0, reset.TargetZ);
        Assert.Equal(40f, reset.Distance, 0.01f);
    }

    [Fact]
    public void Pan_LargerDistance_ShouldMoveMoreWorldUnits()
    {
        var near = new SceneCameraState
        {
            TargetX = 0, TargetZ = 0, Distance = 20,
            FieldOfViewDegrees = 55, NearPlane = 0.1f, FarPlane = 1000f
        };
        var far = new SceneCameraState
        {
            TargetX = 0, TargetZ = 0, Distance = 80,
            FieldOfViewDegrees = 55, NearPlane = 0.1f, FarPlane = 1000f
        };

        var movedNear = SceneCameraMotion.Pan(near, 100, 0, 600);
        var movedFar = SceneCameraMotion.Pan(far, 100, 0, 600);

        // 远距离下同样的像素移动应产生更大的世界位移
        Assert.True(Math.Abs(movedFar.TargetX) > Math.Abs(movedNear.TargetX));
    }

    [Fact]
    public void Pan_ZeroViewportHeight_ShouldReturnOriginal()
    {
        var state = SceneCameraDefaults.CreateDefault();
        var result = SceneCameraMotion.Pan(state, 10, 0, 0);
        Assert.Equal(state.TargetX, result.TargetX);
        Assert.Equal(state.TargetZ, result.TargetZ);
    }

    [Fact]
    public void Pan_ClampTargetX()
    {
        var state = new SceneCameraState
        {
            TargetX = 0, TargetZ = 0, Distance = 40f,
            FieldOfViewDegrees = 55, NearPlane = 0.1f, FarPlane = 1000f
        };
        // 尝试大幅平移
        var result = SceneCameraMotion.Pan(state, 100000, 0, 600);
        Assert.True(result.TargetX <= SceneCameraLimits.MaxTargetX);
        Assert.True(result.TargetX >= SceneCameraLimits.MinTargetX);
    }
}
