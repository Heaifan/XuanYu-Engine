using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Swapchain;

/// <summary>SurfaceCapabilities / SurfaceFormats / PresentModes 查询 + 格式选择。供 VulkanSwapchainProbe 内部使用。</summary>
sealed unsafe class VulkanSwapchainProbeSurfaceQuery
{
    public SurfaceCapabilitiesKHR QueryCaps(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR surf, nint fnPtr)
    {
        var fn = Marshal.GetDelegateForFunctionPointer<GetCapsPtr>(fnPtr);
        SurfaceCapabilitiesKHR caps;
        fn(pd, surf, &caps);
        return caps;
    }

    public SurfaceFormatKHR[] QueryFormats(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR surf, nint fnPtr)
    {
        var fn = Marshal.GetDelegateForFunctionPointer<GetFormatsPtr>(fnPtr);
        uint count = 0;
        if (fn(pd, surf, &count, null) != Result.Success || count == 0) return [];
        var formats = new SurfaceFormatKHR[(int)count];
        fixed (SurfaceFormatKHR* f = formats) fn(pd, surf, &count, f);
        return formats;
    }

    public PresentModeKHR[] QueryModes(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR surf, nint fnPtr)
    {
        var fn = Marshal.GetDelegateForFunctionPointer<GetModesPtr>(fnPtr);
        uint count = 0;
        if (fn(pd, surf, &count, null) != Result.Success || count == 0) return [];
        var modes = new PresentModeKHR[(int)count];
        fixed (PresentModeKHR* m = modes) fn(pd, surf, &count, m);
        return modes;
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

    public Extent2D ChooseExtent(SurfaceCapabilitiesKHR caps, uint fw, uint fh)
    {
        if (caps.CurrentExtent.Width != uint.MaxValue) return caps.CurrentExtent;
        return new Extent2D(Math.Clamp(fw, caps.MinImageExtent.Width, caps.MaxImageExtent.Width),
            Math.Clamp(fh, caps.MinImageExtent.Height, caps.MaxImageExtent.Height));
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate Result GetCapsPtr(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR surf, SurfaceCapabilitiesKHR* caps);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate Result GetFormatsPtr(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR surf, uint* count, SurfaceFormatKHR* f);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate Result GetModesPtr(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR surf, uint* count, PresentModeKHR* m);
}
