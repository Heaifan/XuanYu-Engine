using System.Diagnostics;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Swapchain;

/// <summary>
/// 使用 vkGetDeviceProcAddr 加载 swapchain 函数，创建并立即释放 Swapchain。
/// 使用 SwapchainKHR* 而非 out 参数避免调用约定问题。
/// </summary>
public static unsafe class VulkanSwapchainProbe
{
    public static VulkanSwapchainInfo ProbeWindows(nint hinstance, nint hwnd, uint fallbackWidth, uint fallbackHeight)
    {
        var sw = Stopwatch.StartNew();
        Vk? vk = null;
        Silk.NET.Vulkan.Instance instance = default;
        SurfaceKHR surface = default;
        Silk.NET.Vulkan.Device device = default;
        SwapchainKHR swapchain = default;
        bool instanceCreated = false, surfaceCreated = false, deviceCreated = false, swapchainCreated = false;
        nint fnDestroySwapchain = 0;
        nint fnDestroySurface = 0;

        try
        {
            vk = Vk.GetApi();
            if (hinstance == 0 || hwnd == 0) return Fail("句柄不可用。", sw);
            if (!CreateInstance(vk, out instance)) return Fail("Instance 创建失败。", sw);
            instanceCreated = true;

            fnDestroySurface = LoadInstanceProc(vk, instance, "vkDestroySurfaceKHR");
            if (!CreateSurface(vk, instance, hinstance, hwnd, out surface)) return Fail("Surface 创建失败。", sw);
            surfaceCreated = true;

            if (!SelectPhysicalDevice(vk, instance, surface, out var pd, out var qi, out _)) return Fail("未找到 Graphics+Present 队列。", sw);

            if (!CreateDevice(vk, pd, qi, out device)) return Fail("Device 创建失败。", sw);
            deviceCreated = true;

            // Load device-level swapchain functions
            fnDestroySwapchain = LoadDeviceProc(vk, device, "vkDestroySwapchainKHR");
            var fnCreateSwapchain = LoadDeviceProc(vk, device, "vkCreateSwapchainKHR");
            var fnGetCaps = LoadDeviceProc(vk, device, "vkGetPhysicalDeviceSurfaceCapabilitiesKHR");
            var fnGetFormats = LoadDeviceProc(vk, device, "vkGetPhysicalDeviceSurfaceFormatsKHR");
            var fnGetModes = LoadDeviceProc(vk, device, "vkGetPhysicalDeviceSurfacePresentModesKHR");
            var fnGetImages = LoadDeviceProc(vk, device, "vkGetSwapchainImagesKHR");

            if (fnCreateSwapchain == 0 || fnGetCaps == 0 || fnGetFormats == 0 || fnGetModes == 0 || fnGetImages == 0 || fnDestroySwapchain == 0)
                return Fail("无法加载 Swapchain 扩展函数。", sw);

            // Query surface capabilities
            var caps = QuerySurfaceCaps(pd, instance, surface, fnGetCaps);

            // Query formats
            var formats = QuerySurfaceFormats(pd, instance, surface, fnGetFormats);
            if (formats.Length == 0) return Fail("无可用 Surface 格式。", sw);
            var chosenFormat = ChooseFormat(formats);

            // Query present modes
            var modes = QueryPresentModes(pd, instance, surface, fnGetModes);
            var chosenMode = ChoosePresentMode(modes);

            var extent = ChooseExtent(caps, fallbackWidth, fallbackHeight);
            var imageCount = Math.Clamp(caps.MinImageCount + 1, caps.MinImageCount, caps.MaxImageCount > 0 ? caps.MaxImageCount : uint.MaxValue);

            // Create swapchain
            var ci = new SwapchainCreateInfoKHR
            {
                SType = StructureType.SwapchainCreateInfoKhr, Surface = surface,
                MinImageCount = imageCount, ImageFormat = chosenFormat.Format,
                ImageColorSpace = chosenFormat.ColorSpace, ImageExtent = extent,
                ImageArrayLayers = 1, ImageUsage = ImageUsageFlags.ColorAttachmentBit,
                ImageSharingMode = SharingMode.Exclusive, PreTransform = caps.CurrentTransform,
                CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
                PresentMode = chosenMode, Clipped = Vk.True
            };

            var createSwapchainFn = Marshal.GetDelegateForFunctionPointer<CreateSwapchainKHRPointer>(fnCreateSwapchain);
            SwapchainKHR sc;
            if (createSwapchainFn(device, &ci, null, &sc) != Result.Success)
                return Fail("Swapchain 创建失败。", sw);
            swapchain = sc;
            swapchainCreated = true;

            // Get image count
            var getImagesFn = Marshal.GetDelegateForFunctionPointer<GetSwapchainImagesKHRPointer>(fnGetImages);
            uint actualCount = 0;
            getImagesFn(device, swapchain, &actualCount, null);

            sw.Stop();
            var result = new VulkanSwapchainInfo(VulkanSwapchainStatus.Created,
                "创建成功，并已立即释放。", actualCount,
                chosenFormat.Format.ToString(), chosenMode.ToString(),
                extent.Width, extent.Height, sw.Elapsed.TotalMilliseconds);

            // Done - cleanup happens in finally
            return result;
        }
        finally
        {
            if (swapchainCreated && fnDestroySwapchain != 0)
                Marshal.GetDelegateForFunctionPointer<DestroySwapchainKHRPointer>(fnDestroySwapchain)(device, swapchain, null);
            if (deviceCreated && vk != null && device.Handle != 0) vk.DestroyDevice(device, null);
            if (surfaceCreated && fnDestroySurface != 0 && vk != null)
                Marshal.GetDelegateForFunctionPointer<DestroySurfaceKHRPointer>(fnDestroySurface)(instance, surface, null);
            if (instanceCreated && vk != null) vk.DestroyInstance(instance, null);
        }
    }

