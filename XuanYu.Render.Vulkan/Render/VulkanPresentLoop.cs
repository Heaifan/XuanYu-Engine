using System;
using System.Threading;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Swapchain;
using XuanYu.Render.Vulkan.Render;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace XuanYu.Render.Vulkan.Render;

// VK4-D（D3）：Present 泵。独立后台线程跑 Acquire→Submit→Present 单帧闭环（clear 由 ClearFrameOwner 录制）。严禁 UI 线程等 GPU；Detach/Resize 先 Stop 再重建；单 in-flight 帧 + 单栅栏。
public sealed unsafe class VulkanPresentLoop : IDisposable
{
    readonly Vk _vk; readonly VulkanDeviceOwner _deviceOwner; readonly VulkanSwapchainOwner _swapchainOwner; readonly VulkanClearFrameOwner _clearFrame; readonly Action<string>? _log;
    Semaphore _imageAvailable; Semaphore _renderFinished; Fence _fence; Thread? _thread; bool _stop; bool _syncCreated;
    const ulong AcquireTimeoutNs = 1_000_000_000; const ulong FenceTimeoutNs = 1_000_000_000;

    public VulkanPresentLoop(Vk vk, VulkanDeviceOwner deviceOwner, VulkanSwapchainOwner swapchainOwner, VulkanClearFrameOwner clearFrame, Action<string>? log)
    { _vk = vk; _deviceOwner = deviceOwner; _swapchainOwner = swapchainOwner; _clearFrame = clearFrame; _log = log; }

    public void Start()
    {
        if (_thread is not null) return;
        if (!_syncCreated) CreateSync();
        _stop = false;
        _thread = new Thread(Run) { IsBackground = true, Name = "VulkanPresent" };
        _thread.Start();
        Log(_log, VulkanClearFrameLogFormatter.LoopStarted());
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
            if (res != Result.Success) { if (res == Result.ErrorOutOfDateKhr || res == Result.SuboptimalKhr) { Thread.Sleep(1); continue; } break; }
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
            khr.QueuePresent(_deviceOwner.PresentQueue, &present);
        }
    }

    public void Stop()
    {
        if (_thread is null) return;
        _stop = true; _thread.Join(2000); _thread = null;
        Log(_log, VulkanClearFrameLogFormatter.LoopStopped());
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
    static void Log(Action<string>? log, string m) { log?.Invoke(m); }
}
