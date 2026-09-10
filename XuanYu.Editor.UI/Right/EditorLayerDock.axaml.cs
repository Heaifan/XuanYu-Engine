using Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class EditorLayerDock : UserControl
{
    bool _collapsed;
    double _expandedMinHeight;

    public EditorLayerDock()
    {
        InitializeComponent();
        _expandedMinHeight = Root.MinHeight;
    }

    void CollapseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _collapsed = !_collapsed;
        LayerContent.IsVisible = !_collapsed;
        Root.MinHeight = _collapsed ? 0 : _expandedMinHeight;
        CollapseButton.Content = _collapsed ? "展开" : "收起";
    }
}
