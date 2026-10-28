using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan;
using Xunit;

namespace XuanYu.Render.Vulkan.Tests;

public sealed class VulkanInstanceVersionProbeTests
{
    [Fact]
    public void Missing_entry_point_uses_explicit_vulkan_10_fallback()
    {
        var result = VulkanInstanceVersionResolution.Resolve(false, Result.Success, 0);

        Assert.True(result.Success);
        Assert.Equal(Vk.Version10, result.Version);
        Assert.True(result.UsedVersion10Fallback);
    }

    [Fact]
    public void Query_error_is_not_hidden_as_a_version_10_fallback()
    {
        var result = VulkanInstanceVersionResolution.Resolve(true,
            Result.ErrorInitializationFailed, 0);

        Assert.False(result.Success);
        Assert.False(result.UsedVersion10Fallback);
        Assert.Contains("ErrorInitializationFailed", result.Error);
    }

    [Fact]
    public void Successful_query_preserves_reported_version()
    {
        var result = VulkanInstanceVersionResolution.Resolve(true,
            Result.Success, Vk.Version11);

        Assert.True(result.Success);
        Assert.Equal(Vk.Version11, result.Version);
        Assert.False(result.UsedVersion10Fallback);
    }
}
