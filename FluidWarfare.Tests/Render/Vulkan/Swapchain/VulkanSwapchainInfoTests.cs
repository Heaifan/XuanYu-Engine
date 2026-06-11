using FluidWarfare.Render.Vulkan.Swapchain;

namespace FluidWarfare.Tests.Render.Vulkan.Swapchain;

public sealed class VulkanSwapchainInfoTests
{
    [Fact]
    public void NotChecked_ShouldUseNotCheckedStatus()
    {
        var info = VulkanSwapchainInfo.NotChecked;

        Assert.Equal(VulkanSwapchainStatus.NotChecked, info.Status);
        Assert.False(info.IsCreated);
        Assert.Equal(0u, info.ImageCount);
    }

    [Fact]
    public void CreatedStatus_ShouldSetIsCreatedTrue()
    {
        var info = new VulkanSwapchainInfo(VulkanSwapchainStatus.Created, "成功。", 3, "B8G8R8A8", "Mailbox", 640, 360, 12.5);

        Assert.True(info.IsCreated);
        Assert.Equal(VulkanSwapchainStatus.Created, info.Status);
    }

    [Fact]
    public void FailedStatus_ShouldSetIsCreatedFalse()
    {
        var info = new VulkanSwapchainInfo(VulkanSwapchainStatus.Failed, "失败。", 0, "未知", "未知", 0, 0, 5.0);

        Assert.False(info.IsCreated);
        Assert.Equal(VulkanSwapchainStatus.Failed, info.Status);
    }

    [Fact]
    public void Message_ShouldPreserveChineseText()
    {
        var info = new VulkanSwapchainInfo(VulkanSwapchainStatus.Failed, "Swapchain 创建失败。", 0, "未知", "未知", 0, 0, 0);

        Assert.Equal("Swapchain 创建失败。", info.Message);
    }

    [Fact]
    public void ImageCount_ShouldBePreserved()
    {
        var info = new VulkanSwapchainInfo(VulkanSwapchainStatus.Created, "成功。", 5, "R8G8B8A8", "Fifo", 1920, 1080, 8.0);

        Assert.Equal(5u, info.ImageCount);
    }

    [Fact]
    public void SurfaceFormatText_ShouldBePreserved()
    {
        var info = new VulkanSwapchainInfo(VulkanSwapchainStatus.Created, "成功。", 2, "B8G8R8A8Srgb", "Immediate", 800, 600, 3.0);

        Assert.Equal("B8G8R8A8Srgb", info.SurfaceFormatText);
    }

    [Fact]
    public void PresentModeText_ShouldBePreserved()
    {
        var info = new VulkanSwapchainInfo(VulkanSwapchainStatus.Created, "成功。", 2, "Fmt", "Mailbox", 800, 600, 3.0);

        Assert.Equal("Mailbox", info.PresentModeText);
    }

    [Fact]
    public void Extent_ShouldBePreserved()
    {
        var info = new VulkanSwapchainInfo(VulkanSwapchainStatus.Created, "成功。", 2, "Fmt", "Fifo", 1280, 720, 4.5);

        Assert.Equal(1280u, info.Width);
        Assert.Equal(720u, info.Height);
    }

    [Fact]
    public void ElapsedMilliseconds_ShouldBePreserved()
    {
        var info = new VulkanSwapchainInfo(VulkanSwapchainStatus.Created, "成功。", 2, "Fmt", "Fifo", 640, 480, 15.25);

        Assert.Equal(15.25, info.ElapsedMilliseconds);
    }
}
