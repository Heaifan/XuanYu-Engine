using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanResizeContractTests
{
    [Theory]
    [InlineData(1.0, 800, 600)]
    [InlineData(1.25, 1000, 750)]
    [InlineData(1.5, 1200, 900)]
    [InlineData(2.0, 1600, 1200)]
    public void DpiConversionResizeTest(double dpi, int expectedWidth, int expectedHeight)
    {
        var snap = new NativeHostHandleSnapshot(NativeHostLifecycleState.Resized, 1, 800, 600, dpi, true, 1, DateTimeOffset.UtcNow);
        var handle = NativeHostSurfaceContract.ToSurfaceHandle(snap);
        Assert.Equal(expectedWidth, handle.Width);
        Assert.Equal(expectedHeight, handle.Height);
    }

    [Fact]
    public void BridgeResizeContractUsesPhysicalPixels()
    {
        var snap = new NativeHostHandleSnapshot(NativeHostLifecycleState.Resized, 1, 800, 600, 1.5, true, 1, DateTimeOffset.UtcNow);
        var handle = NativeHostSurfaceContract.ToSurfaceHandle(snap);
        Assert.Equal((1200, 900), (handle.Width, handle.Height));
        Assert.NotEqual((800, 600), (handle.Width, handle.Height));
    }
}
