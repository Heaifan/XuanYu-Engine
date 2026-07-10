using System;
using System.Threading;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Swapchain;
using XuanYu.Render.Vulkan.Render;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace XuanYu.Render.Vulkan.Render;

// VK4-D-R1：Present 泵独立后台线程；记录首帧 Present 成功一次；Acquire/Submit/Present 失败记录中文错误。
// RZ-VK5-A-R2：OutOfDate 不再永久 break，改经 onOutOfDate 回调请求 RenderSession 统一自愈；无回调时退回 continue。
public sealed unsafe class VulkanPresentLoop : IDisposable
{
    readonly Vk _vk; readonly VulkanDeviceOwner _deviceOwner; readonly VulkanSwapchainOwner _swapchainOwner; readonly VulkanClearFrameOwner _clearFrame; readonly Func<string, bool>? _onOutOfDate; readonly Action<string>? _log;
    Semaphore _imageAvailable; Semaphore _renderFinished; Fence _fence; Thread? _thread; bool _stop; bool _syncCreated; bool _firstPresentLogged;
    const ulong AcquireTimeoutNs = 1_000_000_000; const ulong FenceTimeoutNs = 1_000_000_000;

    public VulkanPresentLoop(Vk vk, VulkanDeviceOwner deviceOwner, VulkanSwapchainOwner swapchainOwner, VulkanClearFrameOwner clearFrame, Func<string, bool>? onOutOfDate, Action<string>? log)
    { _vk = vk; _deviceOwner = deviceOwner; _swapchainOwner = swapchainOwner; _clearFrame = clearFrame; _onOutOfDate = onOutOfDate; _log = log; }

    public void Start()
    {
        if (_thread is not null) return;
        if (!_syncCreated) CreateSync();
        _stop = false; _thread = new Thread(Run) { IsBackground = true, Name = "VulkanPresent" };
        _thread.Start();
        Log(VulkanClearFrameLogFormatter.LoopStarted());
    }

    void CreateSync()
    {
        var semInfo = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo };
        _vk.CreateSemaphore(_deviceOwner.LogicalDevice, &semInfo, null, out _imageAvailable);
        _vk.CreateSemaphore(_deviceOwner.LogicalDevice, &semInfo, null, out _renderFinished);
        var fenceInfo = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
        _vk.CreateFence(_deviceOwner.LogicalDevice, &fenceInfo, null, out _fence);
        _syncCreated = true;
    }

    void Run()
    {
        var device = _deviceOwner.LogicalDevice; var khr = _swapchainOwner.Khr;
        var stage = PipelineStageFlags.ColorAttachmentOutputBit; var imgAvail = _imageAvailable; var renderDone = _renderFinished; var fence = _fence;
        var submit = new SubmitInfo { SType = StructureType.SubmitInfo }; var present = new PresentInfoKHR { SType = StructureType.PresentInfoKhr };
        bool submitted = false;
        while (!_stop)
        {
            var swapchain = _swapchainOwner.Swapchain; uint idx;
            var res = khr.AcquireNextImage(device, swapchain, AcquireTimeoutNs, imgAvail, default, &idx);
            if (res != Result.Success && res != Result.SuboptimalKhr)
            {
                if (res == Result.ErrorOutOfDateKhr) { if (!(_onOutOfDate?.Invoke("AcquireNextImage") ?? true)) break; continue; }
                Log(VulkanClearFrameLogFormatter.PresentError($"AcquireNextImage 失败：{res}")); break;
            }
            if (submitted) { _vk.WaitForFences(device, 1, &fence, true, FenceTimeoutNs); _vk.ResetFences(device, 1, &fence); }
            var cmds = _clearFrame.CommandBuffers;
            fixed (CommandBuffer* pCmd = cmds)
            {
                submit.WaitSemaphoreCount = 1; submit.PWaitSemaphores = &imgAvail; submit.PWaitDstStageMask = &stage;
                submit.CommandBufferCount = 1; submit.PCommandBuffers = &pCmd[idx];
                submit.SignalSemaphoreCount = 1; submit.PSignalSemaphores = &renderDone;
                _vk.QueueSubmit(_deviceOwner.GraphicsQueue, 1, &submit, fence);
            }
            submitted = true;
            present.WaitSemaphoreCount = 1; present.PWaitSemaphores = &renderDone;
            present.SwapchainCount = 1; present.PSwapchains = &swapchain; present.PImageIndices = &idx;
            var pres = khr.QueuePresent(_deviceOwner.PresentQueue, &present);
            if (pres == Result.ErrorOutOfDateKhr) { if (!(_onOutOfDate?.Invoke("QueuePresent") ?? true)) break; continue; }
            if (pres != Result.Success && pres != Result.SuboptimalKhr)
            { Log(VulkanClearFrameLogFormatter.PresentError($"QueuePresent 失败：{pres}")); break; }
            if (!_firstPresentLogged) { _firstPresentLogged = true; Log(VulkanClearFrameLogFormatter.FirstPresented(idx)); }
        }
    }

    public void Stop()
    {
        var t = _thread;
        if (t is null) return;
        _stop = true; t.Join(2000); _thread = null;
        Log(VulkanClearFrameLogFormatter.LoopStopped());
    }

    public void Dispose()
    {
        Stop();
        if (_syncCreated)
        {
            _vk.DestroySemaphore(_deviceOwner.LogicalDevice, _imageAvailable, null);
            _vk.DestroySemaphore(_deviceOwner.LogicalDevice, _renderFinished, null);
            _vk.DestroyFence(_deviceOwner.LogicalDevice, _fence, null);
        }
    }
    // VK4-D-R2：后台线程日志回调异常必须被吞掉，绝不能让日志错误终止 Present 泵或炸进程。
    void Log(string m)
    {
        try { _log?.Invoke(m); }
        catch { }
    }
}
