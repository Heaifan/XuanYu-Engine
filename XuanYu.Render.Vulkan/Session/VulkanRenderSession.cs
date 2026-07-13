using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Diagnostic;
using XuanYu.Render.Vulkan.Pipeline;
using XuanYu.Render.Vulkan.Render;
using XuanYu.Render.Vulkan.Swapchain;

namespace XuanYu.Render.Vulkan.Session;

// VK-LIFE-1：组合根负责失败回滚，不把半初始化资源留给 Bridge。
public sealed partial class VulkanRenderSession : IDisposable
{
    readonly VulkanDeviceOwner _deviceOwner;
    readonly VulkanSwapchainOwner _swapchainOwner;
    readonly VulkanClearFrameOwner _clearFrame;
    readonly VulkanPresentLoop _presentLoop;
    readonly VulkanGraphicsPipelineOwner? _pipeline;
    readonly Action<string>? _log;
    readonly NativeHostSurfaceHandle? _surfaceHandle;
    readonly object _rebuildLock = new();
    uint _generation;
    int _recoverTries;
    bool _disposed;
    const int MaxRecoverTries = 5;

    VulkanRenderSession(VulkanDeviceOwner deviceOwner, VulkanSwapchainOwner swapchainOwner,
        VulkanClearFrameOwner clearFrame, VulkanPresentLoop presentLoop,
        VulkanGraphicsPipelineOwner? pipeline, Action<string>? log, NativeHostSurfaceHandle? surfaceHandle)
    {
        _deviceOwner = deviceOwner;
        _swapchainOwner = swapchainOwner;
        _clearFrame = clearFrame;
        _presentLoop = presentLoop;
        _pipeline = pipeline;
        _log = log;
        _surfaceHandle = surfaceHandle;
    }

    public static VulkanRenderSession? Create(Vk vk, VulkanDeviceOwner? deviceOwner,
        VulkanSwapchainOwner? swapchainOwner, VulkanPhysicalDeviceSelection? selection,
        Action<string>? log, NativeHostSurfaceHandle? surfaceHandle = null)
    {
        if (deviceOwner is null || swapchainOwner is null || selection?.Queue is null || !selection.Success)
        {
            log?.Invoke(VulkanClearFrameLogFormatter.Skipped("设备/Swapchain/选择不可用"));
            return null;
        }
        VulkanClearFrameOwner? clear = null;
        VulkanGraphicsPipelineOwner? pipeline = null;
        VulkanPresentLoop? loop = null;
        try
        {
            VulkanRenderSession? session = null;
            clear = new VulkanClearFrameOwner(vk, deviceOwner, swapchainOwner, selection.Queue.GraphicsFamily, log);
            pipeline = VulkanGraphicsPipelineOwner.Create(vk, deviceOwner, clear, swapchainOwner, log);
            if (pipeline is not null) clear.SetPipeline(pipeline.Pipeline);
            loop = new VulkanPresentLoop(vk, deviceOwner, swapchainOwner, clear, source => session!.RecoverFromOutOfDate(source), log);
            session = new VulkanRenderSession(deviceOwner, swapchainOwner, clear, loop, pipeline, log, surfaceHandle);
            if (!loop.Start()) throw new InvalidOperationException("Present 泵启动失败");
            return session;
        }
        catch (Exception ex)
        {
            loop?.Dispose();
            pipeline?.Dispose();
            clear?.Dispose();
            log?.Invoke(VulkanClearFrameLogFormatter.PresentError($"创建异常：{ex.Message}"));
            return null;
        }
    }

    public bool Resize(int width, int height)
    {
        if (_disposed) return false;
        if (_swapchainOwner.Extent.Width == (uint)width && _swapchainOwner.Extent.Height == (uint)height)
        {
            _log?.Invoke(VulkanClearFrameLogFormatter.ResizeFastSkipped(_generation, width, height));
            return true;
        }
        VulkanResizeTracer.StartTrace();
        _log?.Invoke(VulkanResizeTracer.Stage(_generation, "Resize开始", $"请求逻辑尺寸={width}x{height}"));
        if (!_presentLoop.Stop()) return false;
        lock (_rebuildLock)
        {
            if (!_swapchainOwner.Recreate(width, height, _generation)) return FailResize("Swapchain 重建失败");
            if (!_clearFrame.RebuildFramebuffers(_generation)) return FailResize("Framebuffer 重建失败");
            _generation++;
            _log?.Invoke(VulkanResizeTracer.Stage(_generation, "Resize完成", $"{_swapchainOwner.Extent.Width}x{_swapchainOwner.Extent.Height}；{_clearFrame.CommandBuffers.Length} 张 CB"));
        }
        if (!_presentLoop.Start()) return FailResize("Present 泵重启失败");
        return true;
    }

}
