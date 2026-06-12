using System.Diagnostics;
using System.Runtime.InteropServices;
using FluidWarfare.Render.Vulkan.Camera;
using FluidWarfare.Render.Vulkan.Scene3D.Depth;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>
/// Scene3D 渲染流程编排层。
/// 各子步骤由专用模块完成：ShaderModules / PipelineLayout / Pipelines / VertexBuffers / CommandRecorder / Depth。
/// 资源持有与释放由 RenderResources 统一管理。
/// </summary>
public static unsafe class VulkanScene3dRenderer
{
    private const float ClearR = 0.03f, ClearG = 0.08f, ClearB = 0.18f, ClearA = 1.0f;

    public static VulkanScene3dInfo RenderWindows(
        nint hinstance, nint hwnd, uint reqW, uint reqH,
        VulkanCameraInfo camera,
        ReadOnlySpan<VulkanScene3dVertex> gridVertices,
        ReadOnlySpan<VulkanScene3dVertex> unitVertices,
        ReadOnlySpan<VulkanScene3dUnitDrawInfo> unitDraws)
    {
        var sw = Stopwatch.StartNew();
        var r = new VulkanScene3dRenderResources();
        var drawCalls = 0;
        var renderedUnitCount = 0;
        var ignoredObjectCount = 0;
        Format depthFormat = Format.Undefined;
        int depthAttachmentCount = 0;
        bool depthTestEnabled = false;

        try
        {
            r.Vk = Vk.GetApi();
            if (hinstance == 0 || hwnd == 0) return Fail("句柄不可用。", sw);

            // 1. Instance
            if (!CreateInstance(r.Vk, out r.Instance)) return Fail("Instance 创建失败。", sw);
            r.InstOk = true;
            r.FnDestroySurface = LoadProc(r.Vk, r.Instance, "vkDestroySurfaceKHR");

            // 2. Surface
            if (!CreateSurface(r.Vk, r.Instance, hinstance, hwnd, out r.Surface)) return Fail("Surface 创建失败。", sw);
            r.SurfOk = true;

            // 3. Physical Device
            if (!SelectDevice(r.Vk, r.Instance, r.Surface, out var pd, out var qi, out _))
                return Fail("未找到 Graphics+Present 队列。", sw);

            // 4. Logical Device
            if (!CreateDevice(r.Vk, pd, qi, out r.Device)) return Fail("Device 创建失败。", sw);
            r.DevOk = true;

            // Instance-level function pointers
            var fnGetCaps = LoadProc(r.Vk, r.Instance, "vkGetPhysicalDeviceSurfaceCapabilitiesKHR");
            var fnGetFmts = LoadProc(r.Vk, r.Instance, "vkGetPhysicalDeviceSurfaceFormatsKHR");
            var fnGetModes = LoadProc(r.Vk, r.Instance, "vkGetPhysicalDeviceSurfacePresentModesKHR");
            if (fnGetCaps == 0 || fnGetFmts == 0 || fnGetModes == 0)
                return Fail("无法加载 Surface 查询函数。", sw);

            r.FnDestroySwapchain = LoadDeviceProc(r.Vk, r.Device, "vkDestroySwapchainKHR");
            var fnCreateSwapchain = LoadDeviceProc(r.Vk, r.Device, "vkCreateSwapchainKHR");
            var fnGetImages = LoadDeviceProc(r.Vk, r.Device, "vkGetSwapchainImagesKHR");
            var fnAcquire = LoadDeviceProc(r.Vk, r.Device, "vkAcquireNextImageKHR");
            var fnQueuePresent = LoadDeviceProc(r.Vk, r.Device, "vkQueuePresentKHR");
            if (fnCreateSwapchain == 0 || r.FnDestroySwapchain == 0 || fnGetImages == 0 ||
                fnAcquire == 0 || fnQueuePresent == 0)
                return Fail("无法加载 Swapchain 设备扩展函数。", sw);

            // 5. Surface capabilities & Swapchain
            var caps = QueryCaps(pd, r.Surface, fnGetCaps);
            var formats = QueryFormats(pd, r.Surface, fnGetFmts);
            if (formats.Length == 0) return Fail("无可用 Surface 格式。", sw);
            var chosenFmt = ChooseFormat(formats).Format;
            var extent = ChooseExtent(caps, reqW, reqH);
            var imageCount = Math.Clamp(caps.MinImageCount + 1, caps.MinImageCount,
                caps.MaxImageCount > 0 ? caps.MaxImageCount : uint.MaxValue);

            var scCI = new SwapchainCreateInfoKHR
            {
                SType = StructureType.SwapchainCreateInfoKhr, Surface = r.Surface,
                MinImageCount = imageCount, ImageFormat = chosenFmt,
                ImageColorSpace = ChooseFormat(formats).ColorSpace,
                ImageExtent = extent, ImageArrayLayers = 1,
                ImageUsage = ImageUsageFlags.ColorAttachmentBit,
                ImageSharingMode = SharingMode.Exclusive,
                PreTransform = caps.CurrentTransform,
                CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
                PresentMode = ChoosePresentMode(QueryModes(pd, r.Surface, fnGetModes)),
                Clipped = Vk.True
            };
            var createScFn = Marshal.GetDelegateForFunctionPointer<CreateSwapchainPtr>(fnCreateSwapchain);
            SwapchainKHR sc;
            if (createScFn(r.Device, &scCI, null, &sc) != Result.Success) return Fail("Swapchain 创建失败。", sw);
            r.Swapchain = sc; r.ScOk = true;

            var getImgsFn = Marshal.GetDelegateForFunctionPointer<GetSwapchainImagesPtr>(fnGetImages);
            uint imgCount = 0;
            getImgsFn(r.Device, r.Swapchain, &imgCount, null);
            if (imgCount == 0) return Fail("Swapchain 图像数为 0。", sw);
            var images = new Image[imgCount];
            fixed (Image* ip = images) getImgsFn(r.Device, r.Swapchain, &imgCount, ip);

            // 6. Color ImageViews
            r.ImageViews = new ImageView[imgCount];
            for (var i = 0; i < imgCount; i++)
            {
                var ivCI = new ImageViewCreateInfo
                {
                    SType = StructureType.ImageViewCreateInfo, Image = images[i],
                    ViewType = ImageViewType.Type2D, Format = chosenFmt,
                    Components = new ComponentMapping { R = ComponentSwizzle.Identity, G = ComponentSwizzle.Identity, B = ComponentSwizzle.Identity, A = ComponentSwizzle.Identity },
                    SubresourceRange = new ImageSubresourceRange { AspectMask = ImageAspectFlags.ColorBit, BaseMipLevel = 0, LevelCount = 1, BaseArrayLayer = 0, LayerCount = 1 }
                };
                if (r.Vk.CreateImageView(r.Device, &ivCI, null, out r.ImageViews[i]) != Result.Success)
                    return Fail($"ImageView {i} 创建失败。", sw);
            }

            // 7. Depth format selection
            var depthInfo = VulkanScene3dDepthFormatSelector.Select(r.Vk, pd);
            if (!depthInfo.IsSupported)
                return Fail(depthInfo.Message, sw);

            depthFormat = depthInfo.ChosenFormat;
            depthAttachmentCount = (int)imgCount;
            depthTestEnabled = true;
            r.DepthFormat = depthFormat;
            r.DepthAttachmentCount = (int)imgCount;

            // 7b. Depth attachments
            r.DepthImages = new Image[imgCount];
            r.DepthMemories = new DeviceMemory[imgCount];
            r.DepthViews = new ImageView[imgCount];
            if (!VulkanScene3dDepthAttachments.Create(r.Vk, pd, r.Device,
                    extent, depthFormat, imgCount,
                    r.DepthImages, r.DepthMemories, r.DepthViews, out var depthErr))
                return Fail(depthErr, sw);
            r.DepthOk = true;

            // 8. RenderPass (color + depth)
            var colorAttDesc = new AttachmentDescription
            {
                Format = chosenFmt, Samples = SampleCountFlags.Count1Bit,
                LoadOp = AttachmentLoadOp.Clear, StoreOp = AttachmentStoreOp.Store,
                StencilLoadOp = AttachmentLoadOp.DontCare, StencilStoreOp = AttachmentStoreOp.DontCare,
                InitialLayout = ImageLayout.Undefined, FinalLayout = ImageLayout.PresentSrcKhr
            };
            var depthAttDesc = new AttachmentDescription
            {
                Format = depthFormat, Samples = SampleCountFlags.Count1Bit,
                LoadOp = AttachmentLoadOp.Clear, StoreOp = AttachmentStoreOp.DontCare,
                StencilLoadOp = AttachmentLoadOp.DontCare, StencilStoreOp = AttachmentStoreOp.DontCare,
                InitialLayout = ImageLayout.Undefined,
                FinalLayout = ImageLayout.DepthStencilAttachmentOptimal
            };
            var attachments = stackalloc[] { colorAttDesc, depthAttDesc };

            var colorRef = new AttachmentReference { Attachment = 0, Layout = ImageLayout.ColorAttachmentOptimal };
            var depthRef = new AttachmentReference { Attachment = 1, Layout = ImageLayout.DepthStencilAttachmentOptimal };

            var subpass = new SubpassDescription
            {
                PipelineBindPoint = PipelineBindPoint.Graphics,
                ColorAttachmentCount = 1,
                PColorAttachments = &colorRef,
                PDepthStencilAttachment = &depthRef
            };

            var rpCI = new RenderPassCreateInfo
            {
                SType = StructureType.RenderPassCreateInfo,
                AttachmentCount = 2,
                PAttachments = attachments,
                SubpassCount = 1,
                PSubpasses = &subpass
            };
            if (r.Vk.CreateRenderPass(r.Device, &rpCI, null, out r.RenderPass) != Result.Success)
                return Fail("RenderPass 创建失败（含 Depth）。", sw);
            r.RpOk = true;

            // === Scene3D 专用资源 ===

            // 9. Shader Modules
            if (!VulkanScene3dShaderModules.Create(r.Vk, r.Device,
                    out r.VertModule, out r.FragModule, out var shaderErr))
                return Fail(shaderErr, sw);
            r.VertModOk = true; r.FragModOk = true;

            // 10. Pipeline Layout
            if (!VulkanScene3dPipelineLayout.Create(r.Vk, r.Device,
                    out r.PipelineLayout, out var layoutErr))
                return Fail(layoutErr, sw);
            r.LayoutOk = true;

            // 11. Graphics Pipelines (with depth state)
            if (!VulkanScene3dPipelines.Create(r.Vk, r.Device,
                    r.RenderPass, r.PipelineLayout,
                    r.VertModule, r.FragModule,
                    extent.Width, extent.Height,
                    out r.GridPipeline, out r.UnitPipeline, out var pipeErr))
                return Fail(pipeErr, sw);
            r.GridPipeOk = true; r.UnitPipeOk = true;

            // 12. Vertex Buffers
            if (!VulkanScene3dVertexBuffers.Create(r.Vk, pd, r.Device,
                    gridVertices, unitVertices,
                    out r.GridBuffer, out r.GridMemory,
                    out r.UnitBuffer, out r.UnitMemory,
                    out var gVc, out var uVc, out var bufErr))
                return Fail(bufErr, sw);
            r.GridBufOk = true; r.UnitBufOk = true;

            // 13. Framebuffers (color + depth)
            r.Framebuffers = new Framebuffer[imgCount];
            var fba = stackalloc ImageView[2];
            for (var i = 0; i < imgCount; i++)
            {
                fba[0] = r.ImageViews[i];
                fba[1] = r.DepthViews[i];
                var fbCI = new FramebufferCreateInfo
                {
                    SType = StructureType.FramebufferCreateInfo,
                    RenderPass = r.RenderPass,
                    AttachmentCount = 2,
                    PAttachments = (ImageView*)fba,
                    Width = extent.Width,
                    Height = extent.Height,
                    Layers = 1
                };
                if (r.Vk.CreateFramebuffer(r.Device, &fbCI, null, out r.Framebuffers[i]) != Result.Success)
                    return Fail($"Framebuffer {i} 创建失败（含 Depth）。", sw);
            }

            // 14. Command Pool + Buffer
            var poolCI = new CommandPoolCreateInfo { SType = StructureType.CommandPoolCreateInfo, QueueFamilyIndex = qi };
            if (r.Vk.CreateCommandPool(r.Device, &poolCI, null, out r.CommandPool) != Result.Success)
                return Fail("CommandPool 创建失败。", sw);
            r.PoolOk = true;

            var allocCI = new CommandBufferAllocateInfo { SType = StructureType.CommandBufferAllocateInfo, CommandPool = r.CommandPool, Level = CommandBufferLevel.Primary, CommandBufferCount = 1 };
            if (r.Vk.AllocateCommandBuffers(r.Device, &allocCI, out r.CommandBuffer) != Result.Success)
                return Fail("CommandBuffer 创建失败。", sw);

            // 15. Sync Objects
            var semCI = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo };
            var fenceCI = new FenceCreateInfo { SType = StructureType.FenceCreateInfo, Flags = FenceCreateFlags.SignaledBit };
            if (r.Vk.CreateSemaphore(r.Device, &semCI, null, out r.SemAvail) != Result.Success ||
                r.Vk.CreateSemaphore(r.Device, &semCI, null, out r.SemFin) != Result.Success ||
                r.Vk.CreateFence(r.Device, &fenceCI, null, out r.Fence) != Result.Success)
                return Fail("同步对象创建失败。", sw);
            r.SyncOk = true;

            // 16. Acquire image
            r.Vk.WaitForFences(r.Device, 1, ref r.Fence, Vk.True, ulong.MaxValue);
            r.Vk.ResetFences(r.Device, 1, ref r.Fence);
            uint imgIndex = 0;
            var acquireFn = Marshal.GetDelegateForFunctionPointer<AcquireNextImagePtr>(fnAcquire);
            var acqRes = acquireFn(r.Device, r.Swapchain, ulong.MaxValue, r.SemAvail, default, &imgIndex);
            if (acqRes == Result.ErrorOutOfDateKhr) return Fail("Acquire 返回 OutOfDate。", sw);
            if (acqRes != Result.Success && acqRes != Result.SuboptimalKhr)
                return Fail($"AcquireNextImage 失败：{acqRes}。", sw);

            // 17. VP matrix (shared view-projection)
            var aspect = extent.Width / (float)extent.Height;
            var vp = VulkanCameraMatrices.ComputeVulkanMVP(camera, aspect);

            // 18. Per-object unit MVP array
            var unitMvpList = new List<float[]>();
            foreach (var draw in unitDraws)
            {
                // Model = translation * scale (column-major)
                var trans = VulkanCameraMatrices.CreateTranslation(draw.X, draw.Y, draw.Z);
                var scale = VulkanCameraMatrices.CreateScale(draw.Scale);
                var model = VulkanCameraMatrices.Mul(trans, scale);
                // MVP = VP * Model
                var mvp = VulkanCameraMatrices.Mul(vp, model);
                unitMvpList.Add(mvp);
                renderedUnitCount++;
            }

            // 19. Record Command Buffer
            if (!VulkanScene3dCommandRecorder.Record(r.Vk, r.CommandBuffer,
                    r.RenderPass, r.Framebuffers[imgIndex], extent,
                    r.GridPipeline, r.UnitPipeline, r.PipelineLayout,
                    vp, r.GridBuffer, gVc,
                    r.UnitBuffer, uVc,
                    [.. unitMvpList],
                    out drawCalls, out var cmdErr))
                return Fail(cmdErr, sw);

            // 20. Submit
            var queue = default(Queue);
            r.Vk.GetDeviceQueue(r.Device, qi, 0, out queue);
            var waitSem = stackalloc[] { r.SemAvail };
            var waitStage = stackalloc[] { PipelineStageFlags.ColorAttachmentOutputBit };
            var sigSem = stackalloc[] { r.SemFin };
            var cBufs = stackalloc[] { r.CommandBuffer };
            var submitInfo = new SubmitInfo
            {
                SType = StructureType.SubmitInfo,
                WaitSemaphoreCount = 1, PWaitSemaphores = waitSem,
                PWaitDstStageMask = waitStage,
                CommandBufferCount = 1, PCommandBuffers = cBufs,
                SignalSemaphoreCount = 1, PSignalSemaphores = sigSem
            };
            if (r.Vk.QueueSubmit(queue, 1, &submitInfo, r.Fence) != Result.Success)
                return Fail("QueueSubmit 失败。", sw);

            // 21. Present
            var presentFn = Marshal.GetDelegateForFunctionPointer<QueuePresentPtr>(fnQueuePresent);
            var scArr = stackalloc[] { r.Swapchain };
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
                return Fail($"QueuePresent 失败：{presentRes}。", sw);

            r.Vk.DeviceWaitIdle(r.Device);
            sw.Stop();

            var depthFmtName = VulkanScene3dDepthFormatSelector.FormatName(depthFormat);
            return new VulkanScene3dInfo(
                VulkanScene3dStatus.Succeeded,
                $"Vulkan 3D 场景绘制成功：" +
                $"RenderObject {unitDraws.Length}，" +
                $"Unit {renderedUnitCount}，" +
                $"单体顶点 {uVc}，" +
                $"Grid {gVc}，" +
                $"Depth {depthFmtName}，" +
                $"DepthAttachment {depthAttachmentCount}，" +
                $"DrawCall {drawCalls}，" +
                $"用时 {sw.Elapsed.TotalMilliseconds:F2} ms。",
                gVc, gVc / 2, uVc, uVc / 3,
                unitDraws.Length, renderedUnitCount, ignoredObjectCount,
                depthFmtName, depthAttachmentCount, depthTestEnabled,
                drawCalls, (int)extent.Width, (int)extent.Height,
                camera.ToSummary(),
                sw.Elapsed.TotalMilliseconds);
        }
        finally
        {
            r.Dispose();
        }
    }

    // ─── 基础 Vulkan 创建辅助 ─────────────────────────────────────

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

    private static bool SelectDevice(Vk vk, Silk.NET.Vulkan.Instance inst, SurfaceKHR surf,
        out Silk.NET.Vulkan.PhysicalDevice pd, out uint qi, out string name)
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

    private static SurfaceCapabilitiesKHR QueryCaps(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR surf, nint fn)
    { var f = Marshal.GetDelegateForFunctionPointer<GetCapsPtr>(fn); SurfaceCapabilitiesKHR c; f(pd, surf, &c); return c; }

    private static SurfaceFormatKHR[] QueryFormats(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR surf, nint fn)
    { var f = Marshal.GetDelegateForFunctionPointer<GetFormatsPtr>(fn); uint c = 0; if (f(pd, surf, &c, null) != Result.Success || c == 0) return []; var r = new SurfaceFormatKHR[c]; fixed (SurfaceFormatKHR* p = r) f(pd, surf, &c, p); return r; }

    private static PresentModeKHR[] QueryModes(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR surf, nint fn)
    { var f = Marshal.GetDelegateForFunctionPointer<GetModesPtr>(fn); uint c = 0; if (f(pd, surf, &c, null) != Result.Success || c == 0) return []; var r = new PresentModeKHR[c]; fixed (PresentModeKHR* p = r) f(pd, surf, &c, p); return r; }

    private static SurfaceFormatKHR ChooseFormat(SurfaceFormatKHR[] f)
    { foreach (var x in f) if (x.Format == Format.B8G8R8A8Srgb || x.Format == Format.R8G8B8A8Srgb) return x; return f[0]; }

    private static PresentModeKHR ChoosePresentMode(PresentModeKHR[] m)
    { foreach (var x in m) if (x == PresentModeKHR.MailboxKhr || x == PresentModeKHR.ImmediateKhr) return x; return PresentModeKHR.FifoKhr; }

    private static Extent2D ChooseExtent(SurfaceCapabilitiesKHR c, uint fw, uint fh)
    { if (c.CurrentExtent.Width != uint.MaxValue) return c.CurrentExtent; return new Extent2D(Math.Clamp(fw, c.MinImageExtent.Width, c.MaxImageExtent.Width), Math.Clamp(fh, c.MinImageExtent.Height, c.MaxImageExtent.Height)); }

    private static VulkanScene3dInfo Fail(string msg, Stopwatch sw) =>
        new(VulkanScene3dStatus.Failed, msg, 0, 0, 0, 0, 0, 0, 0, "无", 0, false, 0, 0, 0, "无", sw.Elapsed.TotalMilliseconds);

    private static uint PackVer(uint a, uint b, uint c) => (a << 22) | (b << 12) | c;

    // ─── 委托定义 ────────────────────────────────────────────────

    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result CreateWin32SurfacePtr(Silk.NET.Vulkan.Instance i, Win32SurfaceCreateInfoKHR* ci, AllocationCallbacks* a, SurfaceKHR* s);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result SurfaceSupportPtr(Silk.NET.Vulkan.PhysicalDevice pd, uint qi, SurfaceKHR s, int* supported);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result GetCapsPtr(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR s, SurfaceCapabilitiesKHR* c);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result GetFormatsPtr(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR s, uint* c, SurfaceFormatKHR* f);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result GetModesPtr(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR s, uint* c, PresentModeKHR* m);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result CreateSwapchainPtr(Silk.NET.Vulkan.Device d, SwapchainCreateInfoKHR* ci, AllocationCallbacks* a, SwapchainKHR* sc);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result GetSwapchainImagesPtr(Silk.NET.Vulkan.Device d, SwapchainKHR sc, uint* c, Image* imgs);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result AcquireNextImagePtr(Silk.NET.Vulkan.Device d, SwapchainKHR sc, ulong t, Silk.NET.Vulkan.Semaphore s, Fence f, uint* i);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result QueuePresentPtr(Queue q, PresentInfoKHR* p);
}
