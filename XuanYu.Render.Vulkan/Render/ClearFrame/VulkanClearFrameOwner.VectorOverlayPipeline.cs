using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    Silk.NET.Vulkan.Pipeline _vectorOverlayPipeline;
    PipelineLayout _vectorOverlayPipelineLayout;
    Silk.NET.Vulkan.Pipeline _vectorStrokePipeline;
    PipelineLayout _vectorStrokePipelineLayout;

    public void SetVectorOverlayPipelines(Silk.NET.Vulkan.Pipeline pipeline, PipelineLayout layout,
        Silk.NET.Vulkan.Pipeline strokePipeline, PipelineLayout strokeLayout)
    {
        _vectorOverlayPipeline = pipeline;
        _vectorOverlayPipelineLayout = layout;
        _vectorStrokePipeline = strokePipeline;
        _vectorStrokePipelineLayout = strokeLayout;
        if (_views.Length > 0 && !RecordCommandBuffers(_views))
            throw new InvalidOperationException("Vector Overlay 管线注入后 CommandBuffer 重录失败");
    }
}
