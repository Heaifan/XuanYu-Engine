namespace XuanYu.Render.Vulkan.Swapchain;

// VK4-C：Swapchain 中文生命周期日志格式器。纯文本，无副作用。
public static class VulkanSwapchainLogFormatter
{
    public static string Creating() => "【VulkanSwapchain】开始创建 Swapchain";
    public static string Created(int views) => $"【VulkanSwapchain】Swapchain 创建成功；ImageView 创建成功 {views} 张";
    public static string Recreating() => "【VulkanSwapchain】开始重建 Swapchain";
    public static string Recreated(uint w, uint h, int views) => $"【VulkanSwapchain】Swapchain 重建成功；新 extent={w}x{h}；新 ImageView={views}";
    public static string Disposed() => "【VulkanSwapchain】Swapchain 释放成功";
    public static string Skipped(string reason) => $"【VulkanSwapchain】Swapchain 创建跳过：{reason}";
    public static string Failed(string detail) => $"【VulkanSwapchain】Swapchain 创建失败：{detail}";
}
