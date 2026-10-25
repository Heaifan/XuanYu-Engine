namespace XuanYu.Core.Spatial;

public static class RayAabbIntersection
{
    public static bool TryHit(SpatialRayQuery query, SpatialAabb box, out RayAabbHit hit)
    {
        if (!RayBoxInterval.TryIntersect(query, box, out var distance))
        {
            hit = default;
            return false;
        }

        hit = new RayAabbHit(distance, query.Ray.Origin + (query.Ray.Direction * distance));
        return true;
    }

}
