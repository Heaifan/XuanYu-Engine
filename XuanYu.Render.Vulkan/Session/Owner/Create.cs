using System;
using System.Threading;
using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Pipeline;
using XuanYu.Render.Vulkan.Pipeline.Sets;
using XuanYu.Render.Vulkan.Render;
using XuanYu.Render.Vulkan.Swapchain;
using XuanYu.Render.Vulkan.Render.Frame;
using XuanYu.Render.Vulkan.Diagnostics.Performance;
using XuanYu.Render.Abstractions.Diagnostics.Performance;
namespace XuanYu.Render.Vulkan.Session;

public sealed partial class VulkanRenderSession
{
    public static VulkanRenderSession? Create(Vk vk, VulkanDeviceOwner? deviceOwner,
        VulkanSwapchainOwner? swapchainOwner, VulkanPhysicalDeviceSelection? selection,
        Action<string>? log, NativeHostSurfaceHandle? surfaceHandle = null,
        RenderProjectionResult? projection = null, int framesInFlight = 1,
        Action<VulkanFenceWaitObservation>? onFenceWait = null)
    {
        if (deviceOwner is null || swapchainOwner is null || selection?.Queue is null || !selection.Success)
        {
            log?.Invoke(VulkanClearFrameLogFormatter.Skipped("设备/Swapchain/选择不可用"));
            return null;
        }
        VulkanClearFrameOwner? clear = null;
        VulkanGraphicsPipelineOwner? pipeline = null;
        VulkanGraphicsPipelineOwner? skyPipeline = null;
        VulkanGridAndOverlayPipelineSet? gridPipelines = null;
        VulkanPresentLoop? loop = null;
        try
        {
            VulkanRenderSession? session = null;
            var performanceSamples = swapchainOwner.EventSession is { ProbeEnabled: true } events
                ? new VulkanFenceWaitSampleSink(events.SessionId) : null;
            Action<VulkanFenceWaitObservation>? fenceWaitCallback =
                performanceSamples is null && onFenceWait is null ? null : observation =>
                {
                    performanceSamples?.Record(observation);
                    onFenceWait?.Invoke(observation);
                };
            clear = new VulkanClearFrameOwner(vk, deviceOwner, swapchainOwner,
                selection.Queue.GraphicsFamily, log, framesInFlight);
            if (projection is { Success: true } ok) clear.SetRenderProjection(ok.Projection);
            else if (projection is { Success: false } fail)
                log?.Invoke(VulkanClearFrameLogFormatter.RenderProjectionSkipped(
                    fail.FailureReason ?? "未知原因"));
            pipeline = VulkanScenePipelineFactory.Create(vk, deviceOwner, clear, log);
            if (pipeline is not null) clear.SetPipeline(pipeline.Pipeline, pipeline.Layout);
            skyPipeline = VulkanSkyPipelineFactory.Create(vk, deviceOwner, clear, log);
            if (skyPipeline is not null) clear.SetSkyPipeline(skyPipeline.Pipeline, skyPipeline.Layout);
            // F2-R2：网格/世界轴/世界原点三个全屏 Pass（设备不支持则禁用对应 Pass）。
            gridPipelines = VulkanGridAndOverlayPipelineSet.Create(vk, deviceOwner, clear, selection.Handle, log);
            loop = new VulkanPresentLoop(vk, deviceOwner, swapchainOwner, clear,
                source => session!.RecoverFromOutOfDate(source),
                reason => session!.MarkFailed(reason),
                log, framesInFlight, fenceWaitCallback);
            session = new VulkanRenderSession(deviceOwner, swapchainOwner, clear, loop, pipeline,
                skyPipeline, gridPipelines, log, surfaceHandle, performanceSamples);
            loop.AttachSwapchainEvents();
            session.AttachVectorOverlayPipeline(vk);
            session.AttachMapLabelPipeline(vk);
            session.AttachTerrainPipeline(vk);
            if (!loop.Start()) throw new InvalidOperationException("Present 泵启动失败");
            return session;
        }
        catch (Exception ex)
        {
            loop?.Dispose();
            gridPipelines?.Dispose();
            skyPipeline?.Dispose();
            pipeline?.Dispose();
            clear?.Dispose();
            log?.Invoke(VulkanClearFrameLogFormatter.PresentError($"创建异常：{ex.Message}"));
            return null;
        }
    }
}
