namespace XuanYu.Engine.Editor.Windows.Panels.Viewport;

/// <summary>
/// Editor 中 Vulkan 视口宿主的占位状态。
/// </summary>
public enum VulkanViewportHostState
{
    NotCreated,
    WaitingForSurface,
    Disabled
}
