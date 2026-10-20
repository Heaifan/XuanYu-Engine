using XuanYu.Render.Abstractions.Diagnostics;

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

    internal void AttachSwapchainEvents() => _swapchainOwner.AttachEventSession(_eventSession);

    void PublishLoopStarted() => _eventSession?.PublishLifecycleEvent(
        RendererEventCodes.PresentLoopStarted, RendererEventLevel.Info,
        generation: _swapchainOwner.ResourceGeneration, actualWidth: _swapchainOwner.Extent.Width,
        actualHeight: _swapchainOwner.Extent.Height);
}
