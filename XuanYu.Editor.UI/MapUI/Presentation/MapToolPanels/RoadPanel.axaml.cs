using Avalonia.Controls;
using Avalonia.Interactivity;

namespace XuanYu.Editor.UI;

public partial class RoadPanel : UserControl
{
    public RoadPanel() => InitializeComponent();
    async void RoadDrawing_Click(object? sender, RoutedEventArgs e) { if (DataContext is UiVm vm) await vm.BeginRoadDrawingAsync(); }
    void Undo_Click(object? sender, RoutedEventArgs e) => (DataContext as UiVm)?.UndoRoadDrawingVertex();
    void Redo_Click(object? sender, RoutedEventArgs e) => (DataContext as UiVm)?.RedoRoadDrawingVertex();
    void Complete_Click(object? sender, RoutedEventArgs e) => (DataContext as UiVm)?.CompleteRoadDrawing();
    void Cancel_Click(object? sender, RoutedEventArgs e) => (DataContext as UiVm)?.CancelRoadDrawing();
}
