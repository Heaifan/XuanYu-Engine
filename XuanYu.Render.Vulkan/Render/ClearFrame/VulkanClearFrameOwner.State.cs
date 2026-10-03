using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    public CommandBuffer[] CommandBuffers => _commandBuffers;
    public Extent2D Extent => _extent;
    public RenderPass RenderPass => _renderPass;
    public RenderProjection RenderProjection => _frameState.Projection;
    public bool HasRenderProjection => _frameState.HasProjection;
    internal VulkanFrameState FrameState => _frameState;
    internal VulkanGpuResourceState GpuResourceState => _gpuResourceState;

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
        _frameState.Apply(projection);
        if (_views.Length > 0 && !RecordCommandBuffers(_views))
            throw new InvalidOperationException("Render projection 注入后 CommandBuffer 重录失败");
        return true;
    }

    public void ClearRenderProjection() => _frameState.Clear();
}
