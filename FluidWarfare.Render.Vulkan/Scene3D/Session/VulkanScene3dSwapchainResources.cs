using System.Runtime.InteropServices;
using FluidWarfare.Render.Vulkan.Scene3D.Depth;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// 持有与当前 Swapchain 尺寸相关的资源。
/// resize 时销毁全部，按新尺寸重建。
/// 不持有 Instance / Device 等会话级资源。
/// </summary>
public sealed unsafe class VulkanScene3dSwapchainResources : IDisposable
{
    public Vk? Vk;
    public Silk.NET.Vulkan.Device Device;
    public SwapchainKHR Swapchain;
    public ImageView[] ColorViews = [];
    public Image[] DepthImages = [];
    public DeviceMemory[] DepthMemories = [];
    public ImageView[] DepthViews = [];
    public RenderPass RenderPass;
    public Framebuffer[] Framebuffers = [];
    public CommandPool CommandPool;
    public CommandBuffer CommandBuffer;
    public Silk.NET.Vulkan.Semaphore SemAvail, SemFin;
    public Fence Fence;
    public Extent2D Extent;
    public SurfaceFormatKHR SurfaceFormat;
    public Format DepthFormat;
    public int ImageCount;

    // Function pointers
    public nint FnDestroySwapchain;
    public nint FnDestroySurface;

    // Success flags
    public bool ScOk, RpOk, PoolOk, SyncOk, DepthOk;

