namespace FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain.Lifecycle;

/// <summary>VulkanScene3dSwapchainResources 幂等 Dispose 辅助。</summary>
internal static class VulkanScene3dSwapchainDispose
{
    public static void DisposeResources(VulkanScene3dSwapchainResources r)
    {
        r.Dispose();
    }
}
