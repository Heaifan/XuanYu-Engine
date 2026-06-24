namespace FluidWarfare.Editor.Windows.Panels.Viewport;

/// <summary>Partial：Vulkan 后端状态显示。</summary>
sealed partial class ViewportPlaceholderPanel
{
    public void ShowVulkanBackendStatus(string t)
    { if (_vulkanBackendStatusText is not null) _vulkanBackendStatusText.Text = t; }

    public void ShowVulkanInstanceStatus(string t)
    { if (_vulkanInstanceStatusText is not null) _vulkanInstanceStatusText.Text = t; }

    public void ShowVulkanDeviceStatus(string t)
    { if (_vulkanDeviceStatusText is not null) _vulkanDeviceStatusText.Text = t; }
}
