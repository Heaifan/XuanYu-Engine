using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    public CommandBuffer[] CommandBuffers => _commandBuffers;
    public Extent2D Extent => _extent;
    public RenderPass RenderPass => _renderPass;
    public RenderProjection RenderProjection => _renderProjection;
    public bool HasRenderProjection => _hasRenderProjection;

    public void SetPipeline(Silk.NET.Vulkan.Pipeline pipeline, PipelineLayout layout)
    {
        _pipeline = pipeline;
        _pipelineLayout = layout;
        if (_views.Length > 0 && !RecordCommandBuffers(_views))
            throw new InvalidOperationException("Pipeline 注入后 CommandBuffer 重录失败");
    }

    public bool SetRenderProjection(RenderProjection projection)
    {
        if (_hasRenderProjection && _renderProjection == projection) return false;
        _renderProjection = projection;
        _hasRenderProjection = true;
        if (_views.Length > 0 && !RecordCommandBuffers(_views))
            throw new InvalidOperationException("Render projection 注入后 CommandBuffer 重录失败");
        return true;
    }

    public void ClearRenderProjection() => _hasRenderProjection = false;
}
