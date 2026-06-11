using FluidWarfare.Render.Vulkan.Markers;

namespace FluidWarfare.Tests.Render.Vulkan.Markers;

public sealed class VulkanMarkerDrawInfoTests
{
    [Fact]
    public void CreateAtCenter_ShouldUseCenterCoordinates()
    {
        var info = VulkanMarkerDrawInfo.CreateAtCenter("sample_unit", 800, 600);
        Assert.Equal(400, info.PixelX);
        Assert.Equal(300, info.PixelY);
        Assert.Equal(12, info.PixelSize);
        Assert.Equal("rgba(1.00, 0.82, 0.20, 1.00)", info.ColorText);
    }

    [Fact]
    public void CreateAtCenter_ShouldPreserveDisplayName()
    {
        var info = VulkanMarkerDrawInfo.CreateAtCenter("test_unit", 640, 360);
        Assert.Equal("test_unit", info.DisplayName);
    }

    [Fact]
    public void FromWorldPosition_Origin_ShouldMapToCenter()
    {
        var info = VulkanMarkerDrawInfo.FromWorldPosition("origin", 0, 0, 640, 360);
        Assert.Equal(320, info.PixelX);
        Assert.Equal(180, info.PixelY);
    }

    [Fact]
    public void FromWorldPosition_PositiveX_ShouldShiftRight()
    {
        var info = VulkanMarkerDrawInfo.FromWorldPosition("right", 10, 0, 640, 360);
        Assert.Equal(420, info.PixelX);  // 320 + 10*10
    }

    [Fact]
    public void FromWorldPosition_PositiveZ_ShouldShiftUp()
    {
        var info = VulkanMarkerDrawInfo.FromWorldPosition("up", 0, 5, 640, 360);
        Assert.Equal(130, info.PixelY);  // 180 - 5*10
    }

    [Fact]
    public void FromWorldPosition_ShouldUseDefaultColorAndSize()
    {
        var info = VulkanMarkerDrawInfo.FromWorldPosition("unit", 0, 0, 1920, 1080);
        Assert.Equal("rgba(1.00, 0.82, 0.20, 1.00)", info.ColorText);
        Assert.Equal(12, info.PixelSize);
    }

    [Fact]
    public void FromWorldPosition_ShouldAcceptCustomSize()
    {
        var info = VulkanMarkerDrawInfo.FromWorldPosition("unit", 0, 0, 640, 360, 24);
        Assert.Equal(24, info.PixelSize);
    }
}
