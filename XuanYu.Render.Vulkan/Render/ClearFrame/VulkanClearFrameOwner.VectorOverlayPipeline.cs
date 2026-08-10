using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    Silk.NET.Vulkan.Pipeline _vectorOverlayPipeline;
    PipelineLayout _vectorOverlayPipelineLayout;

    public void SetVectorOverlayPipeline(Silk.NET.Vulkan.Pipeline pipeline, PipelineLayout layout)
    {
        _vectorOverlayPipeline = pipeline;
        _vectorOverlayPipelineLayout = layout;
        if (_views.Length > 0 && !RecordCommandBuffers(_views))
            throw new InvalidOperationException("Vector Overlay 管线注入后 CommandBuffer 重录失败");
    }
}
