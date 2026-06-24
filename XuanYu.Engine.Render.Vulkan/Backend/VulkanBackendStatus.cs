namespace XuanYu.Engine.Render.Vulkan.Backend;

/// <summary>
/// Vulkan 后端当前状态。
/// 本轮只表示尚未探测、可用或不可用。
/// </summary>
public enum VulkanBackendStatus
{
    NotChecked,
    Available,
    Unavailable
}
