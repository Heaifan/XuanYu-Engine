using Avalonia.Controls;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery.Views;

public partial class IconographyUsageSection : UserControl
{
    public IconographyUsageSection()
    {
        InitializeComponent();
        ToolbarHost.Children.Add(new XYToolbar(
            new XYToolGroup(
                new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.Search } },
                new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.Locate } },
                new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.MoreHorizontal } })));
    }
}