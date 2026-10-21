using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Render;
using Xunit;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanPresentFenceQueuePolicyTests
{
    [Theory]
    [InlineData(Result.Success, PresentQueueDisposition.Enqueued)]
    [InlineData(Result.SuboptimalKhr, PresentQueueDisposition.Enqueued)]
    [InlineData(Result.ErrorOutOfDateKhr, PresentQueueDisposition.Enqueued)]
    [InlineData(Result.ErrorSurfaceLostKhr, PresentQueueDisposition.Enqueued)]
    [InlineData(Result.ErrorOutOfHostMemory, PresentQueueDisposition.NotEnqueued)]
    [InlineData(Result.ErrorOutOfDeviceMemory, PresentQueueDisposition.NotEnqueued)]
    [InlineData(Result.ErrorDeviceLost, PresentQueueDisposition.DeviceLost)]
    public void Present_result_controls_whether_its_fence_may_be_waited(
        Result result, PresentQueueDisposition expected)
    {
        var disposition = VulkanPresentFenceQueuePolicy.Classify(result);
        Assert.Equal(expected, disposition);
        Assert.Equal(expected == PresentQueueDisposition.Enqueued,
            VulkanPresentFenceQueuePolicy.RequiresFenceWait(disposition));
    }

    [Theory]
    [InlineData(true, false, PresentQueueDisposition.Enqueued, false)]
    [InlineData(true, true, PresentQueueDisposition.Enqueued, true)]
    [InlineData(false, true, PresentQueueDisposition.Enqueued, false)]
    [InlineData(true, false, PresentQueueDisposition.NotEnqueued, true)]
    [InlineData(true, false, PresentQueueDisposition.DeviceLost, true)]
    public void Resource_release_requires_graphics_and_present_completion(
        bool graphicsComplete, bool presentComplete, PresentQueueDisposition disposition, bool expected) =>
        Assert.Equal(expected, VulkanPresentFenceQueuePolicy.CanRelease(
            graphicsComplete, presentComplete, disposition));

    [Fact]
    public void Queue_api_seam_waits_only_when_vulkan_guarantees_the_present_was_enqueued()
    {
        var waitCalls = 0;
        Result Wait() { waitCalls++; return Result.Success; }
        var completed = VulkanPresentFenceQueuePolicy.Complete(
            Result.ErrorOutOfDateKhr, Wait, out var disposition);
        Assert.True(completed);
        Assert.Equal(PresentQueueDisposition.Enqueued, disposition);
        Assert.Equal(1, waitCalls);
        completed = VulkanPresentFenceQueuePolicy.Complete(
            Result.ErrorOutOfHostMemory, Wait, out disposition);
        Assert.False(completed);
        Assert.Equal(PresentQueueDisposition.NotEnqueued, disposition);
        Assert.Equal(1, waitCalls);
    }
}
