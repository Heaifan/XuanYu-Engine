using Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class EditorRightTabs : UserControl
{
    public EditorRightTabs()
    {
        InitializeComponent();
        _ = new TopTabStripController(SideTabs);
    }
}
