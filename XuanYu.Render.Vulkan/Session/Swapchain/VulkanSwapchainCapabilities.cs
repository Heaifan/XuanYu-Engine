using System;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using XuanYu.Core.Diagnostics;

namespace XuanYu.Render.Vulkan.Swapchain;

// VK4-C：Swapchain 能力查询（纯数据，不创建 Swapchain）。
// 复用 VK4-A 已选 PhysicalDevice 与 Surface，不重枚举；偏好 B8G8R8A8+SRGB / FIFO（垂直同步优先）。
public static unsafe class VulkanSwapchainCapabilities
{
    public static VulkanSwapchainCapabilitiesResult Query(
        Vk vk, Instance instance, PhysicalDevice physicalDevice, SurfaceKHR surface,
        int width, int height, Action<string>? log)
    {
        if (!vk.TryGetInstanceExtension(instance, out KhrSurface? khr) || khr is null)
            return VulkanSwapchainCapabilitiesResult.Failed("缺 VK_KHR_surface 实例扩展");

        var capsResult = khr.GetPhysicalDeviceSurfaceCapabilities(
            physicalDevice, surface, out var caps);
        if (capsResult != Result.Success)
            return VulkanSwapchainCapabilitiesResult.Failed(
                $"查询 Surface 能力失败：{capsResult}");
        if (!TryGetFormats(khr, physicalDevice, surface, out var formats))
            return VulkanSwapchainCapabilitiesResult.Failed("查询 Surface 格式列表失败");
        if (!TryGetPresentModes(khr, physicalDevice, surface, out var presentModes))
            return VulkanSwapchainCapabilitiesResult.Failed("查询 Surface 呈现模式列表失败");
        if (!TryChooseFormat(formats, out var format))
            return VulkanSwapchainCapabilitiesResult.Failed("Surface 未提供可用格式");
        if (presentModes.Length == 0)
            return VulkanSwapchainCapabilitiesResult.Failed("Surface 未提供呈现模式");
        var presentMode = ChoosePresentMode(presentModes);
        var extent = ChooseExtent(caps, width, height);
        uint minImages = caps.MinImageCount + 1;
        if (caps.MaxImageCount != 0 && minImages > caps.MaxImageCount) minImages = caps.MaxImageCount;

        Log(log, $"【VulkanSwapchain】能力查询成功；请求逻辑尺寸={width}x{height}；Surface 当前尺寸={caps.CurrentExtent.Width}x{caps.CurrentExtent.Height}；选择物理尺寸={extent.Width}x{extent.Height}；格式={format.Format}；呈现模式={presentMode}；最小图像数={minImages}；SupportedUsage={caps.SupportedUsageFlags}");
        if (TerrainReplayOptions.Enabled)
            ViewportProbe.Log("present", $"[SURFACE-USAGE] SupportedUsage={caps.SupportedUsageFlags};" +
                $"TransferSrcSupported={(caps.SupportedUsageFlags & ImageUsageFlags.TransferSrcBit) != 0}");
        return VulkanSwapchainCapabilitiesResult.Ok(new SwapchainCaps(format, presentMode, extent, minImages, caps.CurrentTransform, caps.SupportedUsageFlags));
    }

    static bool TryGetFormats(KhrSurface khr, PhysicalDevice physicalDevice,
        SurfaceKHR surface, out SurfaceFormatKHR[] formats)
        => VulkanSwapchainEnumeration.TryEnumerate(
            () =>
            {
                uint count = 0;
                var result = khr.GetPhysicalDeviceSurfaceFormats(
                    physicalDevice, surface, &count, null);
                return (result, count);
            }, capacity =>
            {
                var values = new SurfaceFormatKHR[(int)capacity];
                var count = capacity;
                Result result;
                fixed (SurfaceFormatKHR* pointer = values)
                    result = khr.GetPhysicalDeviceSurfaceFormats(
                        physicalDevice, surface, &count, pointer);
                return new VulkanEnumerationFetch<SurfaceFormatKHR>(
                    result, count, values);
            }, out formats);

    static bool TryGetPresentModes(KhrSurface khr, PhysicalDevice physicalDevice,
        SurfaceKHR surface, out PresentModeKHR[] modes)
        => VulkanSwapchainEnumeration.TryEnumerate(
            () =>
            {
                uint count = 0;
                var result = khr.GetPhysicalDeviceSurfacePresentModes(
                    physicalDevice, surface, &count, null);
                return (result, count);
            }, capacity =>
            {
                var values = new PresentModeKHR[(int)capacity];
                var count = capacity;
                Result result;
                fixed (PresentModeKHR* pointer = values)
                    result = khr.GetPhysicalDeviceSurfacePresentModes(
                        physicalDevice, surface, &count, pointer);
                return new VulkanEnumerationFetch<PresentModeKHR>(
                    result, count, values);
            }, out modes);

    internal static bool TryChooseFormat(SurfaceFormatKHR[] formats,
        out SurfaceFormatKHR format)
    {
        format = default;
        foreach (var f in formats)
            if (f.Format == Format.B8G8R8A8Unorm && f.ColorSpace == ColorSpaceKHR.SpaceSrgbNonlinearKhr)
            { format = f; return true; }
        foreach (var f in formats)
            if (f.Format == Format.B8G8R8A8Unorm)
            { format = f; return true; }
        if (formats.Length == 0) return false;
        format = formats[0];
        return true;
    }

    // VK-PERF-R1：FIFO（垂直同步）为首选，Mailbox 不再作为默认；Vulkan 规范保证 FIFO 必被支持。
    internal static PresentModeKHR ChoosePresentMode(PresentModeKHR[] modes)
    {
        foreach (var m in modes)
            if (m == PresentModeKHR.FifoKhr) return m;
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
    SurfaceTransformFlagsKHR Transform,
    ImageUsageFlags SupportedUsage);

public readonly record struct VulkanSwapchainCapabilitiesResult(bool Success, SwapchainCaps? Caps, string Error)
{
    public static VulkanSwapchainCapabilitiesResult Ok(SwapchainCaps caps) => new(true, caps, "");
    public static VulkanSwapchainCapabilitiesResult Failed(string error) => new(false, null, error);
}
