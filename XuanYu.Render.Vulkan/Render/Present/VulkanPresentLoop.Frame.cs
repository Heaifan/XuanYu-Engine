using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using System.Diagnostics;
using System.Threading;
using XuanYu.Render.Abstractions.Diagnostics;
using XuanYu.Render.Vulkan.Render.Frame;

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
        foreach (var frame in _frames)
            if (frame.GraphicsPending && !WaitFence(device, frame)) return false;
        return true;
    }

    bool WaitFence(Silk.NET.Vulkan.Device device, VulkanFrameContext frame,
        VulkanFenceWaitTarget target = VulkanFenceWaitTarget.FrameSlotGraphicsFence,
        VulkanFrameContext? sampleFrame = null)
    {
        if (!frame.GraphicsPending) return true;
        var fence = frame.GraphicsFence;
        var start = _onFenceWait is null ? 0 : Stopwatch.GetTimestamp();
        var startThread = _onFenceWait is null ? 0 : Environment.CurrentManagedThreadId;
        var result = _vk.WaitForFences(device, 1, &fence, true, FenceTimeoutNs);
        if (_onFenceWait is not null)
            PublishFenceWait(sampleFrame ?? frame, target, start, startThread, result);
        if (result == Result.Success)
        {
            frame.GraphicsPending = false;
            _clearFrame.CompleteFrameResources(frame);
            _graphicsPending = _frames.Any(x => x.GraphicsPending);
        }
        if (result == Result.ErrorDeviceLost) _deviceLost = true;
        return Check(result, "WaitForFences");
    }

    void PublishFenceWait(VulkanFrameContext frame, VulkanFenceWaitTarget target,
        long startTimestamp, int startThreadId, Result result)
    {
        var observation = new VulkanFenceWaitObservation(
            new VulkanFrameIdentity(_eventSession?.SessionId, frame.FrameId,
                frame.CameraRevision, frame.FrameIndex,
                frame.FrameId == 0 ? null : frame.LastImageIndex,
                frame.FrameId == 0 ? null : frame.SwapchainGeneration, _framesInFlight),
            target, startTimestamp, Stopwatch.GetTimestamp(), Stopwatch.Frequency,
            startThreadId, Environment.CurrentManagedThreadId, result);
        try { _onFenceWait?.Invoke(observation); }
        catch (Exception error) { Log($"Frame fence probe callback failed: {error.Message}"); }
    }

    bool ResetFence(Silk.NET.Vulkan.Device device, VulkanFrameContext frame)
    {
        var fence = frame.GraphicsFence;
        return Check(_vk.ResetFences(device, 1, &fence), "ResetFences");
    }

    void LogFirstPresent(uint idx)
    {
        _firstPresentLogged = true;
        Log(VulkanClearFrameLogFormatter.FirstPresented(idx));
    }
}
