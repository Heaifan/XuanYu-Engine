using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

public partial class Left
{
    void AddCube_Click(object? sender, RoutedEventArgs e) =>
        (DataContext as UiVm)?.AddCubeEntity();

    void Delete_Click(object? sender, RoutedEventArgs e) =>
        (DataContext as UiVm)?.DeleteSelectedEntity();

    void Rename_Click(object? sender, RoutedEventArgs e) =>
        (DataContext as UiVm)?.BeginRenameSelectedEntity();

    void RenameTextBox_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is TextBox box && box.IsVisible)
        {
            box.Focus();
            box.SelectAll();
        }
    }

    void RenameTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: EditorTreeNode node }) return;
        if (e.Key == Key.Enter) (DataContext as UiVm)?.CommitInlineRename(node);
        else if (e.Key == Key.Escape) (DataContext as UiVm)?.CancelInlineRename(node);
        else return;
        e.Handled = true;
    }

    void RenameTextBox_LostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is TextBox { DataContext: EditorTreeNode node })
            (DataContext as UiVm)?.CommitInlineRename(node);
    }
}
