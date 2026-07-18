namespace XuanYu.Core.Spatial;

public sealed class SpatialQueryResult
{
    public SpatialQueryResult(IReadOnlyList<SpatialBounds> candidates, SpatialQueryStats stats)
    {
        Candidates = candidates;
        Stats = stats;
    }

    public IReadOnlyList<SpatialBounds> Candidates { get; }

    public SpatialQueryStats Stats { get; }
}
