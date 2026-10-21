using System;
using System.Threading;
using XuanYu.Core.Diagnostics;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Swapchain;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace XuanYu.Render.Vulkan.Render;

// VK-LIFE-1：Present 泵必须确认停止成功后，才允许释放同步对象。
public sealed unsafe partial class VulkanPresentLoop : IDisposable
{
    readonly Vk _vk;
    readonly VulkanDeviceOwner _deviceOwner;
    readonly VulkanSwapchainOwner _swapchainOwner;
    readonly VulkanClearFrameOwner _clearFrame;
    readonly Func<string, bool>? _onOutOfDate;
    readonly Action<string>? _onFatal;
    readonly Action<string>? _log;
    Semaphore _imageAvailable;
    Fence _fence;
    Thread? _thread;
    int _stopRequested;
    bool _syncCreated;
    bool _firstPresentLogged;
    const ulong AcquireTimeoutNs = 1_000_000_000;
    const ulong FenceTimeoutNs = 1_000_000_000;

    public VulkanPresentLoop(Vk vk, VulkanDeviceOwner deviceOwner, VulkanSwapchainOwner swapchainOwner,
        VulkanClearFrameOwner clearFrame, Func<string, bool>? onOutOfDate, Action<string>? onFatal, Action<string>? log)
    {
        _vk = vk;
        _deviceOwner = deviceOwner;
        _swapchainOwner = swapchainOwner;
        _clearFrame = clearFrame;
        _onOutOfDate = onOutOfDate;
        _onFatal = onFatal;
        _log = log;
    }

    public bool Start()
    {
        if (_thread is not null) return true;
        if (!_syncCreated && !CreateSync()) return false;
        Volatile.Write(ref _stopRequested, 0);
        _thread = new Thread(Run) { IsBackground = true, Name = "VulkanPresent" };
        _thread.Start();
        PublishLoopStarted();
        Log(VulkanClearFrameLogFormatter.LoopStarted());
        return true;
    }

    void Run()
    {
        try { RunFrames(); }
        catch (Exception ex) { Fatal($"Present 线程异常：{ex.Message}"); }
    }

    // R4-R3-R2：修正同步顺序。先等待上一帧 Fence（确保 GPU 已完成旧 CommandBuffer），
    // 再应用 Pending RenderProjection（重录/释放 CommandBuffer），最后 ResetFence 紧贴 QueueSubmit。
    void RunFrames()
    {
        var device = _deviceOwner.LogicalDevice;
        var khr = _swapchainOwner.Khr;
        var stage = PipelineStageFlags.ColorAttachmentOutputBit;
        var submitted = false;
        while (Volatile.Read(ref _stopRequested) == 0)
        {
            if (submitted && !WaitFence(device)) break;
            if (!EnsurePresentGeneration()) { Fatal("Swapchain generation synchronization could not be safely replaced."); break; }
            if (!_clearFrame.TryApplyPendingRenderProjection()) { Fatal("Render Projection CommandBuffer 重录失败。"); break; }
            if (!_clearFrame.HasRenderProjection) { Thread.Sleep(16); continue; }
            var acquireDecision = AcquireImage(device, out var swapchain, out var idx);
            switch (acquireDecision)
            {
                case AcquireDecision.Acquired:
                    break;
                case AcquireDecision.TimeoutRetry:
                case AcquireDecision.RetryAfterRecreate:
                    continue;
                default:
                    return;
            }
            if (!ResetFence(device)) break;
            var gpuFrameId = NextGpuFrameId();
            var cameraRevision = _clearFrame.RenderProjection.Camera.Revision;
            if (!SubmitFrame(device, idx, stage, gpuFrameId, cameraRevision)) break;
            if (TerrainReplayOptions.Enabled) ViewportProbe.FrameSubmitted(gpuFrameId, idx, cameraRevision);
            submitted = true;
            if (!HandlePresent(khr, swapchain, idx, gpuFrameId, cameraRevision)) break;
        }
    }

}
