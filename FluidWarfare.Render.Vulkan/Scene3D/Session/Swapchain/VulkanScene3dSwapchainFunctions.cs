using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain;

/// <summary>
/// 不可变的 Swapchain 函数集合。
/// 所有函数指针在构造时加载并验证，缺失任何一个则创建失败。
/// 生命周期与 Session/Device 一致，避免公共可变 nint 字段风险。
/// </summary>
public sealed unsafe class VulkanScene3dSwapchainFunctions
{
    public required SwapchainCreateFunc Create { get; init; }
    public required SwapchainDestroyFunc Destroy { get; init; }
    public required SwapchainGetImagesFunc GetImages { get; init; }
    public required AcquireNextImageFunc AcquireNextImage { get; init; }
    public required QueuePresentFunc QueuePresent { get; init; }
    public required GetCapsFunc GetCapabilities { get; init; }
    public required GetFormatsFunc GetFormats { get; init; }
    public required GetPresentModesFunc GetPresentModes { get; init; }

    /// <summary>
    /// 从已加载的 nint 指针创建函数集合。
    /// 任一指针为 0 则返回 null。
    /// </summary>
    public static VulkanScene3dSwapchainFunctions? TryLoad(
        nint fnCreateSwapchain,
        nint fnDestroySwapchain,
        nint fnGetImages,
        nint fnAcquireNextImage,
        nint fnQueuePresent,
        nint fnGetCaps,
        nint fnGetFormats,
        nint fnGetModes)
    {
        if (fnCreateSwapchain == 0 || fnDestroySwapchain == 0 || fnGetImages == 0 ||
            fnAcquireNextImage == 0 || fnQueuePresent == 0 ||
            fnGetCaps == 0 || fnGetFormats == 0 || fnGetModes == 0)
            return null;

        return new VulkanScene3dSwapchainFunctions
        {
            Create = Marshal.GetDelegateForFunctionPointer<SwapchainCreateFunc>(fnCreateSwapchain),
            Destroy = Marshal.GetDelegateForFunctionPointer<SwapchainDestroyFunc>(fnDestroySwapchain),
            GetImages = Marshal.GetDelegateForFunctionPointer<SwapchainGetImagesFunc>(fnGetImages),
            AcquireNextImage = Marshal.GetDelegateForFunctionPointer<AcquireNextImageFunc>(fnAcquireNextImage),
            QueuePresent = Marshal.GetDelegateForFunctionPointer<QueuePresentFunc>(fnQueuePresent),
            GetCapabilities = Marshal.GetDelegateForFunctionPointer<GetCapsFunc>(fnGetCaps),
            GetFormats = Marshal.GetDelegateForFunctionPointer<GetFormatsFunc>(fnGetFormats),
            GetPresentModes = Marshal.GetDelegateForFunctionPointer<GetPresentModesFunc>(fnGetModes),
        };
    }

    // ─── 委托定义 ──────────────────────────────────────────────
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate Result SwapchainCreateFunc(Silk.NET.Vulkan.Device d, SwapchainCreateInfoKHR* ci, AllocationCallbacks* a, SwapchainKHR* sc);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void SwapchainDestroyFunc(Silk.NET.Vulkan.Device d, SwapchainKHR sc, AllocationCallbacks* a);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate Result SwapchainGetImagesFunc(Silk.NET.Vulkan.Device d, SwapchainKHR sc, uint* c, Image* imgs);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate Result AcquireNextImageFunc(Silk.NET.Vulkan.Device d, SwapchainKHR sc, ulong t, Silk.NET.Vulkan.Semaphore s, Fence f, uint* i);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate Result QueuePresentFunc(Queue q, PresentInfoKHR* p);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate Result GetCapsFunc(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR s, SurfaceCapabilitiesKHR* c);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate Result GetFormatsFunc(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR s, uint* c, SurfaceFormatKHR* f);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate Result GetPresentModesFunc(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR s, uint* c, PresentModeKHR* m);
}
