using XuanYu.Engine.Render.Vulkan.Surface;

namespace XuanYu.Engine.Tests.Render.Vulkan.Surface;

public sealed class VulkanSurfaceInfoTests
{
    [Fact]
    public void NotChecked_ShouldUseNotCheckedStatus()
    {
        Assert.Equal(VulkanSurfaceStatus.NotChecked, VulkanSurfaceInfo.NotChecked.Status);
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
        var info = new VulkanSurfaceInfo(
            VulkanSurfaceStatus.Failed,
            "Surface 创建失败。",
            "Windows",
            true,
            1.5);

        Assert.False(info.IsCreated);
    }

    [Fact]
    public void UnsupportedPlatform_ShouldSetIsCreatedFalse()
    {
        var info = new VulkanSurfaceInfo(
            VulkanSurfaceStatus.UnsupportedPlatform,
            "当前平台不支持 Windows Vulkan Surface 创建。",
            "Linux",
            false,
            0.5);

        Assert.False(info.IsCreated);
    }

    [Fact]
    public void Message_ShouldPreserveChineseText()
    {
        var info = CreateCreated();

        Assert.Equal("Vulkan Surface 创建成功，并已立即释放。", info.Message);
    }

    [Fact]
    public void PlatformText_ShouldBePreserved()
    {
        var info = CreateCreated();

        Assert.Equal("Windows", info.PlatformText);
    }

    [Fact]
    public void HasNativeHandle_ShouldBePreserved()
    {
        var info = CreateCreated();

        Assert.True(info.HasNativeHandle);
    }

    [Fact]
    public void ElapsedMilliseconds_ShouldBePreserved()
    {
        var info = CreateCreated();

        Assert.Equal(2.75, info.ElapsedMilliseconds);
    }

    private static VulkanSurfaceInfo CreateCreated()
    {
        return new VulkanSurfaceInfo(
            VulkanSurfaceStatus.Created,
            "Vulkan Surface 创建成功，并已立即释放。",
            "Windows",
            true,
            2.75);
    }
}
