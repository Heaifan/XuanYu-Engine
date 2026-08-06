using Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class Right : UserControl
{
    readonly TopTabStripController _topTabs;

    public Right()
    {
        InitializeComponent();
        _topTabs = new TopTabStripController(SideTabs); // ARCH-UI-SPEC-R1-D3：顶层页签单行溢出控制器
    }
}
