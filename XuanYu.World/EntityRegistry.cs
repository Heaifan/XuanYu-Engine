using XuanYu.Core.Identity;
using XuanYu.Core.Scene;
using XuanYu.Core.Spatial;

namespace XuanYu.World;

public sealed partial class EntityRegistry
{
    readonly Dictionary<EntityId, WorldEntitySnapshot> _entities = new();
    int _nextId = 1;

    public int Count => _entities.Count;

    public IReadOnlyList<WorldEntitySnapshot> Snapshot =>
        _entities.Values.OrderBy(item => item.EntityKey.Value).ToArray();

    public WorldEntitySnapshot Create(
        string name,
        string type = "WorldEntity",
        CommittedTransform? transform = null,
        RegionKey? region = null,
        WorldEntityActivity activity = WorldEntityActivity.Active,
        SpatialAabb extent = default)
    {
        var committed = transform ?? CommittedTransform.Identity;
        var entity = new WorldEntitySnapshot(
            EntityId.FromInt(_nextId++),
            name,
            type,
            committed,
            committed.Position,
            region ?? RegionKey.Origin,
            activity,
            extent);
        _entities.Add(entity.EntityKey, entity);
        return entity;
    }

    public bool Destroy(EntityId entityKey)
    {
        if (!entityKey.IsValid) return false;
        return _entities.Remove(entityKey);
    }

    public bool UpdateTransform(EntityId entityKey, CommittedTransform transform)
    {
        if (!TryGet(entityKey, out var entity)) return false;
        _entities[entityKey] = new WorldEntitySnapshot(
            entity.EntityKey,
            entity.Name,
            entity.Type,
            transform,
            transform.Position,
            entity.RegionKey,
            entity.Activity,
            entity.Extent);
        return true;
    }

    public bool UpdatePartition(EntityId entityKey, RegionKey region, WorldEntityActivity activity)
    {
        if (!TryGet(entityKey, out var entity)) return false;
        _entities[entityKey] = new WorldEntitySnapshot(
            entity.EntityKey,
            entity.Name,
            entity.Type,
            entity.Transform,
            entity.GlobalPosition,
            region,
            activity,
            entity.Extent);
        return true;
    }

    public WorldEntitySnapshot Get(EntityId entityKey)
    {
        if (TryGet(entityKey, out var entity)) return entity;
        throw new KeyNotFoundException($"实体不存在：{entityKey}");
    }

    public bool TryGet(EntityId entityKey, out WorldEntitySnapshot entity)
    {
        if (!entityKey.IsValid)
        {
            entity = default;
            return false;
        }

        return _entities.TryGetValue(entityKey, out entity);
    }

    public bool Exists(EntityId entityKey) => TryGet(entityKey, out _);
}
