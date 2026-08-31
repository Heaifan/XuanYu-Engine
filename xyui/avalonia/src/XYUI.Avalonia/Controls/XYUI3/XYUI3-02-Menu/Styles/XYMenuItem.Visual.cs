using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

internal sealed class XYMenuItemVisual : Grid
{
    public XYMenuItemVisual(XYMenuItem item)
    {
        ColumnDefinitions = new ColumnDefinitions("24,*,Auto,24");
        Children.Add(Leading(item)); Children.Add(Label(item)); Children.Add(Shortcut(item)); Children.Add(Chevron(item));
        Grid.SetColumn(Children[1], 1); Grid.SetColumn(Children[2], 2); Grid.SetColumn(Children[3], 3);
    }
    static Control Leading(XYMenuItem item)
    {
        if (item.Icon is XyuiVectorIcon icon) return new XYIcon { Icon = icon, Size = XyuiIconSize.Small, Classes = { "xyui-menu-icon" } };
        if (item.CheckKind == XyuiMenuCheckKind.Check && item.IsChecked) return Check();
        if (item.CheckKind == XyuiMenuCheckKind.Radio) return Radio(item.IsChecked);
        return new Border();
    }
    static Control Radio(bool selected) => new Grid { Classes = { "xyui-menu-radio" }, Children = { new Ellipse { Classes = { "xyui-menu-radio-ring" } }, selected ? new Ellipse { Classes = { "xyui-menu-radio-dot" } } : new Border() } };
    static Control Check() => new Grid { Classes = { "xyui-menu-check" }, Children = { new Line { StartPoint = new Point(2, 7), EndPoint = new Point(5, 10), Classes = { "xyui-menu-check-line" } }, new Line { StartPoint = new Point(5, 10), EndPoint = new Point(12, 2), Classes = { "xyui-menu-check-line" } } } };
    static TextBlock Label(XYMenuItem item) => new() { Text = item.Label, Classes = { "xyui-menu-label" }, VerticalAlignment = VerticalAlignment.Center };
    static TextBlock Shortcut(XYMenuItem item) => new() { Text = item.Shortcut, Classes = { "xyui-menu-shortcut" }, VerticalAlignment = VerticalAlignment.Center };
    static Control Chevron(XYMenuItem item) => item.HasSubMenu ? new XYIcon { Icon = XyuiVectorIcon.ChevronRight, Size = XyuiIconSize.Small, Classes = { "xyui-menu-chevron" } } : new Border();
}
