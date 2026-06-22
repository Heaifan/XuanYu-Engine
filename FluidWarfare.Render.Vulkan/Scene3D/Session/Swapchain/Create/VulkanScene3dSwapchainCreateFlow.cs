using FluidWarfare.Render.Vulkan.Scene3D.Session.Surface;
using FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain.Create;

/// <summary>Swapchain 创建流程：Surface caps / 格式 / PresentMode / 创建 / 获取 Image。</summary>
internal static unsafe class VulkanScene3dSwapchainCreateFlow
{
    public static (SwapchainKHR Swapchain, Image[] Images, SurfaceFormatKHR Format, Extent2D Extent,
        PresentModeKHR PresentMode, uint Width, uint Height, string? Error) Execute(
        Vk vk, Silk.NET.Vulkan.Device device, PhysicalDevice physicalDevice,
        SurfaceKHR surface, uint requestedWidth, uint requestedHeight,
        VulkanScene3dSwapchainFunctions functions, SwapchainKHR oldSwapchain)
    {
        uint w = requestedWidth, h = requestedHeight;

        // Surface capabilities
        SurfaceCapabilitiesKHR caps;
        var capsResult = functions.GetCapabilities(physicalDevice, surface, &caps);
        if (capsResult != Result.Success)
            return (default(SwapchainKHR), Array.Empty<Image>(), default(SurfaceFormatKHR), default(Extent2D), default(PresentModeKHR), w, h,
                $"查询 Surface 能力失败：{capsResult}（请求尺寸 {w}x{h}）。");

        // Surface formats
        if (!VulkanScene3dSurfaceFormats.TryEnumerate(functions.GetFormats, physicalDevice, surface, out var fmts, out var fmtErr))
            return (default(SwapchainKHR), Array.Empty<Image>(), default(SurfaceFormatKHR), default(Extent2D), default(PresentModeKHR), w, h, fmtErr);

        var chosenFmt = VulkanScene3dSwapchainSelection.ChooseFormat(fmts);
        var extent = VulkanScene3dSwapchainExtent.ChooseExtent(caps, w, h);
        var imgCount = Math.Clamp(caps.MinImageCount + 1, caps.MinImageCount, caps.MaxImageCount > 0 ? caps.MaxImageCount : uint.MaxValue);

        // Present modes
        if (!VulkanScene3dPresentModes.TryEnumerate(functions.GetPresentModes, physicalDevice, surface, out var modes, out var modeErr))
            return (default(SwapchainKHR), Array.Empty<Image>(), default(SurfaceFormatKHR), default(Extent2D), default(PresentModeKHR), w, h, modeErr);

        var presentMode = VulkanScene3dSwapchainSelection.ChoosePresentMode(modes);

        // Create swapchain
        var scCI = new SwapchainCreateInfoKHR
        {
            SType = StructureType.SwapchainCreateInfoKhr, Surface = surface,
            MinImageCount = imgCount, ImageFormat = chosenFmt.Format, ImageColorSpace = chosenFmt.ColorSpace,
            ImageExtent = extent, ImageArrayLayers = 1, ImageUsage = ImageUsageFlags.ColorAttachmentBit,
            ImageSharingMode = SharingMode.Exclusive, PreTransform = caps.CurrentTransform,
            CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr, PresentMode = presentMode,
            OldSwapchain = oldSwapchain, Clipped = Vk.True
        };

        SwapchainKHR sc;
        var createResult = functions.Create(device, &scCI, null, &sc);
        if (createResult != Result.Success)
            return (default(SwapchainKHR), Array.Empty<Image>(), default(SurfaceFormatKHR), default(Extent2D), default(PresentModeKHR), w, h,
                $"Swapchain 创建失败：{createResult}（请求尺寸 {w}x{h}）。");

        // Get images (two-stage + Incomplete retry)
        const int maxRetries = 3;
        Image[] swapchainImages = [];
        for (var attempt = 0; attempt < maxRetries; attempt++)
        {
            uint count = 0;
            if (functions.GetImages(device, sc, &count, null) != Result.Success)
                return (default(SwapchainKHR), Array.Empty<Image>(), default(SurfaceFormatKHR), default(Extent2D), default(PresentModeKHR), w, h,
                    $"GetSwapchainImages 第一阶段失败（尝试 {attempt + 1}）。");
            if (count == 0)
                return (default(SwapchainKHR), Array.Empty<Image>(), default(SurfaceFormatKHR), default(Extent2D), default(PresentModeKHR), w, h, "Swapchain 图像数为 0。");

            var buffer = new Image[count];
            uint written = count;
            fixed (Image* imgPtr = buffer)
            {
                var fillResult = functions.GetImages(device, sc, &written, imgPtr);
                if (fillResult == Result.Success) { swapchainImages = buffer; break; }
                if (fillResult != Result.Incomplete)
                    return (default(SwapchainKHR), Array.Empty<Image>(), default(SurfaceFormatKHR), default(Extent2D), default(PresentModeKHR), w, h,
                        $"GetSwapchainImages 第二阶段失败：{fillResult}。");
            }
        }
        if (swapchainImages.Length == 0)
            return (default(SwapchainKHR), Array.Empty<Image>(), default(SurfaceFormatKHR), default(Extent2D), default(PresentModeKHR), w, h,
                $"GetSwapchainImages 超过最大重试次数（{maxRetries}）。");

        return (sc, swapchainImages, chosenFmt, extent, presentMode, w, h, null);
    }
}

