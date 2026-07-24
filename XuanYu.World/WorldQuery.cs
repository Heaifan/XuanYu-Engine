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

    // Mutation is reserved for the World authority chain (GlobalWorld). WorldQuery is
    // the single owned SpatialIndexOwner; no other World consumer may write (R2-R1).
    internal void Insert(WorldEntitySnapshot entity) => _index.Insert(ToBounds(entity));

    internal void Update(WorldEntitySnapshot entity)
    {
        var bounds = ToBounds(entity);
        if (!_index.Update(bounds)) LastStats = LastStats with { SpatialRevision = _index.SpatialRevision };
    }

    internal bool Remove(EntityId entityKey) => _index.Remove(entityKey);

    internal void Rebuild(IEnumerable<WorldEntitySnapshot> entities)
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

    // Consume the entity's explicit spatial bounds. WorldQuery never invents a default
    // size -- the extent is the entity's own description (R2-R1).
    static SpatialBounds ToBounds(WorldEntitySnapshot entity) => entity.Bounds;

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
