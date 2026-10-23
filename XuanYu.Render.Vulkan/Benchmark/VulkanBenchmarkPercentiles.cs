namespace XuanYu.Render.Vulkan.Benchmark;

public readonly record struct VulkanBenchmarkPercentiles(double P50, double P95, double P99);

public static class VulkanBenchmarkPercentileCalculator
{
    public static VulkanBenchmarkPercentiles Calculate(IReadOnlyList<double> samples)
    {
        ArgumentNullException.ThrowIfNull(samples);
        if (samples.Count == 0 || samples.Any(x => !double.IsFinite(x) || x < 0))
            throw new ArgumentException("Percentile 样本必须非空、有限且非负。", nameof(samples));
        var ordered = samples.Order().ToArray();
        return new(NearestRank(ordered, 0.50), NearestRank(ordered, 0.95), NearestRank(ordered, 0.99));
    }

    static double NearestRank(double[] sorted, double percentile) =>
        sorted[Math.Max(0, (int)Math.Ceiling(percentile * sorted.Length) - 1)];
}
