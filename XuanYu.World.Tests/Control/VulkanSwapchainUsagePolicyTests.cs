using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Swapchain;

namespace XuanYu.World.Tests.Xyc;

public sealed class VulkanSwapchainUsagePolicyTests
{
    [Fact]
    public void Readback_usage_is_requested_only_when_surface_supports_it()
    {
        var supported = ImageUsageFlags.ColorAttachmentBit | ImageUsageFlags.TransferSrcBit;
        Assert.Equal(supported, VulkanSwapchainUsagePolicy.Select(supported, true));
        Assert.Equal(ImageUsageFlags.ColorAttachmentBit,
            VulkanSwapchainUsagePolicy.Select(ImageUsageFlags.ColorAttachmentBit, true));
        Assert.Equal(ImageUsageFlags.ColorAttachmentBit,
            VulkanSwapchainUsagePolicy.Select(supported, false));
    }
}
