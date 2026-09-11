using Avalonia.Controls;
using Avalonia.Interactivity;
using XYUI.Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class MarkerInspectorPanel : UserControl
{
    public MarkerInspectorPanel()
    {
        InitializeComponent();
    }

    void ApplyPosition_Click(object? sender, RoutedEventArgs e) =>
        (DataContext as UiVm)?.CommitMarkerPosition(PositionProperty.X, PositionProperty.Y);
}
