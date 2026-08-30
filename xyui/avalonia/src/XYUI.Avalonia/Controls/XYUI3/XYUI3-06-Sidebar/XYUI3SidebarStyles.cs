using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiComponentStyles
{
    static void SidebarRailTabs(Styles styles)
    {
        var sidebar = new Style(x => x.OfType<XYSidebar>().Class("xyui-sidebar")); Brush(sidebar, Border.BackgroundProperty, "XY.Brush.Surface.Panel"); sidebar.Setters.Add(new Setter(Border.WidthProperty, XyuiComponentTokens.SidebarWidth)); sidebar.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(8))); styles.Add(sidebar);
        var header = new Style(x => x.OfType<Border>().Class("xyui-sidebar-header")); Brush(header, Border.BackgroundProperty, "XY.Brush.Surface.PanelAlt"); header.Setters.Add(new Setter(Border.HeightProperty, XyuiComponentTokens.SidebarHeaderHeight)); header.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(14, 0))); styles.Add(header);
        TextStyle(styles, "xyui-sidebar-title", 13, 600, "XY.Brush.Text.Primary");
        var rail = new Style(x => x.OfType<XYNavigationRail>().Class("xyui-navigation-rail")); Brush(rail, Border.BackgroundProperty, "XY.Brush.Surface.Panel"); rail.Setters.Add(new Setter(Border.WidthProperty, XyuiComponentTokens.NavigationRailWidth)); rail.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(7))); styles.Add(rail);
        var railItem = new Style(x => x.OfType<XYNavigationItem>().Class("xyui-rail-item")); railItem.Setters.Add(new Setter(Border.HeightProperty, XyuiComponentTokens.NavigationRailItemHeight)); railItem.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(6, 0))); styles.Add(railItem);
        var tabs = new Style(x => x.OfType<XYTabs>().Class("xyui-tabs")); Brush(tabs, Border.BackgroundProperty, "XY.Brush.Surface.Panel"); tabs.Setters.Add(new Setter(Border.HeightProperty, 50d)); styles.Add(tabs);
        var tab = new Style(x => x.OfType<XYTab>().Class("xyui-tab")); tab.Setters.Add(new Setter(Border.HeightProperty, XyuiComponentTokens.TabHeight)); tab.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(XyuiComponentTokens.TabPaddingX, 0))); tab.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Transparent)); styles.Add(tab);
        var selected = new Style(x => x.OfType<XYTab>().Class("xyui-tab-selected")); Brush(selected, Border.BackgroundProperty, "XY.Brush.Surface.Selected"); selected.Setters.Add(new Setter(Border.BorderBrushProperty, new SolidColorBrush(Color.Transparent))); selected.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(0, 0, 0, 3))); selected.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(10, 0, 10, 0))); styles.Add(selected);
        TextStyle(styles, "xyui-tab-label", 13, 400, "XY.Brush.Text.Secondary");
        var dot = new Style(x => x.OfType<Border>().Class("xyui-tab-modified")); Brush(dot, Border.BackgroundProperty, "XY.Brush.Text.Secondary"); dot.Setters.Add(new Setter(Border.WidthProperty, 3d)); dot.Setters.Add(new Setter(Border.HeightProperty, 3d)); dot.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(2))); styles.Add(dot);
    }
}
