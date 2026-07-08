namespace XuanYu.Render.Vulkan;

// VK3-C1/C2-R1：NativeHost → Instance+Surface 桥接中文生命周期日志格式器。纯文本，无副作用。
public static class VulkanBridgeLogFormatter
{
    public static string Attached(nint hwnd) =>
        $"【VulkanBridge】附加成功：Instance + Surface 已创建；窗口句柄：0x{hwnd:X}";

    public static string Resized(int width, int height) =>
        $"【VulkanBridge】尺寸变化已接收：不重建 Surface；宽度：{width}；高度：{height}";

    public static string ResizedSkipped(int width, int height) =>
        $"【VulkanBridge】收到尺寸变化但尚未 Attach，不处理 Surface；宽度：{width}；高度：{height}";

    public static string Detached() =>
        "【VulkanBridge】分离完成：Surface + Instance 已释放";

    public static string SurfaceDisposed() =>
        "【VulkanBridge】Surface 已释放";

    public static string InstanceDisposed() =>
        "【VulkanBridge】Instance 已销毁";

    public static string DetachedSkipped() =>
        "【VulkanBridge】跳过分离：尚未 Attach";

    public static string AttachFailed(string reason) =>
        $"【VulkanBridge】附加失败：{reason}；Surface 未创建";

    public static void Emit(Action<string>? log, string message)
    {
        log?.Invoke(message);
        Console.WriteLine(message);
    }
}
