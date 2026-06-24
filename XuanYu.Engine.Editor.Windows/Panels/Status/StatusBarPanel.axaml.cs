using Avalonia.Controls;

namespace XuanYu.Engine.Editor.Windows.Panels.Status;

public sealed partial class StatusBarPanel : UserControl
{
    private TextBlock? _currentSelectionText;
    private TextBlock? _vulkanStatusText;
    private TextBlock? _groundCoordText;
    private TextBlock? _dirtyStateText;

    public StatusBarPanel()
    {
        InitializeComponent();
        _currentSelectionText = this.FindControl<TextBlock>("CurrentSelectionText");
        _vulkanStatusText = this.FindControl<TextBlock>("VulkanStatusText");
        _groundCoordText = this.FindControl<TextBlock>("GroundCoordText");
        _dirtyStateText = this.FindControl<TextBlock>("DirtyStateText");
    }

    public void SetVulkanStatus(string vulkanStatus)
    {
        if (_vulkanStatusText is not null)
        {
            _vulkanStatusText.Text = $"Vulkan：{vulkanStatus}";
        }
    }

    public void SetCurrentSelection(string? displayName)
    {
        if (_currentSelectionText is not null)
        {
            _currentSelectionText.Text = $"当前选择：{displayName ?? "无"}";
        }
    }

    public void SetGroundPosition(string text)
    {
        if (_groundCoordText is not null)
        {
            _groundCoordText.Text = text;
        }
    }

    public void SetDirtyState(bool isDirty)
    {
        if (_dirtyStateText is not null)
        {
            _dirtyStateText.Text = isDirty ? "场景：已修改" : "场景：未修改";
        }
    }
}
