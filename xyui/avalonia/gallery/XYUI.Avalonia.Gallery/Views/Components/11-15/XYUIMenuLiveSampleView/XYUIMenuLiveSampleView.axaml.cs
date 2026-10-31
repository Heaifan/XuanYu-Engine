using Avalonia.Controls;

namespace XYUI.Avalonia.Gallery.Views;

public partial class XYUIMenuLiveSampleView : UserControl
{
    public XYUIMenuLiveSampleView()
    {
        InitializeComponent();
        DataContext = new XYUIMenuSampleViewModel();
    }
}
