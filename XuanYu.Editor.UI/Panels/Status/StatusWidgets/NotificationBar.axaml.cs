using Avalonia.Controls;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D5：四级通知条（Info/Success/Warning/Error 图标 + 合并计数 + 关闭按钮）。
// 纯绑定展示；关闭按钮直接更新 NotificationPresenter。
public partial class NotificationBar : UserControl
{
    public NotificationBar()
    {
        InitializeComponent();
    }

    void Dismiss_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is NotificationPresenter presenter) presenter.Dismiss();
    }
}
