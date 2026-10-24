using XuanYu.Core.Time;

namespace XuanYu.Core.Tests.Foundation;

public sealed class SimulationTimeBoundsTests
{
    [Fact]
    public void Advance_rejects_finite_inputs_that_overflow()
    {
        var now = SimulationTime.FromSeconds(double.MaxValue);
        var step = TimeStep.FromSeconds(double.MaxValue);
        Assert.Throws<OverflowException>(() => now.Advance(step));
    }

    [Fact]
    public void Ordinary_advance_keeps_valid_elapsed_time()
    {
        var now = SimulationTime.FromSeconds(1.0);
        Assert.Equal(1.5, now.Advance(TimeStep.FromMilliseconds(500.0)).Seconds);
    }

    [Fact]
    public void Millisecond_step_that_underflows_seconds_is_rejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TimeStep.FromMilliseconds(double.Epsilon));
    }

    [Fact]
    public void Small_representable_millisecond_step_stays_positive()
    {
        Assert.True(TimeStep.FromMilliseconds(0.001).IsPositive);
    }
}
