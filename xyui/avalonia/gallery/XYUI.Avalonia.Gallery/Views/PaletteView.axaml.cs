using Avalonia.Controls;

namespace XYUI.Avalonia.Gallery.Views;

public partial class PaletteView : UserControl
{
    public PaletteView()
    {
        InitializeComponent();
        DataContext = PaletteViewModel.Create();
    }
}
