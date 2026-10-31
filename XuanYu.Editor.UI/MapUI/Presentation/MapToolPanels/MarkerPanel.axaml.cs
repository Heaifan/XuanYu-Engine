using Avalonia.Controls;
using Avalonia.Interactivity;

namespace XuanYu.Editor.UI;

public partial class MarkerPanel : UserControl
{
    public MarkerPanel() => InitializeComponent();
    async void MarkerPlacement_Click(object? sender, RoutedEventArgs e) { if (DataContext is UiVm vm) await vm.BeginMarkerPlacementAsync(); }
}
