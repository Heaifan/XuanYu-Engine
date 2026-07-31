using XuanYu.Core.Identity;
using XuanYu.Core.Scene;
using XuanYu.Core.Spatial;

namespace XuanYu.World;

public sealed partial class GlobalWorld
{
    readonly EntityRegistry _registry = new();
    readonly WorldPartitionMembership _partition = new();
    readonly WorldQuery _query = new();
    readonly IWorldPartitionStrategy _partitionStrategy;

    public GlobalWorld() : this(new GridWorldPartitionStrategy()) { }

    public GlobalWorld(IWorldPartitionStrategy partitionStrategy)
    {
        _partitionStrategy = partitionStrategy;
    }

    // Creates an entity. The optional `extent` is the entity's OWN spatial description
    // (local box relative to its position); WorldQuery only consumes it and never
    // invents a default size. Callers that omit extent get a zero-size point (R2-R1).
    public WorldEntitySnapshot Create(
        string name,
        string type = "WorldEntity",
        CommittedTransform? transform = null,
        SpatialAabb extent = default)
    {
        var committed = transform ?? CommittedTransform.Identity;
        var entity = _registry.Create(name, type, committed, ResolveRegion(committed), WorldEntityActivity.Active, extent);
        _partition.Add(entity.EntityKey, entity.RegionKey);
        _query.Insert(entity);
        return entity;
    }

    public bool Destroy(EntityId entityKey)
    {
        if (!_registry.Destroy(entityKey)) return false;
        _partition.Remove(entityKey);
        _query.Remove(entityKey);
        return true;
    }

    public bool UpdateTransform(EntityId entityKey, CommittedTransform transform)
    {
        if (!_registry.UpdateTransform(entityKey, transform)) return false;
        var region = _partitionStrategy.RegionFor(transform.Position);
        _partition.MoveToRegion(entityKey, region);
        var updated = _registry.UpdatePartition(entityKey, region, GetActivity(entityKey));
        if (updated) _query.Update(_registry.Get(entityKey));
        return updated;
    }

    public bool MoveToRegion(EntityId entityKey, RegionKey region)
    {
        if (!TryGet(entityKey, out var entity) || region != ResolveRegion(entity.Transform)) return false;
        if (!_partition.MoveToRegion(entityKey, region)) return false;
        return _registry.UpdatePartition(entityKey, region, GetActivity(entityKey));
    }

    public bool SetActivity(EntityId entityKey, WorldEntityActivity activity)
    {
        if (activity == WorldEntityActivity.Externalized) return false;
        if (!Exists(entityKey) || !_partition.SetActivity(entityKey, activity)) return false;
        return _registry.UpdatePartition(entityKey, GetRegion(entityKey), activity);
    }

    public RegionKey GetRegion(EntityId entityKey) => _partition.GetRegion(entityKey);

    public WorldEntityActivity GetActivity(EntityId entityKey) => _partition.GetActivity(entityKey);

    public RegionKey ResolveRegion(CommittedTransform transform) =>
        _partitionStrategy.RegionFor(transform.Position);

    public WorldEntitySnapshot Get(EntityId entityKey) => _registry.Get(entityKey);

    public bool TryGet(EntityId entityKey, out WorldEntitySnapshot entity) =>
        _registry.TryGet(entityKey, out entity);

    public bool Exists(EntityId entityKey) => _registry.Exists(entityKey);

    public void RebuildSpatialIndexFromWorld() => _query.Rebuild(_registry.Snapshot);

    public void Replace(IReadOnlyList<WorldEntitySnapshot> entities)
    {
        _registry.Replace(entities);
        _partition.Clear();
        foreach (var entity in _registry.Snapshot)
        {
            _partition.Add(entity.EntityKey, entity.RegionKey);
        }
        _query.Rebuild(_registry.Snapshot);
    }
}
