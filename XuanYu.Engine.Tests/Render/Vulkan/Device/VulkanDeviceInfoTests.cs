using FluidWarfare.Render.Vulkan.Device;

namespace FluidWarfare.Tests.Render.Vulkan.Device;

public sealed class VulkanDeviceInfoTests
{
    [Fact]
    public void NotChecked_ShouldUseNotCheckedStatus()
    {
        Assert.Equal(VulkanDeviceStatus.NotChecked, VulkanDeviceInfo.NotChecked.Status);
    }

    [Fact]
    public void CreatedStatus_ShouldSetIsCreatedTrue()
    {
        var info = CreateCreated();

        Assert.True(info.IsCreated);
    }

    [Fact]
    public void FailedStatus_ShouldSetIsCreatedFalse()
    {
        var info = new VulkanDeviceInfo(
            VulkanDeviceStatus.Failed,
            "创建失败。",
            "未知",
            "未知",
            -1,
            1.5);

        Assert.False(info.IsCreated);
    }

    [Fact]
    public void Message_ShouldPreserveChineseText()
    {
        var info = new VulkanDeviceInfo(
            VulkanDeviceStatus.Failed,
            "Vulkan Device 创建失败：未找到支持 Graphics Queue 的物理设备。",
            "未知",
            "未知",
            -1,
            1.5);

        Assert.Equal("Vulkan Device 创建失败：未找到支持 Graphics Queue 的物理设备。", info.Message);
    }

    [Fact]
    public void PhysicalDeviceName_ShouldBePreserved()
    {
        var info = CreateCreated();

        Assert.Equal("NVIDIA GeForce RTX 3060", info.PhysicalDeviceName);
    }

    [Fact]
    public void PhysicalDeviceTypeText_ShouldBePreserved()
    {
        var info = CreateCreated();

        Assert.Equal("DiscreteGpu", info.PhysicalDeviceTypeText);
    }

    [Fact]
    public void GraphicsQueueFamilyIndex_ShouldBePreserved()
    {
        var info = CreateCreated();

        Assert.Equal(0, info.GraphicsQueueFamilyIndex);
    }

    [Fact]
    public void ElapsedMilliseconds_ShouldBePreserved()
    {
        var info = CreateCreated();

        Assert.Equal(2.75, info.ElapsedMilliseconds);
    }

    [Fact]
    public void Probe_ShouldReturnNonNotCheckedStatus()
    {
        var info = VulkanDeviceProbe.Probe();

        Assert.NotEqual(VulkanDeviceStatus.NotChecked, info.Status);
    }

    [Fact]
    public void Probe_ShouldReturnNonEmptyMessage()
    {
        var info = VulkanDeviceProbe.Probe();

        Assert.False(string.IsNullOrWhiteSpace(info.Message));
    }

    [Fact]
    public void Probe_ShouldReportElapsedMilliseconds()
    {
        var info = VulkanDeviceProbe.Probe();

        Assert.True(info.ElapsedMilliseconds >= 0.0);
    }

    private static VulkanDeviceInfo CreateCreated()
    {
        return new VulkanDeviceInfo(
            VulkanDeviceStatus.Created,
            "创建成功。",
            "NVIDIA GeForce RTX 3060",
            "DiscreteGpu",
            0,
            2.75);
    }
}
