using XuanYu.Core.Identity;

using XuanYu.Core.Spatial;
namespace XuanYu.World.Spatial;

public interface ISpatialIndex
{
    int Count { get; }

    void Insert(SpatialBounds bounds);

    bool Remove(EntityId entityKey);

    bool Update(SpatialBounds bounds);

    SpatialQueryResult Query(SpatialAabb area, SpatialQueryCategory mask);

    SpatialQueryResult Query(SpatialRayQuery ray, SpatialQueryCategory mask);
}
