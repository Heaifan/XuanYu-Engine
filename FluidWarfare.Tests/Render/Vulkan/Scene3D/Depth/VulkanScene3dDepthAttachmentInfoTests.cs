using FluidWarfare.Render.Vulkan.Scene3D.Depth;
using Silk.NET.Vulkan;

namespace FluidWarfare.Tests.Render.Vulkan.Scene3D.Depth;

public sealed class VulkanScene3dDepthAttachmentInfoTests
{
    [Fact]
    public void Unsupported_ShouldSetIsSupportedFalse()
    {
        var info = VulkanScene3dDepthAttachmentInfo.Unsupported("测试失败");
        Assert.False(info.IsSupported);
        Assert.Equal(Format.Undefined, info.ChosenFormat);
        Assert.False(info.HasStencil);
        Assert.Equal(0, info.AttachmentCount);
        Assert.Equal("测试失败", info.Message);
    }

    [Fact]
    public void Supported_ShouldPreserveFormatAndCount()
    {
        var info = new VulkanScene3dDepthAttachmentInfo(true, Format.D32Sfloat, false, 3, "OK");
        Assert.True(info.IsSupported);
        Assert.Equal(Format.D32Sfloat, info.ChosenFormat);
        Assert.False(info.HasStencil);
        Assert.Equal(3, info.AttachmentCount);
        Assert.Equal("D32Sfloat", info.FormatName);
    }

    [Fact]
    public void SupportedWithStencil_ShouldSetHasStencil()
    {
        var info = new VulkanScene3dDepthAttachmentInfo(true, Format.D32SfloatS8Uint, true, 3, "OK");
        Assert.True(info.HasStencil);
    }
}
