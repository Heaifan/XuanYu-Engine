using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Swapchain;

namespace XuanYu.World.Tests.Render;

// VK-PERF-R1：Present Mode 选择合同——FIFO（垂直同步）为首选，Mailbox 不再是默认。
public sealed class VulkanPresentModeSelectionTests
{
    [Fact]
    public void Fifo_is_preferred_when_supported()
    {
        var modes = new[] { PresentModeKHR.MailboxKhr, PresentModeKHR.FifoKhr };
        Assert.Equal(PresentModeKHR.FifoKhr, VulkanSwapchainCapabilities.ChoosePresentMode(modes));
    }

    [Fact]
    public void Fifo_is_preferred_over_mailbox_in_any_order()
    {
        var modes = new[] { PresentModeKHR.FifoKhr, PresentModeKHR.MailboxKhr };
        Assert.Equal(PresentModeKHR.FifoKhr, VulkanSwapchainCapabilities.ChoosePresentMode(modes));
    }

    [Fact]
    public void Mailbox_is_not_the_default()
    {
        var modes = new[] { PresentModeKHR.MailboxKhr, PresentModeKHR.ImmediateKhr };
        Assert.NotEqual(PresentModeKHR.MailboxKhr, VulkanSwapchainCapabilities.ChoosePresentMode(modes));
    }

    [Fact]
    public void Selection_is_deterministic()
    {
        var modes = new[] { PresentModeKHR.ImmediateKhr, PresentModeKHR.MailboxKhr, PresentModeKHR.FifoKhr };
        Assert.Equal(
            VulkanSwapchainCapabilities.ChoosePresentMode(modes),
            VulkanSwapchainCapabilities.ChoosePresentMode(modes));
    }
}
