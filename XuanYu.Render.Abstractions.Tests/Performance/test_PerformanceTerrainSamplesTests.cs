using XuanYu.Render.Abstractions.Diagnostics.Performance;

namespace XuanYu.Render.Abstractions.Tests.Performance;

public sealed class PerformanceTerrainSamplesTests
{
    [Fact]
    public void Maps_cache_chunk_geometry_and_byte_counters_to_one_frame()
    {
        var identity = new PerformanceFrameIdentity(
            "s", "b", "f", 17, 4, "terrain-r2", 3, PerformanceFrameMode.Double);
        var counters = new PerformanceTerrainCounters(
            21, 13, 8192, true, 8, 2, 3, 1, 1, 4096, 65536, 131072);

        var samples = PerformanceTerrainSamples.Create(identity, counters);

        Assert.All(samples, sample => Assert.Equal(identity, sample.Identity));
        Assert.Equal(21, Value(samples, PerformanceMetric.SelectedNodeCount));
        Assert.Equal(13, Value(samples, PerformanceMetric.SubmittedChunkCount));
        Assert.Equal(8192, Value(samples, PerformanceMetric.SubmittedTriangleCount));
        Assert.Equal(1, Value(samples, PerformanceMetric.LodBudgetSaturated));
        Assert.Equal(8, Value(samples, PerformanceMetric.CacheHit));
        Assert.Equal(2, Value(samples, PerformanceMetric.CacheMiss));
        Assert.Equal(3, Value(samples, PerformanceMetric.ResourceCreated));
        Assert.Equal(1, Value(samples, PerformanceMetric.ResourceEvicted));
        Assert.Equal(1, Value(samples, PerformanceMetric.ResourceRetired));
        Assert.Equal(4096, Value(samples, PerformanceMetric.UploadBytes));
        Assert.Equal(65536, Value(samples, PerformanceMetric.CacheResidentBytes));
        Assert.Equal(131072, Value(samples, PerformanceMetric.CacheHighWaterBytes));
        Assert.Equal(PerformanceUnit.Bytes,
            samples.Single(x => x.Metric == PerformanceMetric.UploadBytes).Unit);
    }

    static double Value(IEnumerable<PerformanceFrameSample> samples,
        PerformanceMetric metric) => samples.Single(x => x.Metric == metric).Value!.Value;
}
