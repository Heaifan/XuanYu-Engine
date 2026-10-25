namespace XuanYu.Core.Spatial;

public sealed class SpatialQueryResult
{
    public SpatialQueryResult(IReadOnlyList<SpatialBounds> candidates, SpatialQueryStats stats)
    {
        ArgumentNullException.ThrowIfNull(candidates);
        Candidates = candidates;
        Stats = stats;
    }

    public IReadOnlyList<SpatialBounds> Candidates { get; }

    public SpatialQueryStats Stats { get; }
}
