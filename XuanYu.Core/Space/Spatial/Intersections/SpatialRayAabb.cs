namespace XuanYu.Core.Spatial;
public static class SpatialRayAabb
{
    public static bool Intersects(SpatialRayQuery query, SpatialAabb box) =>
        RayBoxInterval.TryIntersect(query, box, out _);
}
