using FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session.Surface;

/// <summary>
/// 两阶段枚举 PresentModeKHR，处理 Success / Incomplete，有限重试。
/// 职责单一：仅枚举 PresentMode。
/// </summary>
public static unsafe class VulkanScene3dPresentModes
{
    private const int MaxRetries = 3;

    /// <summary>
    /// 枚举 Surface 支持的 PresentMode。
    /// </summary>
    /// <param name="getPresentModes">vkGetPhysicalDeviceSurfacePresentModesKHR 委托。</param>
    /// <param name="physicalDevice">物理设备。</param>
    /// <param name="surface">Surface。</param>
    /// <param name="modes">输出 PresentMode 数组。</param>
    /// <param name="errorMessage">失败时的诊断信息。</param>
    /// <returns>成功返回 true，失败返回 false。</returns>
    public static bool TryEnumerate(
        VulkanScene3dSwapchainFunctions.GetPresentModesFunc getPresentModes,
        PhysicalDevice physicalDevice,
        SurfaceKHR surface,
        out PresentModeKHR[] modes,
        out string errorMessage)
    {
        modes = [];
        errorMessage = string.Empty;

        uint count = 0;
        var firstResult = getPresentModes(physicalDevice, surface, &count, null);
        if (firstResult != Result.Success)
        {
            errorMessage =
                $"PresentMode 枚举第一阶段失败。\n" +
                $"VkResult：{firstResult}\n" +
                $"返回数量：{count}";
            return false;
        }

        if (count == 0)
        {
            errorMessage = "PresentMode 枚举返回 0 个可用模式。";
            return false;
        }

        for (var attempt = 0; attempt < MaxRetries; attempt++)
        {
            var buffer = new PresentModeKHR[count];
            uint written = count;
            fixed (PresentModeKHR* p = buffer)
            {
                var result = getPresentModes(physicalDevice, surface, &written, p);

                if (result == Result.Success)
                {
                    modes = buffer;
                    return true;
                }

                if (result == Result.Incomplete)
                {
                    count = written;
                    continue;
                }

                errorMessage =
                    $"PresentMode 枚举失败。\n" +
                    $"VkResult：{result}\n" +
                    $"尝试次数：{attempt + 1}\n" +
                    $"返回数量：{written}";
                return false;
            }
        }

        errorMessage =
            $"PresentMode 枚举超过最大重试次数（{MaxRetries}）。\n" +
            $"VkResult：Incomplete（持续变化）";
        return false;
    }
}
