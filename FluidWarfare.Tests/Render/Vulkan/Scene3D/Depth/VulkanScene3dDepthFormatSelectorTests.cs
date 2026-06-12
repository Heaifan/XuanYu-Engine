using FluidWarfare.Render.Vulkan.Scene3D.Depth;

namespace FluidWarfare.Tests.Render.Vulkan.Scene3D.Depth;

public sealed class VulkanScene3dDepthFormatSelectorTests
{
    [Fact]
    public void FormatName_D32Sfloat_ShouldReturnD32Sfloat()
    {
        var name = VulkanScene3dDepthFormatSelector.FormatName(Silk.NET.Vulkan.Format.D32Sfloat);
        Assert.Equal("D32Sfloat", name);
    }

    [Fact]
    public void FormatName_D32SfloatS8Uint_ShouldReturnCorrectName()
    {
        var name = VulkanScene3dDepthFormatSelector.FormatName(Silk.NET.Vulkan.Format.D32SfloatS8Uint);
        Assert.Equal("D32SfloatS8Uint", name);
    }

    [Fact]
    public void FormatName_D24UnormS8Uint_ShouldReturnCorrectName()
    {
        var name = VulkanScene3dDepthFormatSelector.FormatName(Silk.NET.Vulkan.Format.D24UnormS8Uint);
        Assert.Equal("D24UnormS8Uint", name);
    }

    [Fact]
    public void HasStencilComponent_D32Sfloat_ShouldReturnFalse()
    {
        Assert.False(VulkanScene3dDepthFormatSelector.HasStencilComponent(Silk.NET.Vulkan.Format.D32Sfloat));
    }

    [Fact]
    public void HasStencilComponent_D32SfloatS8Uint_ShouldReturnTrue()
    {
        Assert.True(VulkanScene3dDepthFormatSelector.HasStencilComponent(Silk.NET.Vulkan.Format.D32SfloatS8Uint));
    }

    [Fact]
    public void HasStencilComponent_D24UnormS8Uint_ShouldReturnTrue()
    {
        Assert.True(VulkanScene3dDepthFormatSelector.HasStencilComponent(Silk.NET.Vulkan.Format.D24UnormS8Uint));
    }
}
