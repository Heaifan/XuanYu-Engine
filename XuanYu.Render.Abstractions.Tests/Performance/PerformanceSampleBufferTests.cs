using XuanYu.Render.Abstractions.Diagnostics.Performance;

namespace XuanYu.Render.Abstractions.Tests.Performance;

public sealed class PerformanceSampleBufferTests
{
    [Fact]
    public void Full_buffer_counts_rejected_samples_and_preserves_accepted_rows()
    {
        var buffer = new PerformanceSampleBuffer(1);
        var first = Sample(1);
        var second = Sample(2);

        Assert.True(buffer.TryWrite(first));
        Assert.False(buffer.TryWrite(second));
        var rows = buffer.Drain();

        Assert.Single(rows);
        Assert.Equal(first.Identity, rows[0].Identity);
        Assert.Equal(1, rows[0].SampleSequence);
        Assert.Equal(1, buffer.DroppedCount);

        Assert.True(buffer.TryWrite(Sample(3)));
        Assert.Equal(3, Assert.Single(buffer.Drain()).SampleSequence);
    }

    [Fact]
    public void Disabled_buffer_does_not_count_or_retain_samples()
    {
        var buffer = new PerformanceSampleBuffer(4, enabled: false);

        Assert.False(buffer.TryWrite(Sample(1)));
        Assert.Empty(buffer.Drain());
        Assert.Equal(0, buffer.DroppedCount);
    }

    static PerformanceFrameSample Sample(long frame) => PerformanceFrameSample.Valid(
        new("s", "b", "f", frame, 0, "terrain:1", 1, PerformanceFrameMode.Single),
        PerformanceMetric.CpuFrameTotal, 1, PerformanceUnit.Nanoseconds, 1, 2, 3, 4, 10, 0, 0);
}
