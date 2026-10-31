namespace XuanYu.Editor.UI;

public static class EditorLogNoiseFilter
{
    public static bool SuppressRenderBackendInfo(string message) =>
        message.StartsWith("【VulkanSwapchain】能力查询成功") ||
        message.StartsWith("能力查询成功") ||
        message.StartsWith("【VulkanSwapchain】Swapchain 创建跳过") ||
        message.StartsWith("Swapchain 创建跳过") ||
        message.StartsWith("【Resize跳过】") ||
        message.StartsWith("【VulkanClearFrame】Resize 快速跳过") ||
        message.StartsWith("Resize 快速跳过") ||
        message.Contains("Swapchain 自愈查询：旧物理尺寸") ||
        message.Contains("Resize 开始：请求逻辑尺寸");
}
