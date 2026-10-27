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

}
