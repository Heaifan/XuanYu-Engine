namespace XuanYu.World.Tests.Render;

public sealed class VulkanLabelUploadDeviceLostShutdownTests
{
    [Fact]
    public void Upload_device_loss_marks_present_loop_device_lost_before_worker_exit()
    {
        var source = Read("Render", "Frame", "PresentLoop", "VulkanPresentLoop.cs");
        var handler = source.IndexOf("catch (VulkanLabelUploadDeviceLostException", StringComparison.Ordinal);
        Assert.True(handler >= 0);
        var mark = source.IndexOf("_deviceLost = true", handler, StringComparison.Ordinal);
        var fatal = source.IndexOf("Fatal(", handler, StringComparison.Ordinal);
        Assert.True(mark > handler && fatal > mark);
    }

    [Fact]
    public void Device_lost_stop_skips_fences_and_allows_sync_destruction()
    {
        var fences = Read("Render", "Frame", "PresentSync", "VulkanPresentLoop.PresentFence.cs");
        var lifecycle = Read("Render", "Frame", "PresentLoop", "VulkanPresentLoop.Lifecycle.cs");
        var stop = fences.IndexOf("bool CompleteForStop()", StringComparison.Ordinal);
        var deviceLost = fences.IndexOf("if (_deviceLost) return true", stop, StringComparison.Ordinal);
        var fenceWait = fences.IndexOf("WaitFence(_deviceOwner.LogicalDevice)", stop, StringComparison.Ordinal);
        Assert.True(deviceLost > stop && fenceWait > deviceLost);
        Assert.Contains("bool CanDestroySync => _deviceLost", fences);
        Assert.Contains("if (!Stop()) return;\n        DestroySync();", lifecycle);
    }

    static string Read(params string[] path) =>
        File.ReadAllText(Path.Combine(new[] { TestSourceRoot.Root, "XuanYu.Render.Vulkan" }.Concat(path).ToArray()));
}
