namespace XuanYu.Core.Spatial;

public static class RayAabbIntersection
{
    public static bool TryHit(SpatialRayQuery query, SpatialAabb box, out RayAabbHit hit)
    {
        var tMin = 0.0;
        var tMax = query.MaxDistance;
        if (!Axis(query.Ray.Origin.X, query.Ray.Direction.X, box.Min.X, box.Max.X, ref tMin, ref tMax)
            || !Axis(query.Ray.Origin.Y, query.Ray.Direction.Y, box.Min.Y, box.Max.Y, ref tMin, ref tMax)
            || !Axis(query.Ray.Origin.Z, query.Ray.Direction.Z, box.Min.Z, box.Max.Z, ref tMin, ref tMax))
        {
            hit = default;
            return false;
        }

        var distance = global::System.Math.Max(0.0, tMin);
        hit = new RayAabbHit(distance, query.Ray.Origin + (query.Ray.Direction * distance));
        return true;
    }

    static bool Axis(double origin, double direction, double min, double max, ref double tMin, ref double tMax)
    {
        if (global::System.Math.Abs(direction) < 0.000000001)
        {
            return origin >= min && origin <= max;
        }

        var inv = 1.0 / direction;
        var near = (min - origin) * inv;
        var far = (max - origin) * inv;
        if (near > far) (near, far) = (far, near);
        tMin = global::System.Math.Max(tMin, near);
        tMax = global::System.Math.Min(tMax, far);
        return tMin <= tMax;
    }
}
