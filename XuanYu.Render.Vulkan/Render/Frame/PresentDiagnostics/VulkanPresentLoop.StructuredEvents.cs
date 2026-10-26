using XuanYu.Render.Abstractions.Diagnostics;
using XuanYu.Render.Vulkan.Render.Frame;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanPresentLoop
{
    internal void PublishLifecycle(string code, RendererEventLevel level, uint generation,
        int requestedWidth, int requestedHeight, uint actualWidth, uint actualHeight,
        string? reason = null) => _eventSession?.PublishLifecycleEvent(code, level, generation,
            requestedWidth, requestedHeight, actualWidth, actualHeight, _swapchainOwner.ImageViews.Length,
            reason: reason);

    internal void PublishRendererEvent(string code, RendererEventLevel level,
        string? result = null, string? reason = null) => _eventSession?.PublishLifecycleEvent(code, level,
            generation: _swapchainOwner.ResourceGeneration, actualWidth: _swapchainOwner.Extent.Width,
            actualHeight: _swapchainOwner.Extent.Height, imageViewCount: _swapchainOwner.ImageViews.Length,
            vulkanResult: result, reason: reason);

    internal void AttachSwapchainEvents() => _eventSession = _swapchainOwner.EventSession;

    void PublishLoopStarted() => _eventSession?.PublishLifecycleEvent(
        RendererEventCodes.PresentLoopStarted, RendererEventLevel.Info,
        generation: _swapchainOwner.ResourceGeneration, actualWidth: _swapchainOwner.Extent.Width,
        actualHeight: _swapchainOwner.Extent.Height);

    void PublishTerrainResources(VulkanFrameContext frame, uint imageIndex)
    {
        if (_eventSession?.ProbeEnabled != true) return;
        var resources = _clearFrame.RenderProjection.TerrainResources;
        if (resources.Count == 0)
        {
            _eventSession.PublishTerrainFrame(RendererEventCodes.TerrainProbeUnavailable,
                frame.FrameId, imageIndex, frame.CameraRevision, reason: "NoTerrain",
                frameSlot: frame.FrameIndex, framesInFlight: _framesInFlight,
                swapchainGeneration: frame.SwapchainGeneration);
            return;
        }
        foreach (var terrain in resources)
            _eventSession.PublishTerrainFrame(RendererEventCodes.TerrainResourcePresented,
                frame.FrameId, imageIndex, frame.CameraRevision, terrain.TerrainId, terrain.Revision,
                frameSlot: frame.FrameIndex, framesInFlight: _framesInFlight,
                swapchainGeneration: frame.SwapchainGeneration);
    }
}
