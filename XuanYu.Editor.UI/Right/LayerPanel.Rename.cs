using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace XuanYu.Editor.UI;

public partial class LayerPanel
{
    void LayerName_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not TextBlock { DataContext: MapLayerRowViewModel row } name || !row.CanRename) return;
        row.BeginRename();
        Dispatcher.UIThread.Post(() =>
        {
            if (name.Parent is Panel panel && panel.Children.OfType<TextBox>().FirstOrDefault() is { } box)
            {
                box.Focus(); box.SelectAll();
            }
        }, DispatcherPriority.Input);
        e.Handled = true;
    }

    async void LayerRename_KeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: MapLayerRowViewModel row } || DataContext is not UiVm vm) return;
        if (e.Key == Key.Escape) row.CancelRename();
        else if (e.Key == Key.Enter) { row.CompleteRename(); await vm.CommitLayerRenameAsync(row.RenameText); }
        else return;
        e.Handled = true;
    }

    async void LayerRename_LostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is not TextBox { DataContext: MapLayerRowViewModel row } || !row.IsRenaming || DataContext is not UiVm vm) return;
        row.CompleteRename();
        await vm.CommitLayerRenameAsync(row.RenameText);
    }
}
