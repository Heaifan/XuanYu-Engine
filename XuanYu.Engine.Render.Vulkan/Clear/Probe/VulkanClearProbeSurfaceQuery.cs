using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Clear;

sealed unsafe class VulkanClearProbeSurfaceQuery
{
    public SurfaceCapabilitiesKHR QueryCaps(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR surf, nint fn)
    {
        var f = Marshal.GetDelegateForFunctionPointer<GetCapsPtr>(fn);
        SurfaceCapabilitiesKHR c;
        f(pd, surf, &c);
        return c;
    }

    public SurfaceFormatKHR[] QueryFormats(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR surf, nint fn)
    {
        var f = Marshal.GetDelegateForFunctionPointer<GetFormatsPtr>(fn);
        uint c = 0;
        if (f(pd, surf, &c, null) != Result.Success || c == 0) return [];
        var r = new SurfaceFormatKHR[c];
        fixed (SurfaceFormatKHR* p = r) f(pd, surf, &c, p);
        return r;
    }

    public PresentModeKHR[] QueryModes(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR surf, nint fn)
    {
        var f = Marshal.GetDelegateForFunctionPointer<GetModesPtr>(fn);
        uint c = 0;
        if (f(pd, surf, &c, null) != Result.Success || c == 0) return [];
        var r = new PresentModeKHR[c];
        fixed (PresentModeKHR* p = r) f(pd, surf, &c, p);
        return r;
    }

    public SurfaceFormatKHR ChooseFormat(SurfaceFormatKHR[] formats)
    {
        foreach (var x in formats)
            if (x.Format == Format.B8G8R8A8Srgb || x.Format == Format.R8G8B8A8Srgb) return x;
        return formats[0];
    }

    public PresentModeKHR ChoosePresentMode(PresentModeKHR[] modes)
    {
        foreach (var x in modes)
            if (x == PresentModeKHR.MailboxKhr || x == PresentModeKHR.ImmediateKhr) return x;
        return PresentModeKHR.FifoKhr;
    }

    public Extent2D ChooseExtent(SurfaceCapabilitiesKHR c, uint fw, uint fh)
    {
        if (c.CurrentExtent.Width != uint.MaxValue) return c.CurrentExtent;
        return new Extent2D(Math.Clamp(fw, c.MinImageExtent.Width, c.MaxImageExtent.Width),
            Math.Clamp(fh, c.MinImageExtent.Height, c.MaxImageExtent.Height));
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)] delegate Result GetCapsPtr(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR s, SurfaceCapabilitiesKHR* c);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] delegate Result GetFormatsPtr(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR s, uint* c, SurfaceFormatKHR* f);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] delegate Result GetModesPtr(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR s, uint* c, PresentModeKHR* m);
}
