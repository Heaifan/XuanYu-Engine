using Silk.NET.Vulkan;

namespace XuanYu.Viewport.CompositionSpike;

static class VulkanResultExtensions
{
    public static void Ensure(this Result result)
    {
        if (result != Result.Success) throw new InvalidOperationException("Vulkan result=" + result);
    }
}
