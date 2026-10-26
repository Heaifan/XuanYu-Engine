using System;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using VulkanDevice = Silk.NET.Vulkan.Device;
using XuanYu.Render.Abstractions.Diagnostics;
using XuanYu.Render.Vulkan.Swapchain.Diagnostics;

namespace XuanYu.Render.Vulkan.Swapchain.Creation;

static unsafe class VulkanSwapchainCandidateCleanup
{
    internal static bool Validate(Vk vk, KhrSwapchain khr, VulkanDevice device,
        SwapchainKHR swapchain, ImageView[] views, RendererEventSession? events,
        int width, int height, Action<string>? log)
    {
        if (views.Length > 0 && Array.TrueForAll(views, view => view.Handle != 0)) return true;
        foreach (var view in views) if (view.Handle != 0) vk.DestroyImageView(device, view, null);
        khr.DestroySwapchain(device, swapchain, null);
        VulkanSwapchainEvents.PublishFailure(events, width, height, null, "ImageViewCreationIncomplete");
        log?.Invoke("Swapchain 候选代际创建不完整；已销毁候选 ImageView 与 Swapchain");
        return false;
    }
}
