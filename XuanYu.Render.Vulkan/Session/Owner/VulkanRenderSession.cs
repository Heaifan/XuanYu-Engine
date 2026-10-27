using System;
using System.Threading;
using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Diagnostic;
using XuanYu.Render.Vulkan.Pipeline;
using XuanYu.Render.Vulkan.Pipeline.Sets;
using XuanYu.Render.Vulkan.Render;
using XuanYu.Render.Vulkan.Swapchain;
using XuanYu.Render.Vulkan.Render.Frame;
using XuanYu.Render.Vulkan.Diagnostics.Performance;
using XuanYu.Render.Abstractions.Diagnostics.Performance;
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
    readonly VulkanGridAndOverlayPipelineSet? _gridPipelines; // 全屏网格与编辑器 Overlay 管线组合
    readonly Action<string>? _log;
    readonly VulkanFenceWaitSampleSink? _performanceSamples;
    readonly NativeHostSurfaceHandle? _surfaceHandle;
    readonly object _rebuildLock = new();
    uint _generation;
    int _recoverTries;
    bool _disposed;
    bool _disposeBlocked;
    int _failed;
    bool _resizeStopping;
    string? _failureReason;
    const int MaxRecoverTries = VulkanAcquireResultPolicy.MaxRecoveryRetries;
    VulkanRenderSession(VulkanDeviceOwner deviceOwner, VulkanSwapchainOwner swapchainOwner,
        VulkanClearFrameOwner clearFrame, VulkanPresentLoop presentLoop,
        VulkanGraphicsPipelineOwner? pipeline, VulkanGraphicsPipelineOwner? skyPipeline,
        VulkanGridAndOverlayPipelineSet? gridPipelines,
        Action<string>? log, NativeHostSurfaceHandle? surfaceHandle,
        VulkanFenceWaitSampleSink? performanceSamples)
    {
        _deviceOwner = deviceOwner;
        _swapchainOwner = swapchainOwner;
        _clearFrame = clearFrame;
        _presentLoop = presentLoop;
        _pipeline = pipeline;
        _skyPipeline = skyPipeline;
        _gridPipelines = gridPipelines;
        _log = log;
        _surfaceHandle = surfaceHandle;
        _performanceSamples = performanceSamples;
    }

    public bool IsFailed => Volatile.Read(ref _failed) != 0;
    public string? FailureReason => Volatile.Read(ref _failureReason);
    public long PerformanceSampleDroppedCount => _performanceSamples?.DroppedCount ?? 0;
    public IReadOnlyList<PerformanceFrameSample> DrainPerformanceSamples() =>
        _performanceSamples?.Drain() ?? Array.Empty<PerformanceFrameSample>();

    public bool TrySetFramesInFlight(int frameCount)
    {
        if (frameCount is < 1 or > 2) return false;
        lock (_rebuildLock)
        {
            if (_disposed || IsFailed) return false;
            if (_resizeStopping) return false;
            _resizeStopping = true;
        }
        var changed = false;
        try { changed = _presentLoop.TrySetFramesInFlight(frameCount); }
        catch (Exception ex)
        {
            _log?.Invoke($"FramesInFlight 切换异常：{ex.Message}");
        }
        finally { lock (_rebuildLock) _resizeStopping = false; }
        if (!changed) MarkFailed($"FramesInFlight 切换失败：count={frameCount}");
        return changed;
    }
}
