using Avalonia.Controls;
using Avalonia.Interactivity;

namespace XuanYu.Editor.UI;

public partial class Top : UserControl
{
    public Top()
    {
        InitializeComponent();
    }

    async void RegionDrawing_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is UiVm vm) await vm.BeginRegionDrawingAsync();
    }
}
