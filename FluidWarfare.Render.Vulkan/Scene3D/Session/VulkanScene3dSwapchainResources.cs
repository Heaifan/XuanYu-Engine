using System.Runtime.InteropServices;
using FluidWarfare.Render.Vulkan.Scene3D.Depth;
using FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// 持有与当前 Swapchain 尺寸相关的资源。
/// 私有构造 + 强制函数集合，不允许先创建再补传销毁指针。
/// Dispose 幂等，每个 native 资源销毁后置空句柄。
/// </summary>
public sealed unsafe class VulkanScene3dSwapchainResources : IDisposable
{
    private readonly Vk _vk;
    private readonly Silk.NET.Vulkan.Device _device;
    private readonly VulkanScene3dSwapchainFunctions _functions;
    private bool _disposed;

    public Silk.NET.Vulkan.Device Device => _device;
    public SwapchainKHR Swapchain { get; private set; }
    public ImageView[] ColorViews { get; private set; } = [];
    public Image[] DepthImages { get; private set; } = [];
    public DeviceMemory[] DepthMemories { get; private set; } = [];
    public ImageView[] DepthViews { get; private set; } = [];
    public RenderPass RenderPass { get; private set; }
    public Framebuffer[] Framebuffers { get; private set; } = [];
    public CommandPool CommandPool { get; private set; }
    public CommandBuffer CommandBuffer { get; private set; }
    public Silk.NET.Vulkan.Semaphore SemAvail { get; private set; }
    public Silk.NET.Vulkan.Semaphore SemFin { get; private set; }
    public Fence Fence { get; private set; }
    public Extent2D Extent { get; private set; }
    public Format DepthFormat { get; private set; }
    public int ImageCount { get; private set; }

    // 生命周期计数器
    public static int TotalCreateCount;
    public static int TotalDestroyCount;
    public int SwapchainDestroyCount { get; private set; }

    // ─── 私有构造 ──────────────────────────────────────────────

    private VulkanScene3dSwapchainResources(Vk vk, Silk.NET.Vulkan.Device device,
        VulkanScene3dSwapchainFunctions functions)
    {
        _vk = vk;
        _device = device;
        _functions = functions;
    }

    // ─── 工厂方法 ──────────────────────────────────────────────

    /// <summary>
    /// 创建 swapchain 级资源。
    /// <paramref name="oldSwapchain"/>：首次启动传 default，resize 时任传当前有效 Swapchain。
    /// </summary>
    public static VulkanScene3dSwapchainCreateResult TryCreate(
        Vk vk,
        Silk.NET.Vulkan.Device device,
        Silk.NET.Vulkan.PhysicalDevice physicalDevice,
        SurfaceKHR surface,
        uint requestedWidth,
        uint requestedHeight,
        uint queueFamilyIndex,
        VulkanScene3dSwapchainFunctions functions,
        SwapchainKHR oldSwapchain)
    {
        var r = new VulkanScene3dSwapchainResources(vk, device, functions);
        uint w = requestedWidth, h = requestedHeight;

        // 局部失败出口：清理已创建资源后返回结构化错误
        VulkanScene3dSwapchainCreateResult Fail(
            VulkanScene3dSwapchainStage stage, Result? result, string message)
        {
            r.Dispose();
            return VulkanScene3dSwapchainCreateResult.Failed(stage, result, w, h, message);
        }

        // Surface capabilities
        SurfaceCapabilitiesKHR caps;
        functions.GetCapabilities(physicalDevice, surface, &caps);

        // Surface formats
        uint fmtCount = 0;
        functions.GetFormats(physicalDevice, surface, &fmtCount, null);
        if (fmtCount == 0)
            return Fail(VulkanScene3dSwapchainStage.SurfaceFormats, null, "无可用 Surface 格式。");
        var fmts = new SurfaceFormatKHR[fmtCount];
        fixed (SurfaceFormatKHR* fp = fmts) functions.GetFormats(physicalDevice, surface, &fmtCount, fp);

        var chosenFmt = ChooseFormat(fmts);
        var extent = ChooseExtent(caps, w, h);
        var imgCount = Math.Clamp(caps.MinImageCount + 1,
            caps.MinImageCount,
            caps.MaxImageCount > 0 ? caps.MaxImageCount : uint.MaxValue);

        // Present modes
        uint modeCount = 0;
        functions.GetPresentModes(physicalDevice, surface, &modeCount, null);
        if (modeCount == 0)
            return Fail(
                VulkanScene3dSwapchainStage.PresentModes, null, "无可用 PresentMode。");
        var modes = new PresentModeKHR[modeCount];
        fixed (PresentModeKHR* mp = modes) functions.GetPresentModes(physicalDevice, surface, &modeCount, mp);
        var presentMode = ChoosePresentMode(modes);

        // Create swapchain with OldSwapchain
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
            OldSwapchain = oldSwapchain,
            Clipped = Vk.True
        };

