using XuanYu.Render.Abstractions.Diagnostics;

namespace XuanYu.Render.Vulkan.Swapchain.Diagnostics;

public static class VulkanSwapchainEvents
{
    public static void PublishFailure(RendererEventSession? events, int width, int height,
        string? result, string reason) => events?.PublishLifecycleEvent(
            RendererEventCodes.SwapchainFailed, RendererEventLevel.Error,
            requestedWidth: width, requestedHeight: height,
            vulkanResult: result, reason: reason);
}
