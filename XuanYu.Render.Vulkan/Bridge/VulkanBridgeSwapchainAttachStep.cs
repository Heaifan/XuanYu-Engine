using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Swapchain;

namespace XuanYu.Render.Vulkan.Bridge;

// VK4-C：在设备 step 之后链式驱动 Swapchain 创建（Swapchain + Images + ImageViews）。
// 不重枚举、不建 RenderPass；设备/选择不可用或创建失败仅返回 null，不影响已附加的 Instance+Surface+Device。
public sealed class VulkanBridgeSwapchainAttachStep
{
    public static VulkanSwapchainOwner? Run(
        Vk vk, Instance instance, VulkanDeviceOwner? deviceOwner, SurfaceKHR surface,
        VulkanPhysicalDeviceSelection? selection, int width, int height, Action<string>? log)
    {
        if (deviceOwner is null || selection is null || !selection.Success)
        {
            var skip = "【VulkanSwapchain】Swapchain 创建跳过：LogicalDevice 或物理设备选择不可用";
            log?.Invoke(skip); Console.WriteLine(skip); return null;
        }
        try
        {
            return VulkanSwapchainOwner.Create(vk, instance, deviceOwner, surface, selection.Handle, width, height, log);
        }
        catch (Exception ex)
        {
            var msg = $"【VulkanSwapchain】Swapchain 创建异常：{ex.Message}；Instance + Surface + Device 保持已附加状态";
            log?.Invoke(msg); Console.WriteLine(msg);
            return null;
        }
    }
}