        SwapchainKHR sc;
        var createResult = functions.Create(device, &scCI, null, &sc);
        if (createResult != Result.Success)
            return Fail(
                VulkanScene3dSwapchainStage.CreateSwapchain, createResult,
                $"Swapchain 创建失败：{createResult}（请求尺寸 {w}x{h}）。");
        r.Swapchain = sc;
        TotalCreateCount++;

        // Get images
        uint actualCount = 0;
        if (functions.GetImages(device, sc, &actualCount, null) != Result.Success)
            return Fail(
                VulkanScene3dSwapchainStage.GetSwapchainImages, null, "GetSwapchainImages 第一阶段失败。");
        if (actualCount == 0)
            return Fail(
                VulkanScene3dSwapchainStage.GetSwapchainImages, null, "Swapchain 图像数为 0。");
        var swapchainImages = new Image[actualCount];
        fixed (Image* ip = swapchainImages) functions.GetImages(device, sc, &actualCount, ip);
        r.ImageCount = (int)actualCount;
        r.Extent = extent;

        // Color ImageViews
        r.ColorViews = new ImageView[actualCount];
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
            ImageView newCv = default;
            if (vk.CreateImageView(device, &ivCI, null, out newCv) != Result.Success)
                return Fail(
                    VulkanScene3dSwapchainStage.ColorImageViews, null, $"Color ImageView {i} 创建失败。");
            r.ColorViews[i] = newCv;
        }

        // Depth format + attachments
        var depthInfo = VulkanScene3dDepthFormatSelector.Select(vk, physicalDevice);
        if (!depthInfo.IsSupported)
            return Fail(
                VulkanScene3dSwapchainStage.DepthAttachments, null, depthInfo.Message);
        r.DepthFormat = depthInfo.ChosenFormat;
        r.DepthImages = new Image[actualCount];
        r.DepthMemories = new DeviceMemory[actualCount];
        r.DepthViews = new ImageView[actualCount];
        if (!VulkanScene3dDepthAttachments.Create(vk, physicalDevice, device,
                extent, depthInfo.ChosenFormat, (uint)actualCount,
                r.DepthImages, r.DepthMemories, r.DepthViews, out var depthErr))
            return Fail(
                VulkanScene3dSwapchainStage.DepthAttachments, null, depthErr);

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
        RenderPass newRp = default;
        if (vk.CreateRenderPass(device, &rpCI, null, out newRp) != Result.Success)
            return Fail(
                VulkanScene3dSwapchainStage.RenderPass, null, "RenderPass 创建失败。");
        r.RenderPass = newRp;

        // Framebuffers
        r.Framebuffers = new Framebuffer[actualCount];
        for (var i = 0; i < actualCount; i++)
        {
            var fba = stackalloc ImageView[] { r.ColorViews[i], r.DepthViews[i] };
            var fbCI = new FramebufferCreateInfo
            {
                SType = StructureType.FramebufferCreateInfo,
                RenderPass = r.RenderPass,
                AttachmentCount = 2,
                PAttachments = (ImageView*)fba,
                Width = extent.Width, Height = extent.Height, Layers = 1
            };
            Framebuffer newFb = default;
            if (vk.CreateFramebuffer(device, &fbCI, null, out newFb) != Result.Success)
                return Fail(
                    VulkanScene3dSwapchainStage.Framebuffers, null, $"Framebuffer {i} 创建失败。");
            r.Framebuffers[i] = newFb;
        }

        // CommandPool
        var poolCI = new CommandPoolCreateInfo { SType = StructureType.CommandPoolCreateInfo, QueueFamilyIndex = queueFamilyIndex };
        CommandPool newPool = default;
        if (vk.CreateCommandPool(device, &poolCI, null, out newPool) != Result.Success)
            return Fail(
                VulkanScene3dSwapchainStage.CommandPool, null, "CommandPool 创建失败。");
        r.CommandPool = newPool;

        // CommandBuffer
        var allocCI = new CommandBufferAllocateInfo
        {
            SType = StructureType.CommandBufferAllocateInfo,
            CommandPool = r.CommandPool,
            Level = CommandBufferLevel.Primary,
            CommandBufferCount = 1
        };
        CommandBuffer newCmd = default;
        if (vk.AllocateCommandBuffers(device, &allocCI, out newCmd) != Result.Success)
            return Fail(
                VulkanScene3dSwapchainStage.CommandBuffer, null, "CommandBuffer 创建失败。");
        r.CommandBuffer = newCmd;

        // Sync objects
        var semCI = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo };
        var fenceCI = new FenceCreateInfo { SType = StructureType.FenceCreateInfo, Flags = FenceCreateFlags.SignaledBit };
        Silk.NET.Vulkan.Semaphore newAvail = default, newFin = default;
        Fence newFence = default;
        if (vk.CreateSemaphore(device, &semCI, null, out newAvail) != Result.Success ||
            vk.CreateSemaphore(device, &semCI, null, out newFin) != Result.Success ||
            vk.CreateFence(device, &fenceCI, null, out newFence) != Result.Success)
        {
            if (newAvail.Handle != 0) vk.DestroySemaphore(device, newAvail, null);
            if (newFin.Handle != 0) vk.DestroySemaphore(device, newFin, null);
            return Fail(
                VulkanScene3dSwapchainStage.Synchronization, null, "同步对象创建失败。");
        }
        r.SemAvail = newAvail;
        r.SemFin = newFin;
        r.Fence = newFence;

        return new VulkanScene3dSwapchainCreateResult(true, r,
            VulkanScene3dSwapchainStage.CreateSwapchain, Result.Success, w, h,
            "Swapchain 创建成功。");
    }

    // ─── 幂等 Dispose ─────────────────────────────────────────

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        try { _vk.DeviceWaitIdle(_device); } catch { }

        // Fence / Semaphores
        if (SemAvail.Handle != 0 || SemFin.Handle != 0 || Fence.Handle != 0)
        {
            if (SemAvail.Handle != 0) { _vk.DestroySemaphore(_device, SemAvail, null); SemAvail = default; }
            if (SemFin.Handle != 0) { _vk.DestroySemaphore(_device, SemFin, null); SemFin = default; }
            if (Fence.Handle != 0) { _vk.DestroyFence(_device, Fence, null); Fence = default; }
        }

        // CommandPool
        if (CommandPool.Handle != 0) { _vk.DestroyCommandPool(_device, CommandPool, null); CommandPool = default; }

        // Framebuffers
        foreach (var fb in Framebuffers)
            if (fb.Handle != 0) _vk.DestroyFramebuffer(_device, fb, null);
        Framebuffers = [];

        // Depth ImageViews
        foreach (var dv in DepthViews)
            if (dv.Handle != 0) _vk.DestroyImageView(_device, dv, null);
        DepthViews = [];

        // Depth Images
        foreach (var di in DepthImages)
            if (di.Handle != 0) _vk.DestroyImage(_device, di, null);
        DepthImages = [];

        // Depth Memory
        foreach (var dm in DepthMemories)
            if (dm.Handle != 0) _vk.FreeMemory(_device, dm, null);
        DepthMemories = [];

        // RenderPass
        if (RenderPass.Handle != 0) { _vk.DestroyRenderPass(_device, RenderPass, null); RenderPass = default; }

        // Color ImageViews
        foreach (var iv in ColorViews)
            if (iv.Handle != 0) _vk.DestroyImageView(_device, iv, null);
        ColorViews = [];

        // Swapchain — 使用强制传入的函数集合
        if (Swapchain.Handle != 0)
        {
            _functions.Destroy(_device, Swapchain, null);
            Swapchain = default;
            SwapchainDestroyCount++;
            TotalDestroyCount++;
        }
    }

    // ─── 辅助 ───────────────────────────────────────────────────

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
}
