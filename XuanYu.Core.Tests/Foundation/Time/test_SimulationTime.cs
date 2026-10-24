using XuanYu.Core.Time;

namespace XuanYu.Core.Tests.Foundation.Time;

public sealed class SimulationTimeTests
{
    [Fact]
    public void StartsAtZero()
    {
        Assert.True(SimulationTime.Zero.IsZero);
        Assert.Equal(0, SimulationTime.Zero.Seconds);
    }

    [Fact]
    public void ConvertsUnits()
    {
        Assert.Equal(1.5, SimulationTime.FromMilliseconds(1500).Seconds);
        Assert.Equal(2500, SimulationTime.FromSeconds(2.5).Milliseconds);
    }

    [Fact]
    public void Advances()
    {
        var time = SimulationTime.FromSeconds(1);
        var step = TimeStep.FromMilliseconds(250);

        Assert.Equal(1.25, time.Advance(step).Seconds);
    }

    [Fact]
    public void RejectsInvalid()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => SimulationTime.FromSeconds(-1));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => SimulationTime.FromMilliseconds(double.NaN));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => SimulationTime.Zero.Advance(default));
    }

    [Fact]
    public void RejectsOverflow()
    {
        var time = SimulationTime.FromSeconds(double.MaxValue);
        var step = TimeStep.FromSeconds(double.MaxValue);

        Assert.Throws<OverflowException>(() => time.Advance(step));
    }
}
