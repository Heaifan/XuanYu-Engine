using Avalonia.Controls;
using Avalonia.Input;

namespace XuanYu.Editor.UI;

public partial class Left : UserControl
{
    public Left()
    {
        InitializeComponent();
    }

    void SelectionList_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape) return;
        (DataContext as UiVm)?.CancelInteractionFromEscape();
        ProjectList.SelectedItem = null;
        HierarchyList.SelectedItem = null;
        e.Handled = true;
    }
}
