using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D;

/// <summary>诊断探针的 Swapchain 创建——函数指针、查询、选择、创建。</summary>
public static unsafe partial class VulkanScene3dRenderer
{
    static bool ProbeCreateSwapchain(VulkanScene3dRenderResources r,
        Silk.NET.Vulkan.PhysicalDevice pd, nint fnGetCaps, nint fnGetFmts, nint fnGetModes,
        nint fnCreateSwapchain, nint fnGetImages,
        uint reqW, uint reqH,
        out Extent2D extent, out Format chosenFmt, out Image[] images, out uint imgCount,
        out string error)
    {
        extent = default; chosenFmt = default; images = null!; imgCount = 0; error = string.Empty;

        var caps = QueryCaps(pd, r.Surface, fnGetCaps);
        var formats = QueryFormats(pd, r.Surface, fnGetFmts);
        if (formats.Length == 0) { error = "无可用 Surface 格式。"; return false; }
        chosenFmt = ChooseFormat(formats).Format;
        extent = ChooseExtent(caps, reqW, reqH);
        var imageCount = Math.Clamp(caps.MinImageCount + 1, caps.MinImageCount, caps.MaxImageCount > 0 ? caps.MaxImageCount : uint.MaxValue);
        var scCI = new SwapchainCreateInfoKHR { SType = StructureType.SwapchainCreateInfoKhr, Surface = r.Surface, MinImageCount = imageCount, ImageFormat = chosenFmt, ImageColorSpace = ChooseFormat(formats).ColorSpace, ImageExtent = extent, ImageArrayLayers = 1, ImageUsage = ImageUsageFlags.ColorAttachmentBit, ImageSharingMode = SharingMode.Exclusive, PreTransform = caps.CurrentTransform, CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr, PresentMode = ChoosePresentMode(QueryModes(pd, r.Surface, fnGetModes)), Clipped = Vk.True };
        var createScFn = Marshal.GetDelegateForFunctionPointer<CreateSwapchainPtr>(fnCreateSwapchain);
        SwapchainKHR sc;
        if (createScFn(r.Device, &scCI, null, &sc) != Result.Success) { error = "Swapchain 创建失败。"; return false; }
        r.Swapchain = sc; r.ScOk = true;
        var getImgsFn = Marshal.GetDelegateForFunctionPointer<GetSwapchainImagesPtr>(fnGetImages);
        var localImgCount = 0u; getImgsFn(r.Device, r.Swapchain, &localImgCount, null);
        if (localImgCount == 0) { error = "Swapchain 图像数为 0。"; return false; }
        images = new Image[localImgCount];
        fixed (Image* ip = images) getImgsFn(r.Device, r.Swapchain, &localImgCount, ip);
        imgCount = localImgCount;
        return true;
    }
}
