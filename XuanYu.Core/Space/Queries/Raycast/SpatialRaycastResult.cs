namespace XuanYu.Core.Spatial;

public sealed class SpatialRaycastResult
{
    public SpatialRaycastResult(SpatialRaycastHit? hit, SpatialRaycastStats stats)
    {
        Hit = hit;
        Stats = stats;
    }

    public SpatialRaycastHit? Hit { get; }

    public bool HasHit => Hit.HasValue;

    public SpatialRaycastStats Stats { get; }
}
