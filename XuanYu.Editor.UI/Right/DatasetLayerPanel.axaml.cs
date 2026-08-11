using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace XuanYu.Editor.UI;

public partial class DatasetLayerPanel : UserControl
{
    public DatasetLayerPanel() => InitializeComponent();

    void DatasetRow_Pressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (e.Source is Button) return;
        if (sender is Border { DataContext: MapDatasetRow row } && DataContext is UiVm vm)
            vm.SelectDataset(row.Id);
    }

    void DatasetAction_PointerPressed(object? sender, PointerPressedEventArgs e) => e.Handled = true;

    async void Visibility_Click(object? sender, RoutedEventArgs e)
    {
        e.Handled = true;
        if (sender is Button { DataContext: MapDatasetRow row } && DataContext is UiVm vm)
            await vm.ToggleDatasetVisibilityAsync(row.Id);
    }

    async void Lock_Click(object? sender, RoutedEventArgs e)
    {
        e.Handled = true;
        if (sender is Button { DataContext: MapDatasetRow row } && DataContext is UiVm vm)
            await vm.ToggleDatasetLockAsync(row.Id);
    }
}
