using Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class UiWin : Window
{
    public UiWin()
    {
        InitializeComponent();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        (DataContext as UiVm)?.CancelInteractionFromWindowClosing();
        base.OnClosing(e);
    }
}
