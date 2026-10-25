using XuanYu.Render.Abstractions.Diagnostics.Performance;

namespace XuanYu.Render.Abstractions.Tests.Performance;

public sealed class PerformanceFenceWaitAdapterTests
{
    [Fact]
    public void Maps_single_frame_fields_and_keeps_timeout_duration_valid()
    {
        var sample = PerformanceFenceWaitAdapter.Convert(
            Observation("Timeout", "FrameSlotGraphicsFence", 42, 0, 1, 7, 1),
            Run(PerformanceFrameMode.Single));

        Assert.True(sample.IsValid);
        Assert.Equal(10_000_000d, sample.Value);
        Assert.Equal("session-b", sample.Identity.SessionId);
        Assert.Equal("build", sample.Identity.BuildIdentity);
        Assert.Equal("source", sample.Identity.SourceFingerprint);
        Assert.Equal("terrain-r4", sample.Identity.DataRevision);
        Assert.Equal(42, sample.Identity.FrameId);
        Assert.Equal(9, sample.Identity.CameraRevision);
        Assert.Equal(0, sample.Identity.FrameSlot);
        Assert.Equal<uint?>(1, sample.Identity.ImageIndex);
        Assert.Equal<uint?>(7, sample.Identity.SwapchainGeneration);
        Assert.Equal(1, sample.Identity.FramesInFlight);
        Assert.Equal(PerformanceFrameMode.Single, sample.Identity.FrameMode);
        Assert.Equal(PerformanceUnit.Nanoseconds, sample.Unit);
        Assert.Equal(PerformanceOperationOutcome.Timeout, sample.OperationOutcome);
        Assert.Contains("FrameSlotGraphicsFence", sample.Detail);
    }

    [Fact]
    public void Maps_double_frame_identity_and_c_buffer_sequence()
    {
        var first = PerformanceFenceWaitAdapter.Convert(
            Observation("Success", "AcquiredImageOwnerGraphicsFence", 51, 1, 0, 8, 2),
            Run(PerformanceFrameMode.Double));
        var second = PerformanceFenceWaitAdapter.Convert(
            Observation("Success", "SwapchainImagePresentFence", 52, 0, 2, 8, 2),
            Run(PerformanceFrameMode.Double));
        var buffer = new PerformanceSampleBuffer(2);

        Assert.True(buffer.TryWrite(first));
        Assert.True(buffer.TryWrite(second));
        var rows = buffer.Drain();

        Assert.Equal(1, rows[0].SampleSequence);
        Assert.Equal(2, rows[1].SampleSequence);
        Assert.Equal(1, rows[0].Identity.FrameSlot);
        Assert.Equal<uint?>(0, rows[0].Identity.ImageIndex);
        Assert.Equal(0, rows[1].Identity.FrameSlot);
        Assert.Equal<uint?>(2, rows[1].Identity.ImageIndex);
        Assert.Equal(PerformanceFrameMode.Double, rows[1].Identity.FrameMode);
    }

    [Fact]
    public void Device_lost_result_does_not_invalidate_captured_wait_duration()
    {
        var sample = PerformanceFenceWaitAdapter.Convert(
            Observation("ErrorDeviceLost", "SwapchainImagePresentFence",
                55, 1, 2, 8, 2), Run(PerformanceFrameMode.Double));

        Assert.True(sample.IsValid);
        Assert.Equal(PerformanceOperationOutcome.DeviceLost,
            sample.OperationOutcome);
        Assert.Equal(10_000_000d, sample.Value);
    }

    [Fact]
    public void Invalid_clock_or_unknown_target_has_no_duration()
    {
        var invalidClock = Observation("Success", "FrameSlotGraphicsFence",
            61, 0, 1, 4, 1) with { EndTimestamp = 10, StartTimestamp = 20 };
        var invalidTarget = Observation("Success", "FutureFence",
            62, 0, 1, 4, 1);

        var clockSample = PerformanceFenceWaitAdapter.Convert(
            invalidClock, Run(PerformanceFrameMode.Single));
        var targetSample = PerformanceFenceWaitAdapter.Convert(
            invalidTarget, Run(PerformanceFrameMode.Single));

        Assert.Equal(PerformanceSampleStatus.Invalid, clockSample.Status);
        Assert.Equal(PerformanceSampleStatus.Invalid, targetSample.Status);
        Assert.Null(clockSample.Value);
        Assert.Null(targetSample.Value);
    }

    static RawFenceWaitObservation Observation(string result, string target,
        long frame, int slot, uint image, uint generation, int frames) => new(
        new("session-b", frame, 9, slot, image, generation, frames), target,
        100, 110, 1_000, 7, 8, result);

    static PerformanceRunIdentity Run(PerformanceFrameMode mode) =>
        new("fallback-session", "build", "source", "terrain-r4", mode);
}
