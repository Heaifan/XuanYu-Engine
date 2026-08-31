using Avalonia.Controls;

namespace XYUI.Avalonia.Gallery.Views;

public partial class ShapeView : UserControl
{
    public ShapeView()
    {
        InitializeComponent();
        DataContext = new ShapeViewModel(ShapeCatalog.BuildSections());
    }
}
