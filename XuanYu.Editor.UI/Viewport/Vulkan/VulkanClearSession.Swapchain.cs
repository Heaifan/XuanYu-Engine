using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace XuanYu.Editor.UI;

public sealed unsafe partial class VulkanClearSession
{
    void CreateSwapchain()
    {
        if (_khrSwapchain is null) throw new InvalidOperationException("缺少 VK_KHR_swapchain");
        _khrSurface!.GetPhysicalDeviceSurfaceCapabilities(_physicalDevice, _surface, out var caps);
        var format = PickFormat();
        var extent = caps.CurrentExtent.Width == uint.MaxValue
            ? new Extent2D(Math.Max(1, _width), Math.Max(1, _height)) : caps.CurrentExtent;
        var imageCount = Math.Clamp(caps.MinImageCount + 1, caps.MinImageCount, caps.MaxImageCount == 0 ? caps.MinImageCount + 1 : caps.MaxImageCount);
        var create = new SwapchainCreateInfoKHR
        {
            SType = StructureType.SwapchainCreateInfoKhr, Surface = _surface, MinImageCount = imageCount,
            ImageFormat = format.Format, ImageColorSpace = format.ColorSpace, ImageExtent = extent,
            ImageArrayLayers = 1, ImageUsage = ImageUsageFlags.ColorAttachmentBit,
            ImageSharingMode = SharingMode.Exclusive, PreTransform = caps.CurrentTransform,
            CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
            PresentMode = PresentModeKHR.FifoKhr, Clipped = true
        };
        Check(_khrSwapchain.CreateSwapchain(_device, &create, null, out _swapchain), "创建 Swapchain 失败");
    }

    SurfaceFormatKHR PickFormat()
    {
        uint count = 0;
        _khrSurface!.GetPhysicalDeviceSurfaceFormats(_physicalDevice, _surface, &count, null);
        var formats = stackalloc SurfaceFormatKHR[(int)count];
        _khrSurface.GetPhysicalDeviceSurfaceFormats(_physicalDevice, _surface, &count, formats);
        for (var i = 0; i < count; i++)
            if (formats[i].Format == Format.B8G8R8A8Srgb) return formats[i];
        return formats[0];
    }

    void DestroySwapchain()
    {
        if (_swapchain.Handle == 0 || _khrSwapchain is null) return;
        _vk.DeviceWaitIdle(_device);
        _khrSwapchain.DestroySwapchain(_device, _swapchain, null);
        _swapchain = default;
    }
}
