using System;
using System.Diagnostics;
using System.Threading;
using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions.Diagnostics;
using XuanYu.Render.Vulkan.Render.Generation;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanPresentLoop
{
    public bool Stop()
    {
        var t = _thread;
        if (t is null) return CompleteForStop();
        Volatile.Write(ref _stopRequested, 1);
        if (!t.Join(2000))
        {
            PublishRendererEvent(RendererEventCodes.PresentLoopStopTimeout, RendererEventLevel.Error,
                reason: "停止等待超过 2000 ms");
            Log(VulkanClearFrameLogFormatter.LoopStopTimedOut());
            return false;
        }
        _thread = null;
        if (!CompleteForStop()) return false;
        _eventSession?.PublishLifecycleEvent(RendererEventCodes.PresentLoopStopped,
            RendererEventLevel.Info);
        Log(VulkanClearFrameLogFormatter.LoopStopped());
        return true;
    }

    internal bool RetireGenerationForSwapchainReplacement()
    {
        if (_deviceLost || _presentUnknown) return false;
        if (_graphicsPending && !WaitFence(_deviceOwner.LogicalDevice)) return false;
        if (_presentPending && !WaitPresentFence()) return false;
        if (_syncGeneration is null || !VulkanPresentGenerationPolicy.CanRetire(
                !_graphicsPending, !_presentPending, _presentUnknown, _deviceLost)) return false;
        var retiredGeneration = _syncGeneration.Generation;
        var retiredImageCount = _syncGeneration.ImageCount;
        var retiredSwapchain = _syncGeneration.Swapchain.Handle;
        _syncGeneration.Dispose();
        Log($"PresentSync generation retired: generation={retiredGeneration}; imageCount={retiredImageCount}; swapchain={retiredSwapchain}; graphics=complete; present=complete");
        _syncGeneration = null;
        _clearFrame.ReleaseSwapchainGeneration();
        return true;
    }
    public void Dispose()
    {
        if (!Stop()) return;
        DestroySync();
    }

    bool Check(Result res, string op, bool allowSuboptimal = false)
    {
        if (res == Result.ErrorDeviceLost) _deviceLost = true;
        if (res == Result.Success) return true;
        if (allowSuboptimal && res == Result.SuboptimalKhr) return true;
        PublishRendererEvent(RendererEventCodes.VulkanOperationFailed, RendererEventLevel.Error,
            res.ToString(), op);
        Fatal($"{op} 失败：{res}");
        return false;
    }

    void Fatal(string reason)
    {
        PublishRendererEvent(RendererEventCodes.PresentFatal, RendererEventLevel.Error, reason: reason);
        Log(VulkanClearFrameLogFormatter.PresentError(reason));
        _onFatal?.Invoke(reason);
    }

    void Log(string message)
    {
        try
        {
            _log?.Invoke(message);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(VulkanClearFrameLogFormatter.LogFallback(ex.Message, message));
        }
    }
}
