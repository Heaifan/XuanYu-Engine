using XuanYu.Render.Vulkan.Render.Frame;
using XuanYu.Render.Abstractions.Diagnostics;

namespace XuanYu.World.Tests.Render.Frame;

public sealed class VulkanFrameInFlightPolicyTests
{
    [Theory]
    [InlineData(0, 1, 0)]
    [InlineData(0, 2, 1)]
    [InlineData(1, 2, 0)]
    public void Advances_frame_slots_independently_of_swapchain_images(
        int current, int frameCount, int expected)
        => Assert.Equal(expected, VulkanFrameInFlightPolicy.NextFrameIndex(current, frameCount));

    [Theory]
    [InlineData(0, 0, 3, 0)]
    [InlineData(0, 2, 3, 2)]
    [InlineData(1, 0, 3, 3)]
    [InlineData(1, 2, 3, 5)]
    public void Command_buffer_indices_are_frame_slot_major(
        int frame, uint image, int imageCount, int expected)
        => Assert.Equal(expected, VulkanFrameInFlightPolicy.CommandBufferIndex(
            frame, image, imageCount, 2));

    [Fact]
    public void Reuses_a_frame_slot_only_after_its_graphics_and_present_work_completes()
    {
        Assert.False(VulkanFrameInFlightPolicy.CanReuseFrame(true, false, false, true));
        Assert.False(VulkanFrameInFlightPolicy.CanReuseFrame(false, true, true, false));
        Assert.True(VulkanFrameInFlightPolicy.CanReuseFrame(true, true, true, true));
    }

    [Fact]
    public void Rejects_invalid_image_indices_and_stale_swapchain_generations()
    {
        Assert.False(VulkanFrameInFlightPolicy.CanUseImage(3, 3, 7, 7, true));
        Assert.False(VulkanFrameInFlightPolicy.CanUseImage(1, 3, 6, 7, true));
        Assert.False(VulkanFrameInFlightPolicy.CanUseImage(1, 3, 7, 7, false));
        Assert.True(VulkanFrameInFlightPolicy.CanUseImage(1, 3, 7, 7, true));
    }

    [Fact]
    public void Mode_changes_require_every_frame_slot_to_be_drained()
    {
        Assert.False(VulkanFrameInFlightPolicy.CanChangeFrameCount(2, 1));
        Assert.False(VulkanFrameInFlightPolicy.CanChangeFrameCount(2, 2));
        Assert.True(VulkanFrameInFlightPolicy.CanChangeFrameCount(2, 0));
    }

    [Theory]
    [InlineData(true, true, false, false, true)]
    [InlineData(true, false, false, false, false)]
    [InlineData(false, true, false, false, false)]
    [InlineData(true, true, true, false, false)]
    [InlineData(false, false, false, true, false)]
    public void Retires_a_generation_only_after_all_completion_proofs(
        bool graphicsComplete, bool presentComplete, bool unknown, bool deviceLost,
        bool expected)
        => Assert.Equal(expected, VulkanFrameInFlightPolicy.CanRetireGeneration(
            graphicsComplete, presentComplete, unknown, deviceLost));

    [Fact]
    public void Resource_retirement_waits_for_every_frame_slot_lease()
    {
        var retirement = new VulkanFrameResourceRetirement(2);
        var resource = new TrackedResource();
        retirement.Track(0, resource);
        retirement.Track(1, resource);
        retirement.Retire(resource);
        retirement.Complete(0);
        Assert.False(resource.Disposed);
        retirement.Complete(1);
        Assert.True(resource.Disposed);
    }

    [Fact]
    public void Frame_event_identity_keeps_slot_image_zero_and_generation_distinct()
    {
        var item = new RendererEvent("FrameSubmitted", RendererEventChannel.Probe,
            RendererEventLevel.Info, "run", "session", DateTimeOffset.UtcNow, "build",
            FrameId: 41, ImageIndex: 0, CameraRevision: 7, FrameSlot: 1,
            FramesInFlight: 2, SwapchainGeneration: 9);
        Assert.Equal(41, item.FrameId);
        Assert.Equal(0u, item.ImageIndex);
        Assert.Equal(7, item.CameraRevision);
        Assert.Equal(1, item.FrameSlot);
        Assert.Equal(2, item.FramesInFlight);
        Assert.Equal(9u, item.SwapchainGeneration);
    }

    sealed class TrackedResource : IDisposable
    {
        public bool Disposed { get; private set; }
        public void Dispose() => Disposed = true;
    }
}
