using FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session.Surface;

/// <summary>
/// 两阶段枚举 SurfaceFormatKHR，处理 Success / Incomplete，有限重试。
/// 不进行 Swapchain 创建以外的职责。
/// </summary>
public static unsafe class VulkanScene3dSurfaceFormats
{
    private const int MaxRetries = 3;

    /// <summary>
    /// 枚举 Surface 支持的格式。
    /// </summary>
    /// <param name="getFormats">vkGetPhysicalDeviceSurfaceFormatsKHR 委托。</param>
    /// <param name="physicalDevice">物理设备。</param>
    /// <param name="surface">Surface。</param>
    /// <param name="formats">输出格式数组。</param>
    /// <param name="errorMessage">失败时的诊断信息。</param>
    /// <returns>成功返回 true，失败返回 false。</returns>
    public static bool TryEnumerate(
        VulkanScene3dSwapchainFunctions.GetFormatsFunc getFormats,
        PhysicalDevice physicalDevice,
        SurfaceKHR surface,
        out SurfaceFormatKHR[] formats,
        out string errorMessage)
    {
        formats = [];
        errorMessage = string.Empty;

        uint count = 0;
        var firstResult = getFormats(physicalDevice, surface, &count, null);
        if (firstResult != Result.Success)
        {
            errorMessage =
                $"Surface 格式枚举第一阶段失败。\n" +
                $"VkResult：{firstResult}\n" +
                $"返回数量：{count}";
            return false;
        }

        if (count == 0)
        {
            errorMessage = "Surface 格式枚举返回 0 个可用格式。";
            return false;
        }

        for (var attempt = 0; attempt < MaxRetries; attempt++)
        {
            var buffer = new SurfaceFormatKHR[count];
            uint written = count;
            fixed (SurfaceFormatKHR* p = buffer)
            {
                var result = getFormats(physicalDevice, surface, &written, p);

                if (result == Result.Success)
                {
                    formats = buffer;
                    return true;
                }

                if (result == Result.Incomplete)
                {
                    // 驱动返回的数量比实际少，用新数量重试
                    count = written;
                    continue;
                }

                // 其他错误
                errorMessage =
                    $"Surface 格式枚举失败。\n" +
                    $"VkResult：{result}\n" +
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
