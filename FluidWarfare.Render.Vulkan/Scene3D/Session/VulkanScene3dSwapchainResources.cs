using FluidWarfare.Render.Vulkan.Scene3D.Depth;
using FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain;
using FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain.Create;
using FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain.Images;
using FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain.Lifecycle;
using FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain.Sync;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

public sealed unsafe class VulkanScene3dSwapchainResources : IDisposable
{
    readonly Vk _vk;
    readonly Silk.NET.Vulkan.Device _device;
    readonly VulkanScene3dSwapchainFunctions _functions;
    bool _disposed;

    public Silk.NET.Vulkan.Device Device => _device;
    public SwapchainKHR Swapchain; // 字段（非属性 — 支持 ref 传递给 Dispose 辅助）
    public ImageView[] ColorViews = [];
    public Image[] DepthImages = [];
    public DeviceMemory[] DepthMemories = [];
    public ImageView[] DepthViews = [];
    public RenderPass RenderPass;
    public Framebuffer[] Framebuffers = [];
    public CommandPool CommandPool;
    public CommandBuffer CommandBuffer;
    public Silk.NET.Vulkan.Semaphore SemAvail;
    public Silk.NET.Vulkan.Semaphore SemFin;
    public Fence Fence;
    public Extent2D Extent;
    public Format DepthFormat;
    public int ImageCount;
    public static int TotalCreateCount;
    public static int TotalDestroyCount;
    public static int LiveCount => TotalCreateCount - TotalDestroyCount;
    public int SwapchainDestroyCount;

    VulkanScene3dSwapchainResources(Vk vk, Silk.NET.Vulkan.Device device,
        VulkanScene3dSwapchainFunctions functions)
    { _vk = vk; _device = device; _functions = functions; }

    public static VulkanScene3dSwapchainCreateResult TryCreate(
        Vk vk, Silk.NET.Vulkan.Device device, PhysicalDevice physicalDevice,
        SurfaceKHR surface, uint w, uint h, uint queueFamilyIndex,
        VulkanScene3dSwapchainFunctions functions, SwapchainKHR oldSwapchain)
    {
        var r = new VulkanScene3dSwapchainResources(vk, device, functions);
        var flow = VulkanScene3dSwapchainCreateFlow.Execute(
            vk, device, physicalDevice, surface, w, h, functions, oldSwapchain);
        if (flow.Error is not null)
        { r.Dispose(); return VulkanScene3dSwapchainCreateResult.Failed(
            VulkanScene3dSwapchainStage.SurfaceCapabilities, null, w, h, flow.Error); }

        r.Swapchain = flow.Swapchain; TotalCreateCount++;
        r.Extent = flow.Extent;

        r.ColorViews = VulkanScene3dSwapchainImageViews.Create(vk, device, flow.Images, flow.Format.Format);
        if (r.ColorViews.Length == 0) { r.Dispose(); return VulkanScene3dSwapchainCreateResult.Failed(
            VulkanScene3dSwapchainStage.ColorImageViews, null, w, h, "Color ImageView 创建失败。"); }

        var di = VulkanScene3dDepthFormatSelector.Select(vk, physicalDevice);
        if (!di.IsSupported) { r.Dispose(); return VulkanScene3dSwapchainCreateResult.Failed(
            VulkanScene3dSwapchainStage.DepthAttachments, null, w, h, di.Message); }
        r.DepthFormat = di.ChosenFormat;
        var ic = flow.Images.Length;
        r.DepthImages = new Image[ic]; r.DepthMemories = new DeviceMemory[ic]; r.DepthViews = new ImageView[ic];
        if (!VulkanScene3dDepthAttachments.Create(vk, physicalDevice, device,
            flow.Extent, di.ChosenFormat, (uint)ic, r.DepthImages, r.DepthMemories, r.DepthViews, out var de))
        { r.Dispose(); return VulkanScene3dSwapchainCreateResult.Failed(
            VulkanScene3dSwapchainStage.DepthAttachments, null, w, h, de); }

        r.RenderPass = VulkanScene3dSwapchainFramebuffers.CreateRenderPass(vk, device, flow.Format.Format, di.ChosenFormat);
        if (r.RenderPass.Handle == 0) { r.Dispose(); return VulkanScene3dSwapchainCreateResult.Failed(
            VulkanScene3dSwapchainStage.RenderPass, null, w, h, "RenderPass 创建失败。"); }

        r.Framebuffers = VulkanScene3dSwapchainFramebuffers.CreateFramebuffers(
            vk, device, r.RenderPass, r.ColorViews, r.DepthViews, flow.Extent);
        if (r.Framebuffers.Length == 0) { r.Dispose(); return VulkanScene3dSwapchainCreateResult.Failed(
            VulkanScene3dSwapchainStage.Framebuffers, null, w, h, "Framebuffer 创建失败。"); }

        r.CommandPool = VulkanScene3dSwapchainSync.CreateCommandPool(vk, device, queueFamilyIndex);
        if (r.CommandPool.Handle == 0) { r.Dispose(); return VulkanScene3dSwapchainCreateResult.Failed(
            VulkanScene3dSwapchainStage.CommandPool, null, w, h, "CommandPool 创建失败。"); }

        r.CommandBuffer = VulkanScene3dSwapchainSync.AllocateCommandBuffer(vk, device, r.CommandPool);
        if (r.CommandBuffer.Handle == 0) { r.Dispose(); return VulkanScene3dSwapchainCreateResult.Failed(
            VulkanScene3dSwapchainStage.CommandBuffer, null, w, h, "CommandBuffer 创建失败。"); }

        if (!VulkanScene3dSwapchainSync.CreateSemaphore(vk, device, out var imgAvail, out _))
        { r.Dispose(); return VulkanScene3dSwapchainCreateResult.Failed(
            VulkanScene3dSwapchainStage.Synchronization, null, w, h, "图像可用 Semaphore 创建失败。"); }
        if (!VulkanScene3dSwapchainSync.CreateSemaphore(vk, device, out var renFin, out _))
        { vk.DestroySemaphore(device, imgAvail, null); r.Dispose(); return VulkanScene3dSwapchainCreateResult.Failed(
            VulkanScene3dSwapchainStage.Synchronization, null, w, h, "渲染完成 Semaphore 创建失败。"); }
        if (!VulkanScene3dSwapchainSync.CreateFence(vk, device, out var frameFence, out _))
        { vk.DestroySemaphore(device, renFin, null); vk.DestroySemaphore(device, imgAvail, null); r.Dispose(); return VulkanScene3dSwapchainCreateResult.Failed(
            VulkanScene3dSwapchainStage.Synchronization, null, w, h, "帧 Fence 创建失败。"); }

        r.SemAvail = imgAvail; r.SemFin = renFin; r.Fence = frameFence;
        r.ImageCount = ic;
        return new VulkanScene3dSwapchainCreateResult(true, r,
            VulkanScene3dSwapchainStage.CreateSwapchain, Result.Success, w, h, null);
    }

    public void Dispose()
    {
        VulkanScene3dSwapchainDispose.DisposeResources(_vk, _device, _functions, ref _disposed,
            ref Swapchain, ref ColorViews, ref DepthImages, ref DepthMemories,
            ref DepthViews, ref RenderPass, ref Framebuffers, ref CommandPool,
            ref SemAvail, ref SemFin, ref Fence, ref SwapchainDestroyCount);
    }
}
