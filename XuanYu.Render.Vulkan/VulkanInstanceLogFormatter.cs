namespace XuanYu.Render.Vulkan;

// VK3-B1：Vulkan Instance 生命周期中文日志格式器。纯文本生成，无副作用。
public static class VulkanInstanceLogFormatter
{
    public static string FormatVersion(uint version) =>
        $"{version >> 22}.{(version >> 12) & 0x3ff}.{version & 0xfff}";

    public static string Created(uint apiVersion) =>
        $"【VulkanInstance】创建 Vulkan 实例成功；API 版本：{FormatVersion(apiVersion)}；启用扩展：VK_KHR_surface、VK_KHR_win32_surface";

    public static string Disposed(nint handle) =>
        handle == 0
            ? "【VulkanInstance】释放 Vulkan 实例跳过：实例已释放"
            : $"【VulkanInstance】释放 Vulkan 实例成功；实例句柄：0x{handle:X}";

    public static string CreateFailed(string errorType, string errorMessage) =>
        $"【VulkanInstance】创建 Vulkan 实例失败；错误类型：{errorType}；错误详情：{errorMessage}";
}
