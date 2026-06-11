using FluidWarfare.Render.Vulkan.Markers;

namespace FluidWarfare.Tests.Render.Vulkan.Markers;

public sealed class VulkanMarkerDrawResultTests
{
    [Fact]
    public void NotChecked_ShouldUseNotCheckedStatus()
    {
        var result = VulkanMarkerDrawResult.NotChecked;
        Assert.Equal(VulkanMarkerDrawStatus.NotChecked, result.Status);
        Assert.False(result.IsSucceeded);
        Assert.Equal(0, result.DrawnMarkerCount);
        Assert.Equal(0, result.ElapsedMilliseconds);
    }

    [Fact]
    public void SucceededStatus_ShouldSetIsSucceededTrue()
    {
        var result = new VulkanMarkerDrawResult(VulkanMarkerDrawStatus.Succeeded, "成功。", 1, 12.5);
        Assert.True(result.IsSucceeded);
        Assert.Equal(VulkanMarkerDrawStatus.Succeeded, result.Status);
    }

    [Fact]
    public void FailedStatus_ShouldSetIsSucceededFalse()
    {
        var result = new VulkanMarkerDrawResult(VulkanMarkerDrawStatus.Failed, "失败。", 0, 5.0);
        Assert.False(result.IsSucceeded);
        Assert.Equal(VulkanMarkerDrawStatus.Failed, result.Status);
    }

    [Fact]
    public void DrawnMarkerCount_ShouldBePreserved()
    {
        var result = new VulkanMarkerDrawResult(VulkanMarkerDrawStatus.Succeeded, "成功。", 3, 8.0);
        Assert.Equal(3, result.DrawnMarkerCount);
    }

    [Fact]
    public void ElapsedMilliseconds_ShouldBePreserved()
    {
        var result = new VulkanMarkerDrawResult(VulkanMarkerDrawStatus.Succeeded, "成功。", 1, 25.5);
        Assert.Equal(25.5, result.ElapsedMilliseconds);
    }

    [Fact]
    public void Message_ShouldPreserveChineseText()
    {
        var result = new VulkanMarkerDrawResult(VulkanMarkerDrawStatus.Failed, "点位绘制失败。", 0, 0);
        Assert.Equal("点位绘制失败。", result.Message);
    }

    [Fact]
    public void SucceededWithMultipleMarkers_ShouldKeepCount()
    {
        var result = new VulkanMarkerDrawResult(VulkanMarkerDrawStatus.Succeeded, "绘制成功。", 5, 30.0);
        Assert.True(result.IsSucceeded);
        Assert.Equal(5, result.DrawnMarkerCount);
        Assert.Equal(30.0, result.ElapsedMilliseconds);
    }
}
