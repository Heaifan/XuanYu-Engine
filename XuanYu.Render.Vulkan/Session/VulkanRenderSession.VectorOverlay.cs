using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Pipeline;

namespace XuanYu.Render.Vulkan.Session;

public sealed partial class VulkanRenderSession
{
    VulkanGraphicsPipelineOwner? _vectorOverlayPipeline;

    void AttachVectorOverlayPipeline(Vk vk)
    {
        var pipeline = VulkanGraphicsPipelineOwner.Create(vk, _deviceOwner, _clearFrame,
            _swapchainOwner, _log, depthTest: false, depthWrite: false);
        if (pipeline is null) return;
        try
        {
            _clearFrame.SetVectorOverlayPipeline(pipeline.Pipeline, pipeline.Layout);
            _vectorOverlayPipeline = pipeline;
        }
        catch
        {
            pipeline.Dispose();
            throw;
        }
    }
}
