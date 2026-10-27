using System.IO;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanFramebufferRebuildFailureTests
{
    [Fact]
    public void Candidate_framebuffers_are_local_and_partial_candidates_are_destroyed()
    {
        var source = Read("Render", "ClearFrame", "Commands", "VulkanClearFrameOwner.Commands.cs");

        Assert.Contains("var nextFramebuffers = new Framebuffer[_views.Length]", source);
        Assert.Contains("DestroyCandidateFramebuffers(nextFramebuffers, nextDepthAttachments)", source);
        Assert.Contains("DestroyCandidateFramebuffers", source);
    }

    [Fact]
    public void Failed_rebuild_invalidates_command_buffers_and_all_frame_target_handles()
    {
        var owner = Read("Render", "ClearFrame", "VulkanClearFrameOwner.Initialize.cs");
        var lifecycle = Read("Render", "ClearFrame", "VulkanClearFrameOwner.Lifecycle.cs");
        var commands = Read("Render", "ClearFrame", "Commands", "VulkanClearFrameOwner.Commands.cs");

        Assert.Contains("FailClosedFrameTargets", owner);
        Assert.Contains("_commandBuffers = []", lifecycle);
        Assert.Contains("_views = []", lifecycle);
        Assert.Contains("_framebufferGeneration++", lifecycle);
        Assert.Contains("return false", commands);
    }

    [Fact]
    public void Depth_attachment_partial_creation_destroys_image_before_bound_memory()
    {
        var source = Read("Render", "ClearFrame", "Resources", "VulkanDepthAttachment.cs");
        var imageDestroy = source.IndexOf("DestroyImage", StringComparison.Ordinal);
        var memoryFree = source.IndexOf("FreeMemory", StringComparison.Ordinal);

        Assert.True(imageDestroy >= 0 && memoryFree > imageDestroy);
        Assert.Contains("finally", source);
    }

    [Fact]
    public void Resize_stops_present_before_replacing_swapchain_and_fails_closed()
    {
        var resize = Read("Session", "Runtime", "Resize.cs");
        var stop = resize.IndexOf("_presentLoop.Stop()", StringComparison.Ordinal);
        var swapchain = resize.IndexOf("TryRecreateToCurrent", StringComparison.Ordinal);
        var rebuild = resize.IndexOf("RebuildFramebuffers", StringComparison.Ordinal);

        Assert.True(stop >= 0 && stop < swapchain && swapchain < rebuild);
        Assert.Contains("if (!_clearFrame.RebuildFramebuffers", resize);
        Assert.Contains("return FailResize", resize);
    }

    static string Read(params string[] path) =>
        File.ReadAllText(Path.Combine(new[] { TestSourceRoot.Root, "XuanYu.Render.Vulkan" }.Concat(path).ToArray()));
}
