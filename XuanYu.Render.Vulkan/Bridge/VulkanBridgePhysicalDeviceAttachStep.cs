using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;

namespace XuanYu.Render.Vulkan.Bridge;

// VK4-A-R1：将 Attach 后的 PhysicalDevice 选择与中文日志从 VulkanNativeHostSurfaceBridge 迁出，
// 使 Bridge 只保留生命周期编排（Attach/Resize/Detach）。仅枚举与选择，不创建 VkDevice / Queue / Swapchain。
public sealed class VulkanBridgePhysicalDeviceAttachStep
{
    public static void Run(Vk vk, Instance instance, SurfaceKHR surface, Action<string>? log)
    {
        try
        {
            VulkanPhysicalDeviceSelector.Select(vk, instance, surface, log);
        }
        catch (Exception ex)
        {
            var msg = $"【VulkanDevice】物理设备选择异常：{ex.Message}；Instance + Surface 保持已附加状态";
            log?.Invoke(msg); Console.WriteLine(msg);
        }
    }
}
