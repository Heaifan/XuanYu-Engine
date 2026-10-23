using XuanYu.Render.Abstractions.Diagnostics.Performance;

namespace XuanYu.Render.Abstractions.Tests.Performance;

public sealed class PerformanceSampleContractTests
{
    [Fact]
    public void Sample_retains_frame_identity_and_units()
    {
        var identity = Identity(17);
        var sample = PerformanceFrameSample.Valid(identity, PerformanceMetric.GpuRenderPass,
            2.5, PerformanceUnit.Milliseconds, 0, 0, null, null, 0, 0, 0);

        Assert.Equal(identity, sample.Identity);
        Assert.Equal(17, sample.Identity.FrameId);
        Assert.Equal(PerformanceUnit.Milliseconds, sample.Unit);
    }

    [Fact]
    public void Missing_metric_has_no_fabricated_zero_value()
    {
        var sample = PerformanceFrameSample.Unavailable(Identity(3),
            PerformanceMetric.FenceWait, PerformanceSampleStatus.Missing, "query not read");

        Assert.Null(sample.Value);
        Assert.Equal(PerformanceSampleStatus.Missing, sample.Status);
    }

    [Fact]
    public void Delayed_timestamp_uses_the_frame_token_that_recorded_the_queries()
    {
        var submitted = Identity(41);
        var current = Identity(42);
        var token = new GpuTimestampFrameToken(submitted, 0, 1);

        var sample = GpuTimestampConverter.CreateSample(token, 100, 250, 32, 2.0, true);

        Assert.Equal(42, current.FrameId);
        Assert.Equal(submitted.FrameId, sample.Identity.FrameId);
        Assert.Equal(300.0, sample.Value);
        Assert.Equal(PerformanceUnit.Nanoseconds, sample.Unit);
    }

    [Fact]
    public void Unsupported_timestamp_metadata_produces_unavailable_sample()
    {
        var sample = GpuTimestampConverter.CreateSample(
            new GpuTimestampFrameToken(Identity(9), 2, 3), 10, 20, 0, 1, true);

        Assert.Null(sample.Value);
        Assert.Equal(PerformanceSampleStatus.Unsupported,
            sample.Status);
    }

    [Fact]
    public void Not_ready_gpu_query_is_missing_not_zero()
    {
        var sample = GpuTimestampConverter.CreateSample(
            new GpuTimestampFrameToken(Identity(10), 2, 3), 10, 20, 32, 1, false);

        Assert.Null(sample.Value);
        Assert.Equal(PerformanceSampleStatus.Missing, sample.Status);
    }


    [Fact]
    public void Timestamp_conversion_handles_counter_wrap_for_32_and_64_bit_devices()
    {
        var token = new GpuTimestampFrameToken(Identity(11), 4, 5);
        var wrapped32 = GpuTimestampConverter.CreateSample(token, 0xFFFF_FFF0, 5, 32, 1, true);
        var wrapped64 = GpuTimestampConverter.CreateSample(token,
            ulong.MaxValue - 2, 2, 64, 1, true);

        Assert.Equal(21, wrapped32.Value);
        Assert.Equal(5, wrapped64.Value);
    }

    [Fact]
    public void Monotonic_conversion_rejects_invalid_clock_metadata()
    {
        Assert.Equal(5_000_000_000d, PerformanceClock.ElapsedNanoseconds(10, 20, 2));
        Assert.Null(PerformanceClock.ElapsedNanoseconds(20, 10, 2));
        Assert.Null(PerformanceClock.ElapsedNanoseconds(1, 2, 0));
    }
    static PerformanceFrameIdentity Identity(long frameId) => new(
        "session", "build", "source", frameId, 7, "terrain:4", 2,
        PerformanceFrameMode.Single);
}
