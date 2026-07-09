using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;

namespace XuanYu.Render.Vulkan.Bridge;

// VK4-B：在 VK4-A 物理设备选择成功后，基于其选择结果创建 LogicalDevice（VkDevice + 队列）。
// 不重新枚举、不建 Swapchain；选择或创建设备异常不影响已附加的 Instance + Surface。
public sealed class VulkanBridgeDeviceAttachStep
{
    public static VulkanDeviceOwner? Run(Vk vk, VulkanPhysicalDeviceSelection? sel, Action<string>? log, string requiredDeviceExtension)
    {
        if (sel is null || !sel.Success)
        {
            var skip = "【VulkanDevice】LogicalDevice 创建跳过：VK4-A 选择结果不可用";
            log?.Invoke(skip); return null;
        }
        try
        {
            return VulkanDeviceOwner.Create(vk, sel, requiredDeviceExtension, log);
        }
        catch (Exception ex)
        {
            var msg = $"【VulkanDevice】LogicalDevice 创建异常：{ex.Message}；Instance + Surface + 物理设备保持已附加状态";
            log?.Invoke(msg);
            return null;
        }
    }
}
