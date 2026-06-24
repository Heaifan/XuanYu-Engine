using System.Diagnostics;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Swapchain;

/// <summary>
/// 使用临时 Instance + Surface + Device 创建并立即释放 Swapchain。
/// 内部使用 ContextScope / DeviceSelector / SurfaceQuery 完成子任务。
/// </summary>
public static unsafe class VulkanSwapchainProbe
{
    public static VulkanSwapchainInfo ProbeWindows(nint hinstance, nint hwnd, uint fw, uint fh)
    {
        var sw = Stopwatch.StartNew();
        if (hinstance == 0 || hwnd == 0) return Fail("句柄不可用。", sw);

        using var scope = new VulkanSwapchainProbeContextScope();
        if (!scope.CreateInstance()) return Fail("Instance 创建失败。", sw);
        if (!scope.CreateSurface(hinstance, hwnd)) return Fail("Surface 创建失败。", sw);

        var selector = new VulkanSwapchainProbeDeviceSelector();
        if (!selector.TrySelect(scope.Vk, scope.Instance, scope.Surface, scope.FnSurfaceSupport,
                out var pd, out var qi, out _))
            return Fail("未找到 Graphics+Present 队列。", sw);

        if (!scope.CreateDevice(pd, qi)) return Fail("Device 创建失败。", sw);

        var query = new VulkanSwapchainProbeSurfaceQuery();
        var caps = query.QueryCaps(pd, scope.Surface, scope.FnGetCaps);
        var formats = query.QueryFormats(pd, scope.Surface, scope.FnGetFormats);
        if (formats.Length == 0) return Fail("无可用 Surface 格式。", sw);
        var modes = query.QueryModes(pd, scope.Surface, scope.FnGetModes);
        var cf = query.ChooseFormat(formats);
        var cm = query.ChoosePresentMode(modes);
        var extent = query.ChooseExtent(caps, fw, fh);

        var ic = Math.Clamp(caps.MinImageCount + 1, caps.MinImageCount,
            caps.MaxImageCount > 0 ? caps.MaxImageCount : uint.MaxValue);

        var ci = new SwapchainCreateInfoKHR
        {
            SType = StructureType.SwapchainCreateInfoKhr, Surface = scope.Surface,
            MinImageCount = ic, ImageFormat = cf.Format,
            ImageColorSpace = cf.ColorSpace, ImageExtent = extent,
            ImageArrayLayers = 1, ImageUsage = ImageUsageFlags.ColorAttachmentBit,
            ImageSharingMode = SharingMode.Exclusive, PreTransform = caps.CurrentTransform,
            CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
            PresentMode = cm, Clipped = Vk.True
        };

        var fnCreate = Marshal.GetDelegateForFunctionPointer<CreateSwapchainKHRPtr>(scope.FnCreateSwapchain);
        SwapchainKHR sc;
        if (fnCreate(scope.Device, &ci, null, &sc) != Result.Success)
            return Fail("Swapchain 创建失败。", sw);

        var fnGetImages = Marshal.GetDelegateForFunctionPointer<GetSwapchainImagesKHRPtr>(scope.FnGetSwapchainImages);
        uint actualCount = 0;
        fnGetImages(scope.Device, sc, &actualCount, null);

        Marshal.GetDelegateForFunctionPointer<DestroySwapchainKHRPtr>(scope.FnDestroySwapchain)(scope.Device, sc, null);

        sw.Stop();
        return new VulkanSwapchainInfo(VulkanSwapchainStatus.Created,
            "创建成功，并已立即释放。", actualCount,
            cf.Format.ToString(), cm.ToString(), extent.Width, extent.Height, sw.Elapsed.TotalMilliseconds);
    }

    static VulkanSwapchainInfo Fail(string msg, Stopwatch sw) =>
        new(VulkanSwapchainStatus.Failed, msg, 0, "未知", "未知", 0, 0, sw.Elapsed.TotalMilliseconds);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate Result CreateSwapchainKHRPtr(Silk.NET.Vulkan.Device d, SwapchainCreateInfoKHR* ci, AllocationCallbacks* a, SwapchainKHR* s);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate void DestroySwapchainKHRPtr(Silk.NET.Vulkan.Device d, SwapchainKHR s, AllocationCallbacks* a);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate Result GetSwapchainImagesKHRPtr(Silk.NET.Vulkan.Device d, SwapchainKHR s, uint* c, Image* i);
}
