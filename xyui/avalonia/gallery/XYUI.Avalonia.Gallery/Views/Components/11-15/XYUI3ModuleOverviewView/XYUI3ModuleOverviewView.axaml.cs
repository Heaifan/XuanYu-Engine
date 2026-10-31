using Avalonia.Controls;
using Avalonia.Interactivity;

namespace XYUI.Avalonia.Gallery.Views;

public partial class XYUI3ModuleOverviewView : UserControl
{
    public XYUI3ModuleOverviewView() => InitializeComponent();

    void OnComponentClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is XYUI1DocumentationViewModel model && sender is Button { Tag: string id })
            model.SelectXYUI3(id);
    }
}
