using XuanYu.Core.Space;

namespace XuanYu.Core.Spatial;

public readonly record struct SpatialRayQuery
{
    public SpatialRayQuery(WorldRay ray, double maxDistance)
    {
        if (!double.IsFinite(maxDistance) || maxDistance <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxDistance));
        }

        Ray = ray;
        MaxDistance = maxDistance;
    }

    public WorldRay Ray { get; }

    public double MaxDistance { get; }
}
