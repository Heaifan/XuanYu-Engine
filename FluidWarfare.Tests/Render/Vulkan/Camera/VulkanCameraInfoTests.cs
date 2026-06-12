using FluidWarfare.Render.Vulkan.Camera;

namespace FluidWarfare.Tests.Render.Vulkan.Camera;

public sealed class VulkanCameraInfoTests
{
    [Fact]
    public void DefaultBattlefield_ShouldUseFixedPosition()
    {
        var cam = VulkanCameraInfo.DefaultBattlefield;
        Assert.Equal(0, cam.PositionX);
        Assert.Equal(22, cam.PositionY);
        Assert.Equal(32, cam.PositionZ);
    }

    [Fact]
    public void DefaultBattlefield_ShouldTargetOrigin()
    {
        var cam = VulkanCameraInfo.DefaultBattlefield;
        Assert.Equal(0, cam.TargetX);
        Assert.Equal(0, cam.TargetY);
        Assert.Equal(0, cam.TargetZ);
    }

    [Fact]
    public void DefaultBattlefield_ShouldUseUpY()
    {
        var cam = VulkanCameraInfo.DefaultBattlefield;
        Assert.Equal(0, cam.UpX);
        Assert.Equal(1, cam.UpY);
        Assert.Equal(0, cam.UpZ);
    }

    [Fact]
    public void DefaultBattlefield_ShouldUse55Fov()
    {
        var cam = VulkanCameraInfo.DefaultBattlefield;
        Assert.Equal(55, cam.FieldOfViewDegrees);
    }

    [Fact]
    public void DefaultBattlefield_ShouldHaveReasonableNearFar()
    {
        var cam = VulkanCameraInfo.DefaultBattlefield;
        Assert.Equal(0.1f, cam.NearPlane);
        Assert.Equal(1000f, cam.FarPlane);
    }

    [Fact]
    public void ToSummary_ShouldContainCameraDetails()
    {
        var cam = VulkanCameraInfo.DefaultBattlefield;
        var summary = cam.ToSummary();
        Assert.Contains("Position", summary);
        Assert.Contains("Target", summary);
        Assert.Contains("FOV", summary);
        Assert.Contains("Near", summary);
        Assert.Contains("Far", summary);
    }

    [Fact]
    public void CustomCamera_ShouldPreserveParameters()
    {
        var cam = new VulkanCameraInfo(5, 10, 15, 1, 0, 0, 0, 1, 0, 45, 0.5f, 500);
        Assert.Equal(5, cam.PositionX);
        Assert.Equal(10, cam.PositionY);
        Assert.Equal(15, cam.PositionZ);
        Assert.Equal(45, cam.FieldOfViewDegrees);
        Assert.Equal(0.5f, cam.NearPlane);
        Assert.Equal(500, cam.FarPlane);
    }
}
