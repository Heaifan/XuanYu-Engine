using XuanYu.Render.Vulkan.Benchmark;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanBenchmarkPercentileTests
{
    [Fact]
    public void Uses_nearest_rank_for_unsorted_gpu_samples()
    {
        var samples = Enumerable.Range(1, 100).Reverse().Select(x => (double)x).ToArray();

        var result = VulkanBenchmarkPercentileCalculator.Calculate(samples);

        Assert.Equal(50, result.P50);
        Assert.Equal(95, result.P95);
    }

    [Fact]
    public void Uses_nearest_rank_for_three_process_stage_samples()
    {
        var result = VulkanBenchmarkPercentileCalculator.Calculate([12d, 3d, 7d]);

        Assert.Equal(7, result.P50);
        Assert.Equal(12, result.P95);
    }
}
