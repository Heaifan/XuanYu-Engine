using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYDockTab : Border
{
    XYIcon _grip = null!;
    public XYTab Tab { get; }
    public XYIcon Grip => _grip;

    public XYDockTab(XYTab tab)
    {
        Tab = tab;
        Classes.Add("xyui-dock-tab"); Tab.Classes.Add("xyui-dock-tab-inner");
        Tab.VerticalAlignment = VerticalAlignment.Center;
        Tab.PropertyChanged += (_, change) => { if (change.Property == XYTab.IsSelectedProperty) RefreshSelected(); };
        Child = Build(); RefreshSelected(); InitializeInteraction();
    }

    Grid Build()
    {
        _grip = new XYIcon
        {
            Icon = XyuiVectorIcon.DragGrip,
            Size = XyuiIconSize.Tiny,
            Classes = { "xyui-dock-grip" },
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        var indicator = new Border { Classes = { "xyui-dock-drop-indicator" }, IsVisible = false, IsHitTestVisible = false, Width = 2, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Stretch };
        var divider = new XYSeparator { Variant = XyuiSeparatorVariant.VerticalSplit, Classes = { "xyui-dock-divider" } };
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("20,*,Auto") };
        Add(grid, _grip, 0); Add(grid, Tab, 1); Add(grid, divider, 2);
        grid.Children.Add(indicator); Grid.SetColumnSpan(indicator, 3); return grid;
    }

    void RefreshSelected()
    {
        Classes.Set("xyui-dock-tab-selected", Tab.IsSelected);
    }

    internal void SetDropIndicator(bool visible, bool after = false)
    {
        if (Child is not Grid grid) return;
        var indicator = grid.Children.OfType<Border>().FirstOrDefault(x => x.Classes.Contains("xyui-dock-drop-indicator"));
        if (indicator is null) return;
        indicator.IsVisible = visible; indicator.HorizontalAlignment = after ? HorizontalAlignment.Right : HorizontalAlignment.Left;
    }

    static void Add(Grid grid, Control control, int column)
    { grid.Children.Add(control); Grid.SetColumn(control, column); }
}
