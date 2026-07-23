using XuanYu.Core.Identity;
using XuanYu.Core.Scene;

namespace XuanYu.Core.World;

public sealed class GlobalWorld
{
    readonly EntityRegistry _registry = new();
    readonly WorldPartitionMembership _partition = new();

    public int EntityCount => _registry.Count;

    public IReadOnlyList<WorldEntitySnapshot> Entities => _registry.Snapshot;

    public WorldEntitySnapshot Create(
        string name,
        string type = "WorldEntity",
        CommittedTransform? transform = null,
        RegionKey? region = null)
    {
        var entity = _registry.Create(name, type, transform, region);
        _partition.Add(entity.EntityKey, entity.RegionKey);
        return entity;
    }

    public bool Destroy(EntityId entityKey)
    {
        if (!_registry.Destroy(entityKey)) return false;
        _partition.Remove(entityKey);
        return true;
    }

    public bool UpdateTransform(EntityId entityKey, CommittedTransform transform)
    {
        if (!_registry.UpdateTransform(entityKey, transform)) return false;
        var region = WorldPartitionMembership.RegionFor(transform.Position);
        _partition.MoveToRegion(entityKey, region);
        return _registry.UpdatePartition(entityKey, region, GetActivity(entityKey));
    }

    public bool MoveToRegion(EntityId entityKey, RegionKey region)
    {
        if (!Exists(entityKey) || !_partition.MoveToRegion(entityKey, region)) return false;
        return _registry.UpdatePartition(entityKey, region, GetActivity(entityKey));
    }

    public bool SetActivity(EntityId entityKey, WorldEntityActivity activity)
    {
        if (!Exists(entityKey) || !_partition.SetActivity(entityKey, activity)) return false;
        return _registry.UpdatePartition(entityKey, GetRegion(entityKey), activity);
    }

    public RegionKey GetRegion(EntityId entityKey) => _partition.GetRegion(entityKey);

    public WorldEntityActivity GetActivity(EntityId entityKey) => _partition.GetActivity(entityKey);

    public IReadOnlyList<EntityId> EntitiesIn(RegionKey region) => _partition.EntitiesIn(region);

    public WorldEntitySnapshot Get(EntityId entityKey) => _registry.Get(entityKey);

    public bool TryGet(EntityId entityKey, out WorldEntitySnapshot entity) =>
        _registry.TryGet(entityKey, out entity);

    public bool Exists(EntityId entityKey) => _registry.Exists(entityKey);
}
