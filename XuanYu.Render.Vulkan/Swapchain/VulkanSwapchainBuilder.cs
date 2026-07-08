using System;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using VulkanDevice = Silk.NET.Vulkan.Device;
using XuanYu.Render.Vulkan.Device;

namespace XuanYu.Render.Vulkan.Swapchain;

// VK4-C：Swapchain 构建细节（创建 Swapchain + 取 Images + 建 ImageViews）。纯逻辑，不持有状态。
public static unsafe class VulkanSwapchainBuilder
{
    public static (SwapchainKHR swapchain, Image[] images, ImageView[] views, bool ok) Build(
        Vk vk, Instance instance, PhysicalDevice physicalDevice, SurfaceKHR surface,
        KhrSwapchain khr, VulkanDevice device, int width, int height, Action<string>? log)
    {
        var caps = VulkanSwapchainCapabilities.Query(vk, instance, physicalDevice, surface, width, height, log);
        if (!caps.Success || caps.Caps is null) return default;
        var chosen = caps.Caps.Value;
        var swapchain = CreateSwapchain(khr, device, surface, chosen, log);
        if (swapchain.Handle == 0) return default;
        var (images, views) = CreateImagesAndViews(vk, khr, device, swapchain, chosen.Format.Format, log);
        if (views.Length != images.Length) return default;
        return (swapchain, images, views, true);
    }

    static SwapchainKHR CreateSwapchain(KhrSwapchain khr, VulkanDevice device, SurfaceKHR surface, SwapchainCaps caps, Action<string>? log)
    {
        var info = new SwapchainCreateInfoKHR
        {
            SType = StructureType.SwapchainCreateInfoKhr,
            Surface = surface,
            MinImageCount = caps.MinImageCount,
            ImageFormat = caps.Format.Format,
            ImageColorSpace = caps.Format.ColorSpace,
            ImageExtent = caps.Extent,
            ImageArrayLayers = 1,
            ImageUsage = ImageUsageFlags.ColorAttachmentBit,
            ImageSharingMode = SharingMode.Exclusive,
            PreTransform = caps.Transform,
            CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
            PresentMode = caps.PresentMode,
            Clipped = true
        };
        var result = khr.CreateSwapchain(device, &info, null, out var swapchain);
        if (result != Result.Success) { Log(log, VulkanSwapchainLogFormatter.Failed($"CreateSwapchain 失败：{result}")); return default; }
        return swapchain;
    }

    static (Image[], ImageView[]) CreateImagesAndViews(Vk vk, KhrSwapchain khr, VulkanDevice device, SwapchainKHR swapchain, Format format, Action<string>? log)
    {
        uint count;
        khr.GetSwapchainImages(device, swapchain, &count, null);
        var images = new Image[count];
        fixed (Image* p = images) khr.GetSwapchainImages(device, swapchain, &count, p);
        var views = new ImageView[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            var vi = new ImageViewCreateInfo
            {
                SType = StructureType.ImageViewCreateInfo,
                Image = images[i],
                ViewType = ImageViewType.Type2D,
                Format = format,
                Components = new ComponentMapping { R = ComponentSwizzle.R, G = ComponentSwizzle.G, B = ComponentSwizzle.B, A = ComponentSwizzle.A },
                SubresourceRange = new ImageSubresourceRange { AspectMask = ImageAspectFlags.ColorBit, BaseMipLevel = 0, LevelCount = 1, BaseArrayLayer = 0, LayerCount = 1 }
            };
            if (vk.CreateImageView(device, &vi, null, out var view) != Result.Success) return (images, views);
            views[i] = view;
        }
        return (images, views);
    }

    static void Log(Action<string>? log, string m) { log?.Invoke(m); Console.WriteLine(m); }
}
