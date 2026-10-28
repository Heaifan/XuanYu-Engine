using XuanYu.Render.Vulkan;
using Xunit;

namespace XuanYu.Render.Vulkan.Tests;

public sealed class VulkanNativeHostSurfaceBridgeThreadTests
{
    [Fact]
    public void Resize_rejects_a_thread_other_than_the_creator()
    {
        using var bridge = new VulkanNativeHostSurfaceBridge();

        Exception? exception = null;
        var thread = new Thread(() => exception = Record.Exception(() => bridge.Resize(640, 480)));
        thread.Start();
        thread.Join();

        Assert.IsType<InvalidOperationException>(exception);
    }
}
