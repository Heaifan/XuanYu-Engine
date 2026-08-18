using Avalonia.Controls;

namespace XYUI.Avalonia.Gallery.Views;

public partial class XYUI1DocumentationView : UserControl
{
    public XYUI1DocumentationView()
    {
        InitializeComponent();
        DataContext = new XYUI1DocumentationViewModel();
    }
}
