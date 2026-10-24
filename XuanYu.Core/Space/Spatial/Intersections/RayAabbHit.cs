using XuanYu.Core.Math;

namespace XuanYu.Core.Spatial;

public readonly record struct RayAabbHit
{
    public RayAabbHit(double distance, Vector3d point)
    {
        if (!double.IsFinite(distance) || distance < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(distance));
        }

        Distance = distance;
        Point = point;
    }

    public double Distance { get; }

    public Vector3d Point { get; }
}
