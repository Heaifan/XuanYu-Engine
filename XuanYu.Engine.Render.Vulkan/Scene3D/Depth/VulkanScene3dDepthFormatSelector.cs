using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.Depth;

/// <summary>
/// 查询物理设备支持的 Scene3D 深度格式。
/// 按优先顺序尝试：D32Sfloat → D32SfloatS8Uint → D24UnormS8Uint。
/// 不创建 Image，不持有资源。
/// </summary>
public static class VulkanScene3dDepthFormatSelector
{
    /// <summary>
    /// 优先顺序：D32Sfloat（无 Stencil）> D32SfloatS8Uint > D24UnormS8Uint。
    /// </summary>
    private static readonly (Format Format, bool HasStencil)[] Precedence =
    [
        (Format.D32Sfloat, false),
        (Format.D32SfloatS8Uint, true),
        (Format.D24UnormS8Uint, true),
    ];

    /// <summary>
    /// 选择第一个支持 DepthStencilAttachment 的深度格式。
    /// 全部不支持时返回 Unsupported 状态。
    /// </summary>
    public static VulkanScene3dDepthAttachmentInfo Select(Vk vk, PhysicalDevice physicalDevice)
    {
        foreach (var (format, hasStencil) in Precedence)
        {
            var props = vk.GetPhysicalDeviceFormatProperties(physicalDevice, format);
            if ((props.OptimalTilingFeatures & FormatFeatureFlags.DepthStencilAttachmentBit) != 0)
            {
                return new VulkanScene3dDepthAttachmentInfo(
                    true, format, hasStencil, 0,
                    $"深度格式：{FormatName(format)}");
            }
        }

        return VulkanScene3dDepthAttachmentInfo.Unsupported(
            "Scene3D 深度附件创建失败：当前物理设备没有支持的深度格式。");
    }

    /// <summary>
    /// 返回格式的简短名称，用于诊断。
    /// </summary>
    public static string FormatName(Format format)
    {
        return format switch
        {
            Format.D32Sfloat => "D32Sfloat",
            Format.D32SfloatS8Uint => "D32SfloatS8Uint",
            Format.D24UnormS8Uint => "D24UnormS8Uint",
            _ => format.ToString()
        };
    }

    /// <summary>
    /// 判断格式是否包含 Stencil 通道。
    /// </summary>
    public static bool HasStencilComponent(Format format)
    {
        return format is Format.D32SfloatS8Uint or Format.D24UnormS8Uint;
    }
}
