using Avalonia.Controls;
using Avalonia.Input;

namespace XuanYu.Editor.UI;

public partial class HierarchyWorkspace : UserControl
{
    public HierarchyWorkspace()
    {
        InitializeComponent();
        InitializeContextMenu();
    }

    void SelectionList_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape) return;
        (DataContext as UiVm)?.CancelInteractionFromEscape();
        HierarchyList.SelectedItem = null;
        e.Handled = true;
    }

    void HierarchyToggle_Pressed(object? sender, PointerPressedEventArgs e)
    {
        if ((sender as Control)?.DataContext is EditorTreeNode node)
            (DataContext as UiVm)?.ToggleHierarchyNode(node);
        e.Handled = true;
    }

    void HierarchyRow_Pressed(object? sender, PointerPressedEventArgs e)
    {
        var control = sender as Control;
        if (e.GetCurrentPoint(control).Properties.PointerUpdateKind == PointerUpdateKind.RightButtonPressed
            && control?.DataContext is EditorTreeNode selected)
            (DataContext as UiVm)!.SelectedHierarchyItem = selected;
        if (TryToggleFromArrowSlot(sender, e, out var node))
            (DataContext as UiVm)?.ToggleHierarchyNode(node);
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
