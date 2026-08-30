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
        var collapse = new Style(x => x.OfType<Button>().Class("xyui-sidebar-collapse")); collapse.Setters.Add(new Setter(Control.HorizontalAlignmentProperty, HorizontalAlignment.Right)); collapse.Setters.Add(new Setter(Control.VerticalAlignmentProperty, VerticalAlignment.Center)); collapse.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Transparent)); collapse.Setters.Add(new Setter(Button.BorderThicknessProperty, new Thickness(0))); collapse.Setters.Add(new Setter(Button.FontSizeProperty, 22d)); styles.Add(collapse);
        TextStyle(styles, "xyui-sidebar-title", 13, 600, "XY.Brush.Text.Primary");
        var rail = new Style(x => x.OfType<XYNavigationRail>().Class("xyui-navigation-rail")); Brush(rail, Border.BackgroundProperty, "XY.Brush.Surface.Panel"); rail.Setters.Add(new Setter(Border.WidthProperty, XyuiComponentTokens.NavigationRailWidth)); rail.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(7))); styles.Add(rail);
        var railItem = new Style(x => x.OfType<XYNavigationItem>().Class("xyui-rail-item")); railItem.Setters.Add(new Setter(Border.HeightProperty, XyuiComponentTokens.NavigationRailItemHeight)); railItem.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(6, 0))); styles.Add(railItem);
        var railLabel = new Style(x => x.OfType<XYNavigationItem>().Class("xyui-rail-item").Descendant().OfType<TextBlock>().Class("xyui-navigation-label")); railLabel.Setters.Add(new Setter(Visual.IsVisibleProperty, false)); styles.Add(railLabel);
        var railIcon = new Style(x => x.OfType<XYNavigationItem>().Class("xyui-rail-item").Descendant().OfType<XYIcon>().Class("xyui-navigation-icon")); railIcon.Setters.Add(new Setter(Control.MarginProperty, new Thickness(10, 0, 0, 0))); styles.Add(railIcon);
        var context = new Style(x => x.OfType<TextBlock>().Class("xyui-sidebar-context-label")); context.Setters.Add(new Setter(Control.HeightProperty, 20d)); context.Setters.Add(new Setter(Control.MarginProperty, new Thickness(6, 8, 6, 4))); Set(context, TextBlock.ForegroundProperty, "XY.Brush.Text.Secondary"); styles.Add(context);
        var contextItem = new Style(x => x.OfType<TextBlock>().Class("xyui-sidebar-context-item")); contextItem.Setters.Add(new Setter(Control.HeightProperty, 28d)); contextItem.Setters.Add(new Setter(Control.MarginProperty, new Thickness(16, 0, 0, 0))); Set(contextItem, TextBlock.ForegroundProperty, "XY.Brush.Text.Primary"); styles.Add(contextItem);
        var footer = new Style(x => x.OfType<XYSidebarFooter>().Class("xyui-sidebar-footer")); footer.Setters.Add(new Setter(Border.HeightProperty, 36d)); footer.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(38, 0))); styles.Add(footer); TextStyle(styles, "xyui-sidebar-footer-label", 13, 400, "XY.Brush.Text.Primary");
        var tabs = new Style(x => x.OfType<XYTabs>().Class("xyui-tabs")); Brush(tabs, Border.BackgroundProperty, "XY.Brush.Surface.Panel"); tabs.Setters.Add(new Setter(Border.HeightProperty, 50d)); styles.Add(tabs);
        var tab = new Style(x => x.OfType<XYTab>().Class("xyui-tab")); tab.Setters.Add(new Setter(Border.HeightProperty, XyuiComponentTokens.TabHeight)); tab.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(XyuiComponentTokens.TabPaddingX, 0))); tab.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Transparent)); styles.Add(tab);
        var tabClose = new Style(x => x.OfType<Button>().Class("xyui-tab-close")); tabClose.Setters.Add(new Setter(Control.WidthProperty, 18d)); tabClose.Setters.Add(new Setter(Control.HeightProperty, 18d)); tabClose.Setters.Add(new Setter(Button.PaddingProperty, new Thickness(0))); tabClose.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Transparent)); tabClose.Setters.Add(new Setter(Button.BorderThicknessProperty, new Thickness(0))); styles.Add(tabClose);
        var selected = new Style(x => x.OfType<XYTab>().Class("xyui-tab-selected")); Brush(selected, Border.BackgroundProperty, "XY.Brush.Surface.Selected"); styles.Add(selected);
        var tabAccent = new Style(x => x.OfType<Border>().Class("xyui-tab-accent")); Brush(tabAccent, Border.BackgroundProperty, "XY.Brush.Accent.Default"); tabAccent.Setters.Add(new Setter(Border.HeightProperty, 3d)); tabAccent.Setters.Add(new Setter(Border.VerticalAlignmentProperty, global::Avalonia.Layout.VerticalAlignment.Bottom)); styles.Add(tabAccent);
        TextStyle(styles, "xyui-tab-label", 13, 400, "XY.Brush.Text.Secondary");
        var dot = new Style(x => x.OfType<Border>().Class("xyui-tab-modified")); Brush(dot, Border.BackgroundProperty, "XY.Brush.Text.Secondary"); dot.Setters.Add(new Setter(Border.WidthProperty, 3d)); dot.Setters.Add(new Setter(Border.HeightProperty, 3d)); dot.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(2))); styles.Add(dot);
    }
}
