using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using System.Threading;
using XuanYu.Render.Abstractions.Diagnostics;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanPresentLoop
{
    long _nextGpuFrameId;
    RendererEventSession? _eventSession;

    long NextGpuFrameId() => Interlocked.Increment(ref _nextGpuFrameId);

    AcquireDecision HandleAcquireResult(Result res)
    {
        if (res == Result.ErrorOutOfDateKhr)
        {
            PublishRendererEvent(RendererEventCodes.AcquireOutOfDate, RendererEventLevel.Warning,
                res.ToString());
        }
        if (res == Result.SuboptimalKhr)
            PublishRendererEvent(RendererEventCodes.AcquireSuboptimal, RendererEventLevel.Warning,
                res.ToString());
        if (res != Result.Success && res != Result.SuboptimalKhr &&
            res != Result.Timeout && res != Result.ErrorOutOfDateKhr)
            PublishRendererEvent(RendererEventCodes.AcquireFailed, RendererEventLevel.Error,
                res.ToString(), "AcquireNextImage");
        var decision = VulkanAcquireResultPolicy.Decide(res,
            res == Result.ErrorOutOfDateKhr ? () => _onOutOfDate?.Invoke("AcquireNextImage") ?? true : null);
        if (decision == AcquireDecision.StopOrFatal && res != Result.ErrorOutOfDateKhr)
            Check(res, "AcquireNextImage");
        return decision;
    }

    // R4-R3-R2：WaitFence 与 ResetFence 拆分，Wait 必须在 CommandBuffer 重录前
    // 以确保旧 CommandBuffer 不再被 GPU 使用；Reset 必须在 QueueSubmit 前。
    bool WaitFence(Silk.NET.Vulkan.Device device)
    {
        var fence = _fence;
        var result = _vk.WaitForFences(device, 1, &fence, true, FenceTimeoutNs);
        if (result == Result.Success) _graphicsPending = false;
        if (result == Result.ErrorDeviceLost) _deviceLost = true;
        return Check(result, "WaitForFences");
    }

    bool ResetFence(Silk.NET.Vulkan.Device device)
    {
        var fence = _fence;
        return Check(_vk.ResetFences(device, 1, &fence), "ResetFences");
    }

    bool SubmitFrame(Silk.NET.Vulkan.Device device, uint idx, PipelineStageFlags stage,
        long frameId, long cameraRevision)
    {
        var cmds = _clearFrame.CommandBuffers;
        var submit = new SubmitInfo { SType = StructureType.SubmitInfo };
        var imageAvailable = _imageAvailable;
        if (_syncGeneration is null || !_syncGeneration.TryGetImageSemaphore(idx, _swapchainOwner.Swapchain, out var renderFinished))
        { Fatal("Acquire image does not belong to the active synchronization generation."); return false; }
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
            var success = Check(_vk.QueueSubmit(_deviceOwner.GraphicsQueue, 1, &submit, fence), "QueueSubmit");
            if (success) _graphicsPending = true;
            _eventSession?.PublishFrameEvent(success ? RendererEventCodes.FrameSubmitted :
                RendererEventCodes.QueueSubmitFailed, success ? RendererEventLevel.Info :
                RendererEventLevel.Error, frameId, idx, cameraRevision);
            return success;
        }
    }

    void LogFirstPresent(uint idx)
    {
        _firstPresentLogged = true;
        Log(VulkanClearFrameLogFormatter.FirstPresented(idx));
    }
}
