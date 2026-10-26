using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions.Diagnostics.Performance;
using XuanYu.Render.Vulkan.Diagnostics.Performance;
using XuanYu.Render.Vulkan.Render.Frame;

namespace XuanYu.World.Tests.Render.Frame;

public sealed class VulkanFenceWaitSampleSinkTests
{
    [Fact]
    public void Record_MapsLiveFenceObservationIntoSampleBuffer()
    {
        var sink = new VulkanFenceWaitSampleSink("session-live");
        sink.Record(new VulkanFenceWaitObservation(new VulkanFrameIdentity("session-live",
            17, 23, 1, 4, 9, 2), VulkanFenceWaitTarget.FrameSlotGraphicsFence,
            100, 250, 1_000_000, 11, 12, Result.Success));

        var sample = Assert.Single(sink.Drain());
        Assert.True(sample.IsValid);
        Assert.Equal(150_000d, sample.Value);
        Assert.Equal(PerformanceUnit.Nanoseconds, sample.Unit);
        Assert.Equal(PerformanceMetric.FenceWait, sample.Metric);
        Assert.Equal("session-live", sample.Identity.SessionId);
        Assert.Equal(17, sample.Identity.FrameId);
        Assert.Equal(23, sample.Identity.CameraRevision);
        Assert.Equal(1, sample.Identity.FrameSlot);
        Assert.Equal((uint)4, sample.Identity.ImageIndex);
        Assert.Equal((uint)9, sample.Identity.SwapchainGeneration);
        Assert.Equal(2, sample.Identity.FramesInFlight);
        Assert.Equal(PerformanceFrameMode.Double, sample.Identity.FrameMode);
        Assert.Equal("NOT AVAILABLE", sample.Identity.DataRevision);
        Assert.Equal(PerformanceOperationOutcome.Succeeded, sample.OperationOutcome);
    }
}
