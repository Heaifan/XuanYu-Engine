using XuanYu.Core.Math;
using XuanYu.Core.Spatial;

namespace XuanYu.Core.Scene;

public sealed class SceneStateOwner : ISceneRenderSnapshotSource
{
    readonly SpatialIndexOwner _spatialIndex = new();
    SceneRenderSnapshot _snapshot;

    public SceneStateOwner()
    {
        _snapshot = SceneRenderSnapshot.TestEntityAtOrigin;
        _spatialIndex.Insert(ToSpatialBounds(_snapshot.Entity));
    }

    public SceneRenderSnapshot RenderSnapshot => _snapshot;

    public long SpatialRevision => _spatialIndex.SpatialRevision;

    public event Action<SceneRenderSnapshot>? RenderSnapshotChanged;

    public SpatialQueryResult QuerySpatial(SpatialAabb area, SpatialQueryCategory mask) => _spatialIndex.Query(area, mask);

    public SpatialQueryResult QuerySpatial(SpatialRayQuery ray, SpatialQueryCategory mask) => _spatialIndex.Query(ray, mask);

    public SpatialRaycastResult RaycastSpatial(SpatialRayQuery ray, SpatialQueryCategory mask) => _spatialIndex.Raycast(ray, mask);

    public bool CommitPosition(Vector3d position)
    {
        var current = _snapshot.Entity;
        var transform = new CommittedTransform(position);
        if (current.Transform == transform) return false;
        var next = current with
        {
            Transform = transform
        };
        _spatialIndex.Update(ToSpatialBounds(next));
        _snapshot = new SceneRenderSnapshot(next);
        RenderSnapshotChanged?.Invoke(_snapshot);
        return true;
    }

    static SpatialBounds ToSpatialBounds(SceneEntitySnapshot entity)
    {
        var p = entity.Transform.Position;
        var min = new Vector3d(p.X - 0.5, p.Y - 0.5, p.Z - 0.5);
        var max = new Vector3d(p.X + 0.5, p.Y + 0.5, p.Z + 0.5);
        return new SpatialBounds(entity.EntityKey, new SpatialAabb(min, max), SpatialQueryCategory.SceneEntity);
    }
}
