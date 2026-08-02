using System;
using System.Threading;
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
    readonly VulkanGraphicsPipelineOwner? _skyPipeline;
    readonly Action<string>? _log;
    readonly NativeHostSurfaceHandle? _surfaceHandle;
    readonly object _rebuildLock = new();
    uint _generation;
    int _recoverTries;
    bool _disposed;
    int _failed;
    bool _resizeStopping;
    string? _failureReason;
    const int MaxRecoverTries = 5;

    VulkanRenderSession(VulkanDeviceOwner deviceOwner, VulkanSwapchainOwner swapchainOwner,
        VulkanClearFrameOwner clearFrame, VulkanPresentLoop presentLoop,
        VulkanGraphicsPipelineOwner? pipeline, VulkanGraphicsPipelineOwner? skyPipeline,
        Action<string>? log, NativeHostSurfaceHandle? surfaceHandle)
    {
        _deviceOwner = deviceOwner;
        _swapchainOwner = swapchainOwner;
        _clearFrame = clearFrame;
        _presentLoop = presentLoop;
        _pipeline = pipeline;
        _skyPipeline = skyPipeline;
        _log = log;
        _surfaceHandle = surfaceHandle;
    }

    public static VulkanRenderSession? Create(Vk vk, VulkanDeviceOwner? deviceOwner,
        VulkanSwapchainOwner? swapchainOwner, VulkanPhysicalDeviceSelection? selection,
        Action<string>? log, NativeHostSurfaceHandle? surfaceHandle = null, RenderProjectionResult? projection = null)
    {
        if (deviceOwner is null || swapchainOwner is null || selection?.Queue is null || !selection.Success)
        {
            log?.Invoke(VulkanClearFrameLogFormatter.Skipped("设备/Swapchain/选择不可用"));
            return null;
        }
        VulkanClearFrameOwner? clear = null;
        VulkanGraphicsPipelineOwner? pipeline = null;
        VulkanGraphicsPipelineOwner? skyPipeline = null;
        VulkanGraphicsPipelineOwner? gridPipeline = null;
        VulkanPresentLoop? loop = null;
        try
        {
            VulkanRenderSession? session = null;
            clear = new VulkanClearFrameOwner(vk, deviceOwner, swapchainOwner, selection.Queue.GraphicsFamily, log);
            if (projection is { Success: true } ok) clear.SetRenderProjection(ok.Projection);
            else if (projection is { Success: false } fail)
                log?.Invoke(VulkanClearFrameLogFormatter.RenderProjectionSkipped(
                    fail.FailureReason ?? "未知原因"));
            pipeline = VulkanGraphicsPipelineOwner.Create(vk, deviceOwner, clear, swapchainOwner, log);
            if (pipeline is not null) clear.SetPipeline(pipeline.Pipeline, pipeline.Layout);
            skyPipeline = VulkanGraphicsPipelineOwner.CreateSky(vk, deviceOwner, clear, swapchainOwner, log);
            if (skyPipeline is not null) clear.SetSkyPipeline(skyPipeline.Pipeline, skyPipeline.Layout);
            // F2：独立参考网格管线（192B 独立 PushConstant；设备不支持则明确日志并禁用网格）。
            gridPipeline = VulkanGraphicsPipelineOwner.CreateReferenceGrid(vk, deviceOwner, clear, swapchainOwner, selection.Handle, log);
            if (gridPipeline is not null) clear.SetReferenceGridPipeline(gridPipeline.Pipeline, gridPipeline.Layout);
            loop = new VulkanPresentLoop(vk, deviceOwner, swapchainOwner, clear,
                source => session!.RecoverFromOutOfDate(source),
                reason => session!.MarkFailed(reason),
                log);
            session = new VulkanRenderSession(deviceOwner, swapchainOwner, clear, loop, pipeline, skyPipeline, log, surfaceHandle);
            if (!loop.Start()) throw new InvalidOperationException("Present 泵启动失败");
            return session;
        }
        catch (Exception ex)
        {
            loop?.Dispose();
            gridPipeline?.Dispose();
            skyPipeline?.Dispose();
            pipeline?.Dispose();
            clear?.Dispose();
            log?.Invoke(VulkanClearFrameLogFormatter.PresentError($"创建异常：{ex.Message}"));
            return null;
        }
    }

    public bool IsFailed => Volatile.Read(ref _failed) != 0;
    public string? FailureReason => Volatile.Read(ref _failureReason);

}
