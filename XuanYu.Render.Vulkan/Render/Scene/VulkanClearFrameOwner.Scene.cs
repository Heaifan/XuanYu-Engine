using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    readonly LatestRenderProjectionQueue _projectionQueue = new();

    public void QueueRenderProjection(RenderProjectionResult projection)
    {
        if (projection.Success && _hasRenderProjection && _renderProjection == projection.Projection) return;
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
        _renderProjection = projection.Projection;
        _hasRenderProjection = true;
        SetMapSurface(projection.Projection.Map);
        // F2-R2：每帧全局网格尺度（视口中心射线求交，1/2/5 层级），求交失败沿用上一帧。
        UpdateReferenceGridScale(projection.Projection);
        return _views.Length == 0 || RecordCommandBuffers(_views);
    }
}
