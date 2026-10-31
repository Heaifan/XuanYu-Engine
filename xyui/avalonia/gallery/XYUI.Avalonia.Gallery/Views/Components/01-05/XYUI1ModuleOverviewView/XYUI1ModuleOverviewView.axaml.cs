using Avalonia.Controls;
using Avalonia.Interactivity;

namespace XYUI.Avalonia.Gallery.Views;

public partial class XYUI1ModuleOverviewView : UserControl
{
    public XYUI1ModuleOverviewView() => InitializeComponent();

    void OnComponentClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is XYUI1DocumentationViewModel model && sender is Button { Tag: string id }) model.Select(id);
    }
}
