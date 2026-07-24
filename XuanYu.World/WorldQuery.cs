using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Spatial;
using XuanYu.World.Spatial;

namespace XuanYu.World;

public sealed class WorldQuery
{
    SpatialIndexOwner _index = new();

    public SpatialQueryStats LastStats { get; private set; }

    public int Count => _index.EntityCount;

    public long SpatialRevision => _index.SpatialRevision;

    public void Insert(WorldEntitySnapshot entity) => _index.Insert(ToBounds(entity));

    public void Update(WorldEntitySnapshot entity)
    {
        var bounds = ToBounds(entity);
        if (!_index.Update(bounds)) LastStats = LastStats with { SpatialRevision = _index.SpatialRevision };
    }

    public bool Remove(EntityId entityKey) => _index.Remove(entityKey);

    public void Rebuild(IEnumerable<WorldEntitySnapshot> entities)
    {
        _index = new SpatialIndexOwner();
        foreach (var entity in entities) _index.Insert(ToBounds(entity));
        LastStats = default;
    }

    public IReadOnlyList<EntityId> QueryBounds(SpatialAabb bounds)
    {
        var result = _index.Query(bounds, SpatialQueryCategory.SceneEntity);
        LastStats = result.Stats;
        return result.Candidates.Select(item => item.EntityKey).ToArray();
    }

    public IReadOnlyList<EntityId> QueryRadius(Vector3d center, double radius)
    {
        if (!double.IsFinite(radius) || radius < 0) throw new ArgumentOutOfRangeException(nameof(radius));
        var box = new SpatialAabb(
            new Vector3d(center.X - radius, center.Y - radius, center.Z - radius),
            new Vector3d(center.X + radius, center.Y + radius, center.Z + radius));
        var result = _index.Query(box, SpatialQueryCategory.SceneEntity);
        LastStats = result.Stats;
        var radiusSquared = radius * radius;
        return result.Candidates
            .Where(item => DistanceSquared(Center(item.WorldBounds), center) <= radiusSquared)
            .Select(item => item.EntityKey)
            .ToArray();
    }

    public SpatialQueryResult Query(SpatialAabb area, SpatialQueryCategory mask)
    {
        var result = _index.Query(area, mask);
        LastStats = result.Stats;
        return result;
    }

    public SpatialQueryResult Query(SpatialRayQuery ray, SpatialQueryCategory mask)
    {
        var result = _index.Query(ray, mask);
        LastStats = result.Stats;
        return result;
    }

    public SpatialRaycastResult Raycast(SpatialRayQuery ray, SpatialQueryCategory mask)
    {
        var result = _index.Raycast(ray, mask);
        LastStats = new SpatialQueryStats(
            result.Stats.SpatialRevision,
            result.Stats.TotalEntityCount,
            result.Stats.VisitedNodeCount,
            result.Stats.CandidateCount);
        return result;
    }

    static SpatialBounds ToBounds(WorldEntitySnapshot entity) =>
        new(entity.EntityKey, PointBounds(entity.GlobalPosition), SpatialQueryCategory.SceneEntity);

    // Half-extent matches SceneSpatialBoundsProjection so the unique index serves Picking
    // raycast with the same 1x1x1 box the former Scene B index used (R2 convergence).
    static SpatialAabb PointBounds(Vector3d p) =>
        new(new Vector3d(p.X - 0.5, p.Y - 0.5, p.Z - 0.5), new Vector3d(p.X + 0.5, p.Y + 0.5, p.Z + 0.5));

    static Vector3d Center(SpatialAabb bounds) => new(
        (bounds.Min.X + bounds.Max.X) * 0.5,
        (bounds.Min.Y + bounds.Max.Y) * 0.5,
        (bounds.Min.Z + bounds.Max.Z) * 0.5);

    static double DistanceSquared(Vector3d a, Vector3d b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        var dz = a.Z - b.Z;
        return (dx * dx) + (dy * dy) + (dz * dz);
    }
}
