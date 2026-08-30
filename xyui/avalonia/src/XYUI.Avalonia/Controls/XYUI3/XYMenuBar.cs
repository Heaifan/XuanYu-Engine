using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed class XYMenuBar : Border
{
    IReadOnlyList<XYMenuBarItem> _items = [];
    public IReadOnlyList<XYMenuBarItem> Items { get => _items; set { _items = value; Build(); } }
    public XYMenuBar() { Classes.Add("xyui-menu-bar"); Build(); }
    public XYMenuBar(params XYMenuBarItem[] items) : this() => Items = items;
    void Build()
    {
        var grid = new Grid { RowDefinitions = new RowDefinitions("Auto,1") };
        var items = new StackPanel { Orientation = Orientation.Horizontal, Classes = { "xyui-menu-bar-items" } };
        foreach (var item in Items) items.Children.Add(item);
        grid.Children.Add(items); var divider = new XYSeparator { Variant = XyuiSeparatorVariant.Header };
        grid.Children.Add(divider); Grid.SetRow(divider, 1); Child = grid;
    }
}
