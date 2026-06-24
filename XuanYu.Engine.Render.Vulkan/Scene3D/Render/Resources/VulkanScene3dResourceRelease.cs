using System.Runtime.InteropServices;

namespace XuanYu.Engine.Render.Vulkan.Scene3D;

/// <summary>Vulkan 资源释放步骤。按依赖逆序：Fence→Pool→Buffer→Pipeline→Shader→Framebuffer→DepthView→DepthImage→DepthMemory→RenderPass→ImageView→Swapchain→Device→Surface→Instance。</summary>
unsafe partial class VulkanScene3dRenderResources
{
    void ReleaseSwapchainAndSurface()
    {
        if (SyncOk && Device.Handle != 0)
        {
            if (SemAvail.Handle != 0) Vk!.DestroySemaphore(Device, SemAvail, null);
            if (SemFin.Handle != 0) Vk!.DestroySemaphore(Device, SemFin, null);
            if (Fence.Handle != 0) Vk!.DestroyFence(Device, Fence, null);
        }
        if (PoolOk && Device.Handle != 0) Vk!.DestroyCommandPool(Device, CommandPool, null);
    }

    void ReleaseRenderResources()
    {
        if (Device.Handle == 0) return;

        if (UnitBufOk) { if (UnitBuffer.Handle != 0) Vk!.DestroyBuffer(Device, UnitBuffer, null); if (UnitMemory.Handle != 0) Vk!.FreeMemory(Device, UnitMemory, null); }
        if (GridBufOk) { if (GridBuffer.Handle != 0) Vk!.DestroyBuffer(Device, GridBuffer, null); if (GridMemory.Handle != 0) Vk!.FreeMemory(Device, GridMemory, null); }
        if (UnitPipeOk) Vk!.DestroyPipeline(Device, UnitPipeline, null);
        if (GridPipeOk) Vk!.DestroyPipeline(Device, GridPipeline, null);
        if (LayoutOk) Vk!.DestroyPipelineLayout(Device, PipelineLayout, null);
        if (FragModOk) Vk!.DestroyShaderModule(Device, FragModule, null);
        if (VertModOk) Vk!.DestroyShaderModule(Device, VertModule, null);

        foreach (var fb in Framebuffers) if (fb.Handle != 0) Vk!.DestroyFramebuffer(Device, fb, null);
        if (DepthOk) { foreach (var dv in DepthViews) if (dv.Handle != 0) Vk!.DestroyImageView(Device, dv, null); }
        if (DepthOk) { foreach (var di in DepthImages) if (di.Handle != 0) Vk!.DestroyImage(Device, di, null); }
        if (DepthOk) { foreach (var dm in DepthMemories) if (dm.Handle != 0) Vk!.FreeMemory(Device, dm, null); }
        if (RpOk && RenderPass.Handle != 0) Vk!.DestroyRenderPass(Device, RenderPass, null);
        foreach (var iv in ImageViews) if (iv.Handle != 0) Vk!.DestroyImageView(Device, iv, null);
        if (ScOk && FnDestroySwapchain != 0)
            Marshal.GetDelegateForFunctionPointer<DestroySwapchainPtr>(FnDestroySwapchain)(Device, Swapchain, null);
    }

    void ReleaseDeviceAndInstance()
    {
        if (DevOk && Device.Handle != 0) Vk!.DestroyDevice(Device, null);
        if (SurfOk && FnDestroySurface != 0)
            Marshal.GetDelegateForFunctionPointer<DestroySurfacePtr>(FnDestroySurface)(Instance, Surface, null);
        if (InstOk) Vk!.DestroyInstance(Instance, null);
    }
}
