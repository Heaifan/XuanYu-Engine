using XuanYu.Engine.Core.Time;

namespace XuanYu.Engine.Tests.Core.Time;

public sealed class TimeStepTests
{
    [Fact]
    public void FromSeconds_WithPositiveValue_ShouldCreatePositiveStep()
    {
        var step = TimeStep.FromSeconds(1.0);

        Assert.True(step.IsPositive);
        Assert.Equal(1.0, step.Seconds);
    }

    [Fact]
    public void FromMilliseconds_WithPositiveValue_ShouldConvertToSeconds()
    {
        Assert.Equal(0.5, TimeStep.FromMilliseconds(500.0).Seconds);
    }

    [Fact]
    public void FromSeconds_WithZero_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TimeStep.FromSeconds(0.0));
    }

    [Fact]
    public void FromSeconds_WithNegativeValue_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TimeStep.FromSeconds(-1.0));
    }

    [Fact]
    public void FromSeconds_WithNaN_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TimeStep.FromSeconds(double.NaN));
    }

    [Fact]
    public void FromSeconds_WithInfinity_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TimeStep.FromSeconds(double.PositiveInfinity));
    }

    [Fact]
    public void FromMilliseconds_WithZero_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TimeStep.FromMilliseconds(0.0));
    }

    [Fact]
    public void Default_ShouldNotBePositive()
    {
        var step = default(TimeStep);

        Assert.Equal(0.0, step.Seconds);
        Assert.False(step.IsPositive);
    }

    [Fact]
    public void Milliseconds_ShouldConvertFromSeconds()
    {
        Assert.Equal(250.0, TimeStep.FromSeconds(0.25).Milliseconds);
    }

    [Fact]
    public void SameValue_ShouldBeEqual()
    {
        Assert.Equal(TimeStep.FromSeconds(0.05), TimeStep.FromMilliseconds(50.0));
    }

    [Fact]
    public void ToString_ShouldBeStable()
    {
        Assert.Equal("TimeStep(0.05s)", TimeStep.FromSeconds(0.05).ToString());
    }

    [Fact]
    public void ToString_WithWholeSeconds_ShouldBeStable()
    {
        Assert.Equal("TimeStep(1s)", TimeStep.FromSeconds(1.0).ToString());
    }
}
