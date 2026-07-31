using XuanYu.Core.Identity;

namespace XuanYu.World;

public sealed partial class GlobalWorld
{
    public bool Rename(EntityId entityKey, string name) => _registry.Rename(entityKey, name);

    public bool Restore(WorldEntitySnapshot entity)
    {
        if (!_registry.Restore(entity)) return false;
        _partition.Add(entity.EntityKey, entity.RegionKey);
        _query.Insert(entity);
        return true;
    }
}
