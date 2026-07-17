using System;
using System.Threading;
using Silk.NET.Vulkan;
using XuanYu.Core.Scene;
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
    int _failed;
    bool _resizeStopping;
    string? _failureReason;
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
        Action<string>? log, NativeHostSurfaceHandle? surfaceHandle = null, SceneRenderSnapshot? scene = null)
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
            clear.SetSceneSnapshot(scene ?? SceneRenderSnapshot.TestEntityAtOrigin);
            pipeline = VulkanGraphicsPipelineOwner.Create(vk, deviceOwner, clear, swapchainOwner, log);
            if (pipeline is not null) clear.SetPipeline(pipeline.Pipeline);
            loop = new VulkanPresentLoop(vk, deviceOwner, swapchainOwner, clear,
                source => session!.RecoverFromOutOfDate(source),
                reason => session!.MarkFailed(reason),
                log);
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

    public bool IsFailed => Volatile.Read(ref _failed) != 0;
    public string? FailureReason => Volatile.Read(ref _failureReason);

}
