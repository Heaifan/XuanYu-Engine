using XuanYu.Core.Identity;
using XuanYu.Core.Scene;

namespace XuanYu.Core.World;

public sealed class EntityRegistry
{
    readonly Dictionary<EntityId, WorldEntitySnapshot> _entities = new();
    int _nextId = 1;

    public int Count => _entities.Count;

    public WorldEntitySnapshot Create(
        string name,
        string type = "WorldEntity",
        CommittedTransform? transform = null)
    {
        var entity = new WorldEntitySnapshot(
            EntityId.FromInt(_nextId++),
            name,
            type,
            transform ?? CommittedTransform.Identity);
        _entities.Add(entity.EntityKey, entity);
        return entity;
    }

    public bool Destroy(EntityId entityKey)
    {
        if (!entityKey.IsValid) return false;
        return _entities.Remove(entityKey);
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
