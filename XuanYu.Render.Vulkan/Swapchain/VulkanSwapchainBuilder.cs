using System;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using VulkanDevice = Silk.NET.Vulkan.Device;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Core.Diagnostics;
using XuanYu.Render.Abstractions.Diagnostics;
using XuanYu.Render.Vulkan.Swapchain.Diagnostics;

namespace XuanYu.Render.Vulkan.Swapchain;

// VK4-C：Swapchain 构建细节（创建 Swapchain + 取 Images + 建 ImageViews）。纯逻辑，不持有状态。
public static unsafe class VulkanSwapchainBuilder
{
    public static (SwapchainKHR swapchain, Image[] images, ImageView[] views, Format format, Extent2D extent, bool ok, bool readbackEnabled) Build(
        Vk vk, Instance instance, PhysicalDevice physicalDevice, SurfaceKHR surface,
        KhrSwapchain khr, VulkanDevice device, int width, int height, Action<string>? log,
        SwapchainKHR oldSwapchain = default, RendererEventSession? events = null)
    {
        var caps = VulkanSwapchainCapabilities.Query(vk, instance, physicalDevice, surface, width, height, log);
        if (!caps.Success || caps.Caps is null)
        { VulkanSwapchainEvents.PublishFailure(events, width, height, null, caps.Error); return default; }
        var chosen = caps.Caps.Value;
        var usage = VulkanSwapchainUsagePolicy.Select(chosen.SupportedUsage,
            TerrainReplayOptions.Enabled);
        if ((usage & ImageUsageFlags.ColorAttachmentBit) == 0)
        {
            Log(log, "Swapchain 缺少 ColorAttachment usage 支持");
            VulkanSwapchainEvents.PublishFailure(events, width, height, null, "ColorAttachmentUnsupported");
            return default;
        }
        var swapchain = CreateSwapchain(khr, device, surface, chosen, oldSwapchain, log, events);
        if (swapchain.Handle == 0) return default;
        var (images, views) = CreateImagesAndViews(vk, khr, device, swapchain, chosen.Format.Format, log);
        if (views.Length != images.Length)
        { VulkanSwapchainEvents.PublishFailure(events, width, height, null, "ImageViewCreationIncomplete"); return default; }
        var readback = (usage & ImageUsageFlags.TransferSrcBit) != 0;
        Log(log, $"【VulkanReadback】Swapchain TransferSrc={(readback ? "SUPPORTED" : "UNAVAILABLE")}");
        return (swapchain, images, views, chosen.Format.Format, chosen.Extent, true, readback);
    }

    static SwapchainKHR CreateSwapchain(KhrSwapchain khr, VulkanDevice device, SurfaceKHR surface,
        SwapchainCaps caps, SwapchainKHR oldSwapchain, Action<string>? log, RendererEventSession? events)
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
            ImageUsage = VulkanSwapchainUsagePolicy.Select(caps.SupportedUsage,
                TerrainReplayOptions.Enabled),
            ImageSharingMode = SharingMode.Exclusive,
            PreTransform = caps.Transform,
            CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
            PresentMode = caps.PresentMode,
            Clipped = true,
            OldSwapchain = oldSwapchain
        };
        var result = khr.CreateSwapchain(device, &info, null, out var swapchain);
        if (result != Result.Success)
        {
            VulkanSwapchainEvents.PublishFailure(events, (int)caps.Extent.Width,
                (int)caps.Extent.Height, result.ToString(), "CreateSwapchain");
            Log(log, VulkanSwapchainLogFormatter.Failed($"CreateSwapchain 失败：{result}"));
            return default;
        }
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

    static void Log(Action<string>? log, string m) { log?.Invoke(m); }
}
