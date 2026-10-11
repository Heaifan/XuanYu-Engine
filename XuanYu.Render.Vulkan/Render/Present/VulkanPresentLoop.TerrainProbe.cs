using Silk.NET.Vulkan;
using XuanYu.Core.Diagnostics;
using Silk.NET.Vulkan.Extensions.KHR;
using XuanYu.Render.Vulkan.Swapchain;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanPresentLoop
{
    bool HandlePresent(KhrSwapchain khr, SwapchainKHR swapchain,
        uint imageIndex, long gpuFrameId)
    {
        var result = PresentFrame(khr, swapchain, imageIndex);
        if (result == Result.ErrorOutOfDateKhr)
        {
            return _onOutOfDate?.Invoke("QueuePresent") ?? true;
        }

        if (!Check(result, "QueuePresent", allowSuboptimal: true)) return false;
        if (TerrainReplayOptions.Enabled)
            ViewportProbe.FramePresented(gpuFrameId, imageIndex);
        if (!_firstPresentLogged) LogFirstPresent(imageIndex);
        return true;
    }
}
