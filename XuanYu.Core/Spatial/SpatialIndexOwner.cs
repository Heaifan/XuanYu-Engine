using XuanYu.Core.Identity;

namespace XuanYu.Core.Spatial;

public sealed class SpatialIndexOwner
{
    readonly ISpatialIndex _index;
    long _revision;

    public SpatialIndexOwner()
        : this(new DynamicAabbTree())
    {
    }

    public SpatialIndexOwner(ISpatialIndex index)
    {
        _index = index;
    }

    public long SpatialRevision => _revision;

    public int EntityCount => _index.Count;

    public void Insert(SpatialBounds bounds)
    {
        _index.Insert(bounds);
        _revision++;
    }

    public bool Remove(EntityId entityKey)
    {
        if (!_index.Remove(entityKey)) return false;
        _revision++;
        return true;
    }

    public bool Update(SpatialBounds bounds)
    {
        if (!_index.Update(bounds)) return false;
        _revision++;
        return true;
    }

    public SpatialQueryResult Query(SpatialAabb area, SpatialQueryCategory mask)
    {
        var result = _index.Query(area, mask);
        return WithCurrentStats(result);
    }

    public SpatialQueryResult Query(SpatialRayQuery ray, SpatialQueryCategory mask)
    {
        var result = _index.Query(ray, mask);
        return WithCurrentStats(result);
    }

    SpatialQueryResult WithCurrentStats(SpatialQueryResult result)
    {
        var stats = result.Stats with
        {
            SpatialRevision = _revision,
            TotalEntityCount = _index.Count
        };
        return new SpatialQueryResult(result.Candidates, stats);
    }
}
