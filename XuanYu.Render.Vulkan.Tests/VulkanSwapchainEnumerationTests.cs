using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Swapchain;
using Xunit;

namespace XuanYu.Render.Vulkan.Tests;

public sealed class VulkanSwapchainEnumerationTests
{
    [Fact]
    public void Empty_surface_format_list_is_rejected()
    {
        Assert.False(VulkanSwapchainCapabilities.TryChooseFormat([], out var format));
        Assert.Equal(default, format);
    }

    [Fact]
    public void Retries_incomplete_and_returns_only_written_items()
    {
        var fetches = 0;
        var success = VulkanSwapchainEnumeration.TryEnumerate<int>(
            () => (Result.Success, 2u), _ => ++fetches == 1
                ? new(Result.Incomplete, 2, new[] { 1, 2 })
                : new(Result.Success, 1, new[] { 3, 0 }), out var items);

        Assert.True(success);
        Assert.Equal(new[] { 3 }, items);
        Assert.Equal(2, fetches);
    }

    [Fact]
    public void Rejects_count_query_errors_and_empty_counts()
    {
        Assert.False(VulkanSwapchainEnumeration.TryEnumerate<int>(
            () => (Result.ErrorDeviceLost, 2), _ => throw new InvalidOperationException(), out _));
        Assert.False(VulkanSwapchainEnumeration.TryEnumerate<int>(
            () => (Result.Success, 0), _ => throw new InvalidOperationException(), out _));
    }

    [Fact]
    public void Rejects_fetch_errors_and_count_outside_the_allocated_array()
    {
        Assert.False(VulkanSwapchainEnumeration.TryEnumerate<int>(
            () => (Result.Success, 1u), _ => new(Result.ErrorDeviceLost, 1, [1]), out _));
        Assert.False(VulkanSwapchainEnumeration.TryEnumerate<int>(
            () => (Result.Success, 1u), _ => new(Result.Success, 2, [1]), out _));
    }

    [Fact]
    public void Bounds_incomplete_retries()
    {
        var fetches = 0;
        Assert.False(VulkanSwapchainEnumeration.TryEnumerate<int>(
            () => (Result.Success, 1u), _ =>
            {
                fetches++;
                return new(Result.Incomplete, 1, [1]);
            }, out _));
        Assert.Equal(VulkanSwapchainEnumeration.MaxAttempts, fetches);
    }
}
