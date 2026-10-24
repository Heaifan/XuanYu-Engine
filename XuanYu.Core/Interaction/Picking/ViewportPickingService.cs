using XuanYu.Core.Space;
using XuanYu.Core.Spatial;

namespace XuanYu.Core.Picking;

public static class ViewportPickingService
{
    public static ViewportPickingResult Pick(
        ViewportPickingRequest request,
        Func<SpatialRayQuery, SpatialQueryCategory, SpatialRaycastResult> raycast,
        Func<long> currentViewportRevision,
        Func<long> currentSpatialRevision)
    {
        ArgumentNullException.ThrowIfNull(raycast);
        ArgumentNullException.ThrowIfNull(currentViewportRevision);
        ArgumentNullException.ThrowIfNull(currentSpatialRevision);
        EnsureCurrent(request, currentViewportRevision, currentSpatialRevision);

        var viewProjection = ViewProjectionState.Create(request.Camera, request.Viewport);
        var worldRay = WorldRayFactory.FromViewportPoint(viewProjection, request.LogicalX, request.LogicalY);
        var query = new SpatialRayQuery(worldRay, request.Camera.FarPlane);
        var result = raycast(query, request.Mask);

        EnsureCurrent(request, currentViewportRevision, currentSpatialRevision);
        if (result.Stats.SpatialRevision != request.SpatialRevision)
        {
            throw new InvalidOperationException("Picking 空间代际已变化。");
        }

        return new ViewportPickingResult(
            request.RequestSequence,
            request.Viewport.Revision,
            request.SpatialRevision,
            result);
    }

    static void EnsureCurrent(
        ViewportPickingRequest request,
        Func<long> currentViewportRevision,
        Func<long> currentSpatialRevision)
    {
        if (currentViewportRevision() != request.Viewport.Revision)
        {
            throw new InvalidOperationException("Picking 视口代际已变化。");
        }

        if (currentSpatialRevision() != request.SpatialRevision)
        {
            throw new InvalidOperationException("Picking 空间代际已变化。");
        }
    }
}
