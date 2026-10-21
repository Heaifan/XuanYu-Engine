using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Pipeline;

namespace XuanYu.Render.Vulkan.Session;

public sealed partial class VulkanRenderSession
{
    VulkanGraphicsPipelineOwner? _mapLabelPipeline;
    VulkanGraphicsPipelineOwner? _terrainPipeline;
    VulkanGraphicsPipelineOwner? _vectorOverlayPipeline;
    VulkanGraphicsPipelineOwner? _vectorStrokePipeline;

    void AttachMapLabelPipeline(Vk vk)
    {
        var layout = _clearFrame.MapLabelDescriptorSetLayout;
        if (layout.Handle == 0) return;
        var pipeline = VulkanGraphicsPipelineOwner.CreateMapLabel(vk, _deviceOwner, _clearFrame,
            _swapchainOwner, layout, _log);
        if (pipeline is null) return;
        try
        {
            _clearFrame.SetMapLabelPipeline(pipeline.Pipeline, pipeline.Layout);
            _mapLabelPipeline = pipeline;
        }
        catch { pipeline.Dispose(); throw; }
    }

    void AttachTerrainPipeline(Vk vk)
    {
        var pipeline = VulkanGraphicsPipelineOwner.CreateTerrain(vk, _deviceOwner, _clearFrame, _log);
        TryTransferTerrainPipeline(pipeline,
            value => _clearFrame.SetTerrainPipeline(value.Pipeline, value.Layout),
            value => _terrainPipeline = value, value => value.Dispose());
    }

    internal static bool TryTransferTerrainPipeline<T>(T? pipeline, Action<T> inject,
        Action<T> transfer, Action<T> dispose) where T : class
    {
        if (pipeline is null) return false;
        var sessionOwns = false;
        try
        {
            inject(pipeline);
            transfer(pipeline);
            sessionOwns = true;
            return true;
        }
        finally { if (!sessionOwns) dispose(pipeline); }
    }

    void AttachVectorOverlayPipeline(Vk vk)
    {
        var pipeline = VulkanGraphicsPipelineOwner.Create(vk, _deviceOwner, _clearFrame,
            _swapchainOwner, _log, depthTest: false, depthWrite: false);
        var stroke = VulkanGraphicsPipelineOwner.Create(vk, _deviceOwner, _clearFrame,
            _swapchainOwner, _log, depthTest: false, depthWrite: false, analyticStroke: true);
        if (pipeline is null || stroke is null)
        { pipeline?.Dispose(); stroke?.Dispose(); return; }
        try
        {
            _clearFrame.SetVectorOverlayPipelines(pipeline.Pipeline, pipeline.Layout,
                stroke.Pipeline, stroke.Layout);
            _vectorOverlayPipeline = pipeline;
            _vectorStrokePipeline = stroke;
        }
        catch { pipeline.Dispose(); stroke.Dispose(); throw; }
    }
}
