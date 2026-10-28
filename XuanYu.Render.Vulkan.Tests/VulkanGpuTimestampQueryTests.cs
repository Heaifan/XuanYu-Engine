using XuanYu.Render.Vulkan.Benchmark;
using Xunit;

namespace XuanYu.Render.Vulkan.Tests;

public sealed class VulkanGpuTimestampQueryTests
{
    [Fact]
    public void Dispose_releases_query_pool_once()
    {
        var destroyCalls = 0;
        var query = new VulkanGpuTimestampQuery(null!, null!, default, 0, 0,
            () => destroyCalls++);

        query.Dispose();
        query.Dispose();

        Assert.Equal(1, destroyCalls);
    }
}