    /// <summary>
    /// 创建 swapchain 级资源。
    /// </summary>
    public static bool Create(
        Vk vk, Silk.NET.Vulkan.Device device,
        Silk.NET.Vulkan.PhysicalDevice pd,
        Silk.NET.Vulkan.Instance inst,
        SurfaceKHR surface,
        uint reqW, uint reqH, uint qi,
        nint fnGetCaps, nint fnGetFmts, nint fnGetModes,
        nint fnCreateSwapchain, nint fnGetImages,
        out VulkanScene3dSwapchainResources resources,
        out string error)
    {
        resources = new VulkanScene3dSwapchainResources();
        error = string.Empty;
        resources.Vk = vk;
        resources.Device = device;

        var getCapsFn = Marshal.GetDelegateForFunctionPointer<GetCapsPtr>(fnGetCaps);
        var getFmtsFn = Marshal.GetDelegateForFunctionPointer<GetFormatsPtr>(fnGetFmts);
        var getModesFn = Marshal.GetDelegateForFunctionPointer<GetModesPtr>(fnGetModes);

        var caps = default(SurfaceCapabilitiesKHR);
        getCapsFn(pd, surface, &caps);
        uint fmtCount = 0;
        getFmtsFn(pd, surface, &fmtCount, null);
        if (fmtCount == 0) { error = "无可用 Surface 格式。"; return false; }
        var fmts = new SurfaceFormatKHR[fmtCount];
        fixed (SurfaceFormatKHR* fp = fmts) getFmtsFn(pd, surface, &fmtCount, fp);

        var chosenFmt = ChooseFormat(fmts);
        var extent = ChooseExtent(caps, reqW, reqH);
        var imgCount = Math.Clamp(caps.MinImageCount + 1,
            caps.MinImageCount,
            caps.MaxImageCount > 0 ? caps.MaxImageCount : uint.MaxValue);

        // Create swapchain
        uint modeCount = 0;
        getModesFn(pd, surface, &modeCount, null);
        var modes = new PresentModeKHR[modeCount];
        fixed (PresentModeKHR* mp = modes) getModesFn(pd, surface, &modeCount, mp);
        var presentMode = ChoosePresentMode(modes);

        var scCI = new SwapchainCreateInfoKHR
        {
            SType = StructureType.SwapchainCreateInfoKhr,
            Surface = surface,
            MinImageCount = imgCount,
            ImageFormat = chosenFmt.Format,
            ImageColorSpace = chosenFmt.ColorSpace,
            ImageExtent = extent,
            ImageArrayLayers = 1,
            ImageUsage = ImageUsageFlags.ColorAttachmentBit,
            ImageSharingMode = SharingMode.Exclusive,
            PreTransform = caps.CurrentTransform,
            CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
            PresentMode = presentMode,
            Clipped = Vk.True
        };

        var createScFn = Marshal.GetDelegateForFunctionPointer<CreateSwapchainPtr>(fnCreateSwapchain);
        SwapchainKHR sc;
        if (createScFn(device, &scCI, null, &sc) != Result.Success)
        { error = "Swapchain 创建失败。"; return false; }
        resources.Swapchain = sc;
        resources.ScOk = true;

        var getImgsFn = Marshal.GetDelegateForFunctionPointer<GetSwapchainImagesPtr>(fnGetImages);
        uint actualCount = 0;
        getImgsFn(device, sc, &actualCount, null);
        if (actualCount == 0) { error = "Swapchain 图像数为 0。"; return false; }
        var swapchainImages = new Image[actualCount];
        fixed (Image* ip = swapchainImages) getImgsFn(device, sc, &actualCount, ip);
        resources.ImageCount = (int)actualCount;
        resources.SurfaceFormat = chosenFmt;
        resources.Extent = extent;

        // Color ImageViews
        resources.ColorViews = new ImageView[actualCount];
        for (var i = 0; i < actualCount; i++)
        {
            var ivCI = new ImageViewCreateInfo
            {
                SType = StructureType.ImageViewCreateInfo,
                Image = swapchainImages[i],
                ViewType = ImageViewType.Type2D,
                Format = chosenFmt.Format,
                Components = new ComponentMapping
                {
                    R = ComponentSwizzle.Identity, G = ComponentSwizzle.Identity,
                    B = ComponentSwizzle.Identity, A = ComponentSwizzle.Identity
                },
                SubresourceRange = new ImageSubresourceRange
                {
                    AspectMask = ImageAspectFlags.ColorBit,
                    BaseMipLevel = 0, LevelCount = 1,
                    BaseArrayLayer = 0, LayerCount = 1
                }
            };
            if (vk.CreateImageView(device, &ivCI, null, out resources.ColorViews[i]) != Result.Success)
            { error = $"Color ImageView {i} 创建失败。"; return false; }
        }

        // Depth format selection
        var depthInfo = VulkanScene3dDepthFormatSelector.Select(vk, pd);
        if (!depthInfo.IsSupported) { error = depthInfo.Message; return false; }
        resources.DepthFormat = depthInfo.ChosenFormat;

        // Depth attachments
        resources.DepthImages = new Image[actualCount];
        resources.DepthMemories = new DeviceMemory[actualCount];
        resources.DepthViews = new ImageView[actualCount];
        if (!VulkanScene3dDepthAttachments.Create(vk, pd, device,
                extent, depthInfo.ChosenFormat, (uint)actualCount,
                resources.DepthImages, resources.DepthMemories, resources.DepthViews,
                out var depthErr))
        { error = depthErr; return false; }
        resources.DepthOk = true;

        // RenderPass
        var colorAttDesc = new AttachmentDescription
        {
            Format = chosenFmt.Format, Samples = SampleCountFlags.Count1Bit,
            LoadOp = AttachmentLoadOp.Clear, StoreOp = AttachmentStoreOp.Store,
            StencilLoadOp = AttachmentLoadOp.DontCare, StencilStoreOp = AttachmentStoreOp.DontCare,
            InitialLayout = ImageLayout.Undefined, FinalLayout = ImageLayout.PresentSrcKhr
        };
        var depthAttDesc = new AttachmentDescription
        {
            Format = depthInfo.ChosenFormat, Samples = SampleCountFlags.Count1Bit,
            LoadOp = AttachmentLoadOp.Clear, StoreOp = AttachmentStoreOp.DontCare,
            StencilLoadOp = AttachmentLoadOp.DontCare, StencilStoreOp = AttachmentStoreOp.DontCare,
            InitialLayout = ImageLayout.Undefined,
            FinalLayout = ImageLayout.DepthStencilAttachmentOptimal
        };
        var atts = stackalloc[] { colorAttDesc, depthAttDesc };
        var colorRef = new AttachmentReference { Attachment = 0, Layout = ImageLayout.ColorAttachmentOptimal };
        var depthRef = new AttachmentReference { Attachment = 1, Layout = ImageLayout.DepthStencilAttachmentOptimal };
        var subpass = new SubpassDescription
        {
            PipelineBindPoint = PipelineBindPoint.Graphics,
            ColorAttachmentCount = 1, PColorAttachments = &colorRef,
            PDepthStencilAttachment = &depthRef
        };
        var rpCI = new RenderPassCreateInfo
        {
            SType = StructureType.RenderPassCreateInfo,
            AttachmentCount = 2, PAttachments = atts,
            SubpassCount = 1, PSubpasses = &subpass
        };
        if (vk.CreateRenderPass(device, &rpCI, null, out resources.RenderPass) != Result.Success)
        { error = "RenderPass 创建失败。"; return false; }
        resources.RpOk = true;

        // Framebuffers
        resources.Framebuffers = new Framebuffer[actualCount];
        for (var i = 0; i < actualCount; i++)
        {
            var fba = stackalloc ImageView[] { resources.ColorViews[i], resources.DepthViews[i] };
            var fbCI = new FramebufferCreateInfo
            {
                SType = StructureType.FramebufferCreateInfo,
                RenderPass = resources.RenderPass,
                AttachmentCount = 2,
                PAttachments = (ImageView*)fba,
                Width = extent.Width, Height = extent.Height, Layers = 1
            };
            if (vk.CreateFramebuffer(device, &fbCI, null, out resources.Framebuffers[i]) != Result.Success)
            { error = $"Framebuffer {i} 创建失败。"; return false; }
        }

        // CommandPool + Buffer
        var poolCI = new CommandPoolCreateInfo { SType = StructureType.CommandPoolCreateInfo, QueueFamilyIndex = qi };
        if (vk.CreateCommandPool(device, &poolCI, null, out resources.CommandPool) != Result.Success)
        { error = "CommandPool 创建失败。"; return false; }
        resources.PoolOk = true;

        var allocCI = new CommandBufferAllocateInfo
        {
            SType = StructureType.CommandBufferAllocateInfo,
            CommandPool = resources.CommandPool,
            Level = CommandBufferLevel.Primary,
            CommandBufferCount = 1
        };
        if (vk.AllocateCommandBuffers(device, &allocCI, out resources.CommandBuffer) != Result.Success)
        { error = "CommandBuffer 创建失败。"; return false; }

        // Sync objects
        var semCI = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo };
        var fenceCI = new FenceCreateInfo { SType = StructureType.FenceCreateInfo, Flags = FenceCreateFlags.SignaledBit };
        if (vk.CreateSemaphore(device, &semCI, null, out resources.SemAvail) != Result.Success ||
            vk.CreateSemaphore(device, &semCI, null, out resources.SemFin) != Result.Success ||
            vk.CreateFence(device, &fenceCI, null, out resources.Fence) != Result.Success)
        { error = "同步对象创建失败。"; return false; }
        resources.SyncOk = true;

