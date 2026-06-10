using FluidWarfare.Core.Time;

namespace FluidWarfare.Tests.Core.Time;

public sealed class SimulationTimeTests
{
    [Fact]
    public void Zero_ShouldHaveZeroSeconds()
    {
        Assert.Equal(0.0, SimulationTime.Zero.Seconds);
    }

    [Fact]
    public void Zero_ShouldBeZero()
    {
        Assert.True(SimulationTime.Zero.IsZero);
    }

    [Fact]
    public void FromSeconds_WithZero_ShouldCreateZeroTime()
    {
        Assert.True(SimulationTime.FromSeconds(0.0).IsZero);
    }

    [Fact]
    public void FromSeconds_WithPositiveValue_ShouldCreateTime()
    {
        Assert.Equal(1.5, SimulationTime.FromSeconds(1.5).Seconds);
    }

    [Fact]
    public void FromMilliseconds_WithPositiveValue_ShouldConvertToSeconds()
    {
        Assert.Equal(0.5, SimulationTime.FromMilliseconds(500.0).Seconds);
    }

    [Fact]
    public void FromSeconds_WithNegativeValue_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => SimulationTime.FromSeconds(-1.0));
    }

    [Fact]
    public void FromSeconds_WithNaN_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => SimulationTime.FromSeconds(double.NaN));
    }

    [Fact]
    public void FromSeconds_WithInfinity_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => SimulationTime.FromSeconds(double.PositiveInfinity));
    }

    [Fact]
    public void Advance_ShouldReturnNewTime()
    {
        var time = SimulationTime.FromSeconds(1.0).Advance(TimeStep.FromSeconds(0.5));

        Assert.Equal(1.5, time.Seconds);
    }

    [Fact]
    public void Advance_ShouldNotMutateOriginalTime()
    {
        var original = SimulationTime.FromSeconds(1.0);

        _ = original.Advance(TimeStep.FromSeconds(0.5));

        Assert.Equal(1.0, original.Seconds);
    }

    [Fact]
    public void SameValue_ShouldBeEqual()
    {
        Assert.Equal(SimulationTime.FromSeconds(0.5), SimulationTime.FromMilliseconds(500.0));
    }

    [Fact]
    public void ToString_ShouldBeStable()
    {
        Assert.Equal("SimulationTime(1.5s)", SimulationTime.FromSeconds(1.5).ToString());
    }
}
