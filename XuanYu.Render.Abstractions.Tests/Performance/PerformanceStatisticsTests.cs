using XuanYu.Render.Abstractions.Diagnostics.Performance;

namespace XuanYu.Render.Abstractions.Tests.Performance;

public sealed class PerformanceStatisticsTests
{
    [Fact]
    public void Calculates_nearest_rank_p50_p95_p99_from_valid_values()
    {
        var values = Enumerable.Range(1, 100).Reverse().Select(value => Sample(value)).ToArray();

        var result = PerformanceStatistics.Calculate(PerformanceMetric.GpuRenderPass, values, 2);

        Assert.Equal(100, result.ValidCount);
        Assert.Equal(50, result.P50);
        Assert.Equal(95, result.P95);
        Assert.Equal(99, result.P99);
        Assert.Equal(2, result.DroppedCount);
    }

    [Fact]
    public void No_valid_samples_produces_null_percentiles_not_zero()
    {
        var rows = new[] { PerformanceFrameSample.Unavailable(Identity(),
            PerformanceMetric.MeshBuild, PerformanceSampleStatus.Timeout, "timeout") };

        var result = PerformanceStatistics.Calculate(PerformanceMetric.MeshBuild, rows, 0);

        Assert.Equal(0, result.ValidCount);
        Assert.Null(result.P50);
        Assert.Null(result.P95);
        Assert.Null(result.P99);
        Assert.Equal(1, result.TimeoutCount);
    }

    [Fact]
    public void Invalid_status_and_nonfinite_value_are_counted_as_invalid()
    {
        var identity = Identity();
        var rows = new[]
        {
            PerformanceFrameSample.Unavailable(identity, PerformanceMetric.MeshBuild,
                PerformanceSampleStatus.Invalid, "bad timestamp"),
            PerformanceFrameSample.Valid(identity, PerformanceMetric.MeshBuild,
                double.NaN, PerformanceUnit.Milliseconds, 0, 0, null, null, 0, 0, 0)
        };

        var result = PerformanceStatistics.Calculate(PerformanceMetric.MeshBuild, rows, 0);

        Assert.Equal(2, result.InvalidCount);
        Assert.Equal(0, result.ValidCount);
        Assert.Null(result.P50);
    }

    [Fact]
    public void Operation_timeout_is_counted_separately_from_sample_validity()
    {
        var sample = PerformanceFrameSample.Valid(Identity(), PerformanceMetric.FenceWait,
            10, PerformanceUnit.Nanoseconds, 0, 0, 1, 2, 10, 0, 0,
            PerformanceOperationOutcome.Timeout);

        var result = PerformanceStatistics.Calculate(PerformanceMetric.FenceWait,
            [sample], 0);

        Assert.Equal(1, result.ValidCount);
        Assert.Equal(1, result.OperationTimeoutCount);
        Assert.Equal(10, result.P50);
    }

    static PerformanceFrameSample Sample(double value) => PerformanceFrameSample.Valid(
        Identity(), PerformanceMetric.GpuRenderPass, value, PerformanceUnit.Milliseconds,
        1, 1, (long)value, (long)value + 1, 10_000_000, 0, 0);

    static PerformanceFrameIdentity Identity() => new(
        "s", "b", "f", 1, 1, "terrain:1", 1, PerformanceFrameMode.Single);
}
