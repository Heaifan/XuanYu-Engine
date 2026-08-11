using Avalonia.Controls;
using Avalonia.Interactivity;

namespace XuanYu.Editor.UI;

public partial class DatasetLayerPanel : UserControl
{
    public DatasetLayerPanel() => InitializeComponent();

    void DatasetRow_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: MapDatasetRow row } && DataContext is UiVm vm)
            vm.SelectDataset(row.Id);
    }
}
