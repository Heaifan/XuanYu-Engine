using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class HierarchyWorkspace
{
    void RenameTextField_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is not XYTextField field) return;
        field.PropertyChanged -= RenameTextField_PropertyChanged;
        field.PropertyChanged += RenameTextField_PropertyChanged;
        ActivateRenameTextField(field);
    }

    void RenameTextField_DetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is XYTextField field) field.PropertyChanged -= RenameTextField_PropertyChanged;
    }

    void RenameTextField_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (sender is XYTextField field && e.Property == Visual.IsVisibleProperty)
            ActivateRenameTextField(field);
    }

    static void ActivateRenameTextField(XYTextField field)
    {
        InlineRenameActivation.Schedule(
            () => field.IsVisible,
            action => Dispatcher.UIThread.Post(action, DispatcherPriority.Input),
            () => field.Focus(),
            field.SelectAll);
    }

    void RenameTextField_KeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is not XYTextField { DataContext: EditorTreeNode node }) return;
        if (e.Key == Key.Enter) (DataContext as UiVm)?.CommitInlineRename(node);
        else if (e.Key == Key.Escape) (DataContext as UiVm)?.CancelInlineRename(node);
        else return;
        e.Handled = true;
    }

    void RenameTextField_LostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is XYTextField { DataContext: EditorTreeNode node })
            (DataContext as UiVm)?.CommitInlineRename(node);
    }
}
