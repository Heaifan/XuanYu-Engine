using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D;

/// <summary>诊断探针的 Surface 查询与格式选择辅助。</summary>
public static unsafe partial class VulkanScene3dRenderer
{
    static SurfaceCapabilitiesKHR QueryCaps(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR surf, nint fn)
    { var f = Marshal.GetDelegateForFunctionPointer<GetCapsPtr>(fn); SurfaceCapabilitiesKHR c; f(pd, surf, &c); return c; }
    static SurfaceFormatKHR[] QueryFormats(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR surf, nint fn)
    { var f = Marshal.GetDelegateForFunctionPointer<GetFormatsPtr>(fn); uint c = 0; if (f(pd, surf, &c, null) != Result.Success || c == 0) return []; var r = new SurfaceFormatKHR[c]; fixed (SurfaceFormatKHR* p = r) f(pd, surf, &c, p); return r; }
    static PresentModeKHR[] QueryModes(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR surf, nint fn)
    { var f = Marshal.GetDelegateForFunctionPointer<GetModesPtr>(fn); uint c = 0; if (f(pd, surf, &c, null) != Result.Success || c == 0) return []; var r = new PresentModeKHR[c]; fixed (PresentModeKHR* p = r) f(pd, surf, &c, p); return r; }
    static SurfaceFormatKHR ChooseFormat(SurfaceFormatKHR[] f)
    { foreach (var x in f) if (x.Format == Format.B8G8R8A8Srgb || x.Format == Format.R8G8B8A8Srgb) return x; return f[0]; }
    static PresentModeKHR ChoosePresentMode(PresentModeKHR[] m)
    { foreach (var x in m) if (x == PresentModeKHR.MailboxKhr || x == PresentModeKHR.ImmediateKhr) return x; return PresentModeKHR.FifoKhr; }
    static Extent2D ChooseExtent(SurfaceCapabilitiesKHR c, uint fw, uint fh)
    { if (c.CurrentExtent.Width != uint.MaxValue) return c.CurrentExtent; return new Extent2D(Math.Clamp(fw, c.MinImageExtent.Width, c.MaxImageExtent.Width), Math.Clamp(fh, c.MinImageExtent.Height, c.MaxImageExtent.Height)); }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)] public delegate Result GetCapsPtr(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR s, SurfaceCapabilitiesKHR* c);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] public delegate Result GetFormatsPtr(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR s, uint* c, SurfaceFormatKHR* f);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] public delegate Result GetModesPtr(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR s, uint* c, PresentModeKHR* m);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] public delegate Result CreateSwapchainPtr(Silk.NET.Vulkan.Device d, SwapchainCreateInfoKHR* ci, AllocationCallbacks* a, SwapchainKHR* sc);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] public delegate Result GetSwapchainImagesPtr(Silk.NET.Vulkan.Device d, SwapchainKHR sc, uint* c, Image* imgs);
}
