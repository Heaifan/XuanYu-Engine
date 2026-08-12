using Avalonia.Controls;
using Avalonia.Interactivity;

namespace XuanYu.Editor.UI;

public partial class RegionPanel : UserControl
{
    public RegionPanel() => InitializeComponent();

    async void RegionDrawing_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is UiVm vm) await vm.BeginRegionDrawingAsync();
    }

    void UndoRegionDrawingVertex_Click(object? sender, RoutedEventArgs e) =>
        (DataContext as UiVm)?.UndoRegionDrawingVertex();

    void RedoRegionDrawingVertex_Click(object? sender, RoutedEventArgs e) =>
        (DataContext as UiVm)?.RedoRegionDrawingVertex();

    void CompleteRegionDrawing_Click(object? sender, RoutedEventArgs e) =>
        (DataContext as UiVm)?.CompleteRegionDrawing();

    void CancelRegionDrawing_Click(object? sender, RoutedEventArgs e) =>
        (DataContext as UiVm)?.CancelRegionDrawing();
}
