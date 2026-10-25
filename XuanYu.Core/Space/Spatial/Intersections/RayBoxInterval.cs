namespace XuanYu.Core.Spatial;
internal static class RayBoxInterval
{
    public static bool TryIntersect(SpatialRayQuery q, SpatialAabb box, out double distance)
    {
        var lo = 0.0; var hi = q.MaxDistance;
        var ray = q.Ray;
        var valid = Axis(ray.Origin.X, ray.Direction.X, box.Min.X, box.Max.X, ref lo, ref hi)
            && Axis(ray.Origin.Y, ray.Direction.Y, box.Min.Y, box.Max.Y, ref lo, ref hi)
            && Axis(ray.Origin.Z, ray.Direction.Z, box.Min.Z, box.Max.Z, ref lo, ref hi);
        distance = valid ? global::System.Math.Max(0, lo) : 0;
        return valid;
    }

    static bool Axis(double origin, double direction, double min, double max,
        ref double lo, ref double hi)
    {
        if (direction == 0.0) return origin >= min && origin <= max;
        var a = (min - origin) / direction;
        var b = (max - origin) / direction;
        if (a > b) (a, b) = (b, a);
        lo = global::System.Math.Max(lo, a); hi = global::System.Math.Min(hi, b);
        return lo <= hi;
    }
}
