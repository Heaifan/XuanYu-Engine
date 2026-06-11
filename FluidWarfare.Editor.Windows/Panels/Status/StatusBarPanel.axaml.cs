using Avalonia.Controls;

namespace FluidWarfare.Editor.Windows.Panels.Status;

public sealed partial class StatusBarPanel : UserControl
{
    private TextBlock? _currentSelectionText;
    private TextBlock? _vulkanStatusText;

    public StatusBarPanel()
    {
        InitializeComponent();
        _currentSelectionText = this.FindControl<TextBlock>("CurrentSelectionText");
        _vulkanStatusText = this.FindControl<TextBlock>("VulkanStatusText");
    }

    public void SetVulkanStatus(string vulkanStatus)
    {
        if (_vulkanStatusText is not null)
        {
            _vulkanStatusText.Text = $"Vulkan：{vulkanStatus}";
        }
    }

    public void SetCurrentSelection(string displayName)
    {
        if (_currentSelectionText is not null)
        {
            _currentSelectionText.Text = $"当前选择：{displayName}";
        }
    }
}
