using Avalonia.Controls;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery.Views;

public partial class SizingMatrixSection : UserControl
{
    public SizingMatrixSection()
    {
        InitializeComponent();
        ToolbarCompactHost.Children.Add(CreateToolbar(XyuiIconSize.Small));
        ToolbarDefaultHost.Child = CreateToolbar(XyuiIconSize.Medium);
        ToolbarComfortableHost.Children.Add(CreateToolbar(XyuiIconSize.Large));
        ToolbarTouchHost.Children.Add(CreateToolbar(XyuiIconSize.Large));
    }

    static XYToolbar CreateToolbar(XyuiIconSize iconSize) =>
        new(new XYToolGroup(
            new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.Code, Size = iconSize } },
            new XYButton { Content = "A", Variant = XyuiButtonVariant.Secondary }));
}