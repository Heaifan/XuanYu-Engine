using XuanYu.Engine.Render.Vulkan.Backend;

namespace XuanYu.Engine.Tests.Render.Vulkan.Backend;

public sealed class VulkanBackendInfoTests
{
    [Fact]
    public void NotChecked_ShouldUseNotCheckedStatus()
    {
        var info = VulkanBackendInfo.NotChecked;

        Assert.Equal(VulkanBackendStatus.NotChecked, info.Status);
        Assert.False(info.IsAvailable);
    }

    [Fact]
    public void AvailableStatus_ShouldSetIsAvailableTrue()
    {
        var info = new VulkanBackendInfo(VulkanBackendStatus.Available, "Vulkan 可用。");

        Assert.True(info.IsAvailable);
        Assert.Equal(VulkanBackendStatus.Available, info.Status);
    }

    [Fact]
    public void UnavailableStatus_ShouldSetIsAvailableFalse()
    {
        var info = new VulkanBackendInfo(VulkanBackendStatus.Unavailable, "Vulkan 不可用。");

        Assert.False(info.IsAvailable);
        Assert.Equal(VulkanBackendStatus.Unavailable, info.Status);
    }

    [Fact]
    public void Message_ShouldPreserveChineseText()
    {
        var info = new VulkanBackendInfo(VulkanBackendStatus.Unavailable, "Vulkan 后端尚未就绪。");

        Assert.Equal("Vulkan 后端尚未就绪。", info.Message);
    }

    [Fact]
    public void Probe_ShouldReturnNonEmptyMessage()
    {
        var info = VulkanBackendProbe.Probe();

        Assert.NotEqual(VulkanBackendStatus.NotChecked, info.Status);
        Assert.False(string.IsNullOrWhiteSpace(info.Message));
    }
}
