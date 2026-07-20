namespace XuanYu.Editor.UI;

public static class EditorLogNoiseFilter
{
    public static bool SuppressRenderBackendInfo(string message) =>
        message.StartsWith("【VulkanSwapchain】能力查询成功") ||
        message.StartsWith("【VulkanSwapchain】Swapchain 创建跳过") ||
        message.StartsWith("【Resize跳过】") ||
        message.StartsWith("【VulkanClearFrame】Resize 快速跳过") ||
        message.Contains("Swapchain.TryRecreate：旧 extent") ||
        message.Contains("Resize开始：请求逻辑尺寸");
}