    // ─── 函数地址加载 ──────────────────────────────────────────

    private static nint LoadInstanceProc(Vk vk, Silk.NET.Vulkan.Instance inst, string name)
    {
        var p = Marshal.StringToHGlobalAnsi(name);
        try { return (nint)vk.GetInstanceProcAddr(inst, (byte*)p); }
        finally { Marshal.FreeHGlobal(p); }
    }

    private static nint LoadDeviceProc(Vk vk, Silk.NET.Vulkan.Device dev, string name)
    {
        var p = Marshal.StringToHGlobalAnsi(name);
        try { return (nint)vk.GetDeviceProcAddr(dev, (byte*)p); }
        finally { Marshal.FreeHGlobal(p); }
    }

    // ─── SurfaceCapabilities ────────────────────────────────────

    private static SurfaceCapabilitiesKHR QuerySurfaceCaps(Silk.NET.Vulkan.PhysicalDevice pd, Silk.NET.Vulkan.Instance inst, SurfaceKHR surf, nint fnPtr)
    {
        var fn = Marshal.GetDelegateForFunctionPointer<GetSurfaceCapabilitiesPointer>(fnPtr);
        fn(pd, inst, surf, out var caps);
        return caps;
    }

    private static SurfaceFormatKHR[] QuerySurfaceFormats(Silk.NET.Vulkan.PhysicalDevice pd, Silk.NET.Vulkan.Instance inst, SurfaceKHR surf, nint fnPtr)
    {
        var fn = Marshal.GetDelegateForFunctionPointer<GetSurfaceFormatsPointer>(fnPtr);
        uint count = 0;
        fn(pd, inst, surf, &count, null);
        if (count == 0) return [];
        var formats = new SurfaceFormatKHR[(int)count];
        fixed (SurfaceFormatKHR* f = formats) fn(pd, inst, surf, &count, f);
        return formats;
    }

    private static PresentModeKHR[] QueryPresentModes(Silk.NET.Vulkan.PhysicalDevice pd, Silk.NET.Vulkan.Instance inst, SurfaceKHR surf, nint fnPtr)
    {
        var fn = Marshal.GetDelegateForFunctionPointer<GetSurfacePresentModesPointer>(fnPtr);
        uint count = 0;
        fn(pd, inst, surf, &count, null);
        if (count == 0) return [];
        var modes = new PresentModeKHR[(int)count];
        fixed (PresentModeKHR* p = modes) fn(pd, inst, surf, &count, p);
        return modes;
    }

    // ─── Instance ───────────────────────────────────────────────

