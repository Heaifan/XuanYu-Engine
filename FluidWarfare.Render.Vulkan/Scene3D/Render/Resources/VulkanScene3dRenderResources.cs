using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>持有 Scene3D 本轮创建的 Vulkan 资源句柄。委托 ReleaseXxx 释放。</summary>
public sealed unsafe partial class VulkanScene3dRenderResources : IDisposable
{
    public nint FnDestroySurface, FnDestroySwapchain;
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

    public ShaderModule VertModule, FragModule;
    public PipelineLayout PipelineLayout;
    public Pipeline GridPipeline, UnitPipeline;
    public Silk.NET.Vulkan.Buffer GridBuffer, UnitBuffer;
    public DeviceMemory GridMemory, UnitMemory;

    public bool InstOk, SurfOk, DevOk, ScOk, RpOk, PoolOk, SyncOk;
    public bool VertModOk, FragModOk, LayoutOk;
    public bool GridPipeOk, UnitPipeOk;
    public bool GridBufOk, UnitBufOk;

    public void Dispose()
    {
        if (Vk is null) return;
        try { if (Device.Handle != 0) Vk.DeviceWaitIdle(Device); } catch { }
        ReleaseSwapchainAndSurface();
        ReleaseRenderResources();
        ReleaseDeviceAndInstance();
    }

    // 委托定义（与 Renderer 共享）
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void DestroySurfacePtr(Silk.NET.Vulkan.Instance i, SurfaceKHR s, AllocationCallbacks* a);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void DestroySwapchainPtr(Silk.NET.Vulkan.Device d, SwapchainKHR sc, AllocationCallbacks* a);
}
