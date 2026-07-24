using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Spatial;

namespace XuanYu.World;

public sealed partial class GlobalWorld
{
    public IReadOnlyList<EntityId> QueryBounds(SpatialAabb bounds) => _query.QueryBounds(bounds);

    public IReadOnlyList<EntityId> QueryRadius(Vector3d center, double radius) =>
        _query.QueryRadius(center, radius);
}
