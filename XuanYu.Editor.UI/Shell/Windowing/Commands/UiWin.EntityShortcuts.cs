using Avalonia.Controls;
using Avalonia.Input;

namespace XuanYu.Editor.UI;

public partial class UiWin
{
    bool HandleEntityShortcut(KeyEventArgs e)
    {
        if (e.Source is TextBox) return false;
        if (DataContext is not UiVm vm) return false;
        if (e.Key == Key.F2)
        {
            e.Handled = vm.BeginRenameFromShortcut();
            return e.Handled;
        }
        if (e.Key != Key.Delete) return false;
        e.Handled = vm.DeleteSelectedEntity();
        return e.Handled;
    }
}
