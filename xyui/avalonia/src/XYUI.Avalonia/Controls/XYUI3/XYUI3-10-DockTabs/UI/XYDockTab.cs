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
        Tab.ShowSelectedAccent = false; Tab.VerticalAlignment = VerticalAlignment.Center;
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
        var accent = new Border
        {
            Classes = { "xyui-dock-accent" },
            IsVisible = Tab.IsSelected,
            IsHitTestVisible = false,
            VerticalAlignment = VerticalAlignment.Bottom
        };
        var divider = new XYSeparator { Variant = XyuiSeparatorVariant.VerticalSplit, Classes = { "xyui-dock-divider" } };
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("20,*,Auto") };
        Add(grid, _grip, 0); Add(grid, Tab, 1); Add(grid, divider, 2);
        grid.Children.Add(accent); Grid.SetColumnSpan(accent, 3); return grid;
    }

    void RefreshSelected()
    {
        Classes.Set("xyui-dock-tab-selected", Tab.IsSelected);
        if (Child is Grid grid)
            foreach (var accent in grid.Children.OfType<Border>().Where(x => x.Classes.Contains("xyui-dock-accent"))) accent.IsVisible = Tab.IsSelected;
    }

    static void Add(Grid grid, Control control, int column)
    { grid.Children.Add(control); Grid.SetColumn(control, column); }
}