    private static bool CreateInstance(Vk vk, out Silk.NET.Vulkan.Instance inst)
    {
        inst = default;
        var a = Marshal.StringToHGlobalAnsi("FluidWarfare");
        var e = Marshal.StringToHGlobalAnsi("FluidWarfare");
        var s = Marshal.StringToHGlobalAnsi("VK_KHR_surface");
        var w = Marshal.StringToHGlobalAnsi("VK_KHR_win32_surface");
        try
        {
            var exts = stackalloc byte*[] { (byte*)s, (byte*)w };
            var ai = new ApplicationInfo { SType = StructureType.ApplicationInfo, PApplicationName = (byte*)a, ApplicationVersion = 1, PEngineName = (byte*)e, EngineVersion = 1, ApiVersion = PackApiVersion(1, 0, 0) };
            var ci = new InstanceCreateInfo { SType = StructureType.InstanceCreateInfo, PApplicationInfo = &ai, EnabledExtensionCount = 2, PpEnabledExtensionNames = exts };
            return vk.CreateInstance(&ci, null, out inst) == Result.Success;
        }
        finally { Marshal.FreeHGlobal(a); Marshal.FreeHGlobal(e); Marshal.FreeHGlobal(s); Marshal.FreeHGlobal(w); }
    }

    private static bool CreateSurface(Vk vk, Silk.NET.Vulkan.Instance inst, nint hinstance, nint hwnd, out SurfaceKHR surf)
    {
        surf = default;
        var namePtr = Marshal.StringToHGlobalAnsi("vkCreateWin32SurfaceKHR");
        try
        {
            var addr = (nint)vk.GetInstanceProcAddr(inst, (byte*)namePtr);
            if (addr == 0) return false;
            var fn = Marshal.GetDelegateForFunctionPointer<CreateWin32SurfaceKHRPointer>(addr);
            var ci = new Win32SurfaceCreateInfoKHR { SType = StructureType.Win32SurfaceCreateInfoKhr, Hinstance = hinstance, Hwnd = hwnd };
            fixed (SurfaceKHR* pSurf = &surf)
                return fn(inst, &ci, null, pSurf) == Result.Success;
        }
        finally { Marshal.FreeHGlobal(namePtr); }
    }

    // ─── Physical / Logical Device ──────────────────────────────

    private static bool SelectPhysicalDevice(Vk vk, Silk.NET.Vulkan.Instance inst, SurfaceKHR surf,
        out Silk.NET.Vulkan.PhysicalDevice pd, out uint qi, out string name)
    {
        pd = default; qi = 0; name = "未知";
        uint count = 0;
        if (vk.EnumeratePhysicalDevices(inst, ref count, null) != Result.Success || count == 0) return false;
        var devices = new Silk.NET.Vulkan.PhysicalDevice[count];
        fixed (Silk.NET.Vulkan.PhysicalDevice* p = devices) vk.EnumeratePhysicalDevices(inst, ref count, p);

        var fnSurfaceSupport = LoadInstanceProc(vk, inst, "vkGetPhysicalDeviceSurfaceSupportKHR");
        if (fnSurfaceSupport == 0) return false;
        var supportFn = Marshal.GetDelegateForFunctionPointer<GetPhysicalDeviceSurfaceSupportPointer>(fnSurfaceSupport);

        foreach (var d in devices)
        {
            vk.GetPhysicalDeviceProperties(d, out var props);
            name = Marshal.PtrToStringAnsi((nint)props.DeviceName) ?? "未知";
            uint qc = 0;
            vk.GetPhysicalDeviceQueueFamilyProperties(d, ref qc, null);
            var qProps = new QueueFamilyProperties[qc];
            fixed (QueueFamilyProperties* qp = qProps) vk.GetPhysicalDeviceQueueFamilyProperties(d, ref qc, qp);

            for (uint i = 0; i < qc; i++)
            {
                if (qProps[i].QueueCount > 0 && qProps[i].QueueFlags.HasFlag(QueueFlags.GraphicsBit))
                {
                    int presentSupported = 0;
                    supportFn(d, i, surf, &presentSupported);
                    if (presentSupported != 0) { pd = d; qi = i; return true; }
                }
            }
        }
        return false;
    }

