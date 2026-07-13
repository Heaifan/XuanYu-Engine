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
    public static string LoopStopTimedOut() => "【VulkanClearFrame】Present 泵停止超时：线程仍可能使用 Vulkan 资源，已禁止继续释放";
    public static string Skipped(string r) => $"【VulkanClearFrame】跳过：{r}";
    public static string PresentError(string r) => $"【VulkanClearFrame】Present 异常：{r}";
    public static string PresentFatal(string r) => $"【VulkanClearFrame】Present 致命退出，RenderSession 已失效：{r}";
    public static string SessionDisposed() => "【VulkanRenderSession】释放完成";
    public static string LogFallback(string e, string m) => $"【VulkanClearFrame】日志回调失败：{e}；原日志：{m}";
    public static string FirstPresented(uint idx) => $"【VulkanClearFrame】首帧 Present 成功；imageIndex={idx}";
    // RZ-VK5-A-R2：OutOfDate 自愈探针与恢复日志（替代原永久暂停日志）。
    public static string OutOfDateProbe(string source, Extent2D oldExtent, Extent2D newExtent, uint generation, double dpi)
        => $"【VulkanClearFrame】自愈探针：来源={source}；旧 extent={oldExtent.Width}x{oldExtent.Height}；新 Surface CurrentExtent={newExtent.Width}x{newExtent.Height}（物理像素）；DPI={dpi:F2}；逻辑≈{newExtent.Width / dpi:F0}x{newExtent.Height / dpi:F0}；generation={generation}";
    public static string OutOfDateRecovered(uint generation) => $"【VulkanClearFrame】Swapchain 自愈成功，已恢复 Present；generation={generation}";
    public static string OutOfDateRecoverFailed(string reason) => $"【VulkanClearFrame】Swapchain 过期恢复失败，暂停 Present：{reason}";
    // RZ-VK5-D-R3：尺寸已由自愈恢复一致，Resize 快速跳过（不停启 Present 泵）。
    public static string ResizeFastSkipped(uint generation, int w, int h) => $"【VulkanClearFrame】Resize 快速跳过：尺寸已由自愈恢复（{w}x{h}）；generation={generation}";
    public static string SwapchainGeneration(string source, uint oldGen, uint newGen, bool rebuilt, Extent2D oldExtent, Extent2D newExtent, string decision)
        => $"【Swapchain代际】触发来源：{source}；旧代际：{oldGen}；新代际：{newGen}；是否实际重建：{(rebuilt ? "是" : "否")}；旧Extent：{oldExtent.Width}x{oldExtent.Height}；新Extent：{newExtent.Width}x{newExtent.Height}；处理决定：{decision}";
    public static string ResizeSkipped(string source, uint gen, Extent2D current, int w, int h, string reason)
        => $"【Resize跳过】触发来源：{source}；当前Swapchain代际：{gen}；当前物理尺寸：{current.Width}x{current.Height}；请求逻辑尺寸：{w}x{h}；实际Swapchain重建：否；原因：{reason}";
}
