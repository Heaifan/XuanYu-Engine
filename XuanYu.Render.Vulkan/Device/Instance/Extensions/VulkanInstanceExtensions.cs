namespace XuanYu.Render.Vulkan;

// VK3-B1：Instance 启用的最小扩展名集合；常量保存纯名称，不含 C 字符串终止符。
// 禁止在此添加 Device / Swapchain / 其他扩展。
public static class VulkanInstanceExtensions
{
    public const string Surface = "VK_KHR_surface";
    public const string Win32Surface = "VK_KHR_win32_surface";
    public const string Properties2 = "VK_KHR_get_physical_device_properties2";
    public const string SurfaceMaintenance1 = "VK_EXT_surface_maintenance1";
    public const string SurfaceCapabilities2 = "VK_KHR_get_surface_capabilities2";
}
