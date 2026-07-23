using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Spatial;
using XuanYu.Core.World;

namespace XuanYu.Core.Scene;

public sealed partial class SceneStateOwner : ISceneRenderSnapshotSource
{
    readonly GlobalWorld _world;
    readonly SpatialIndexOwner _spatialIndex = new();
    SceneRenderSnapshot _snapshot;
    EntityId _activeEntityKey;

    public SceneStateOwner() : this(null) { }

    public SceneStateOwner(IWorldPartitionStrategy? partitionStrategy)
    {
        _world = partitionStrategy is null ? new GlobalWorld() : new GlobalWorld(partitionStrategy);
        var entity = _world.Create("基础测试实体", "MinimalSceneEntity");
        _activeEntityKey = entity.EntityKey;
        RefreshSnapshot();
        _spatialIndex.Insert(ToSpatialBounds(_snapshot.Entity));
    }

    public SceneRenderSnapshot RenderSnapshot => _snapshot;
    public IReadOnlyList<WorldEntitySnapshot> Entities => _world.Entities;
    public long SpatialRevision => _spatialIndex.SpatialRevision;
    public event Action<SceneRenderSnapshot>? RenderSnapshotChanged;

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
        RefreshSnapshot();
        RenderSnapshotChanged?.Invoke(_snapshot);
        return new SceneTransformCommitResult(current.EntityKey, current.Transform, transform, true);
    }

    static SpatialBounds ToSpatialBounds(SceneEntitySnapshot entity) =>
        SceneSpatialBoundsProjection.ToSpatialBounds(entity);

    void RefreshSnapshot()
    {
        _snapshot = _world.TryGet(_activeEntityKey, out var active)
            ? SceneWorldProjection.ToRenderSnapshot(active, Entities)
            : SceneRenderSnapshot.Empty;
    }
}
