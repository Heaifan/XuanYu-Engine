namespace XuanYu.Render.Vulkan;

// VK3-B2：Vulkan Surface 生命周期中文日志格式器。纯文本生成，无副作用。
public static class VulkanSurfaceLogFormatter
{
    public static string Created(nint hwnd) =>
        $"【VulkanSurface】创建 Vulkan Surface 成功；窗口句柄：0x{hwnd:X}";

    public static string Disposed(ulong handle) =>
        handle == 0
            ? "【VulkanSurface】释放 Vulkan Surface 跳过：Surface 已释放"
            : $"【VulkanSurface】释放 Vulkan Surface 成功；Surface 句柄：0x{handle:X}";

    public static string CreateFailed(string errorType, string errorMessage) =>
        $"【VulkanSurface】创建 Vulkan Surface 失败；错误类型：{errorType}；错误详情：{errorMessage}";
}
