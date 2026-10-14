using XuanYu.Core.Diagnostics;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    void LogR7ViewportFrame()
    {
        if (!TerrainReplayOptions.Enabled) return;
        ViewportProbe.Log("viewport", $"[R7-RENDER-VIEWPORT] " +
            $"ViewportFrameId={ViewportProbe.CurrentFrameId};" +
            $"SwapchainExtent={_extent.Width}x{_extent.Height};" +
            $"FramebufferGeneration={_framebufferGeneration};" +
            $"SwapchainGeneration={_swapchainOwner.ResourceGeneration}");
    }
}
