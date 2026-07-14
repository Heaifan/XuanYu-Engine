using System;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace XuanYu.Render.Vulkan.Swapchain;

public sealed unsafe partial class VulkanSwapchainOwner
{
    public Format Format => _format;
    public Extent2D Extent => _extent;
    public uint ResourceGeneration => _resourceGeneration;
    public ReadOnlySpan<ImageView> ImageViews => _imageViews;
    public SwapchainKHR Swapchain => _swapchain;
    public KhrSwapchain Khr => _khr!;

    static void Log(Action<string>? log, string m) { log?.Invoke(m); }
}
