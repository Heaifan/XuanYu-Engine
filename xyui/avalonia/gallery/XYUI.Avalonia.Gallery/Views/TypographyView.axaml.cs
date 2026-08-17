using Avalonia.Controls;

namespace XYUI.Avalonia.Gallery.Views;

public partial class TypographyView : UserControl
{
    public TypographyView()
    {
        InitializeComponent();
        DataContext = new TypographyViewModel(TypographyCatalog.BuildSections());
    }
}
