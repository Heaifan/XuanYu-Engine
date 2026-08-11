using Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class EditorLayerDock : UserControl
{
    bool _collapsed;

    public EditorLayerDock()
    {
        InitializeComponent();
    }

    void CollapseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _collapsed = !_collapsed;
        LayerContent.IsVisible = !_collapsed;
        CollapseButton.Content = _collapsed ? "展开" : "收起";
    }
}
