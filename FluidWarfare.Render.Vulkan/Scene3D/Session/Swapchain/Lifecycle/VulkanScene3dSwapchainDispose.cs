using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain.Lifecycle;

/// <summary>VulkanScene3dSwapchainResources 幂等销毁辅助。</summary>
internal static unsafe class VulkanScene3dSwapchainDispose
{
    public static void DisposeResources(
        Vk vk, Silk.NET.Vulkan.Device device,
        VulkanScene3dSwapchainFunctions functions,
        ref bool disposed,
        ref SwapchainKHR swapchain,
        ref ImageView[] colorViews,
        ref Image[] depthImages,
        ref DeviceMemory[] depthMemories,
        ref ImageView[] depthViews,
        ref RenderPass renderPass,
        ref Framebuffer[] framebuffers,
        ref CommandPool commandPool,
        ref Silk.NET.Vulkan.Semaphore semAvail,
        ref Silk.NET.Vulkan.Semaphore semFin,
        ref Fence fence,
        ref int swapchainDestroyCount)
    {
        if (disposed) return;
        disposed = true;
        try { vk.DeviceWaitIdle(device); } catch { }

        if (semAvail.Handle != 0 || semFin.Handle != 0 || fence.Handle != 0)
        {
            if (semAvail.Handle != 0) { vk.DestroySemaphore(device, semAvail, null); semAvail = default; }
            if (semFin.Handle != 0) { vk.DestroySemaphore(device, semFin, null); semFin = default; }
            if (fence.Handle != 0) { vk.DestroyFence(device, fence, null); fence = default; }
        }
        if (commandPool.Handle != 0) { vk.DestroyCommandPool(device, commandPool, null); commandPool = default; }
        foreach (var fb in framebuffers) { if (fb.Handle != 0) vk.DestroyFramebuffer(device, fb, null); }
        framebuffers = [];
        foreach (var dv in depthViews) { if (dv.Handle != 0) vk.DestroyImageView(device, dv, null); }
        depthViews = [];
        foreach (var di in depthImages) { if (di.Handle != 0) vk.DestroyImage(device, di, null); }
        depthImages = [];
        foreach (var dm in depthMemories) { if (dm.Handle != 0) vk.FreeMemory(device, dm, null); }
        depthMemories = [];
        if (renderPass.Handle != 0) { vk.DestroyRenderPass(device, renderPass, null); renderPass = default; }
        foreach (var iv in colorViews) { if (iv.Handle != 0) vk.DestroyImageView(device, iv, null); }
        colorViews = [];
        if (swapchain.Handle != 0)
        {
            functions.Destroy(device, swapchain, null);
            swapchain = default;
            swapchainDestroyCount++;
            VulkanScene3dSwapchainResources.TotalDestroyCount++;
        }
    }
}
