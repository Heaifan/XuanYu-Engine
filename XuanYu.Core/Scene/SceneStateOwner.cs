using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Spatial;
using XuanYu.Core.World;

namespace XuanYu.Core.Scene;

public sealed class SceneStateOwner : ISceneRenderSnapshotSource
{
    readonly GlobalWorld _world = new();
    readonly SpatialIndexOwner _spatialIndex = new();
    SceneRenderSnapshot _snapshot;
    EntityId _activeEntityKey;

    public SceneStateOwner()
    {
        var entity = _world.Create("ARCH-C-R1 Test Entity", "MinimalSceneEntity");
        _activeEntityKey = entity.EntityKey;
        _snapshot = SceneWorldProjection.ToRenderSnapshot(entity);
        _spatialIndex.Insert(ToSpatialBounds(_snapshot.Entity));
    }

    public SceneRenderSnapshot RenderSnapshot => _snapshot;
    public IReadOnlyList<WorldEntitySnapshot> Entities => _world.Entities;
    public long SpatialRevision => _spatialIndex.SpatialRevision;
    public event Action<SceneRenderSnapshot>? RenderSnapshotChanged;

    public WorldEntitySnapshot CreateEntity(string name, string type, CommittedTransform? transform = null)
    {
        var entity = _world.Create(name, type, transform);
        _spatialIndex.Insert(ToSpatialBounds(SceneWorldProjection.ToSceneEntity(entity)));
        if (!_snapshot.HasEntity) SetActiveEntity(entity.EntityKey);
        return entity;
    }

    public bool DestroyEntity(EntityId entityKey)
    {
        if (!_world.Destroy(entityKey)) return false;
        _spatialIndex.Remove(entityKey);
        if (_activeEntityKey == entityKey)
        {
            SetActiveEntity(Entities.FirstOrDefault().EntityKey);
        }
        else
        {
            RenderSnapshotChanged?.Invoke(_snapshot);
        }
        return true;
    }

    public bool TryGetEntity(EntityId entityKey, out WorldEntitySnapshot entity) =>
        _world.TryGet(entityKey, out entity);

    public void SetActiveEntity(EntityId entityKey)
    {
        _activeEntityKey = entityKey;
        _snapshot = _world.TryGet(entityKey, out var entity)
            ? SceneWorldProjection.ToRenderSnapshot(entity)
            : SceneRenderSnapshot.Empty;
        RenderSnapshotChanged?.Invoke(_snapshot);
    }

    public SpatialQueryResult QuerySpatial(SpatialAabb area, SpatialQueryCategory mask) => _spatialIndex.Query(area, mask);
    public SpatialQueryResult QuerySpatial(SpatialRayQuery ray, SpatialQueryCategory mask) => _spatialIndex.Query(ray, mask);
    public SpatialRaycastResult RaycastSpatial(SpatialRayQuery ray, SpatialQueryCategory mask) => _spatialIndex.Raycast(ray, mask);
    public bool CommitPosition(Vector3d position) => CommitPositionWithResult(position).Changed;
    public SceneTransformCommitResult CommitPositionWithResult(Vector3d position) =>
        CommitPositionWithResult(_activeEntityKey, position);

    public SceneTransformCommitResult CommitPositionWithResult(EntityId entityKey, Vector3d position)
    {
        if (!_world.TryGet(entityKey, out var current))
            return new SceneTransformCommitResult(entityKey, CommittedTransform.Identity, CommittedTransform.Identity, false);
        var transform = new CommittedTransform(position);
        if (current.Transform == transform)
            return new SceneTransformCommitResult(entityKey, current.Transform, transform, false);
        return ApplyTransform(current, transform);
    }

    public bool RestoreTransform(EntityId entityKey, CommittedTransform transform)
    {
        if (!_world.TryGet(entityKey, out var current)) return false;
        if (current.Transform == transform) return false;
        ApplyTransform(current, transform);
        return true;
    }

    SceneTransformCommitResult ApplyTransform(WorldEntitySnapshot current, CommittedTransform transform)
    {
        _world.UpdateTransform(current.EntityKey, transform);
        var next = _world.Get(current.EntityKey);
        _spatialIndex.Update(ToSpatialBounds(SceneWorldProjection.ToSceneEntity(next)));
        if (_activeEntityKey == current.EntityKey) _snapshot = SceneWorldProjection.ToRenderSnapshot(next);
        RenderSnapshotChanged?.Invoke(_snapshot);
        return new SceneTransformCommitResult(current.EntityKey, current.Transform, transform, true);
    }

    static SpatialBounds ToSpatialBounds(SceneEntitySnapshot entity) =>
        SceneSpatialBoundsProjection.ToSpatialBounds(entity);
}
