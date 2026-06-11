using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>
/// 持有 Scene3D 本轮创建的 Vulkan 资源句柄。
/// 负责按依赖逆序释放资源，释放顺序不分散在 Renderer 中。
/// </summary>
public sealed unsafe class VulkanScene3dRenderResources : IDisposable
{
    // 函数指针（由 Renderer 设置，用于销毁）
    public nint FnDestroySurface;
    public nint FnDestroySwapchain;

    // Vulkan 核心对象
    public Vk? Vk;
    public Silk.NET.Vulkan.Instance Instance;
    public SurfaceKHR Surface;
    public Silk.NET.Vulkan.Device Device;
    public SwapchainKHR Swapchain;
    public ImageView[] ImageViews = [];
    public RenderPass RenderPass;
    public Framebuffer[] Framebuffers = [];
    public CommandPool CommandPool;
    public CommandBuffer CommandBuffer;
    public Silk.NET.Vulkan.Semaphore SemAvail, SemFin;
    public Fence Fence;

    // Scene3D 专用资源
    public ShaderModule VertModule, FragModule;
    public PipelineLayout PipelineLayout;
    public Pipeline GridPipeline, UnitPipeline;
    public Silk.NET.Vulkan.Buffer GridBuffer, UnitBuffer;
    public DeviceMemory GridMemory, UnitMemory;

    // 成功标记
    public bool InstOk, SurfOk, DevOk, ScOk, RpOk, PoolOk, SyncOk;
    public bool VertModOk, FragModOk, LayoutOk;
    public bool GridPipeOk, UnitPipeOk;
    public bool GridBufOk, UnitBufOk;

    /// <summary>
    /// 按 Vulkan 资源依赖逆序释放。
    /// </summary>
    public void Dispose()
    {
        if (Vk is null) return;

        try { if (Device.Handle != 0) Vk.DeviceWaitIdle(Device); } catch { }

        // Fence / Semaphores
        if (SyncOk && Device.Handle != 0)
        {
            if (SemAvail.Handle != 0) Vk.DestroySemaphore(Device, SemAvail, null);
            if (SemFin.Handle != 0) Vk.DestroySemaphore(Device, SemFin, null);
            if (Fence.Handle != 0) Vk.DestroyFence(Device, Fence, null);
        }

        // CommandPool
        if (PoolOk && Device.Handle != 0) Vk.DestroyCommandPool(Device, CommandPool, null);

        // Vertex Buffers
        if (UnitBufOk && Device.Handle != 0)
        {
            if (UnitBuffer.Handle != 0) Vk.DestroyBuffer(Device, UnitBuffer, null);
            if (UnitMemory.Handle != 0) Vk.FreeMemory(Device, UnitMemory, null);
        }
        if (GridBufOk && Device.Handle != 0)
        {
            if (GridBuffer.Handle != 0) Vk.DestroyBuffer(Device, GridBuffer, null);
            if (GridMemory.Handle != 0) Vk.FreeMemory(Device, GridMemory, null);
        }

        // Pipelines
        if (UnitPipeOk && Device.Handle != 0) Vk.DestroyPipeline(Device, UnitPipeline, null);
        if (GridPipeOk && Device.Handle != 0) Vk.DestroyPipeline(Device, GridPipeline, null);

        // Pipeline Layout
        if (LayoutOk && Device.Handle != 0) Vk.DestroyPipelineLayout(Device, PipelineLayout, null);

        // Shader Modules
        if (FragModOk && Device.Handle != 0) Vk.DestroyShaderModule(Device, FragModule, null);
        if (VertModOk && Device.Handle != 0) Vk.DestroyShaderModule(Device, VertModule, null);

        // Framebuffers
        if (Device.Handle != 0)
            foreach (var fb in Framebuffers) if (fb.Handle != 0) Vk.DestroyFramebuffer(Device, fb, null);

        // RenderPass
        if (RpOk && Device.Handle != 0 && RenderPass.Handle != 0)
            Vk.DestroyRenderPass(Device, RenderPass, null);

        // ImageViews
        if (Device.Handle != 0)
            foreach (var iv in ImageViews) if (iv.Handle != 0) Vk.DestroyImageView(Device, iv, null);

        // Swapchain
        if (ScOk && FnDestroySwapchain != 0)
            Marshal.GetDelegateForFunctionPointer<DestroySwapchainPtr>(FnDestroySwapchain)(Device, Swapchain, null);

        // Device
        if (DevOk && Device.Handle != 0) Vk.DestroyDevice(Device, null);

        // Surface
        if (SurfOk && FnDestroySurface != 0)
            Marshal.GetDelegateForFunctionPointer<DestroySurfacePtr>(FnDestroySurface)(Instance, Surface, null);

        // Instance
        if (InstOk) Vk.DestroyInstance(Instance, null);
    }

    // 委托定义（与 Renderer 共享）
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void DestroySurfacePtr(Silk.NET.Vulkan.Instance i, SurfaceKHR s, AllocationCallbacks* a);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void DestroySwapchainPtr(Silk.NET.Vulkan.Device d, SwapchainKHR sc, AllocationCallbacks* a);
}
