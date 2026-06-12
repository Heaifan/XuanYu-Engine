using System.Runtime.InteropServices;
using FluidWarfare.Render.Vulkan.Scene3D.Depth;
using FluidWarfare.Render.Vulkan.Scene3D.Session.Surface;
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
    public static int LiveCount => TotalCreateCount - TotalDestroyCount;
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

        // ── Surface capabilities ──────────────────────────────────
        SurfaceCapabilitiesKHR caps;
        var capsResult = functions.GetCapabilities(physicalDevice, surface, &caps);
        if (capsResult != Result.Success)
            return Fail(VulkanScene3dSwapchainStage.SurfaceCapabilities, capsResult,
                $"查询 Surface 能力失败：{capsResult}（请求尺寸 {w}x{h}）。");

        // ── Surface formats (two-stage + Incomplete retry) ─────────
        if (!VulkanScene3dSurfaceFormats.TryEnumerate(
                functions.GetFormats, physicalDevice, surface,
                out var fmts, out var fmtErr))
            return Fail(VulkanScene3dSwapchainStage.SurfaceFormats, null, fmtErr);

        var chosenFmt = ChooseFormat(fmts);
        var extent = ChooseExtent(caps, w, h);
        var imgCount = Math.Clamp(caps.MinImageCount + 1,
            caps.MinImageCount,
            caps.MaxImageCount > 0 ? caps.MaxImageCount : uint.MaxValue);

        // ── Present modes (two-stage + Incomplete retry) ──────────
        if (!VulkanScene3dPresentModes.TryEnumerate(
                functions.GetPresentModes, physicalDevice, surface,
                out var modes, out var modeErr))
            return Fail(VulkanScene3dSwapchainStage.PresentModes, null, modeErr);

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

        // Get images (two-stage + Incomplete retry with re-query)
        const int maxImageRetries = 3;
        Image[] swapchainImages = [];
        for (var attempt = 0; attempt < maxImageRetries; attempt++)
        {
            uint count = 0;
            var countResult = functions.GetImages(device, sc, &count, null);
            if (countResult != Result.Success)
                return Fail(VulkanScene3dSwapchainStage.GetSwapchainImages, countResult,
                    $"GetSwapchainImages 第一阶段失败（尝试 {attempt + 1}）。");

            if (count == 0)
                return Fail(
                    VulkanScene3dSwapchainStage.GetSwapchainImages, null,
                    "Swapchain 图像数为 0。");

            var buffer = new Image[count];
            uint written = count;
            fixed (Image* imgPtr = buffer)
            {
                var fillResult = functions.GetImages(device, sc, &written, imgPtr);

                if (fillResult == Result.Success)
                {
                    swapchainImages = buffer;
                    r.ImageCount = (int)count;
                    break;
                }

                if (fillResult != Result.Incomplete)
                    return Fail(VulkanScene3dSwapchainStage.GetSwapchainImages, fillResult,
                        $"GetSwapchainImages 第二阶段失败：{fillResult}。");
                // Incomplete：回到循环顶部重新查询 count
            }
        }
        if (swapchainImages.Length == 0)
            return Fail(
                VulkanScene3dSwapchainStage.GetSwapchainImages, null,
                $"GetSwapchainImages 超过最大重试次数（{maxImageRetries}）。");
        r.Extent = extent;

        // Color ImageViews
        var imageCount = swapchainImages.Length;
        r.ColorViews = new ImageView[imageCount];
        for (var i = 0; i < imageCount; i++)
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
        r.DepthImages = new Image[imageCount];
        r.DepthMemories = new DeviceMemory[imageCount];
        r.DepthViews = new ImageView[imageCount];
        if (!VulkanScene3dDepthAttachments.Create(vk, physicalDevice, device,
                extent, depthInfo.ChosenFormat, (uint)imageCount,
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
        r.Framebuffers = new Framebuffer[imageCount];
        var fba = stackalloc ImageView[2];
        for (var i = 0; i < imageCount; i++)
        {
            fba[0] = r.ColorViews[i];
            fba[1] = r.DepthViews[i];
            var fbCI = new FramebufferCreateInfo
            {
                SType = StructureType.FramebufferCreateInfo,
                RenderPass = r.RenderPass,
                AttachmentCount = 2,
                PAttachments = fba,
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

        // Sync objects (individual creation with per-object VkResult logging)
        var semCI = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo };
        var fenceCI = new FenceCreateInfo { SType = StructureType.FenceCreateInfo, Flags = FenceCreateFlags.SignaledBit };
        Silk.NET.Vulkan.Semaphore imageAvail = default;
        var availResult = vk.CreateSemaphore(device, &semCI, null, out imageAvail);
        if (availResult != Result.Success)
            return Fail(VulkanScene3dSwapchainStage.Synchronization, availResult,
                $"图像可用 Semaphore 创建失败：{availResult}。");

        Silk.NET.Vulkan.Semaphore renderFin = default;
        var finResult = vk.CreateSemaphore(device, &semCI, null, out renderFin);
        if (finResult != Result.Success)
        {
            vk.DestroySemaphore(device, imageAvail, null);
            return Fail(VulkanScene3dSwapchainStage.Synchronization, finResult,
                $"渲染完成 Semaphore 创建失败：{finResult}。");
        }

        Fence frameFence = default;
        var fenceResult = vk.CreateFence(device, &fenceCI, null, out frameFence);
        if (fenceResult != Result.Success)
        {
            vk.DestroySemaphore(device, renderFin, null);
            vk.DestroySemaphore(device, imageAvail, null);
            return Fail(VulkanScene3dSwapchainStage.Synchronization, fenceResult,
                $"帧 Fence 创建失败：{fenceResult}。");
        }

        r.SemAvail = imageAvail;
        r.SemFin = renderFin;
        r.Fence = frameFence;

        // 成功后不得再有 return Failed

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
