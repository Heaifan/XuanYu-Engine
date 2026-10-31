using Avalonia.Controls;
using Avalonia.Input;

namespace XuanYu.Editor.UI;

public partial class ProjectWorkspace : UserControl
{
    public ProjectWorkspace() => InitializeComponent();

    void SelectionList_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape) return;
        (DataContext as UiVm)?.CancelInteractionFromEscape();
        ProjectList.SelectedItem = null;
        e.Handled = true;
    }

    void ProjectToggle_Pressed(object? sender, PointerPressedEventArgs e)
    {
        if ((sender as Control)?.DataContext is EditorTreeNode node)
            (DataContext as UiVm)?.ToggleProjectNode(node);
        e.Handled = true;
    }

    void ProjectRow_Pressed(object? sender, PointerPressedEventArgs e)
    {
        if (TryToggleFromArrowSlot(sender, e, out var node))
            (DataContext as UiVm)?.ToggleProjectNode(node);
    }

    static bool TryToggleFromArrowSlot(object? sender, PointerPressedEventArgs e, out EditorTreeNode node)
    {
        node = null!;
        if (sender is not Control control || control.DataContext is not EditorTreeNode candidate) return false;
        var x = e.GetPosition(control).X;
        if (x < candidate.GuideWidth || x > candidate.GuideWidth + 16 || !candidate.CanToggle) return false;
        node = candidate;
        e.Handled = true;
        return true;
    }
}