        return true;
    }

    /// <summary>
    /// 只销毁 swapchain 级资源（保留 Instance/Device）。
    /// </summary>
    public void Dispose()
    {
        if (Vk is null || Device.Handle == 0) return;
        try { DeviceWaitIdle(); } catch { }

        if (SyncOk)
        {
            if (SemAvail.Handle != 0) Vk.DestroySemaphore(Device, SemAvail, null);
            if (SemFin.Handle != 0) Vk.DestroySemaphore(Device, SemFin, null);
            if (Fence.Handle != 0) Vk.DestroyFence(Device, Fence, null);
        }
        if (PoolOk && CommandPool.Handle != 0)
            Vk.DestroyCommandPool(Device, CommandPool, null);

        if (Device.Handle != 0 && Framebuffers is not null)
            foreach (var fb in Framebuffers) if (fb.Handle != 0) Vk.DestroyFramebuffer(Device, fb, null);

        if (DepthOk)
        {
            if (DepthViews is not null)
                foreach (var dv in DepthViews) if (dv.Handle != 0) Vk.DestroyImageView(Device, dv, null);
            if (DepthImages is not null)
                foreach (var di in DepthImages) if (di.Handle != 0) Vk.DestroyImage(Device, di, null);
            if (DepthMemories is not null)
                foreach (var dm in DepthMemories) if (dm.Handle != 0) Vk.FreeMemory(Device, dm, null);
        }

        if (RpOk && RenderPass.Handle != 0)
            Vk.DestroyRenderPass(Device, RenderPass, null);

        if (ColorViews is not null)
            foreach (var iv in ColorViews) if (iv.Handle != 0) Vk.DestroyImageView(Device, iv, null);

        if (ScOk && FnDestroySwapchain != 0)
        {
            var fn = Marshal.GetDelegateForFunctionPointer<DestroySwapchainFn>(FnDestroySwapchain);
            fn(Device, Swapchain, null);
        }
    }

    private void DeviceWaitIdle()
    {
        try { Vk.DeviceWaitIdle(Device); } catch { }
    }

    private static SurfaceFormatKHR ChooseFormat(SurfaceFormatKHR[] f)
    {
        foreach (var x in f)
            if (x.Format == Format.B8G8R8A8Srgb || x.Format == Format.R8G8B8A8Srgb) return x;
        return f[0];
    }

    private static PresentModeKHR ChoosePresentMode(PresentModeKHR[] m)
    {
        foreach (var x in m)
            if (x == PresentModeKHR.MailboxKhr || x == PresentModeKHR.ImmediateKhr) return x;
        return PresentModeKHR.FifoKhr;
    }

    private static Extent2D ChooseExtent(SurfaceCapabilitiesKHR c, uint fw, uint fh)
    {
        if (c.CurrentExtent.Width != uint.MaxValue) return c.CurrentExtent;
        return new Extent2D(
            Math.Clamp(fw, c.MinImageExtent.Width, c.MaxImageExtent.Width),
            Math.Clamp(fh, c.MinImageExtent.Height, c.MaxImageExtent.Height));
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result GetCapsPtr(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR s, SurfaceCapabilitiesKHR* c);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result GetFormatsPtr(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR s, uint* c, SurfaceFormatKHR* f);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result GetModesPtr(Silk.NET.Vulkan.PhysicalDevice pd, SurfaceKHR s, uint* c, PresentModeKHR* m);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result CreateSwapchainPtr(Silk.NET.Vulkan.Device d, SwapchainCreateInfoKHR* ci, AllocationCallbacks* a, SwapchainKHR* sc);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate void GetSwapchainImagesPtr(Silk.NET.Vulkan.Device d, SwapchainKHR sc, uint* c, Image* imgs);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate void DestroySwapchainFn(Silk.NET.Vulkan.Device d, SwapchainKHR sc, AllocationCallbacks* a);
}
