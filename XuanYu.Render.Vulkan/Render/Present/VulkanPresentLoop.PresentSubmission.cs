using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanPresentLoop
{
    bool PresentFrame(KhrSwapchain khr, SwapchainKHR swapchain, uint idx, out Result result)
    {
        result = Result.ErrorInitializationFailed;
        if (!ResetPresentFence()) return false;
        var present = new PresentInfoKHR { SType = StructureType.PresentInfoKhr };
        var fence = _presentFence;
        var fenceInfo = new SwapchainPresentFenceInfoEXT
        { SType = StructureType.SwapchainPresentFenceInfoExt, SwapchainCount = 1, PFences = &fence };
        present.PNext = &fenceInfo;
        var renderFinished = _renderFinished;
        present.WaitSemaphoreCount = 1; present.PWaitSemaphores = &renderFinished;
        present.SwapchainCount = 1; present.PSwapchains = &swapchain;
        present.PImageIndices = &idx;
        result = khr.QueuePresent(_deviceOwner.PresentQueue, &present);
        return true;
    }
}
