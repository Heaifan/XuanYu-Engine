using System.IO;

namespace XuanYu.World.Tests.Render.Frame;

public sealed class VulkanFrameReconfigurationTests
{
    [Fact]
    public void Frame_mode_switch_drains_before_reconfiguring_and_restarts_after_sync_creation()
    {
        var source = File.ReadAllText(Path.Combine(TestSourceRoot.Root, "XuanYu.Render.Vulkan", "Render", "Frame", "PresentLoop", "VulkanPresentLoop.Lifecycle.cs"));
        var start = source.IndexOf("public bool TrySetFramesInFlight", StringComparison.Ordinal);
        var end = source.IndexOf("public void Dispose()", start, StringComparison.Ordinal);
        var body = source[start..end];
        Assert.True(body.IndexOf("Stop()", StringComparison.Ordinal) <
            body.IndexOf("CompleteForStop()", StringComparison.Ordinal));
        Assert.True(body.IndexOf("CompleteForStop()", StringComparison.Ordinal) <
            body.IndexOf("ReconfigureFrameSlots", StringComparison.Ordinal));
        Assert.True(body.IndexOf("ReconfigureFrameSlots", StringComparison.Ordinal) <
            body.IndexOf("DestroySync()", StringComparison.Ordinal));
        Assert.True(body.IndexOf("DestroySync()", StringComparison.Ordinal) <
            body.IndexOf("CreateSync()", StringComparison.Ordinal));
        Assert.True(body.IndexOf("CreateSync()", StringComparison.Ordinal) <
            body.IndexOf("Start()", StringComparison.Ordinal));
    }

    [Fact]
    public void Each_swapchain_image_owns_a_separate_depth_attachment()
    {
        var source = File.ReadAllText(Path.Combine(TestSourceRoot.Root, "XuanYu.Render.Vulkan", "Render", "ClearFrame", "Commands", "VulkanClearFrameOwner.Commands.cs"));
        var loop = source.IndexOf("for (var i = 0; i < _views.Length; i++)", StringComparison.Ordinal);
        var create = source.IndexOf("VulkanDepthAttachment.Create", loop, StringComparison.Ordinal);
        Assert.True(loop >= 0 && create > loop);
        Assert.Contains("new[] { _views[i], depth.View }", source);
    }

    [Fact]
    public void Command_buffer_allocation_covers_each_frame_slot_and_swapchain_image()
    {
        var source = File.ReadAllText(Path.Combine(TestSourceRoot.Root, "XuanYu.Render.Vulkan", "Render", "ClearFrame", "Commands", "VulkanClearFrameOwner.Commands.cs"));
        Assert.Contains("var next = new CommandBuffer[views.Length * _frameSlotCount]", source);
        Assert.Contains("CommandBufferCount = (uint)next.Length", source);
    }

    static string ReadSource(string relative)
    {
        var root = new DirectoryInfo(TestSourceRoot.Root);
        while (root is not null && !File.Exists(Path.Combine(root.FullName, "XuanYu.Engine.slnx")))
            root = root.Parent;
        Assert.NotNull(root);
        return File.ReadAllText(Path.Combine(root!.FullName, "XuanYu.Render.Vulkan", relative));
    }
}
