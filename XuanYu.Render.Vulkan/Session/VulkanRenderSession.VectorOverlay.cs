using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Pipeline;

namespace XuanYu.Render.Vulkan.Session;

public sealed partial class VulkanRenderSession
{
    VulkanGraphicsPipelineOwner? _vectorOverlayPipeline;
    VulkanGraphicsPipelineOwner? _vectorStrokePipeline;

    void AttachVectorOverlayPipeline(Vk vk)
    {
        var pipeline = VulkanGraphicsPipelineOwner.Create(vk, _deviceOwner, _clearFrame,
            _swapchainOwner, _log, depthTest: false, depthWrite: false);
        var stroke = VulkanGraphicsPipelineOwner.Create(vk, _deviceOwner, _clearFrame,
            _swapchainOwner, _log, depthTest: false, depthWrite: false, analyticStroke: true);
        if (pipeline is null || stroke is null)
        {
            pipeline?.Dispose(); stroke?.Dispose(); return;
        }
        try
        {
            _clearFrame.SetVectorOverlayPipelines(pipeline.Pipeline, pipeline.Layout,
                stroke.Pipeline, stroke.Layout);
            _vectorOverlayPipeline = pipeline;
            _vectorStrokePipeline = stroke;
        }
        catch
        {
            pipeline.Dispose(); stroke.Dispose();
            throw;
        }
    }
}
