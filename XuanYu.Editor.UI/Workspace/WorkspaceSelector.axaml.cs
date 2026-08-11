using Avalonia.Controls;
using Avalonia.Input;

namespace XuanYu.Editor.UI;

public partial class WorkspaceSelector : UserControl
{
    public WorkspaceSelector() => InitializeComponent();

    void ModeSurface_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is UiVm vm) vm.ToggleEditorMode();
        e.Handled = true;
    }
}
