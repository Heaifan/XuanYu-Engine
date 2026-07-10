using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Swapchain;
using XuanYu.Render.Vulkan.Render;
using XuanYu.Render.Vulkan.Pipeline;

namespace XuanYu.Render.Vulkan.Session;

// VK4-D 薄组合根（装配 ClearFrame + PresentLoop + VK5-A Pipeline）。RZ-VK5-A-R2：OutOfDate 经 RecoverFromOutOfDate 自愈；_rebuildLock 防并发；连续自愈上限 5 次。
public sealed class VulkanRenderSession : IDisposable
{
    readonly VulkanDeviceOwner _deviceOwner;
    readonly VulkanSwapchainOwner _swapchainOwner;
    readonly VulkanClearFrameOwner _clearFrame;
    readonly VulkanPresentLoop _presentLoop;
    readonly VulkanGraphicsPipelineOwner? _pipeline;
    readonly Action<string>? _log;
    readonly NativeHostSurfaceHandle? _surfaceHandle;
    readonly object _rebuildLock = new();
    uint _generation; int _recoverTries;
    const int MaxRecoverTries = 5;
    bool _disposed;
    VulkanRenderSession(VulkanDeviceOwner deviceOwner, VulkanSwapchainOwner swapchainOwner,
        VulkanClearFrameOwner clearFrame, VulkanPresentLoop presentLoop, VulkanGraphicsPipelineOwner? pipeline, Action<string>? log, NativeHostSurfaceHandle? surfaceHandle)
    {
        _deviceOwner = deviceOwner; _swapchainOwner = swapchainOwner; _clearFrame = clearFrame;
        _presentLoop = presentLoop; _pipeline = pipeline; _log = log; _surfaceHandle = surfaceHandle;
    }
    public static VulkanRenderSession? Create(Vk vk, VulkanDeviceOwner? deviceOwner,
        VulkanSwapchainOwner? swapchainOwner, VulkanPhysicalDeviceSelection? selection, Action<string>? log, NativeHostSurfaceHandle? surfaceHandle = null)
    {
        if (deviceOwner is null || swapchainOwner is null || selection is null || !selection.Success || selection.Queue is null)
        { log?.Invoke(VulkanClearFrameLogFormatter.Skipped("设备/Swapchain/选择不可用")); return null; }
        try
        {
            VulkanRenderSession? session = null;
            var clear = new VulkanClearFrameOwner(vk, deviceOwner, swapchainOwner, selection.Queue!.GraphicsFamily, log);
            var pipeline = VulkanGraphicsPipelineOwner.Create(vk, deviceOwner, clear, swapchainOwner, log);
            if (pipeline is not null) clear.SetPipeline(pipeline.Pipeline);
            var loop = new VulkanPresentLoop(vk, deviceOwner, swapchainOwner, clear, source => session!.RecoverFromOutOfDate(source), log);
            session = new VulkanRenderSession(deviceOwner, swapchainOwner, clear, loop, pipeline, log, surfaceHandle);
            loop.Start();
            return session;
        }
        catch (Exception ex) { log?.Invoke(VulkanClearFrameLogFormatter.PresentError($"创建异常：{ex.Message}")); return null; }
    }
    // RZ-VK5-A-R2：Resize 走统一入口（Stop 期间重建后重启泵）。
    public void Resize(int width, int height)
    {
        if (_disposed) return;
        _presentLoop.Stop();
        lock (_rebuildLock)
        {
            _swapchainOwner.Recreate(width, height);
            _clearFrame.RebuildFramebuffers();
            _generation++;
            _log?.Invoke(VulkanClearFrameLogFormatter.Rebuilt(_swapchainOwner.Extent, (uint)_clearFrame.CommandBuffers.Length));
        }
        _presentLoop.Start();
    }
    // RZ-VK5-A-R2：OutOfDate 自愈入口（PresentLoop 线程调用）。true=继续 Present，false=放弃并暂停。
    bool RecoverFromOutOfDate(string source)
    {
        if (_disposed) return false;
        lock (_rebuildLock)
        {
            var old = _swapchainOwner.Extent;
            if (!_swapchainOwner.TryRecreateToCurrent(out _))
            {
                LogProbe(old, source);
                if (_recoverTries >= MaxRecoverTries) { _log?.Invoke(VulkanClearFrameLogFormatter.OutOfDateRecoverFailed($"连续 {MaxRecoverTries} 次重建失败")); return false; }
                _recoverTries++;
                return true;
            }
            _clearFrame.RebuildFramebuffers();
            _generation++; _recoverTries = 0;
            LogProbe(old, source);
            _log?.Invoke(VulkanClearFrameLogFormatter.OutOfDateRecovered(_generation));
            return true;
        }
    }
    void LogProbe(Extent2D old, string source)
    {
        var ne = _swapchainOwner.Extent; var dpi = _surfaceHandle?.DpiScale ?? 1.0;
        _log?.Invoke(VulkanClearFrameLogFormatter.OutOfDateProbe(source, old, ne, _generation, dpi));
    }
    public void Dispose()
    {
        if (_disposed) return; _disposed = true;
        _presentLoop.Stop();
        _presentLoop.Dispose();
        _pipeline?.Dispose();
        _clearFrame.Dispose();
        _log?.Invoke(VulkanClearFrameLogFormatter.Disposed());
    }
}
