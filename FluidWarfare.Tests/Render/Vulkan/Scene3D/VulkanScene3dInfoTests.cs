using FluidWarfare.Render.Vulkan.Scene3D;

namespace FluidWarfare.Tests.Render.Vulkan.Scene3D;

public sealed class VulkanScene3dInfoTests
{
    [Fact]
    public void NotChecked_ShouldUseNotCheckedStatus()
    {
        var info = VulkanScene3dInfo.NotChecked;
        Assert.Equal(VulkanScene3dStatus.NotChecked, info.Status);
        Assert.False(info.IsSucceeded);
        Assert.Equal(0, info.GridVertexCount);
        Assert.Equal(0, info.UnitVertexCount);
        Assert.Equal(0, info.RenderedUnitCount);
        Assert.Equal(0, info.DrawCallCount);
        Assert.Equal("无", info.DepthFormat);
        Assert.False(info.DepthTestEnabled);
    }

    [Fact]
    public void Succeeded_ShouldSetIsSucceededTrue()
    {
        var info = new VulkanScene3dInfo(
            VulkanScene3dStatus.Succeeded, "成功。",
            84, 42, 36, 12, 3, 3, 0, "D32Sfloat", 3, true,
            3, 640, 360, "Camera", 15.5);
        Assert.True(info.IsSucceeded);
        Assert.Equal(VulkanScene3dStatus.Succeeded, info.Status);
        Assert.Equal(3, info.RenderedUnitCount);
        Assert.Equal("D32Sfloat", info.DepthFormat);
        Assert.True(info.DepthTestEnabled);
    }

    [Fact]
    public void Failed_ShouldSetIsSucceededFalse()
    {
        var info = new VulkanScene3dInfo(
            VulkanScene3dStatus.Failed, "失败。",
            0, 0, 0, 0, 0, 0, 0, "无", 0, false,
            0, 0, 0, "无", 5.0);
        Assert.False(info.IsSucceeded);
        Assert.Equal(VulkanScene3dStatus.Failed, info.Status);
    }

    [Fact]
    public void ShouldPreserveVertexCounts()
    {
        var info = new VulkanScene3dInfo(
            VulkanScene3dStatus.Succeeded, "成功。",
            100, 50, 36, 12, 2, 2, 0, "D32Sfloat", 3, true,
            2, 800, 600, "Cam", 10.0);
        Assert.Equal(100, info.GridVertexCount);
        Assert.Equal(50, info.GridLineCount);
        Assert.Equal(36, info.UnitVertexCount);
        Assert.Equal(12, info.UnitTriangleCount);
    }

    [Fact]
    public void ShouldPreserveDrawCallCount()
    {
        var info = new VulkanScene3dInfo(
            VulkanScene3dStatus.Succeeded, "成功。",
            84, 42, 36, 12, 3, 3, 0, "D32Sfloat", 3, true,
            3, 640, 480, "Cam", 20.0);
        Assert.Equal(3, info.DrawCallCount);
    }

    [Fact]
    public void ShouldPreserveViewportDimensions()
    {
        var info = new VulkanScene3dInfo(
            VulkanScene3dStatus.Succeeded, "成功。",
            84, 42, 36, 12, 0, 0, 0, "无", 0, false,
            2, 1920, 1080, "Cam", 30.0);
        Assert.Equal(1920, info.ViewportWidth);
        Assert.Equal(1080, info.ViewportHeight);
    }

    [Fact]
    public void ShouldPreserveCameraSummary()
    {
        var info = new VulkanScene3dInfo(
            VulkanScene3dStatus.Succeeded, "成功。",
            84, 42, 36, 12, 0, 0, 0, "无", 0, false,
            2, 640, 360,
            "Position (0,18,24), Target (0,0,0), FOV 60°", 12.0);
        Assert.Contains("Position", info.CameraSummary);
        Assert.Contains("FOV", info.CameraSummary);
    }

    [Fact]
    public void ElapsedMilliseconds_ShouldBePreserved()
    {
        var info = new VulkanScene3dInfo(
            VulkanScene3dStatus.Succeeded, "成功。",
            84, 42, 36, 12, 0, 0, 0, "无", 0, false,
            2, 640, 360, "Cam", 25.5);
        Assert.Equal(25.5, info.ElapsedMilliseconds);
    }
}
