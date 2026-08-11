using Avalonia.Controls;
using Avalonia.Interactivity;

namespace XuanYu.Editor.UI;

public partial class DatasetPanel : UserControl
{
    public DatasetPanel() => InitializeComponent();

    void DatasetRow_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: MapDatasetRow row } && DataContext is UiVm vm)
            vm.SelectDataset(row.Id);
    }

    async void ApplyName_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is UiVm vm) await vm.RenameSelectedDatasetAsync();
    }
}
