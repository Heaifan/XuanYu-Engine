namespace XuanYu.Render.Vulkan;

// VK3-B1：Instance 启用的最小扩展名集合（仅 surface 相关，以 null 结尾字节序列）。
// 禁止在此添加 Device / Swapchain / 其他扩展。
public static class VulkanInstanceExtensions
{
    public const string Surface = "VK_KHR_surface\0";
    public const string Win32Surface = "VK_KHR_win32_surface\0";
}
