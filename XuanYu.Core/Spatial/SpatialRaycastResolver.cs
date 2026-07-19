namespace XuanYu.Core.Spatial;

public sealed class SpatialRaycastResolver
{
    readonly Action<SpatialBounds>? _beforeNarrowCandidate;

    public SpatialRaycastResolver()
    {
    }

    internal SpatialRaycastResolver(Action<SpatialBounds> beforeNarrowCandidate)
    {
        _beforeNarrowCandidate = beforeNarrowCandidate;
    }

    public SpatialRaycastResult Raycast(SpatialIndexOwner owner, SpatialRayQuery ray, SpatialQueryCategory mask)
    {
        var revision = owner.SpatialRevision;
        var candidates = owner.Query(ray, mask);
        if (owner.SpatialRevision != revision) throw new InvalidOperationException("空间索引代际已变化。");

        var hitCount = 0;
        SpatialRaycastHit? best = null;
        foreach (var candidate in candidates.Candidates)
        {
            _beforeNarrowCandidate?.Invoke(candidate);
            if (!RayAabbIntersection.TryHit(ray, candidate.WorldBounds, out var hit)) continue;
            hitCount++;
            var current = new SpatialRaycastHit(candidate.EntityKey, hit.Distance, hit.Point, revision);
            if (IsBetter(current, best)) best = current;
        }

        if (owner.SpatialRevision != revision) throw new InvalidOperationException("空间索引代际已变化。");

        var stats = new SpatialRaycastStats(revision, candidates.Stats.TotalEntityCount, candidates.Stats.VisitedNodeCount, candidates.Stats.CandidateCount, candidates.Candidates.Count, hitCount);
        return new SpatialRaycastResult(best, stats);
    }

    static bool IsBetter(SpatialRaycastHit current, SpatialRaycastHit? best)
    {
        if (!best.HasValue) return true;
        var delta = current.HitDistance - best.Value.HitDistance;
        if (global::System.Math.Abs(delta) > 0.000000001) return delta < 0.0;
        return current.EntityKey.Value < best.Value.EntityKey.Value;
    }
}
