using XuanYu.Core.Math;

namespace XuanYu.Core.Spatial;

public readonly record struct SpatialAabb
{
    public SpatialAabb(Vector3d min, Vector3d max)
    {
        Validate(min, nameof(min));
        Validate(max, nameof(max));
        if (min.X > max.X || min.Y > max.Y || min.Z > max.Z)
        {
            throw new ArgumentOutOfRangeException(nameof(max), "AABB 最大值必须大于或等于最小值。");
        }

        Min = min;
        Max = max;
    }

    public Vector3d Min { get; }

    public Vector3d Max { get; }

    public double SurfaceArea
    {
        get
        {
            var size = Max - Min;
            return 2.0 * ((size.X * size.Y) + (size.Y * size.Z) + (size.Z * size.X));
        }
    }

    public bool Intersects(SpatialAabb other)
    {
        return Min.X <= other.Max.X && Max.X >= other.Min.X
            && Min.Y <= other.Max.Y && Max.Y >= other.Min.Y
            && Min.Z <= other.Max.Z && Max.Z >= other.Min.Z;
    }

    public SpatialAabb Union(SpatialAabb other)
    {
        return new SpatialAabb(
            new Vector3d(global::System.Math.Min(Min.X, other.Min.X), global::System.Math.Min(Min.Y, other.Min.Y), global::System.Math.Min(Min.Z, other.Min.Z)),
            new Vector3d(global::System.Math.Max(Max.X, other.Max.X), global::System.Math.Max(Max.Y, other.Max.Y), global::System.Math.Max(Max.Z, other.Max.Z)));
    }

    static void Validate(Vector3d value, string name)
    {
        if (!double.IsFinite(value.X) || !double.IsFinite(value.Y) || !double.IsFinite(value.Z))
        {
            throw new ArgumentOutOfRangeException(name);
        }
    }
}