    private static bool CreateDevice(Vk vk, Silk.NET.Vulkan.PhysicalDevice pd, uint qi, out Silk.NET.Vulkan.Device dev)
    {
        dev = default;
        var qp = 1.0f;
        var qci = new DeviceQueueCreateInfo { SType = StructureType.DeviceQueueCreateInfo, QueueFamilyIndex = qi, QueueCount = 1, PQueuePriorities = &qp };
        var swapchainExt = Marshal.StringToHGlobalAnsi("VK_KHR_swapchain");
        try
        {
            var exts = stackalloc byte*[] { (byte*)swapchainExt };
            var dci = new DeviceCreateInfo { SType = StructureType.DeviceCreateInfo, QueueCreateInfoCount = 1, PQueueCreateInfos = &qci, EnabledExtensionCount = 1, PpEnabledExtensionNames = exts };
            return vk.CreateDevice(pd, &dci, null, out dev) == Result.Success;
        }
        finally { Marshal.FreeHGlobal(swapchainExt); }
    }

    // ─── Helpers ────────────────────────────────────────────────

    private static SurfaceFormatKHR ChooseFormat(SurfaceFormatKHR[] f)
    {
        foreach (var x in f) if (x.Format == Format.B8G8R8A8Srgb || x.Format == Format.R8G8B8A8Srgb) return x;
        return f[0];
    }

    private static PresentModeKHR ChoosePresentMode(PresentModeKHR[] m)
    {
        foreach (var x in m) if (x == PresentModeKHR.MailboxKhr || x == PresentModeKHR.ImmediateKhr) return x;
        return PresentModeKHR.FifoKhr;
    }

    private static Extent2D ChooseExtent(SurfaceCapabilitiesKHR caps, uint fw, uint fh)
    {
        if (caps.CurrentExtent.Width != uint.MaxValue) return caps.CurrentExtent;
        return new Extent2D(Math.Clamp(fw, caps.MinImageExtent.Width, caps.MaxImageExtent.Width),
            Math.Clamp(fh, caps.MinImageExtent.Height, caps.MaxImageExtent.Height));
    }

    private static VulkanSwapchainInfo Fail(string msg, Stopwatch sw) => new(VulkanSwapchainStatus.Failed, msg, 0, "未知", "未知", 0, 0, sw.Elapsed.TotalMilliseconds);
    private static uint PackApiVersion(uint a, uint b, uint c) => (a << 22) | (b << 12) | c;

    // ─── 委托（全部使用指针，不用 out） ─────────────────────────

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result CreateWin32SurfaceKHRPointer(Silk.NET.Vulkan.Instance inst, Win32SurfaceCreateInfoKHR* ci, AllocationCallbacks* alloc, SurfaceKHR* surf);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate void DestroySurfaceKHRPointer(Silk.NET.Vulkan.Instance inst, SurfaceKHR surf, AllocationCallbacks* alloc);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate void GetSurfaceCapabilitiesPointer(Silk.NET.Vulkan.PhysicalDevice pd, Silk.NET.Vulkan.Instance inst, SurfaceKHR surf, out SurfaceCapabilitiesKHR caps);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result GetSurfaceFormatsPointer(Silk.NET.Vulkan.PhysicalDevice pd, Silk.NET.Vulkan.Instance inst, SurfaceKHR surf, uint* count, SurfaceFormatKHR* formats);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result GetSurfacePresentModesPointer(Silk.NET.Vulkan.PhysicalDevice pd, Silk.NET.Vulkan.Instance inst, SurfaceKHR surf, uint* count, PresentModeKHR* modes);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result GetPhysicalDeviceSurfaceSupportPointer(Silk.NET.Vulkan.PhysicalDevice pd, uint qi, SurfaceKHR surf, int* supported);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result CreateSwapchainKHRPointer(Silk.NET.Vulkan.Device device, SwapchainCreateInfoKHR* ci, AllocationCallbacks* alloc, SwapchainKHR* swapchain);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate void DestroySwapchainKHRPointer(Silk.NET.Vulkan.Device device, SwapchainKHR swapchain, AllocationCallbacks* alloc);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result GetSwapchainImagesKHRPointer(Silk.NET.Vulkan.Device device, SwapchainKHR swapchain, uint* count, Image* images);
}
