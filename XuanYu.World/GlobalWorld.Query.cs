using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Spatial;

namespace XuanYu.World;

public sealed partial class GlobalWorld
{
    public IReadOnlyList<EntityId> QueryBounds(SpatialAabb bounds) => _query.QueryBounds(bounds);

    public IReadOnlyList<EntityId> QueryRadius(Vector3d center, double radius) =>
        _query.QueryRadius(center, radius);

    public SpatialQueryResult QuerySpatial(SpatialAabb area, SpatialQueryCategory mask) => _query.Query(area, mask);

    public SpatialQueryResult QuerySpatial(SpatialRayQuery ray, SpatialQueryCategory mask) => _query.Query(ray, mask);

    public SpatialRaycastResult RaycastSpatial(SpatialRayQuery ray, SpatialQueryCategory mask) => _query.Raycast(ray, mask);

    public long SpatialRevision => _query.SpatialRevision;
}
