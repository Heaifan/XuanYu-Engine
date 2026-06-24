using XuanYu.Engine.Render.Vulkan.Scene3D.Session.Swapchain;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session.Surface;

/// <summary>
/// 两阶段枚举 PresentModeKHR，处理 Success / Incomplete，有限重试。
/// 每次 Incomplete 都重新查询 count，不使用上次 written 作为新容量。
/// 职责单一：仅枚举 PresentMode。
/// </summary>
public static unsafe class VulkanScene3dPresentModes
{
    private const int MaxRetries = 3;

    /// <summary>
    /// 枚举 Surface 支持的 PresentMode。
    /// </summary>
    public static bool TryEnumerate(
        VulkanScene3dSwapchainFunctions.GetPresentModesFunc getPresentModes,
        PhysicalDevice physicalDevice,
        SurfaceKHR surface,
        out PresentModeKHR[] modes,
        out string errorMessage)
    {
        modes = [];
        errorMessage = string.Empty;

        for (var attempt = 0; attempt < MaxRetries; attempt++)
        {
            // 第一阶段：查询容量
            uint count = 0;
            var countResult = getPresentModes(physicalDevice, surface, &count, null);
            if (countResult != Result.Success)
            {
                errorMessage =
                    $"PresentMode 枚举第一阶段失败。\n" +
                    $"VkResult：{countResult}\n" +
                    $"尝试次数：{attempt + 1}";
                return false;
            }

            if (count == 0)
            {
                errorMessage = "PresentMode 枚举返回 0 个可用模式。";
                return false;
            }

            // 第二阶段：填充数组
            var buffer = new PresentModeKHR[count];
            uint written = count;
            fixed (PresentModeKHR* p = buffer)
            {
                var fillResult = getPresentModes(physicalDevice, surface, &written, p);

                if (fillResult == Result.Success)
                {
                    modes = buffer;
                    return true;
                }

                if (fillResult == Result.Incomplete)
                {
                    // 容量不足，回到循环顶部重新查询最新 count 再试
                    continue;
                }

                errorMessage =
                    $"PresentMode 枚举失败。\n" +
                    $"VkResult：{fillResult}\n" +
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
