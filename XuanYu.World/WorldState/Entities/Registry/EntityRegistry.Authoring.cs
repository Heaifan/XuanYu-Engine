using XuanYu.Core.Identity;

namespace XuanYu.World;

public sealed partial class EntityRegistry
{
    public bool Rename(EntityId entityKey, string name)
    {
        if (!TryGet(entityKey, out var entity) || string.IsNullOrWhiteSpace(name)) return false;
        _entities[entityKey] = new WorldEntitySnapshot(
            entity.EntityKey, name, entity.Type, entity.Transform, entity.GlobalPosition,
            entity.RegionKey, entity.Activity, entity.Extent, entity.ParentId, entity.SiblingOrder);
        return true;
    }

    public bool Restore(WorldEntitySnapshot entity)
    {
        if (!entity.EntityKey.IsValid || _entities.ContainsKey(entity.EntityKey)) return false;
        if (!WorldEntityTypes.TryParse(entity.Type, out _)) return false;
        if (entity.ParentId.IsValid && !_entities.ContainsKey(entity.ParentId)) return false;
        if (_entities.Values.Any(x => x.ParentId == entity.ParentId && x.SiblingOrder == entity.SiblingOrder))
            return false;
        _entities.Add(entity.EntityKey, entity);
        _nextId = Math.Max(_nextId, entity.EntityKey.Value + 1);
        return true;
    }

    internal int NextSiblingOrder(EntityId parentId) => _entities.Values
        .Where(x => x.ParentId == parentId)
        .Select(x => x.SiblingOrder + 1)
        .DefaultIfEmpty(0)
        .Max();
}
