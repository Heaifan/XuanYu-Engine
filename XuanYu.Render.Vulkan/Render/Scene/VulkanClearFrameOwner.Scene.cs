using XuanYu.Render.Abstractions;
using XuanYu.Core.Diagnostics;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    readonly LatestRenderProjectionQueue _projectionQueue = new();

    public void QueueRenderProjection(RenderProjectionResult projection)
    {
        _projectionQueue.Publish(projection);
    }

    public bool TryApplyPendingRenderProjection()
    {
        RenderProjectionResult projection;
        if (!_projectionQueue.TryConsume(out projection)) return true;
        if (projection.Success && _frameState.HasProjection &&
            _frameState.Projection == projection.Projection) return true;
        if (!projection.Success)
        {
            ClearRenderProjection();
            Log(VulkanClearFrameLogFormatter.RenderProjectionSkipped(
                projection.FailureReason ?? "未知原因"));
            return true;
        }
        ViewportProbe.BeginFrame();
        _frameState.Apply(projection.Projection);
        var renderState = CurrentViewProjectionState();
        ViewportProbe.CameraSnapshot("T2_RENDER_BEFORE", renderState.Camera,
            default, renderState);
        SetMapSurface(projection.Projection.Map);
        // F2-R2：每帧全局网格尺度（视口中心射线求交，2 倍嵌套层级），求交失败沿用上一帧。
        UpdateReferenceGridScale(projection.Projection);
        // Presentation records only the selected (FrameIndex, ImageIndex) buffer after
        // that frame slot and acquired image have both passed their completion proofs.
        return true;
    }

    public void SetMapSurface(MapRenderSnapshot map) =>
        _mapSurface.SetMapSurface(map, _renderProjection.TerrainResources.Count != 0);

    public void ClearMapSurface() => _mapSurface.ClearMapSurface();
}
