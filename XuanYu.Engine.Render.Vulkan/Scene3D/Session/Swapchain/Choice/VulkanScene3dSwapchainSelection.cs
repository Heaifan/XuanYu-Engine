using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session.Swapchain;

/// <summary>Swapchain 表面格式与 PresentMode 选择。</summary>
internal static class VulkanScene3dSwapchainSelection
{
    public static SurfaceFormatKHR ChooseFormat(SurfaceFormatKHR[] formats)
    {
        foreach (var f in formats)
            if (f.Format == Format.B8G8R8A8Srgb || f.Format == Format.R8G8B8A8Srgb) return f;
        return formats[0];
    }

    public static PresentModeKHR ChoosePresentMode(PresentModeKHR[] modes)
    {
        foreach (var m in modes)
            if (m == PresentModeKHR.MailboxKhr || m == PresentModeKHR.ImmediateKhr) return m;
        return PresentModeKHR.FifoKhr;
    }
}
