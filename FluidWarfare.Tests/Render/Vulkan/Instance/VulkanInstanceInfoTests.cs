using FluidWarfare.Render.Vulkan.Instance;

namespace FluidWarfare.Tests.Render.Vulkan.Instance;

public sealed class VulkanInstanceInfoTests
{
    [Fact]
    public void NotChecked_ShouldUseNotCheckedStatus()
    {
        Assert.Equal(VulkanInstanceStatus.NotChecked, VulkanInstanceInfo.NotChecked.Status);
    }

    [Fact]
    public void CreatedStatus_ShouldSetIsCreatedTrue()
    {
        var info = new VulkanInstanceInfo(
            VulkanInstanceStatus.Created,
            "创建成功。",
            "1.3.280",
            12,
            1.5);

        Assert.True(info.IsCreated);
    }

    [Fact]
    public void FailedStatus_ShouldSetIsCreatedFalse()
    {
        var info = new VulkanInstanceInfo(
            VulkanInstanceStatus.Failed,
            "创建失败。",
            "未知",
            0,
            1.5);

        Assert.False(info.IsCreated);
    }

    [Fact]
    public void Message_ShouldPreserveChineseText()
    {
        var info = new VulkanInstanceInfo(
            VulkanInstanceStatus.Failed,
            "Vulkan Instance 创建失败：未检测到可用入口。",
            "未知",
            0,
            1.5);

        Assert.Equal("Vulkan Instance 创建失败：未检测到可用入口。", info.Message);
    }

    [Fact]
    public void ElapsedMilliseconds_ShouldBePreserved()
    {
        var info = new VulkanInstanceInfo(
            VulkanInstanceStatus.Created,
            "创建成功。",
            "1.3.280",
            12,
            2.75);

        Assert.Equal(2.75, info.ElapsedMilliseconds);
    }

    [Fact]
    public void Probe_ShouldReturnNonNotCheckedStatus()
    {
        var info = VulkanInstanceProbe.Probe();

        Assert.NotEqual(VulkanInstanceStatus.NotChecked, info.Status);
    }

    [Fact]
    public void Probe_ShouldReturnNonEmptyMessage()
    {
        var info = VulkanInstanceProbe.Probe();

        Assert.False(string.IsNullOrWhiteSpace(info.Message));
    }

    [Fact]
    public void Probe_ShouldReportElapsedMilliseconds()
    {
        var info = VulkanInstanceProbe.Probe();

        Assert.True(info.ElapsedMilliseconds >= 0.0);
    }
}
