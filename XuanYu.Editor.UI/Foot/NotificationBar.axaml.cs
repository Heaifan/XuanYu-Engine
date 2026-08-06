using Avalonia.Controls;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D5：四级通知条（Info/Success/Warning/Error 图标 + 单行省略 + 完整 Tooltip）。
// 纯绑定展示，无 code-behind 逻辑。
public partial class NotificationBar : UserControl
{
    public NotificationBar()
    {
        InitializeComponent();
    }
}
