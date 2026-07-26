using System;
using System.Threading;
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
    Semaphore _renderFinished;
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
        Log(VulkanClearFrameLogFormatter.LoopStarted());
        return true;
    }

    void Run()
    {
        try { RunFrames(); }
        catch (Exception ex) { Fatal($"Present 线程异常：{ex.Message}"); }
    }

    void RunFrames()
    {
        var device = _deviceOwner.LogicalDevice;
        var khr = _swapchainOwner.Khr;
        var stage = PipelineStageFlags.ColorAttachmentOutputBit;
        var submitted = false;
        while (Volatile.Read(ref _stopRequested) == 0)
        {
            if (!_clearFrame.TryApplyPendingRenderProjection())
            {
                Fatal("Render Projection CommandBuffer 重录失败。");
                break;
            }
            if (!_clearFrame.HasRenderProjection)
            {
                Thread.Sleep(16);
                continue;
            }
            var swapchain = _swapchainOwner.Swapchain;
            uint idx;
            var res = khr.AcquireNextImage(device, swapchain, AcquireTimeoutNs, _imageAvailable, default, &idx);
            if (res == Result.Timeout) continue;
            if (!HandleAcquireResult(res)) break;
            if (submitted && !WaitAndResetFence(device)) break;
            if (!SubmitFrame(device, idx, stage)) break;
            submitted = true;
            var pres = PresentFrame(khr, swapchain, idx);
            if (pres == Result.ErrorOutOfDateKhr)
            {
                if (!(_onOutOfDate?.Invoke("QueuePresent") ?? true)) break;
                continue;
            }
            if (!Check(pres, "QueuePresent", allowSuboptimal: true)) break;
            if (!_firstPresentLogged) LogFirstPresent(idx);
        }
    }

}
