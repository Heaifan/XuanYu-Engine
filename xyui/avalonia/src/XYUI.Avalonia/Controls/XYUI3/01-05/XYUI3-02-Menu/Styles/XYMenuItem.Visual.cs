using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

internal sealed class XYMenuItemVisual : Grid
{
    Ellipse? _radioDot;
    Grid? _check;

    public XYMenuItemVisual(XYMenuItem item)
    {
        if (item.Classes.Contains("xyui-workspace-item"))
        {
            if (item.Icon is not null)
            {
                ColumnDefinitions = new ColumnDefinitions("24,*,24");
                var label = Label(item); var indicator = WorkspaceIndicator(item);
                Children.Add(Leading(item)); Children.Add(label); Children.Add(indicator);
                Grid.SetColumn(label, 1); Grid.SetColumn(indicator, 2);
            }
            else
            {
                ColumnDefinitions = new ColumnDefinitions("*,24");
                var label = Label(item); var indicator = WorkspaceIndicator(item);
                Children.Add(label); Children.Add(indicator); Grid.SetColumn(indicator, 1);
            }
            return;
        }
        ColumnDefinitions = new ColumnDefinitions("24,*,Auto,24");
        Children.Add(Leading(item)); Children.Add(Label(item)); Children.Add(Shortcut(item)); Children.Add(Chevron(item));
        Grid.SetColumn(Children[1], 1); Grid.SetColumn(Children[2], 2); Grid.SetColumn(Children[3], 3);
    }
    public void Refresh(XYMenuItem item)
    {
        if (_radioDot is not null) _radioDot.IsVisible = item.CheckKind == XyuiMenuCheckKind.Radio && item.IsChecked;
        if (_check is not null) _check.IsVisible = item.CheckKind == XyuiMenuCheckKind.Check && item.IsChecked;
    }
    Control Leading(XYMenuItem item)
    {
        if (item.Icon is XyuiVectorIcon icon) return new XYIcon { Icon = icon, Size = XyuiIconSize.Small, Classes = { "xyui-menu-icon" } };
        if (item.CheckKind == XyuiMenuCheckKind.Check) return Check(item.IsChecked);
        if (item.CheckKind == XyuiMenuCheckKind.Radio) return Radio(item.IsChecked);
        return new Border();
    }
    Control Radio(bool selected)
    {
        _radioDot = new Ellipse { IsVisible = selected, Classes = { "xyui-menu-radio-dot" } };
        return new Grid { HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center, Classes = { "xyui-menu-radio" }, Children = { new Ellipse { Classes = { "xyui-menu-radio-ring" } }, _radioDot } };
    }
    Control Check(bool visible)
    {
        _check = new Grid { IsVisible = visible, Classes = { "xyui-menu-check" } };
        _check.Children.Add(new Line { StartPoint = new Point(2, 7), EndPoint = new Point(5, 10), Classes = { "xyui-menu-check-line" } });
        _check.Children.Add(new Line { StartPoint = new Point(5, 10), EndPoint = new Point(12, 2), Classes = { "xyui-menu-check-line" } }); return _check;
    }
    Control WorkspaceIndicator(XYMenuItem item) => item.CheckKind == XyuiMenuCheckKind.Radio ? Radio(item.IsChecked) : Check(item.IsChecked);
    static TextBlock Label(XYMenuItem item) => new() { Text = item.Label, Classes = { "xyui-menu-label" }, VerticalAlignment = VerticalAlignment.Center };
    static TextBlock Shortcut(XYMenuItem item) => new() { Text = item.Shortcut, Classes = { "xyui-menu-shortcut" }, VerticalAlignment = VerticalAlignment.Center };
    static Control Chevron(XYMenuItem item) => item.HasSubMenu ? new XYIcon { Icon = XyuiVectorIcon.ChevronRight, Size = XyuiIconSize.Small, Classes = { "xyui-menu-chevron" } } : new Border();
}
