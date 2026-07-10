using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Render;

// VK4-D：单色清屏日志格式化（统一经 Bridge 的 Emit 单出口）。
public static class VulkanClearFrameLogFormatter
{
    public static string Created() => "【VulkanClearFrame】RenderPass + Framebuffer 创建成功";
    public static string Rebuilt(Extent2D extent, uint n) => $"【VulkanClearFrame】Framebuffer 重建成功；{extent.Width}x{extent.Height}（物理像素）；{n} 张";
    public static string Disposed() => "【VulkanClearFrame】RenderPass + Framebuffer 释放成功";
    public static string LoopStarted() => "【VulkanClearFrame】Present 泵已启动（独立线程）";
    public static string LoopStopped() => "【VulkanClearFrame】Present 泵已停止";
    public static string Skipped(string r) => $"【VulkanClearFrame】跳过：{r}";
    public static string PresentError(string r) => $"【VulkanClearFrame】Present 异常：{r}";
    public static string FirstPresented(uint idx) => $"【VulkanClearFrame】首帧 Present 成功；imageIndex={idx}";
    // RZ-VK5-A-R2：OutOfDate 自愈探针与恢复日志（替代原永久暂停日志）。
    public static string OutOfDateProbe(string source, Extent2D oldExtent, Extent2D newExtent, uint generation, double dpi)
        => $"【VulkanClearFrame】自愈探针：来源={source}；旧 extent={oldExtent.Width}x{oldExtent.Height}；新 Surface CurrentExtent={newExtent.Width}x{newExtent.Height}（物理像素）；DPI={dpi:F2}；逻辑≈{newExtent.Width / dpi:F0}x{newExtent.Height / dpi:F0}；generation={generation}";
    public static string OutOfDateRecovered(uint generation) => $"【VulkanClearFrame】Swapchain 自愈成功，已恢复 Present；generation={generation}";
    public static string OutOfDateRecoverFailed(string reason) => $"【VulkanClearFrame】Swapchain 过期恢复失败，暂停 Present：{reason}";
}
