using XuanYu.Core.Time;

namespace XuanYu.Core.Tests.Foundation.Time;

public sealed class TimeStepTests
{
    [Fact]
    public void ConvertsUnits()
    {
        var step = TimeStep.FromMilliseconds(250);

        Assert.Equal(0.25, step.Seconds);
        Assert.Equal(2000, TimeStep.FromSeconds(2).Milliseconds);
        Assert.True(step.IsPositive);
    }

    [Fact]
    public void RejectsInvalid()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => TimeStep.FromSeconds(0));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => TimeStep.FromMilliseconds(-1));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => TimeStep.FromSeconds(double.PositiveInfinity));
    }

    [Fact]
    public void RejectsUnderflow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => TimeStep.FromMilliseconds(double.Epsilon));
    }
}
