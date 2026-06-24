namespace XuanYu.Engine.Editor.Windows.Panels.Viewport;

/// <summary>
/// 保存 Vulkan 视口宿主占位显示信息。
/// </summary>
public sealed record VulkanViewportHostInfo(
    VulkanViewportHostState State,
    string Message)
{
    public static VulkanViewportHostInfo NotCreated { get; } =
        new(VulkanViewportHostState.NotCreated,
            "Vulkan 视口宿主尚未创建。");
}
