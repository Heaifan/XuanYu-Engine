using Avalonia.Controls;
using Avalonia.Interactivity;

namespace XuanYu.Editor.UI;

public partial class ContextToolBar : UserControl
{
    public ContextToolBar() => InitializeComponent();
    async void BeginRegionDrawing_Click(object? s, RoutedEventArgs e) { if (DataContext is UiVm vm) await vm.BeginRegionDrawingAsync(); }
    async void BeginRoadDrawing_Click(object? s, RoutedEventArgs e) { if (DataContext is UiVm vm) await vm.BeginRoadDrawingAsync(); }
    async void BeginMarkerPlacement_Click(object? s, RoutedEventArgs e) { if (DataContext is UiVm vm) await vm.BeginMarkerPlacementAsync(); }
    void UndoRegionDrawingVertex_Click(object? s, RoutedEventArgs e) => (DataContext as UiVm)?.UndoRegionDrawingVertex();
    void CompleteRegionDrawing_Click(object? s, RoutedEventArgs e) => (DataContext as UiVm)?.CompleteRegionDrawing();
    void CancelRegionDrawing_Click(object? s, RoutedEventArgs e) => (DataContext as UiVm)?.CancelRegionDrawing();
    void UndoRoadDrawingVertex_Click(object? s, RoutedEventArgs e) => (DataContext as UiVm)?.UndoRoadDrawingVertex();
    void CompleteRoadDrawing_Click(object? s, RoutedEventArgs e) => (DataContext as UiVm)?.CompleteRoadDrawing();
    void CancelRoadDrawing_Click(object? s, RoutedEventArgs e) => (DataContext as UiVm)?.CancelRoadDrawing();
}
