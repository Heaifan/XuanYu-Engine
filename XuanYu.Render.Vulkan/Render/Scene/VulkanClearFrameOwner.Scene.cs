using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    readonly LatestRenderProjectionQueue _projectionQueue = new();

    public void QueueRenderProjection(RenderProjectionResult projection)
    {
        if (projection.Success && _frameState.HasProjection && _frameState.Projection == projection.Projection) return;
        _projectionQueue.Publish(projection);
    }

    public bool TryApplyPendingRenderProjection()
    {
        RenderProjectionResult projection;
        if (!_projectionQueue.TryConsume(out projection)) return true;
        if (!projection.Success)
        {
            ClearRenderProjection();
            Log(VulkanClearFrameLogFormatter.RenderProjectionSkipped(
                projection.FailureReason ?? "未知原因"));
            return true;
        }
        var previous = _frameState.HasProjection ? _frameState.Projection : (RenderProjection?)null;
        _frameState.Apply(projection.Projection);
        _gpuResourceState.Apply(VulkanRenderChangeConsumer.Compare(previous, projection.Projection));
        SetMapSurface(projection.Projection.Map);
        // F2-R2：每帧全局网格尺度（视口中心射线求交，1/2/5 层级），求交失败沿用上一帧。
        UpdateReferenceGridScale(projection.Projection);
        var recorded = _views.Length == 0 || RecordCommandBuffers(_views);
        if (recorded)
        {
            _gpuResourceState.MarkUploadsConsumed();
            _gpuResourceState.MarkFrameRecorded();
            _frameState.MarkRecorded();
        }
        return recorded;
    }
}
