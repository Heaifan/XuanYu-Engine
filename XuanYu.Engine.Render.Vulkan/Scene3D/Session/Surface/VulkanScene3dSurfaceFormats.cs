using XuanYu.Engine.Render.Vulkan.Scene3D.Session.Swapchain;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session.Surface;

/// <summary>
/// 两阶段枚举 SurfaceFormatKHR，处理 Success / Incomplete，有限重试。
/// 每次 Incomplete 都重新查询 count，不使用上次 written 作为新容量（written 仅表示已写入数，不表示总容量）。
/// 不进行 Swapchain 创建以外的职责。
/// </summary>
public static unsafe class VulkanScene3dSurfaceFormats
{
    private const int MaxRetries = 3;

    /// <summary>
    /// 枚举 Surface 支持的格式。
    /// </summary>
    public static bool TryEnumerate(
        VulkanScene3dSwapchainFunctions.GetFormatsFunc getFormats,
        PhysicalDevice physicalDevice,
        SurfaceKHR surface,
        out SurfaceFormatKHR[] formats,
        out string errorMessage)
    {
        formats = [];
        errorMessage = string.Empty;

        for (var attempt = 0; attempt < MaxRetries; attempt++)
        {
            // 第一阶段：查询容量
            uint count = 0;
            var countResult = getFormats(physicalDevice, surface, &count, null);
            if (countResult != Result.Success)
            {
                errorMessage =
                    $"Surface 格式枚举第一阶段失败。\n" +
                    $"VkResult：{countResult}\n" +
                    $"尝试次数：{attempt + 1}";
                return false;
            }

            if (count == 0)
            {
                errorMessage = "Surface 格式枚举返回 0 个可用格式。";
                return false;
            }

            // 第二阶段：填充数组
            var buffer = new SurfaceFormatKHR[count];
            uint written = count;
            fixed (SurfaceFormatKHR* p = buffer)
            {
                var fillResult = getFormats(physicalDevice, surface, &written, p);

                if (fillResult == Result.Success)
                {
                    formats = buffer;
                    return true;
                }

                if (fillResult == Result.Incomplete)
                {
                    // 容量不足，回到循环顶部重新查询最新 count 再试
                    continue;
                }

                // 其他错误
                errorMessage =
                    $"Surface 格式枚举失败。\n" +
                    $"VkResult：{fillResult}\n" +
                    $"尝试次数：{attempt + 1}\n" +
                    $"返回数量：{written}";
                return false;
            }
        }

        errorMessage =
            $"Surface 格式枚举超过最大重试次数（{MaxRetries}）。\n" +
            $"VkResult：Incomplete（持续变化）";
        return false;
    }
}
