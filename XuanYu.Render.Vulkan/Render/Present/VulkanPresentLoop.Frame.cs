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

    // R4-R3-R2：WaitFence 与 ResetFence 拆分，Wait 必须在 CommandBuffer 重录前
    // 以确保旧 CommandBuffer 不再被 GPU 使用；Reset 必须在 QueueSubmit 前。
    bool WaitFence(Silk.NET.Vulkan.Device device)
    {
        var fence = _fence;
        return Check(_vk.WaitForFences(device, 1, &fence, true, FenceTimeoutNs), "WaitForFences");
    }

    bool ResetFence(Silk.NET.Vulkan.Device device)
    {
        var fence = _fence;
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
