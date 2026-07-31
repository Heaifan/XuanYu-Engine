using XuanYu.Core.Identity;
using XuanYu.Core.Scene;
using XuanYu.Core.Spatial;
using XuanYu.World;

namespace XuanYu.World.Scene;

public sealed partial class SceneStateOwner
{
    public WorldEntitySnapshot CreateEntity(
        string name,
        string type,
        CommittedTransform? transform = null,
        SpatialAabb? extent = null)
    {
        var uniqueName = WorldEntityName.Unique(name, Entities);
        var entity = _world.Create(uniqueName, type, transform, extent ?? MinimalSceneEntityExtent);
        if (!_snapshot.HasEntity) _activeEntityKey = entity.EntityKey;
        PublishChanged();
        return entity;
    }

    public WorldEntitySnapshot AddCubeEntity()
    {
        var name = WorldEntityName.Unique("立方体", Entities);
        var entity = CreateEntity(name, WorldEntityTypes.Cube, CommittedTransform.Identity);
        SetActiveEntity(entity.EntityKey);
        return entity;
    }

    public bool RestoreEntity(WorldEntitySnapshot snapshot)
    {
        if (!_world.Restore(snapshot)) return false;
        _activeEntityKey = snapshot.EntityKey;
        PublishChanged();
        return true;
    }

    public bool RenameEntity(EntityId entityKey, string requestedName, out string finalName)
    {
        finalName = "";
        if (!_world.TryGet(entityKey, out var entity)) return false;
        var trimmed = requestedName.Trim();
        if (trimmed.Length == 0) return false;
        finalName = WorldEntityName.Unique(trimmed, Entities, entityKey);
        if (entity.Name == finalName) return false;
        if (!_world.Rename(entityKey, finalName)) return false;
        PublishChanged();
        return true;
    }

    public bool DestroyEntity(EntityId entityKey)
    {
        if (!_world.Destroy(entityKey)) return false;
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

    void PublishChanged()
    {
        RefreshSnapshot();
        RenderSnapshotChanged?.Invoke(_snapshot);
    }
}
