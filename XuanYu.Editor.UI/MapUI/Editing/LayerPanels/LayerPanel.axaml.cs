using Avalonia.Controls;

namespace XuanYu.Editor.UI;

// LAYER-A-R1：通用图层栏内容（纯绑定；领域操作仍由当前 Provider/UiVm 转发）。
public partial class LayerPanel : UserControl
{
    public LayerPanel()
    {
        InitializeComponent();
    }
}
