using XuanYu.Engine.Render.Vulkan.Clear;

namespace FluidWarfare.Tests.Render.Vulkan.Clear;

public sealed class VulkanClearInfoTests
{
    [Fact]
    public void NotChecked_ShouldUseNotCheckedStatus()
    {
        var info = VulkanClearInfo.NotChecked;
        Assert.Equal(VulkanClearStatus.NotChecked, info.Status);
        Assert.False(info.IsSucceeded);
    }

    [Fact]
    public void SucceededStatus_ShouldSetIsSucceededTrue()
    {
        var info = new VulkanClearInfo(VulkanClearStatus.Succeeded, "成功。", "rgba(0.03, 0.08, 0.18, 1.00)", 640, 360, 12.5);
        Assert.True(info.IsSucceeded);
        Assert.Equal(VulkanClearStatus.Succeeded, info.Status);
    }

    [Fact]
    public void FailedStatus_ShouldSetIsSucceededFalse()
    {
        var info = new VulkanClearInfo(VulkanClearStatus.Failed, "失败。", "未知", 0, 0, 5.0);
        Assert.False(info.IsSucceeded);
        Assert.Equal(VulkanClearStatus.Failed, info.Status);
    }

    [Fact]
    public void Message_ShouldPreserveChineseText()
    {
        var info = new VulkanClearInfo(VulkanClearStatus.Failed, "清屏失败。", "未知", 0, 0, 0);
        Assert.Equal("清屏失败。", info.Message);
    }

    [Fact]
    public void ClearColorText_ShouldBePreserved()
    {
        var info = new VulkanClearInfo(VulkanClearStatus.Succeeded, "成功。", "rgba(0.03, 0.08, 0.18, 1.00)", 800, 600, 3.0);
        Assert.Equal("rgba(0.03, 0.08, 0.18, 1.00)", info.ClearColorText);
    }

    [Fact]
    public void Extent_ShouldBePreserved()
    {
        var info = new VulkanClearInfo(VulkanClearStatus.Succeeded, "成功。", "rgba(1,0,0,1)", 1920, 1080, 5.0);
        Assert.Equal(1920u, info.Width);
        Assert.Equal(1080u, info.Height);
    }

    [Fact]
    public void ElapsedMilliseconds_ShouldBePreserved()
    {
        var info = new VulkanClearInfo(VulkanClearStatus.Succeeded, "成功。", "rgba(0,0,0,1)", 640, 480, 25.5);
        Assert.Equal(25.5, info.ElapsedMilliseconds);
    }
}
