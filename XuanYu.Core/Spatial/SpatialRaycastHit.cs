using XuanYu.Core.Identity;
using XuanYu.Core.Math;

namespace XuanYu.Core.Spatial;

public readonly record struct SpatialRaycastHit(
    EntityId EntityKey,
    double HitDistance,
    Vector3d HitPoint,
    long SpatialRevision)
{
    public bool IsHit => EntityKey.IsValid;
}
