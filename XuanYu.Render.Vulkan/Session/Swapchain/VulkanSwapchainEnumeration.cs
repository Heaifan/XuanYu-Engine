using System;
using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Swapchain;

internal readonly record struct VulkanEnumerationFetch<T>(
    Result Result, uint Count, T[] Items);

internal static class VulkanSwapchainEnumeration
{
    internal const int MaxAttempts = 3;

    internal static bool TryEnumerate<T>(
        Func<(Result Result, uint Count)> queryCount,
        Func<uint, VulkanEnumerationFetch<T>> queryItems,
        out T[] items)
    {
        items = [];
        for (var attempt = 0; attempt < MaxAttempts; attempt++)
        {
            var count = queryCount();
            if (count.Result != Result.Success || count.Count == 0 ||
                count.Count > Array.MaxLength)
                return false;

            var fetched = queryItems(count.Count);
            if (fetched.Result == Result.Incomplete) continue;
            if (fetched.Result != Result.Success || fetched.Count == 0 ||
                fetched.Count > count.Count || fetched.Count > fetched.Items.Length)
                return false;

            items = fetched.Items.AsSpan(0, (int)fetched.Count).ToArray();
            return true;
        }
        return false;
    }
}
