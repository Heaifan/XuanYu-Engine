using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Render;

// VK4-D：单色清屏日志格式化（统一经 Bridge 的 Emit 单出口）。
public static class VulkanClearFrameLogFormatter
{
    public static string Created() => "【VulkanClearFrame】RenderPass + Framebuffer 创建成功";
    public static string Rebuilt(Extent2D extent, uint n) => $"【VulkanClearFrame】Framebuffer 重建成功；{extent.Width}x{extent.Height}（物理像素）；{n} 张";
    public static string OutOfDatePaused() => "【VulkanClearFrame】Swapchain 已过期，暂停 Present，等待 Resize 重建";
    public static string Disposed() => "【VulkanClearFrame】RenderPass + Framebuffer 释放成功";
    public static string LoopStarted() => "【VulkanClearFrame】Present 泵已启动（独立线程）";
    public static string LoopStopped() => "【VulkanClearFrame】Present 泵已停止";
    public static string Skipped(string r) => $"【VulkanClearFrame】跳过：{r}";
    public static string PresentError(string r) => $"【VulkanClearFrame】Present 异常：{r}";
    public static string FirstPresented(uint idx) => $"【VulkanClearFrame】首帧 Present 成功；imageIndex={idx}";
}
