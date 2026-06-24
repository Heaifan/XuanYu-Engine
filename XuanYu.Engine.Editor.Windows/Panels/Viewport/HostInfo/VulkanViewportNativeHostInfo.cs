namespace XuanYu.Engine.Editor.Windows.Panels.Viewport;

/// <summary>
/// Vulkan 视口宿主原生窗口句柄信息。
/// </summary>
public sealed record VulkanViewportNativeHostInfo(
    string PlatformText,
    bool HasNativeHandle,
    string Message,
    nint WindowHandle,
    nint InstanceHandle,
    int Width,
    int Height)
{
    public static VulkanViewportNativeHostInfo NotAvailable { get; } =
        new(
            "未知",
            false,
            "当前视口宿主尚未提供可用于 Vulkan Surface 的原生窗口句柄。",
            0,
            0,
            0,
            0);
}
