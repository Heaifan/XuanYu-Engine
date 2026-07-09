using System;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace XuanYu.Render.Vulkan.Swapchain;

// VK4-C：Swapchain 能力查询（纯数据，不创建 Swapchain）。
// 复用 VK4-A 已选 PhysicalDevice 与 Surface，不重枚举；偏好 B8G8R8A8+SRGB / FIFO（Mailbox 优先）。
public static unsafe class VulkanSwapchainCapabilities
{
    public static VulkanSwapchainCapabilitiesResult Query(
        Vk vk, Instance instance, PhysicalDevice physicalDevice, SurfaceKHR surface,
        int width, int height, Action<string>? log)
    {
        if (!vk.TryGetInstanceExtension(instance, out KhrSurface? khr) || khr is null)
            return VulkanSwapchainCapabilitiesResult.Failed("缺 VK_KHR_surface 实例扩展");

        khr.GetPhysicalDeviceSurfaceCapabilities(physicalDevice, surface, out var caps);

        uint fmtCount;
        khr.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface, &fmtCount, null);
        var formats = new SurfaceFormatKHR[fmtCount];
        fixed (SurfaceFormatKHR* p = formats)
            khr.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface, &fmtCount, p);

        uint pmCount;
        khr.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface, &pmCount, null);
        var presentModes = new PresentModeKHR[pmCount];
        fixed (PresentModeKHR* p = presentModes)
            khr.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface, &pmCount, p);

        var format = ChooseFormat(formats);
        var presentMode = ChoosePresentMode(presentModes);
        var extent = ChooseExtent(caps, width, height);
        uint minImages = caps.MinImageCount + 1;
        if (caps.MaxImageCount != 0 && minImages > caps.MaxImageCount) minImages = caps.MaxImageCount;

        Log(log, $"【VulkanSwapchain】能力查询成功；请求逻辑尺寸={width}x{height}；Surface CurrentExtent={caps.CurrentExtent.Width}x{caps.CurrentExtent.Height}；选择 extent={extent.Width}x{extent.Height}（物理像素）；格式={format.Format}；呈现模式={presentMode}；最小图像数={minImages}");
        return VulkanSwapchainCapabilitiesResult.Ok(new SwapchainCaps(format, presentMode, extent, minImages, caps.CurrentTransform));
    }

    static SurfaceFormatKHR ChooseFormat(SurfaceFormatKHR[] formats)
    {
        foreach (var f in formats)
            if (f.Format == Format.B8G8R8A8Unorm && f.ColorSpace == ColorSpaceKHR.SpaceSrgbNonlinearKhr) return f;
        foreach (var f in formats)
            if (f.Format == Format.B8G8R8A8Unorm) return f;
        return formats.Length > 0 ? formats[0] : new SurfaceFormatKHR { Format = Format.B8G8R8A8Unorm, ColorSpace = ColorSpaceKHR.SpaceSrgbNonlinearKhr };
    }

    static PresentModeKHR ChoosePresentMode(PresentModeKHR[] modes)
    {
        foreach (var m in modes)
            if (m == PresentModeKHR.MailboxKhr) return m;
        return PresentModeKHR.FifoKhr;
    }

    static Extent2D ChooseExtent(SurfaceCapabilitiesKHR caps, int width, int height)
    {
        if (caps.CurrentExtent.Width != uint.MaxValue) return caps.CurrentExtent;
        uint w = (uint)Math.Max(caps.MinImageExtent.Width, Math.Min(caps.MaxImageExtent.Width, (uint)width));
        uint h = (uint)Math.Max(caps.MinImageExtent.Height, Math.Min(caps.MaxImageExtent.Height, (uint)height));
        return new Extent2D(w, h);
    }

    static void Log(Action<string>? log, string m) { log?.Invoke(m); }
}

public readonly record struct SwapchainCaps(
    SurfaceFormatKHR Format,
    PresentModeKHR PresentMode,
    Extent2D Extent,
    uint MinImageCount,
    SurfaceTransformFlagsKHR Transform);

public readonly record struct VulkanSwapchainCapabilitiesResult(bool Success, SwapchainCaps? Caps, string Error)
{
    public static VulkanSwapchainCapabilitiesResult Ok(SwapchainCaps caps) => new(true, caps, "");
    public static VulkanSwapchainCapabilitiesResult Failed(string error) => new(false, null, error);
}
