using XuanYu.Engine.Render.Camera;

namespace FluidWarfare.Tests.Render.Camera;

public sealed class SceneCameraStateTests
{
    [Fact]
    public void Default_ShouldMatchPosition()
    {
        var state = SceneCameraDefaults.CreateDefault();
        var (px, py, pz) = state.ComputePosition();

        // 默认位置应为 (0, 32, 24)
        Assert.Equal(0, px, 1);
        Assert.Equal(32, py, 1);
        Assert.Equal(24, pz, 1);
    }

    [Fact]
    public void Default_ShouldUse55Fov()
    {
        var state = SceneCameraDefaults.CreateDefault();
        Assert.Equal(55, state.FieldOfViewDegrees);
    }

    [Fact]
    public void Default_ShouldUseCorrectDistance()
    {
        var state = SceneCameraDefaults.CreateDefault();
        Assert.Equal(40f, state.Distance, 0.01f);
    }

    [Fact]
    public void Default_ShouldUseDefaultTarget()
    {
        var state = SceneCameraDefaults.CreateDefault();
        Assert.Equal(0, state.TargetX);
        Assert.Equal(0, state.TargetZ);
    }

    [Fact]
    public void ComputePosition_TargetChanged_ShouldMovePosition()
    {
        var state = new SceneCameraState
        {
            TargetX = 10,
            TargetZ = 20,
            Distance = 40f,
            FieldOfViewDegrees = 55,
            NearPlane = 0.1f,
            FarPlane = 1000f
        };

        var (px, _, pz) = state.ComputePosition();
        // 平移 Target 后 Position 应跟随移动
        Assert.Equal(10, px, 1);
        // pz = TargetZ - ViewDirection.Z * Distance = 20 - (-0.6 * 40) = 20 + 24 = 44
        Assert.Equal(44, pz, 1);
    }

    [Fact]
    public void ComputePosition_ZeroViewport_ShouldNotThrow()
    {
        var state = SceneCameraDefaults.CreateDefault();
        var (_, py, _) = state.ComputePosition();
        Assert.True(py > 0);
    }

    [Fact]
    public void ToSummary_ShouldContainKeyInfo()
    {
        var state = SceneCameraDefaults.CreateDefault();
        var summary = state.ToSummary();
        Assert.Contains("Target", summary);
        Assert.Contains("Distance", summary);
        Assert.Contains("Position", summary);
        Assert.Contains("FOV", summary);
    }
}
