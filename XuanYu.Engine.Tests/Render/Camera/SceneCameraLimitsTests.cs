using XuanYu.Engine.Render.Camera;

namespace XuanYu.Engine.Tests.Render.Camera;

public sealed class SceneCameraLimitsTests
{
    [Fact]
    public void MinDistance_ShouldBe8()
    {
        Assert.Equal(8, SceneCameraLimits.MinDistance);
    }

    [Fact]
    public void MaxDistance_ShouldBe120()
    {
        Assert.Equal(120, SceneCameraLimits.MaxDistance);
    }

    [Fact]
    public void MinDistance_ShouldBeLessThanMax()
    {
        Assert.True(SceneCameraLimits.MinDistance < SceneCameraLimits.MaxDistance);
    }
}
