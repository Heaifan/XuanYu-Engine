using XuanYu.Core.Math;

namespace XuanYu.Core.Spatial;

static class SpatialRayAabb
{
    public static bool Intersects(SpatialRayQuery query, SpatialAabb box)
    {
        var tMin = 0.0;
        var tMax = query.MaxDistance;
        return Axis(query.Ray.Origin.X, query.Ray.Direction.X, box.Min.X, box.Max.X, ref tMin, ref tMax)
            && Axis(query.Ray.Origin.Y, query.Ray.Direction.Y, box.Min.Y, box.Max.Y, ref tMin, ref tMax)
            && Axis(query.Ray.Origin.Z, query.Ray.Direction.Z, box.Min.Z, box.Max.Z, ref tMin, ref tMax);
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
