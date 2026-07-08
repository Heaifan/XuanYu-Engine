namespace XuanYu.Render.Vulkan;

// VK3-C1：NativeHost → Instance+Surface 桥接的中文生命周期日志格式器。纯文本，无副作用。
public static class VulkanBridgeLogFormatter
{
    public static string Attached(nint hwnd) =>
        $"【VulkanBridge】附加原生宿主：创建 Vulkan Instance 与 Surface 成功；窗口句柄：0x{hwnd:X}";

    public static string Resized(int width, int height) =>
        $"【VulkanBridge】尺寸变化已接收：仅更新视口，不重建 Surface；宽度：{width}；高度：{height}";

    public static string Detached() =>
        "【VulkanBridge】分离原生宿主：已释放 Surface 与 Instance";
}
