namespace XuanYu.Render.Abstractions.Diagnostics.Performance;

public sealed record PerformanceTerrainCounters(
    long SelectedNodes, long SubmittedChunks, long SubmittedTriangles,
    bool LodBudgetSaturated, long CacheHits, long CacheMisses,
    long ResourceCreates, long ResourceEvictions, long ResourceRetirements,
    long UploadBytes, long CacheBytes, long CacheHighWaterBytes);

public static class PerformanceTerrainSamples
{
    public static IReadOnlyList<PerformanceFrameSample> Create(
        PerformanceFrameIdentity identity, PerformanceTerrainCounters counters)
    {
        ArgumentNullException.ThrowIfNull(counters);
        return
        [
            Count(identity, PerformanceMetric.SelectedNodeCount, counters.SelectedNodes),
            Count(identity, PerformanceMetric.SubmittedChunkCount, counters.SubmittedChunks),
            Count(identity, PerformanceMetric.SubmittedTriangleCount,
                counters.SubmittedTriangles),
            Count(identity, PerformanceMetric.LodBudgetSaturated,
                counters.LodBudgetSaturated ? 1 : 0, PerformanceUnit.Boolean),
            Count(identity, PerformanceMetric.CacheHit, counters.CacheHits),
            Count(identity, PerformanceMetric.CacheMiss, counters.CacheMisses),
            Count(identity, PerformanceMetric.ResourceCreated, counters.ResourceCreates),
            Count(identity, PerformanceMetric.ResourceEvicted, counters.ResourceEvictions),
            Count(identity, PerformanceMetric.ResourceRetired, counters.ResourceRetirements),
            Count(identity, PerformanceMetric.UploadBytes, counters.UploadBytes,
                PerformanceUnit.Bytes),
            Count(identity, PerformanceMetric.CacheResidentBytes, counters.CacheBytes,
                PerformanceUnit.Bytes),
            Count(identity, PerformanceMetric.CacheHighWaterBytes,
                counters.CacheHighWaterBytes, PerformanceUnit.Bytes)
        ];
    }

    static PerformanceFrameSample Count(PerformanceFrameIdentity identity,
        PerformanceMetric metric, long value, PerformanceUnit unit = PerformanceUnit.Count) =>
        PerformanceFrameSample.Valid(identity, metric, value, unit,
            0, 0, null, null, 0, 0, 0);
}
