using XuanYu.Core.Identity;

namespace XuanYu.Core.Spatial;

public interface ISpatialIndex
{
    int Count { get; }

    void Insert(SpatialBounds bounds);

    bool Remove(EntityId entityKey);

    bool Update(SpatialBounds bounds);

    SpatialQueryResult Query(SpatialAabb area, SpatialQueryCategory mask);
}
