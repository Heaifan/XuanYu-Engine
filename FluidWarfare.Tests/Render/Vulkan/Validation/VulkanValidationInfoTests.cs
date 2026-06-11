using FluidWarfare.Render.Vulkan.Validation;

namespace FluidWarfare.Tests.Render.Vulkan.Validation;

public sealed class VulkanValidationInfoTests
{
    [Fact]
    public void Disabled_ShouldUseDisabledStatus()
    {
        var info = VulkanValidationInfo.Disabled;
        Assert.Equal(VulkanValidationStatus.Disabled, info.Status);
        Assert.False(info.IsEnabled);
    }

    [Fact]
    public void Enabled_ShouldSetIsEnabledTrue()
    {
        var info = new VulkanValidationInfo(VulkanValidationStatus.Enabled, "已启用。", 0);
        Assert.True(info.IsEnabled);
        Assert.Equal(VulkanValidationStatus.Enabled, info.Status);
    }

    [Fact]
    public void LayerMissing_ShouldSetIsEnabledFalse()
    {
        var info = new VulkanValidationInfo(VulkanValidationStatus.LayerMissing, "缺少 Layer。", 0);
        Assert.False(info.IsEnabled);
    }

    [Fact]
    public void ExtensionMissing_ShouldSetIsEnabledFalse()
    {
        var info = new VulkanValidationInfo(VulkanValidationStatus.ExtensionMissing, "缺少 Extension。", 0);
        Assert.False(info.IsEnabled);
    }

    [Fact]
    public void Failed_ShouldSetIsEnabledFalse()
    {
        var info = new VulkanValidationInfo(VulkanValidationStatus.Failed, "失败。", 0);
        Assert.False(info.IsEnabled);
    }

    [Fact]
    public void Message_ShouldBeChinese()
    {
        var info = new VulkanValidationInfo(VulkanValidationStatus.Disabled, "Vulkan Validation：未启用。", 0);
        Assert.Equal("Vulkan Validation：未启用。", info.Message);
    }

    [Fact]
    public void MessageCount_ShouldBePreserved()
    {
        var info = new VulkanValidationInfo(VulkanValidationStatus.Enabled, "已启用。", 5);
        Assert.Equal(5, info.MessageCount);
    }
}
