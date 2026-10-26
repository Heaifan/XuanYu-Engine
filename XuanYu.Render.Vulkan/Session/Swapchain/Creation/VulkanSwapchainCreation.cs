using System;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using XuanYu.Render.Abstractions.Diagnostics;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Swapchain.Diagnostics;

namespace XuanYu.Render.Vulkan.Swapchain.Creation;

public static class VulkanSwapchainCreation
{
    public static VulkanSwapchainOwner? Create(Vk vk, Instance instance,
        VulkanDeviceOwner? deviceOwner, SurfaceKHR surface, PhysicalDevice physicalDevice,
        int width, int height, Action<string>? log, RendererEventSession? events = null)
    {
        if (deviceOwner is null)
            return Fail(log, events, width, height, "LogicalDeviceUnavailable", "LogicalDevice 不可用");
        if (!vk.TryGetDeviceExtension(instance, deviceOwner.LogicalDevice,
                out KhrSwapchain? khr) || khr is null)
            return Fail(log, events, width, height, "SwapchainExtensionUnavailable",
                "缺 VK_KHR_swapchain 设备扩展");
        try
        {
            var (swapchain, images, views, format, extent, ok, readback) =
                VulkanSwapchainBuilder.Build(vk, instance, physicalDevice, surface, khr,
                    deviceOwner.LogicalDevice, width, height, log, events: events);
            if (!ok) return null;
            log?.Invoke(VulkanSwapchainLogFormatter.Created(extent, views.Length));
            return new VulkanSwapchainOwner(vk, instance, deviceOwner, surface, physicalDevice,
                khr, swapchain, images, views, format, extent, readback, log);
        }
        catch (Exception ex)
        {
            return Fail(log, events, width, height, ex.Message, $"创建异常：{ex.Message}");
        }
    }

    static VulkanSwapchainOwner? Fail(Action<string>? log, RendererEventSession? events,
        int width, int height, string reason, string message)
    {
        VulkanSwapchainEvents.PublishFailure(events, width, height, null, reason);
        log?.Invoke(VulkanSwapchainLogFormatter.Failed(message));
        return null;
    }
}
