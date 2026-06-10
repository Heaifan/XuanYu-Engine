using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FluidWarfare.Editor.Windows.Panels.Project;

public sealed partial class ProjectPanel : UserControl
{
    public event EventHandler<string>? ProjectItemSelected;

    public ProjectPanel()
    {
        InitializeComponent();
    }

    private void SelectScene(object? sender, RoutedEventArgs e)
    {
        SelectProjectItem("场景");
    }

    private void SelectUnit(object? sender, RoutedEventArgs e)
    {
        SelectProjectItem("单位");
    }

    private void SelectAsset(object? sender, RoutedEventArgs e)
    {
        SelectProjectItem("资源");
    }

    private void SelectConfig(object? sender, RoutedEventArgs e)
    {
        SelectProjectItem("配置");
    }

    private void SelectProjectItem(string itemName)
    {
        ProjectItemSelected?.Invoke(this, itemName);
    }
}
