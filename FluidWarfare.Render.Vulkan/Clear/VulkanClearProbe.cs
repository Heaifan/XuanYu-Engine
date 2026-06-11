using System.Diagnostics;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Clear;

/// <summary>
/// 基于 Windows HWND/HINSTANCE 创建最小 Vulkan 渲染链路，执行一次清屏并释放资源。
/// 不创建 Shader/Pipeline/Mesh/Texture，不绘制 RenderScene 对象。
/// 使用 7.8.1 已验证的指针参数 delegate 模式，避免 0xC0000005。
/// </summary>
public static unsafe class VulkanClearProbe
{
    /* 清屏颜色：深蓝色，与空窗口黑色背景明显区分 */
    private const float ClearR = 0.03f;
    private const float ClearG = 0.08f;
    private const float ClearB = 0.18f;
    private const float ClearA = 1.0f;

    public static VulkanClearInfo ProbeWindows(nint hinstance, nint hwnd, uint reqW, uint reqH)
    {
        var sw = Stopwatch.StartNew();
        Vk? vk = null;
        Silk.NET.Vulkan.Instance inst = default;
        SurfaceKHR surf = default;
        Silk.NET.Vulkan.Device dev = default;
        SwapchainKHR swapchain = default;
        ImageView[] imageViews = [];
        RenderPass renderPass = default;
        Framebuffer[] framebuffers = [];
        CommandPool cmdPool = default;
        CommandBuffer cmdBuf = default;
        Silk.NET.Vulkan.Semaphore semAvail = default, semFin = default;
        Fence fence = default;

        bool instOk = false, surfOk = false, devOk = false, scOk = false;
        bool rpOk = false, poolOk = false, syncOk = false;
        uint imageCount = 0;
        Format chosenFmt = Format.Undefined;
        nint fnDestroySurface = 0, fnDestroySwapchain = 0;
        var clearText = $"rgba({ClearR:F2}, {ClearG:F2}, {ClearB:F2}, {ClearA:F2})";

        try
        {
            vk = Vk.GetApi();
            if (hinstance == 0 || hwnd == 0) return Fail("句柄不可用。", clearText, sw);

            // 1. Instance
            if (!CreateInstance(vk, out inst)) return Fail("Instance 创建失败。", clearText, sw);
            instOk = true;

            fnDestroySurface = LoadProc(vk, inst, "vkDestroySurfaceKHR");

            // 2. Surface
            if (!CreateSurface(vk, inst, hinstance, hwnd, out surf)) return Fail("Surface 创建失败。", clearText, sw);
            surfOk = true;

            // 3. Physical Device
            if (!SelectDevice(vk, inst, surf, out var pd, out var qi, out _)) return Fail("未找到 Graphics+Present 队列。", clearText, sw);

            // 4. Logical Device
            if (!CreateDevice(vk, pd, qi, out dev)) return Fail("Device 创建失败。", clearText, sw);
            devOk = true;

            // Load device functions
            fnDestroySwapchain = LoadDeviceProc(vk, dev, "vkDestroySwapchainKHR");
            var fnCreateSwapchain = LoadDeviceProc(vk, dev, "vkCreateSwapchainKHR");
            var fnGetCaps = LoadDeviceProc(vk, dev, "vkGetPhysicalDeviceSurfaceCapabilitiesKHR");
            var fnGetFmts = LoadDeviceProc(vk, dev, "vkGetPhysicalDeviceSurfaceFormatsKHR");
            var fnGetModes = LoadDeviceProc(vk, dev, "vkGetPhysicalDeviceSurfacePresentModesKHR");
            var fnGetImages = LoadDeviceProc(vk, dev, "vkGetSwapchainImagesKHR");
            var fnAcquire = LoadDeviceProc(vk, dev, "vkAcquireNextImageKHR");
            var fnQueuePresent = LoadDeviceProc(vk, dev, "vkQueuePresentKHR");

            if (fnCreateSwapchain == 0 || fnGetCaps == 0 || fnGetFmts == 0 || fnGetModes == 0 ||
                fnGetImages == 0 || fnAcquire == 0 || fnQueuePresent == 0 || fnDestroySwapchain == 0)
                return Fail("无法加载 Swapchain 扩展函数。", clearText, sw);

            // 5. Query capabilities
            var caps = QueryCaps(pd, inst, surf, fnGetCaps);
            var formats = QueryFormats(pd, inst, surf, fnGetFmts);
            if (formats.Length == 0) return Fail("无可用 Surface 格式。", clearText, sw);
            chosenFmt = ChooseFormat(formats).Format;
            var chosenMode = ChoosePresentMode(QueryModes(pd, inst, surf, fnGetModes));

            imageCount = Math.Clamp(caps.MinImageCount + 1, caps.MinImageCount, caps.MaxImageCount > 0 ? caps.MaxImageCount : uint.MaxValue);
            var extent = ChooseExtent(caps, reqW, reqH);

            // 6. Create Swapchain
            var scCI = new SwapchainCreateInfoKHR
            {
                SType = StructureType.SwapchainCreateInfoKhr, Surface = surf,
                MinImageCount = imageCount, ImageFormat = chosenFmt,
                ImageColorSpace = ChooseFormat(formats).ColorSpace,
                ImageExtent = extent, ImageArrayLayers = 1,
                ImageUsage = ImageUsageFlags.ColorAttachmentBit,
                ImageSharingMode = SharingMode.Exclusive,
                PreTransform = caps.CurrentTransform,
                CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
                PresentMode = chosenMode, Clipped = Vk.True
            };

            var createScFn = Marshal.GetDelegateForFunctionPointer<CreateSwapchainPtr>(fnCreateSwapchain);
            SwapchainKHR sc;
            if (createScFn(dev, &scCI, null, &sc) != Result.Success) return Fail("Swapchain 创建失败。", clearText, sw);
            swapchain = sc; scOk = true;

            var getImgsFn = Marshal.GetDelegateForFunctionPointer<GetSwapchainImagesPtr>(fnGetImages);
            uint imgCount = 0;
            getImgsFn(dev, swapchain, &imgCount, null);
            if (imgCount == 0) return Fail("Swapchain 图像数为 0。", clearText, sw);
            var images = new Image[imgCount];
            fixed (Image* ip = images) getImgsFn(dev, swapchain, &imgCount, ip);

            // 7. ImageViews
            imageViews = new ImageView[imgCount];
            for (var i = 0; i < imgCount; i++)
            {
                var ivCI = new ImageViewCreateInfo
                {
                    SType = StructureType.ImageViewCreateInfo, Image = images[i],
                    ViewType = ImageViewType.Type2D, Format = chosenFmt,
                    Components = new ComponentMapping { R = ComponentSwizzle.Identity, G = ComponentSwizzle.Identity, B = ComponentSwizzle.Identity, A = ComponentSwizzle.Identity },
                    SubresourceRange = new ImageSubresourceRange { AspectMask = ImageAspectFlags.ColorBit, BaseMipLevel = 0, LevelCount = 1, BaseArrayLayer = 0, LayerCount = 1 }
                };
                if (vk.CreateImageView(dev, &ivCI, null, out imageViews[i]) != Result.Success)
                    return Fail($"ImageView {i} 创建失败。", clearText, sw);
            }

            // 8. RenderPass
            var colorAtt = new AttachmentDescription
            {
                Format = chosenFmt, Samples = SampleCountFlags.Count1Bit,
                LoadOp = AttachmentLoadOp.Clear, StoreOp = AttachmentStoreOp.Store,
                StencilLoadOp = AttachmentLoadOp.DontCare, StencilStoreOp = AttachmentStoreOp.DontCare,
                InitialLayout = ImageLayout.Undefined, FinalLayout = ImageLayout.PresentSrcKhr
            };
            var colorRef = new AttachmentReference { Attachment = 0, Layout = ImageLayout.ColorAttachmentOptimal };
            var subpass = new SubpassDescription { PipelineBindPoint = PipelineBindPoint.Graphics, ColorAttachmentCount = 1, PColorAttachments = &colorRef };
            var rpCI = new RenderPassCreateInfo { SType = StructureType.RenderPassCreateInfo, AttachmentCount = 1, PAttachments = &colorAtt, SubpassCount = 1, PSubpasses = &subpass };
            if (vk.CreateRenderPass(dev, &rpCI, null, out renderPass) != Result.Success)
                return Fail("RenderPass 创建失败。", clearText, sw);
            rpOk = true;

            // 9. Framebuffers
            framebuffers = new Framebuffer[imgCount];
            for (var i = 0; i < imgCount; i++)
            {
                var att = stackalloc[] { imageViews[i] };
                var fbCI = new FramebufferCreateInfo { SType = StructureType.FramebufferCreateInfo, RenderPass = renderPass, AttachmentCount = 1, PAttachments = att, Width = extent.Width, Height = extent.Height, Layers = 1 };
                if (vk.CreateFramebuffer(dev, &fbCI, null, out framebuffers[i]) != Result.Success)
                    return Fail($"Framebuffer {i} 创建失败。", clearText, sw);
            }

            // 10. CommandPool
            var poolCI = new CommandPoolCreateInfo { SType = StructureType.CommandPoolCreateInfo, QueueFamilyIndex = qi };
            if (vk.CreateCommandPool(dev, &poolCI, null, out cmdPool) != Result.Success)
                return Fail("CommandPool 创建失败。", clearText, sw);
            poolOk = true;

            // 11. CommandBuffer
            var allocCI = new CommandBufferAllocateInfo { SType = StructureType.CommandBufferAllocateInfo, CommandPool = cmdPool, Level = CommandBufferLevel.Primary, CommandBufferCount = 1 };
            if (vk.AllocateCommandBuffers(dev, &allocCI, out cmdBuf) != Result.Success)
                return Fail("CommandBuffer 创建失败。", clearText, sw);

            // 12. Sync objects
            var semCI = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo };
            var fenceCI = new FenceCreateInfo { SType = StructureType.FenceCreateInfo, Flags = FenceCreateFlags.SignaledBit };
            if (vk.CreateSemaphore(dev, &semCI, null, out semAvail) != Result.Success ||
                vk.CreateSemaphore(dev, &semCI, null, out semFin) != Result.Success ||
                vk.CreateFence(dev, &fenceCI, null, out fence) != Result.Success)
                return Fail("同步对象创建失败。", clearText, sw);
            syncOk = true;

            // 13. AcquireNextImage
            vk.WaitForFences(dev, 1, ref fence, Vk.True, ulong.MaxValue);
            vk.ResetFences(dev, 1, ref fence);

            uint imgIndex = 0;
            var acquireFn = Marshal.GetDelegateForFunctionPointer<AcquireNextImagePtr>(fnAcquire);
            var acqRes = acquireFn(dev, swapchain, ulong.MaxValue, semAvail, default, &imgIndex);
            if (acqRes == Result.ErrorOutOfDateKhr) return Fail("Acquire 返回 OutOfDate。", clearText, sw);
            if (acqRes != Result.Success && acqRes != Result.SuboptimalKhr)
                return Fail($"AcquireNextImage 失败：{acqRes}。", clearText, sw);

            // 14. Record command buffer
            vk.ResetCommandBuffer(cmdBuf, CommandBufferResetFlags.None);
            var beginInfo = new CommandBufferBeginInfo { SType = StructureType.CommandBufferBeginInfo, Flags = CommandBufferUsageFlags.OneTimeSubmitBit };
            vk.BeginCommandBuffer(cmdBuf, &beginInfo);

            var clearVal = new ClearValue { Color = new ClearColorValue { Float32_0 = ClearR, Float32_1 = ClearG, Float32_2 = ClearB, Float32_3 = ClearA } };
            var rpBegin = new RenderPassBeginInfo
            {
                SType = StructureType.RenderPassBeginInfo, RenderPass = renderPass,
                Framebuffer = framebuffers[imgIndex],
                RenderArea = new Rect2D(new Offset2D(0, 0), extent),
                ClearValueCount = 1, PClearValues = &clearVal
            };
            vk.CmdBeginRenderPass(cmdBuf, &rpBegin, SubpassContents.Inline);
            vk.CmdEndRenderPass(cmdBuf);
            vk.EndCommandBuffer(cmdBuf);

            var queue = default(Queue);
            vk.GetDeviceQueue(dev, qi, 0, out queue);

            // 15. QueueSubmit
            var waitSem = stackalloc[] { semAvail };
            var waitStage = stackalloc[] { PipelineStageFlags.ColorAttachmentOutputBit };
            var sigSem = stackalloc[] { semFin };
            var cBufs = stackalloc[] { cmdBuf };
            var submitInfo = new SubmitInfo
            {
                SType = StructureType.SubmitInfo,
                WaitSemaphoreCount = 1, PWaitSemaphores = waitSem,
                PWaitDstStageMask = waitStage,
                CommandBufferCount = 1, PCommandBuffers = cBufs,
                SignalSemaphoreCount = 1, PSignalSemaphores = sigSem
            };
            if (vk.QueueSubmit(queue, 1, &submitInfo, fence) != Result.Success)
                return Fail("QueueSubmit 失败。", clearText, sw);

            // 16. QueuePresent
            var presentFn = Marshal.GetDelegateForFunctionPointer<QueuePresentPtr>(fnQueuePresent);
            var scArr = stackalloc[] { swapchain };
            var idxArr = stackalloc[] { imgIndex };
            var presentInfo = new PresentInfoKHR
            {
                SType = StructureType.PresentInfoKhr,
                WaitSemaphoreCount = 1, PWaitSemaphores = sigSem,
                SwapchainCount = 1, PSwapchains = scArr,
                PImageIndices = idxArr
            };
            var presentRes = presentFn(queue, &presentInfo);
            if (presentRes != Result.Success && presentRes != Result.SuboptimalKhr)
                return Fail($"QueuePresent 失败：{presentRes}。", clearText, sw);

            // 17. Wait idle
            vk.DeviceWaitIdle(dev);

            sw.Stop();
            var result = new VulkanClearInfo(VulkanClearStatus.Succeeded,
                "最小 Vulkan 清屏成功。", clearText, extent.Width, extent.Height, sw.Elapsed.TotalMilliseconds);

            // Cleanup happens in finally
            return result;
        }
        finally
        {
            if (vk is not null) { try { if (dev.Handle != 0) vk.DeviceWaitIdle(dev); } catch { } }
            if (syncOk && dev.Handle != 0 && vk is not null)
            {
                if (semAvail.Handle != 0) vk.DestroySemaphore(dev, semAvail, null);
                if (semFin.Handle != 0) vk.DestroySemaphore(dev, semFin, null);
                if (fence.Handle != 0) vk.DestroyFence(dev, fence, null);
            }
            if (poolOk && dev.Handle != 0 && vk is not null) vk.DestroyCommandPool(dev, cmdPool, null);
            if (dev.Handle != 0 && vk is not null)
            {
                foreach (var fb in framebuffers) if (fb.Handle != 0) vk.DestroyFramebuffer(dev, fb, null);
                if (rpOk && renderPass.Handle != 0) vk.DestroyRenderPass(dev, renderPass, null);
                foreach (var iv in imageViews) if (iv.Handle != 0) vk.DestroyImageView(dev, iv, null);
            }
            if (scOk && fnDestroySwapchain != 0 && vk is not null)
                Marshal.GetDelegateForFunctionPointer<DestroySwapchainPtr>(fnDestroySwapchain)(dev, swapchain, null);
            if (devOk && vk is not null && dev.Handle != 0) vk.DestroyDevice(dev, null);
            if (surfOk && fnDestroySurface != 0 && vk is not null)
                Marshal.GetDelegateForFunctionPointer<DestroySurfacePtr>(fnDestroySurface)(inst, surf, null);
            if (instOk && vk is not null) vk.DestroyInstance(inst, null);
        }
    }

    // ─── 辅助方法 ──────────────────────────────────────────────

    private static nint LoadProc(Vk vk, Silk.NET.Vulkan.Instance inst, string name)
    { var p = Marshal.StringToHGlobalAnsi(name); try { return (nint)vk.GetInstanceProcAddr(inst, (byte*)p); } finally { Marshal.FreeHGlobal(p); } }

    private static nint LoadDeviceProc(Vk vk, Silk.NET.Vulkan.Device dev, string name)
    { var p = Marshal.StringToHGlobalAnsi(name); try { return (nint)vk.GetDeviceProcAddr(dev, (byte*)p); } finally { Marshal.FreeHGlobal(p); } }

    private static bool CreateInstance(Vk vk, out Silk.NET.Vulkan.Instance inst)
    {
        inst = default;
        var a = Marshal.StringToHGlobalAnsi("FluidWarfare"); var e = Marshal.StringToHGlobalAnsi("FluidWarfare");
        var s = Marshal.StringToHGlobalAnsi("VK_KHR_surface"); var w = Marshal.StringToHGlobalAnsi("VK_KHR_win32_surface");
        try
        {
            var exts = stackalloc byte*[] { (byte*)s, (byte*)w };
            var ai = new ApplicationInfo { SType = StructureType.ApplicationInfo, PApplicationName = (byte*)a, ApplicationVersion = 1, PEngineName = (byte*)e, EngineVersion = 1, ApiVersion = PackVer(1, 0, 0) };
            var ci = new InstanceCreateInfo { SType = StructureType.InstanceCreateInfo, PApplicationInfo = &ai, EnabledExtensionCount = 2, PpEnabledExtensionNames = exts };
            return vk.CreateInstance(&ci, null, out inst) == Result.Success;
        }
        finally { Marshal.FreeHGlobal(a); Marshal.FreeHGlobal(e); Marshal.FreeHGlobal(s); Marshal.FreeHGlobal(w); }
    }

    private static bool CreateSurface(Vk vk, Silk.NET.Vulkan.Instance inst, nint hi, nint hw, out SurfaceKHR s)
    {
        s = default;
        var p = Marshal.StringToHGlobalAnsi("vkCreateWin32SurfaceKHR");
        try
        {
            var addr = (nint)vk.GetInstanceProcAddr(inst, (byte*)p);
            if (addr == 0) return false;
            var fn = Marshal.GetDelegateForFunctionPointer<CreateWin32SurfacePtr>(addr);
            var ci = new Win32SurfaceCreateInfoKHR { SType = StructureType.Win32SurfaceCreateInfoKhr, Hinstance = hi, Hwnd = hw };
            fixed (SurfaceKHR* sp = &s) return fn(inst, &ci, null, sp) == Result.Success;
        }
        finally { Marshal.FreeHGlobal(p); }
    }

    private static bool SelectDevice(Vk vk, Silk.NET.Vulkan.Instance inst, SurfaceKHR surf, out Silk.NET.Vulkan.PhysicalDevice pd, out uint qi, out string name)
    {
        pd = default; qi = 0; name = "未知";
        uint count = 0;
        if (vk.EnumeratePhysicalDevices(inst, ref count, null) != Result.Success || count == 0) return false;
        var devices = new Silk.NET.Vulkan.PhysicalDevice[count];
        fixed (Silk.NET.Vulkan.PhysicalDevice* p = devices) vk.EnumeratePhysicalDevices(inst, ref count, p);
        var fnSupport = LoadProc(vk, inst, "vkGetPhysicalDeviceSurfaceSupportKHR");
        if (fnSupport == 0) return false;
        var supportFn = Marshal.GetDelegateForFunctionPointer<SurfaceSupportPtr>(fnSupport);
        foreach (var d in devices)
        {
            vk.GetPhysicalDeviceProperties(d, out var props);
            name = Marshal.PtrToStringAnsi((nint)props.DeviceName) ?? "未知";
            uint qc = 0;
            vk.GetPhysicalDeviceQueueFamilyProperties(d, ref qc, null);
            var qProps = new QueueFamilyProperties[qc];
            fixed (QueueFamilyProperties* qp = qProps) vk.GetPhysicalDeviceQueueFamilyProperties(d, ref qc, qp);
            for (uint i = 0; i < qc; i++)
                if (qProps[i].QueueCount > 0 && qProps[i].QueueFlags.HasFlag(QueueFlags.GraphicsBit))
                {
                    int supported = 0;
                    supportFn(d, i, surf, &supported);
                    if (supported != 0) { pd = d; qi = i; return true; }
                }
        }
        return false;
    }

    private static bool CreateDevice(Vk vk, Silk.NET.Vulkan.PhysicalDevice pd, uint qi, out Silk.NET.Vulkan.Device dev)
    {
        dev = default;
        var qp = 1.0f;
        var qci = new DeviceQueueCreateInfo { SType = StructureType.DeviceQueueCreateInfo, QueueFamilyIndex = qi, QueueCount = 1, PQueuePriorities = &qp };
        var se = Marshal.StringToHGlobalAnsi("VK_KHR_swapchain");
        try
        {
            var exts = stackalloc byte*[] { (byte*)se };
            var dci = new DeviceCreateInfo { SType = StructureType.DeviceCreateInfo, QueueCreateInfoCount = 1, PQueueCreateInfos = &qci, EnabledExtensionCount = 1, PpEnabledExtensionNames = exts };
            return vk.CreateDevice(pd, &dci, null, out dev) == Result.Success;
        }
        finally { Marshal.FreeHGlobal(se); }
    }

    // ─── 查询 ──────────────────────────────────────────────────

    private static SurfaceCapabilitiesKHR QueryCaps(Silk.NET.Vulkan.PhysicalDevice pd, Silk.NET.Vulkan.Instance inst, SurfaceKHR surf, nint fn)
    { var f = Marshal.GetDelegateForFunctionPointer<GetCapsPtr>(fn); f(pd, inst, surf, out var c); return c; }

    private static SurfaceFormatKHR[] QueryFormats(Silk.NET.Vulkan.PhysicalDevice pd, Silk.NET.Vulkan.Instance inst, SurfaceKHR surf, nint fn)
    {
        var f = Marshal.GetDelegateForFunctionPointer<GetFormatsPtr>(fn); uint c = 0; f(pd, inst, surf, &c, null);
        if (c == 0) return []; var r = new SurfaceFormatKHR[c]; fixed (SurfaceFormatKHR* p = r) f(pd, inst, surf, &c, p); return r;
    }

    private static PresentModeKHR[] QueryModes(Silk.NET.Vulkan.PhysicalDevice pd, Silk.NET.Vulkan.Instance inst, SurfaceKHR surf, nint fn)
    {
        var f = Marshal.GetDelegateForFunctionPointer<GetModesPtr>(fn); uint c = 0; f(pd, inst, surf, &c, null);
        if (c == 0) return []; var r = new PresentModeKHR[c]; fixed (PresentModeKHR* p = r) f(pd, inst, surf, &c, p); return r;
    }

    // ─── 选择 ──────────────────────────────────────────────────

    private static SurfaceFormatKHR ChooseFormat(SurfaceFormatKHR[] f)
    { foreach (var x in f) if (x.Format == Format.B8G8R8A8Srgb || x.Format == Format.R8G8B8A8Srgb) return x; return f[0]; }

    private static PresentModeKHR ChoosePresentMode(PresentModeKHR[] m)
    { foreach (var x in m) if (x == PresentModeKHR.MailboxKhr || x == PresentModeKHR.ImmediateKhr) return x; return PresentModeKHR.FifoKhr; }

    private static Extent2D ChooseExtent(SurfaceCapabilitiesKHR c, uint fw, uint fh)
    { if (c.CurrentExtent.Width != uint.MaxValue) return c.CurrentExtent; return new Extent2D(Math.Clamp(fw, c.MinImageExtent.Width, c.MaxImageExtent.Width), Math.Clamp(fh, c.MinImageExtent.Height, c.MaxImageExtent.Height)); }

    private static VulkanClearInfo Fail(string msg, string color, Stopwatch sw) => new(VulkanClearStatus.Failed, msg, color, 0, 0, sw.Elapsed.TotalMilliseconds);
    private static uint PackVer(uint a, uint b, uint c) => (a << 22) | (b << 12) | c;

    // ─── 委托定义（指针参数，避免 0xC0000005）──────────────────

    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result CreateWin32SurfacePtr(Silk.NET.Vulkan.Instance i, Win32SurfaceCreateInfoKHR* ci, AllocationCallbacks* a, SurfaceKHR* s);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate void DestroySurfacePtr(Silk.NET.Vulkan.Instance i, SurfaceKHR s, AllocationCallbacks* a);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result SurfaceSupportPtr(Silk.NET.Vulkan.PhysicalDevice pd, uint qi, SurfaceKHR s, int* supported);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate void GetCapsPtr(Silk.NET.Vulkan.PhysicalDevice pd, Silk.NET.Vulkan.Instance i, SurfaceKHR s, out SurfaceCapabilitiesKHR c);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result GetFormatsPtr(Silk.NET.Vulkan.PhysicalDevice pd, Silk.NET.Vulkan.Instance i, SurfaceKHR s, uint* c, SurfaceFormatKHR* f);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result GetModesPtr(Silk.NET.Vulkan.PhysicalDevice pd, Silk.NET.Vulkan.Instance i, SurfaceKHR s, uint* c, PresentModeKHR* m);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result CreateSwapchainPtr(Silk.NET.Vulkan.Device d, SwapchainCreateInfoKHR* ci, AllocationCallbacks* a, SwapchainKHR* sc);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate void DestroySwapchainPtr(Silk.NET.Vulkan.Device d, SwapchainKHR sc, AllocationCallbacks* a);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result GetSwapchainImagesPtr(Silk.NET.Vulkan.Device d, SwapchainKHR sc, uint* c, Image* imgs);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result AcquireNextImagePtr(Silk.NET.Vulkan.Device d, SwapchainKHR sc, ulong t, Silk.NET.Vulkan.Semaphore s, Fence f, uint* i);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result QueuePresentPtr(Queue q, PresentInfoKHR* p);
}
