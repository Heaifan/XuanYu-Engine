using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Swapchain;
using XuanYu.Render.Vulkan.Render;
using XuanYu.Render.Vulkan.Pipeline;
using XuanYu.Render.Vulkan.Diagnostic;

namespace XuanYu.Render.Vulkan.Session;

// VK4-D 薄组合根。RZ-VK5-A-R2：OutOfDate 自愈。RZ-VK5-D-R1：全链路诊断追踪（T+elapsedMs）。
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
    uint _generation; int _recoverTries; bool _disposed;
    const int MaxRecoverTries = 5;
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
    // RZ-VK5-A-R2：Resize 走统一入口（Stop 期间重建后重启泵）。RZ-VK5-D-R1：T+0 起点追踪。
    public void Resize(int width, int height)
    {
        if (_disposed) return;
        VulkanResizeTracer.StartTrace();
        _log?.Invoke(VulkanResizeTracer.Stage(_generation, "Resize开始", $"请求逻辑尺寸={width}x{height}"));
        _presentLoop.Stop();
        lock (_rebuildLock)
        {
            _swapchainOwner.Recreate(width, height);
            _clearFrame.RebuildFramebuffers();
            _generation++;
            _log?.Invoke(VulkanResizeTracer.Stage(_generation, "Resize完成", $"{_swapchainOwner.Extent.Width}x{_swapchainOwner.Extent.Height}；{_clearFrame.CommandBuffers.Length} 张 CB"));
        }
        _presentLoop.Start();
    }
    // RZ-VK5-A-R2 + R1：自愈入口，带 T+ 追踪。
    bool RecoverFromOutOfDate(string source)
    {
        if (_disposed) return false;
        lock (_rebuildLock)
        {
            var old = _swapchainOwner.Extent;
            _log?.Invoke(VulkanResizeTracer.HealStage(_generation, source, $"{old.Width}x{old.Height}", "查询中..."));
            if (!_swapchainOwner.TryRecreateToCurrent(out _))
            {
                var ne2 = _swapchainOwner.Extent; var dpi = _surfaceHandle?.DpiScale ?? 1.0;
                _log?.Invoke(VulkanClearFrameLogFormatter.OutOfDateProbe(source, old, ne2, _generation, dpi));
                if (_recoverTries >= MaxRecoverTries) { _log?.Invoke(VulkanClearFrameLogFormatter.OutOfDateRecoverFailed($"连续 {MaxRecoverTries} 次重建失败")); return false; }
                _recoverTries++;
                return true;
            }
            _clearFrame.RebuildFramebuffers();
            _generation++; _recoverTries = 0;
            var ne = _swapchainOwner.Extent;
            _log?.Invoke(VulkanResizeTracer.HealStage(_generation, source, $"{old.Width}x{old.Height}", $"{ne.Width}x{ne.Height}", "已恢复 Present"));
            return true;
        }
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
