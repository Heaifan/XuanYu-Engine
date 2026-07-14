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
        if (e.Key != Key.Escape || sender is not ListBox list) return;
        list.SelectedItem = null;
        e.Handled = true;
    }
}
