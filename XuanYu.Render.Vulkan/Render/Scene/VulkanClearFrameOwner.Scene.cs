using XuanYu.Render.Abstractions;
using XuanYu.Core.Diagnostics;

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
        ViewportProbe.BeginFrame();
        var previous = _frameState.HasProjection ? _frameState.Projection : (RenderProjection?)null;
        _frameState.Apply(projection.Projection);
        var renderState = CurrentViewProjectionState();
        ViewportProbe.CameraSnapshot("T2_RENDER_BEFORE", renderState.Camera,
            default, renderState);
        _gpuResourceState.Apply(VulkanRenderChangeConsumer.Compare(previous, projection.Projection));
        SetMapSurface(projection.Projection.Map);
        UpdateMapSurfacePatch();
        // F2-R2：每帧全局网格尺度（视口中心射线求交，2 倍嵌套层级），求交失败沿用上一帧。
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
