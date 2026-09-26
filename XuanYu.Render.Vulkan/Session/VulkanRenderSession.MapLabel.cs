using XuanYu.Render.Vulkan.Pipeline;
using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Session;

public sealed partial class VulkanRenderSession
{
    VulkanGraphicsPipelineOwner? _mapLabelPipeline;

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
}
