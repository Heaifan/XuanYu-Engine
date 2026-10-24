using XuanYu.Core.Identity;
using XuanYu.Core.Spatial;

namespace XuanYu.Core.Picking;

public readonly record struct ViewportPickingResult(
    long RequestSequence,
    long ViewportRevision,
    long SpatialRevision,
    SpatialRaycastResult Raycast)
{
    public bool HasHit => Raycast.HasHit;

    public EntityId? EntityKey => Raycast.Hit?.EntityKey;
}
