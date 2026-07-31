using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Spatial;
using XuanYu.World;

namespace XuanYu.World.Scene;

public sealed partial class SceneStateOwner : ISceneRenderSnapshotSource
{
    readonly GlobalWorld _world;
    SceneRenderSnapshot _snapshot;
    EntityId _activeEntityKey;

    // Placeholder scene entities declare their OWN spatial extent (1m box) at the
    // entity-creation site. This is the entity's own spatial description -- it stands in
    // for a unit-sized minimal scene object -- NOT WorldQuery inventing a universal
    // default size for all entities. Real gameplay entities supply their own bounds.
    // If a future picking tolerance diverges from this spatial extent, that separation
    // belongs to a Pick Proxy concern, not to World Bounds (R2-R1 ownership ruling).
    static readonly SpatialAabb MinimalSceneEntityExtent =
        new(new Vector3d(-0.5, -0.5, -0.5), new Vector3d(0.5, 0.5, 0.5));

    public SceneStateOwner() : this(null, true) { }

    public SceneStateOwner(IWorldPartitionStrategy? partitionStrategy, bool seedInitialEntity = true)
    {
        _world = partitionStrategy is null ? new GlobalWorld() : new GlobalWorld(partitionStrategy);
        if (!seedInitialEntity)
        {
            _activeEntityKey = EntityId.None;
            RefreshSnapshot();
            return;
        }
        var entity = _world.Create("基础测试实体", WorldEntityTypes.LegacyMinimalTriangle,
            null, MinimalSceneEntityExtent);
        _activeEntityKey = entity.EntityKey;
        RefreshSnapshot();
    }

    public SceneRenderSnapshot RenderSnapshot => _snapshot;
    public IReadOnlyList<WorldEntitySnapshot> Entities => _world.Entities;
    public long SpatialRevision => _world.SpatialRevision;
    public EntityId ActiveEntityKey => _activeEntityKey;
    public event Action<SceneRenderSnapshot>? RenderSnapshotChanged;

    public SpatialQueryResult QuerySpatial(SpatialAabb area, SpatialQueryCategory mask) =>
        _world.QuerySpatial(area, mask);
    public SpatialQueryResult QuerySpatial(SpatialRayQuery ray, SpatialQueryCategory mask) =>
        _world.QuerySpatial(ray, mask);
    public SpatialRaycastResult RaycastSpatial(SpatialRayQuery ray, SpatialQueryCategory mask) =>
        _world.RaycastSpatial(ray, mask);

    public bool RestoreTransform(EntityId entityKey, CommittedTransform transform)
    {
        if (!_world.TryGet(entityKey, out var current)) return false;
        if (current.Transform == transform) return false;
        ApplyTransform(current, transform);
        return true;
    }

    SceneTransformCommitResult ApplyTransform(
        WorldEntitySnapshot current,
        CommittedTransform transform)
    {
        _world.UpdateTransform(current.EntityKey, transform);
        RefreshSnapshot();
        RenderSnapshotChanged?.Invoke(_snapshot);
        return new SceneTransformCommitResult(
            current.EntityKey,
            current.Transform,
            transform,
            true);
    }

    void RefreshSnapshot()
    {
        _snapshot = _world.TryGet(_activeEntityKey, out var active)
            ? SceneWorldProjection.ToRenderSnapshot(active, Entities)
            : SceneRenderSnapshot.Empty;
    }

    public void ReplaceEntities(IReadOnlyList<WorldEntitySnapshot> entities)
    {
        _world.Replace(entities);
        _activeEntityKey = Entities.FirstOrDefault().EntityKey;
        RefreshSnapshot();
        RenderSnapshotChanged?.Invoke(_snapshot);
    }
}
