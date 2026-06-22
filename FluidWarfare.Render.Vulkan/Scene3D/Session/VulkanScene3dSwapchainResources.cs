using System.Runtime.InteropServices;
using FluidWarfare.Render.Vulkan.Scene3D.Depth;
using FluidWarfare.Render.Vulkan.Scene3D.Session.Surface;
using FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain;
using FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain.Images;
using FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain.Sync;
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

        var chosenFmt = VulkanScene3dSwapchainSelection.ChooseFormat(fmts);
        var extent = VulkanScene3dSwapchainExtent.ChooseExtent(caps, w, h);
        var imgCount = Math.Clamp(caps.MinImageCount + 1,
            caps.MinImageCount,
            caps.MaxImageCount > 0 ? caps.MaxImageCount : uint.MaxValue);

        // ── Present modes (two-stage + Incomplete retry) ──────────
        if (!VulkanScene3dPresentModes.TryEnumerate(
                functions.GetPresentModes, physicalDevice, surface,
                out var modes, out var modeErr))
            return Fail(VulkanScene3dSwapchainStage.PresentModes, null, modeErr);

        var presentMode = VulkanScene3dSwapchainSelection.ChoosePresentMode(modes);

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
        r.ColorViews = VulkanScene3dSwapchainImageViews.Create(vk, device, swapchainImages, chosenFmt.Format);
        if (r.ColorViews.Length == 0)
            return Fail(VulkanScene3dSwapchainStage.ColorImageViews, null, "Color ImageView 创建失败。");

        // Depth format + attachments
        var depthInfo = VulkanScene3dDepthFormatSelector.Select(vk, physicalDevice);
        if (!depthInfo.IsSupported)
            return Fail(
                VulkanScene3dSwapchainStage.DepthAttachments, null, depthInfo.Message);
        r.DepthFormat = depthInfo.ChosenFormat;
        r.DepthImages = new Image[swapchainImages.Length];
        r.DepthMemories = new DeviceMemory[swapchainImages.Length];
        r.DepthViews = new ImageView[swapchainImages.Length];
        if (!VulkanScene3dDepthAttachments.Create(vk, physicalDevice, device,
                extent, depthInfo.ChosenFormat, (uint)swapchainImages.Length,
                r.DepthImages, r.DepthMemories, r.DepthViews, out var depthErr))
            return Fail(
                VulkanScene3dSwapchainStage.DepthAttachments, null, depthErr);
        // RenderPass
        r.RenderPass = VulkanScene3dSwapchainFramebuffers.CreateRenderPass(vk, device, chosenFmt.Format, depthInfo.ChosenFormat);
        if (r.RenderPass.Handle == 0)
            return Fail(VulkanScene3dSwapchainStage.RenderPass, null, "RenderPass 创建失败。");

        // Framebuffers
        r.Framebuffers = VulkanScene3dSwapchainFramebuffers.CreateFramebuffers(vk, device, r.RenderPass, r.ColorViews, r.DepthViews, extent);
        if (r.Framebuffers.Length == 0)
            return Fail(VulkanScene3dSwapchainStage.Framebuffers, null, "Framebuffer 创建失败。");

        // CommandPool
        // CommandPool
        r.CommandPool = VulkanScene3dSwapchainSync.CreateCommandPool(vk, device, queueFamilyIndex);
        if (r.CommandPool.Handle == 0)
            return Fail(VulkanScene3dSwapchainStage.CommandPool, null, "CommandPool 创建失败。");

        // CommandBuffer
        r.CommandBuffer = VulkanScene3dSwapchainSync.AllocateCommandBuffer(vk, device, r.CommandPool);
        if (r.CommandBuffer.Handle == 0)
            return Fail(VulkanScene3dSwapchainStage.CommandBuffer, null, "CommandBuffer 创建失败。");

        // Sync objects
        if (!VulkanScene3dSwapchainSync.CreateSemaphore(vk, device, out var imageAvail, out var semErr))
            return Fail(VulkanScene3dSwapchainStage.Synchronization, null, semErr);
        if (!VulkanScene3dSwapchainSync.CreateSemaphore(vk, device, out var renderFin, out var finErr))
        { vk.DestroySemaphore(device, imageAvail, null); return Fail(VulkanScene3dSwapchainStage.Synchronization, null, finErr); }
        if (!VulkanScene3dSwapchainSync.CreateFence(vk, device, out var frameFence, out var fenceErr))
        { vk.DestroySemaphore(device, renderFin, null); vk.DestroySemaphore(device, imageAvail, null); return Fail(VulkanScene3dSwapchainStage.Synchronization, null, fenceErr); }

        r.SemAvail = imageAvail;
        r.SemFin = renderFin;
        r.Fence = frameFence;
        // 成功后不得再有 return Failed

        return new VulkanScene3dSwapchainCreateResult(true, r,
            VulkanScene3dSwapchainStage.CreateSwapchain, Result.Success, w, h, null);
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

}
