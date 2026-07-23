using XuanYu.Core.Identity;
using XuanYu.Core.World;

namespace XuanYu.Core.Scene;

public sealed partial class SceneStateOwner
{
    public WorldEntitySnapshot CreateEntity(
        string name,
        string type,
        CommittedTransform? transform = null)
    {
        var entity = _world.Create(name, type, transform);
        _spatialIndex.Insert(ToSpatialBounds(SceneWorldProjection.ToSceneEntity(entity)));
        if (!_snapshot.HasEntity) SetActiveEntity(entity.EntityKey);
        else RefreshSnapshot();
        return entity;
    }

    public bool DestroyEntity(EntityId entityKey)
    {
        if (!_world.Destroy(entityKey)) return false;
        _spatialIndex.Remove(entityKey);
        if (_activeEntityKey == entityKey) SetActiveEntity(Entities.FirstOrDefault().EntityKey);
        else
        {
            RefreshSnapshot();
            RenderSnapshotChanged?.Invoke(_snapshot);
        }
        return true;
    }

    public bool TryGetEntity(EntityId entityKey, out WorldEntitySnapshot entity) =>
        _world.TryGet(entityKey, out entity);

    public RegionKey GetRegion(EntityId entityKey) => _world.GetRegion(entityKey);

    public bool MoveEntityToRegion(EntityId entityKey, RegionKey region)
    {
        if (!_world.MoveToRegion(entityKey, region)) return false;
        RefreshSnapshot();
        RenderSnapshotChanged?.Invoke(_snapshot);
        return true;
    }

    public bool SetEntityActivity(EntityId entityKey, WorldEntityActivity activity)
    {
        if (!_world.SetActivity(entityKey, activity)) return false;
        RefreshSnapshot();
        RenderSnapshotChanged?.Invoke(_snapshot);
        return true;
    }

    public bool SetActiveEntity(EntityId entityKey, bool publish = true)
    {
        if (_activeEntityKey == entityKey) return false;
        _activeEntityKey = entityKey;
        RefreshSnapshot();
        if (publish) RenderSnapshotChanged?.Invoke(_snapshot);
        return true;
    }
}
