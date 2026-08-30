using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Styling;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiComponentStyles
{
    static void NavigationMenu(Styles styles)
    {
        var menu = new Style(x => x.OfType<XYNavigationMenu>().Class("xyui-navigation-menu")); Set(menu, Border.BackgroundProperty, "XY.Brush.Surface.Panel"); menu.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(10))); styles.Add(menu);
        var item = new Style(x => x.OfType<XYNavigationItem>().Class("xyui-navigation-item")); item.Setters.Add(new Setter(Border.HeightProperty, XyuiComponentTokens.NavigationMenuItemHeight)); item.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(XyuiComponentTokens.NavigationMenuPaddingX, 0))); item.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(XyuiComponentTokens.NavigationMenuItemRadius))); item.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Transparent)); styles.Add(item);
        var hover = new Style(x => x.OfType<XYNavigationItem>().Class("xyui-navigation-item").Class(":pointerover")); Set(hover, Border.BackgroundProperty, "XY.Brush.State.Color.Hover"); styles.Add(hover);
        var selected = new Style(x => x.OfType<XYNavigationItem>().Class("xyui-navigation-selected")); Set(selected, Border.BackgroundProperty, "XY.Brush.Surface.Selected"); styles.Add(selected);
        var accent = new Style(x => x.OfType<Border>().Class("xyui-navigation-accent")); Set(accent, Border.BackgroundProperty, "XY.Brush.Accent.Default"); accent.Setters.Add(new Setter(Visual.IsVisibleProperty, false)); styles.Add(accent);
        var shown = new Style(x => x.OfType<XYNavigationItem>().Class("xyui-navigation-selected").Descendant().OfType<Border>().Class("xyui-navigation-accent")); shown.Setters.Add(new Setter(Visual.IsVisibleProperty, true)); styles.Add(shown);
        TextStyle(styles, "xyui-navigation-group", 11, 400, "XY.Brush.Text.Secondary"); var group = new Style(x => x.OfType<TextBlock>().Class("xyui-navigation-group")); group.Setters.Add(new Setter(Control.HeightProperty, XyuiComponentTokens.NavigationMenuGroupLabelHeight)); styles.Add(group); TextStyle(styles, "xyui-navigation-label", 13, 400, "XY.Brush.Text.Primary");
        var icon = new Style(x => x.OfType<XYIcon>().Class("xyui-navigation-icon")); icon.Setters.Add(new Setter(Control.WidthProperty, 14d)); icon.Setters.Add(new Setter(Control.HeightProperty, 14d)); Set(icon, XYIcon.StrokeProperty, "XY.Brush.Text.Secondary"); styles.Add(icon);
        var selectedText = new Style(x => x.OfType<XYNavigationItem>().Class("xyui-navigation-selected").Descendant().OfType<TextBlock>().Class("xyui-navigation-label")); Set(selectedText, TextBlock.ForegroundProperty, "XY.Brush.Accent.Strong"); styles.Add(selectedText);
        var selectedIcon = new Style(x => x.OfType<XYNavigationItem>().Class("xyui-navigation-selected").Descendant().OfType<XYIcon>().Class("xyui-navigation-icon")); Set(selectedIcon, XYIcon.StrokeProperty, "XY.Brush.Accent.Strong"); styles.Add(selectedIcon);
    }
    static void Set(Style style, AvaloniaProperty property, string key) => style.Setters.Add(new Setter(property, new DynamicResourceExtension(key)));
}
