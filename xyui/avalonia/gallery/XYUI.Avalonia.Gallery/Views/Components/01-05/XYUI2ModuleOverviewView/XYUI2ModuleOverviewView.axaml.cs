using Avalonia.Controls;
using Avalonia.Interactivity;

namespace XYUI.Avalonia.Gallery.Views;

public partial class XYUI2ModuleOverviewView : UserControl
{
    public XYUI2ModuleOverviewView() => InitializeComponent();

    void OnComponentClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is XYUI1DocumentationViewModel model && sender is Button { Tag: string id })
            model.SelectXYUI2(id);
    }
}
