using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanPresentLoop
{
    bool HandleAcquireResult(Result res)
    {
        if (res == Result.ErrorOutOfDateKhr)
            return _onOutOfDate?.Invoke("AcquireNextImage") ?? true;
        return Check(res, "AcquireNextImage", allowSuboptimal: true);
    }

    bool WaitAndResetFence(Silk.NET.Vulkan.Device device)
    {
        var fence = _fence;
        if (!Check(_vk.WaitForFences(device, 1, &fence, true, FenceTimeoutNs), "WaitForFences")) return false;
        return Check(_vk.ResetFences(device, 1, &fence), "ResetFences");
    }

    bool SubmitFrame(Silk.NET.Vulkan.Device device, uint idx, PipelineStageFlags stage)
    {
        var cmds = _clearFrame.CommandBuffers;
        var submit = new SubmitInfo { SType = StructureType.SubmitInfo };
        var imageAvailable = _imageAvailable;
        var renderFinished = _renderFinished;
        var fence = _fence;
        fixed (CommandBuffer* pCmd = cmds)
        {
            submit.WaitSemaphoreCount = 1;
            submit.PWaitSemaphores = &imageAvailable;
            submit.PWaitDstStageMask = &stage;
            submit.CommandBufferCount = 1;
            submit.PCommandBuffers = &pCmd[idx];
            submit.SignalSemaphoreCount = 1;
            submit.PSignalSemaphores = &renderFinished;
            return Check(_vk.QueueSubmit(_deviceOwner.GraphicsQueue, 1, &submit, fence), "QueueSubmit");
        }
    }

    Result PresentFrame(KhrSwapchain khr, SwapchainKHR swapchain, uint idx)
    {
        var present = new PresentInfoKHR { SType = StructureType.PresentInfoKhr };
        var renderFinished = _renderFinished;
        present.WaitSemaphoreCount = 1;
        present.PWaitSemaphores = &renderFinished;
        present.SwapchainCount = 1;
        present.PSwapchains = &swapchain;
        present.PImageIndices = &idx;
        return khr.QueuePresent(_deviceOwner.PresentQueue, &present);
    }

    void LogFirstPresent(uint idx)
    {
        _firstPresentLogged = true;
        Log(VulkanClearFrameLogFormatter.FirstPresented(idx));
    }
}
