using Avalonia.Controls;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D4-F1：检查器只读键值行始终单行双列（ReadonlyKeyValueRow），
// 无模式切换（可编辑表单的 <360 上下布局仅适用于真实输入控件，见 EditableFormLayoutModel）。
public partial class InspectorPanel : UserControl
{
    public InspectorPanel()
    {
        InitializeComponent();
    }
}
