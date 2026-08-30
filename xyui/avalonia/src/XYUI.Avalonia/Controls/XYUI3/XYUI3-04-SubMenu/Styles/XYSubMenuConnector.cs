using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public sealed class XYSubMenuConnector : Grid
{
    public bool IsMirrored { get; set; }
    public XYSubMenuConnector()
    {
        Classes.Add("xyui-sub-menu-connector");
        Children.Add(new Border { Classes = { "xyui-sub-menu-line" }, Height = 2, VerticalAlignment = VerticalAlignment.Center });
        Children.Add(new Ellipse { Classes = { "xyui-sub-menu-anchor" }, Width = 6, Height = 6, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
    }
}
