using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session.Swapchain;

/// <summary>Swapchain Extent 计算。</summary>
internal static class VulkanScene3dSwapchainExtent
{
    public static Extent2D ChooseExtent(SurfaceCapabilitiesKHR capabilities, uint requestedWidth, uint requestedHeight)
    {
        if (capabilities.CurrentExtent.Width != uint.MaxValue)
            return capabilities.CurrentExtent;
        return new Extent2D(
            Math.Clamp(requestedWidth, capabilities.MinImageExtent.Width, capabilities.MaxImageExtent.Width),
            Math.Clamp(requestedHeight, capabilities.MinImageExtent.Height, capabilities.MaxImageExtent.Height));
    }
}
