using Avalonia.Controls;

namespace FluidWarfare.Editor.Windows.Panels.Status;

public sealed partial class StatusBarPanel : UserControl
{
    private TextBlock? _currentSelectionText;

    public StatusBarPanel()
    {
        InitializeComponent();
        _currentSelectionText = this.FindControl<TextBlock>("CurrentSelectionText");
    }

    public void SetCurrentSelection(string displayName)
    {
        if (_currentSelectionText is not null)
        {
            _currentSelectionText.Text = $"当前选择：{displayName}";
        }
    }
}
