using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiComponentStyles
{
    static void TabBarDockTabs(Styles styles)
    {
        var bar = new Style(x => x.OfType<XYTabBar>().Class("xyui-tab-bar")); Brush(bar, Border.BackgroundProperty, "XY.Brush.Surface.Panel"); Brush(bar, Border.BorderBrushProperty, "XY.Brush.Border.Color.Default"); bar.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1))); bar.Setters.Add(new Setter(Border.HeightProperty, XyuiCompactNavigationTokens.TabBarHeight)); styles.Add(bar);
        var action = new Style(x => x.OfType<XYIconButton>().Class("xyui-tab-bar-action")); action.Setters.Add(new Setter(Button.PaddingProperty, new Thickness(0))); action.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Transparent)); action.Setters.Add(new Setter(Button.BorderThicknessProperty, new Thickness(0))); styles.Add(action);
        var previous = new Style(x => x.OfType<XYIconButton>().Class("xyui-tab-bar-previous")); previous.Setters.Add(new Setter(Control.WidthProperty, XyuiCompactNavigationTokens.TabBarArrowWidth)); Brush(previous, Button.BackgroundProperty, "XY.Brush.Surface.PanelAlt"); styles.Add(previous);
        var next = new Style(x => x.OfType<XYIconButton>().Class("xyui-tab-bar-next")); next.Setters.Add(new Setter(Control.WidthProperty, XyuiCompactNavigationTokens.TabBarArrowWidth)); Brush(next, Button.BackgroundProperty, "XY.Brush.Surface.PanelAlt"); styles.Add(next);
        var overflow = new Style(x => x.OfType<XYIconButton>().Class("xyui-tab-bar-overflow")); overflow.Setters.Add(new Setter(Control.WidthProperty, XyuiCompactNavigationTokens.TabBarOverflowWidth)); styles.Add(overflow);
        var add = new Style(x => x.OfType<XYIconButton>().Class("xyui-tab-bar-new")); add.Setters.Add(new Setter(Control.WidthProperty, XyuiCompactNavigationTokens.TabBarNewWidth)); styles.Add(add);
        var actionIcon = new Style(x => x.OfType<XYIconButton>().Class("xyui-tab-bar-action").Descendant().OfType<XYIcon>()); Brush(actionIcon, XYIcon.StrokeProperty, "XY.Brush.Text.Secondary"); styles.Add(actionIcon);
        var dockBar = new Style(x => x.OfType<XYDockTabs>().Class("xyui-dock-tabs")); Brush(dockBar, Border.BackgroundProperty, "XY.Brush.Surface.PanelAlt"); dockBar.Setters.Add(new Setter(Border.HeightProperty, XyuiCompactNavigationTokens.DockTabHeight)); styles.Add(dockBar);
        var dock = new Style(x => x.OfType<XYDockTab>().Class("xyui-dock-tab")); dock.Setters.Add(new Setter(Border.HeightProperty, XyuiCompactNavigationTokens.DockTabHeight)); dock.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Transparent)); styles.Add(dock);
        var dockSelected = new Style(x => x.OfType<XYDockTab>().Class("xyui-dock-tab-selected")); Brush(dockSelected, Border.BackgroundProperty, "XY.Brush.Surface.Raised"); styles.Add(dockSelected);
        var inner = new Style(x => x.OfType<XYTab>().Class("xyui-dock-tab-inner")); inner.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Transparent)); inner.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(0))); styles.Add(inner);
        var innerAccent = new Style(x => x.OfType<XYTab>().Class("xyui-dock-tab-inner").Descendant().OfType<Border>().Class("xyui-tab-accent")); innerAccent.Setters.Add(new Setter(Visual.IsVisibleProperty, false)); styles.Add(innerAccent);
        var innerDivider = new Style(x => x.OfType<XYTab>().Class("xyui-dock-tab-inner").Descendant().OfType<Border>().Class("xyui-tab-divider")); innerDivider.Setters.Add(new Setter(Visual.IsVisibleProperty, false)); styles.Add(innerDivider);
        var grip = new Style(x => x.OfType<XYIcon>().Class("xyui-dock-grip")); grip.Setters.Add(new Setter(Control.WidthProperty, 12d)); grip.Setters.Add(new Setter(Control.HeightProperty, 12d)); Brush(grip, XYIcon.StrokeProperty, "XY.Brush.Text.Tertiary"); styles.Add(grip);
        var accent = new Style(x => x.OfType<Border>().Class("xyui-dock-accent")); Brush(accent, Border.BackgroundProperty, "XY.Brush.Accent.Default"); accent.Setters.Add(new Setter(Border.HeightProperty, 3d)); styles.Add(accent);
        var divider = new Style(x => x.OfType<XYSeparator>().Class("xyui-dock-divider")); divider.Setters.Add(new Setter(Border.MarginProperty, new Thickness(0, 7))); styles.Add(divider);
    }
}
