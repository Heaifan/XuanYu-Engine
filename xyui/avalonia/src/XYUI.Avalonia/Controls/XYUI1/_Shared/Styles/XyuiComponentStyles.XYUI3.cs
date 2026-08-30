using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;
using XYUI.Avalonia.Typography;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiComponentStyles
{
    static void AddXYUI3(Styles styles)
    {
        MenuBar(styles); Menu(styles); ContextMenu(styles); SubMenu(styles); MenuText(styles);
    }
    static void MenuBar(Styles styles)
    {
        var bar = new Style(x => x.OfType<XYMenuBar>().Class("xyui-menu-bar"));
        Brush(bar, Border.BackgroundProperty, "XY.Brush.Surface.Panel"); bar.Setters.Add(new Setter(Border.HeightProperty, 46d)); styles.Add(bar);
        var item = new Style(x => x.OfType<XYMenuBarItem>().Class("xyui-menu-bar-item"));
        item.Setters.Add(new Setter(Border.HeightProperty, 35d)); item.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(10, 0))); item.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(3))); styles.Add(item);
        State(styles, typeof(XYMenuBarItem), "xyui-menu-hover", "XY.Brush.State.Color.Hover");
        var active = new Style(x => x.OfType<XYMenuBarItem>().Class("xyui-menu-active")); active.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Transparent)); styles.Add(active);
        TextStyle(styles, "xyui-menu-bar-label", XyuiTypographyTokens.FontSizeBody, XyuiTypographyTokens.WeightRegular, "XY.Brush.Text.Primary");
        TextStyle(styles, "xyui-menu-bar-active-label", XyuiTypographyTokens.FontSizeBody, XyuiTypographyTokens.WeightMedium, "XY.Brush.Accent.Strong");
        var activeText = new Style(x => x.OfType<XYMenuBarItem>().Class("xyui-menu-active").Descendant().OfType<TextBlock>().Class("xyui-menu-bar-label")); Brush(activeText, TextBlock.ForegroundProperty, "XY.Brush.Accent.Strong"); activeText.Setters.Add(new Setter(TextBlock.FontWeightProperty, FontWeight.Medium)); styles.Add(activeText);
        var indicator = new Style(x => x.OfType<Border>().Class("xyui-menu-bar-indicator")); Brush(indicator, Border.BackgroundProperty, "XY.Brush.Accent.Default"); indicator.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(1.5))); indicator.Setters.Add(new Setter(Visual.IsVisibleProperty, false)); styles.Add(indicator);
        var shown = new Style(x => x.OfType<XYMenuBarItem>().Class("xyui-menu-active").Descendant().OfType<Border>().Class("xyui-menu-bar-indicator")); shown.Setters.Add(new Setter(Visual.IsVisibleProperty, true)); styles.Add(shown);
    }
    static void Menu(Styles styles)
    {
        var menu = new Style(x => x.OfType<XYMenu>().Class("xyui-menu")); Brush(menu, Border.BackgroundProperty, "XY.Brush.Surface.Overlay"); Brush(menu, Border.BorderBrushProperty, "XY.Brush.Border.Color.Default"); menu.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1))); menu.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(6))); menu.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(5))); styles.Add(menu);
        var embedded = new Style(x => x.OfType<XYMenu>().Class("xyui-menu-embedded")); embedded.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Transparent)); embedded.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(0))); embedded.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(0))); embedded.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(5, 5, 5, 0))); styles.Add(embedded);
        var item = new Style(x => x.OfType<XYMenuItem>().Class("xyui-menu-item")); item.Setters.Add(new Setter(Border.HeightProperty, 30d)); item.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(10, 0))); item.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(3))); styles.Add(item);
        State(styles, typeof(XYMenuItem), "xyui-menu-hover", "XY.Brush.State.Color.Hover"); State(styles, typeof(XYMenuItem), "xyui-menu-checked", "XY.Brush.Surface.Selected");
        var danger = new Style(x => x.OfType<XYMenuItem>().Class("xyui-menu-danger").Descendant().OfType<TextBlock>().Class("xyui-menu-label")); Brush(danger, TextBlock.ForegroundProperty, "XY.Brush.Semantic.Error.Text"); styles.Add(danger);
        var separator = new Style(x => x.OfType<XYSeparator>().Class("xyui-menu-separator")); separator.Setters.Add(new Setter(Border.MarginProperty, new Thickness(5, 4))); styles.Add(separator);
    }
    static void ContextMenu(Styles styles)
    {
        var root = new Style(x => x.OfType<XYContextMenu>().Class("xyui-context-menu")); Brush(root, Border.BackgroundProperty, "XY.Brush.Surface.Overlay"); Brush(root, Border.BorderBrushProperty, "XY.Brush.Border.Color.Default"); root.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1))); root.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(6))); styles.Add(root);
        var header = new Style(x => x.OfType<Border>().Class("xyui-context-header")); Brush(header, Border.BackgroundProperty, "XY.Brush.Surface.PanelAlt"); header.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(18, 7, 12, 6))); styles.Add(header);
        TextStyle(styles, "xyui-context-type", XyuiTypographyTokens.FontSizeCaption, XyuiTypographyTokens.WeightRegular, "XY.Brush.Text.Secondary"); TextStyle(styles, "xyui-context-name", XyuiTypographyTokens.FontSizeBody, XyuiTypographyTokens.WeightMedium, "XY.Brush.Text.Primary");
    }
    static void SubMenu(Styles styles)
    {
        var root = new Style(x => x.OfType<XYSubMenu>().Class("xyui-sub-menu")); root.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Transparent)); styles.Add(root);
        var line = new Style(x => x.OfType<Border>().Class("xyui-sub-menu-line")); Brush(line, Border.BackgroundProperty, "XY.Brush.Border.Color.Subtle"); styles.Add(line);
        var dot = new Style(x => x.OfType<Ellipse>().Class("xyui-sub-menu-anchor")); Brush(dot, Shape.FillProperty, "XY.Brush.Accent.Default"); styles.Add(dot);
    }
    static void MenuText(Styles styles)
    {
        TextStyle(styles, "xyui-menu-label", XyuiTypographyTokens.FontSizeAuxiliary, XyuiTypographyTokens.WeightRegular, "XY.Brush.Text.Primary"); TextStyle(styles, "xyui-menu-shortcut", XyuiTypographyTokens.FontSizeCaption, XyuiTypographyTokens.WeightRegular, "XY.Brush.Text.Secondary");
        var icon = new Style(x => x.OfType<XYIcon>().Class("xyui-menu-icon")); icon.Setters.Add(new Setter(Control.WidthProperty, 14d)); icon.Setters.Add(new Setter(Control.HeightProperty, 14d)); Brush(icon, XYIcon.StrokeProperty, "XY.Brush.Text.Secondary"); styles.Add(icon);
        var chevron = new Style(x => x.OfType<XYIcon>().Class("xyui-menu-chevron")); Brush(chevron, XYIcon.StrokeProperty, "XY.Brush.Text.Secondary"); styles.Add(chevron);
        var check = new Style(x => x.OfType<global::Avalonia.Controls.Shapes.Path>().Class("xyui-menu-check")); Brush(check, Shape.StrokeProperty, "XY.Brush.Accent.Default"); check.Setters.Add(new Setter(Shape.StrokeThicknessProperty, 2d)); check.Setters.Add(new Setter(Control.WidthProperty, 14d)); check.Setters.Add(new Setter(Control.HeightProperty, 14d)); styles.Add(check);
        var radio = new Style(x => x.OfType<Grid>().Class("xyui-menu-radio")); radio.Setters.Add(new Setter(Control.WidthProperty, 16d)); radio.Setters.Add(new Setter(Control.HeightProperty, 16d)); styles.Add(radio);
        var circle = new Style(x => x.OfType<Ellipse>().Class("xyui-menu-radio-ring")); Brush(circle, Shape.StrokeProperty, "XY.Brush.Accent.Default"); circle.Setters.Add(new Setter(Shape.StrokeThicknessProperty, 1.5d)); styles.Add(circle);
        var radioDot = new Style(x => x.OfType<Ellipse>().Class("xyui-menu-radio-dot")); Brush(radioDot, Shape.FillProperty, "XY.Brush.Accent.Default"); styles.Add(radioDot);
    }
    static void TextStyle(Styles styles, string cls, double size, int weight, string brush)
    { var style = new Style(x => x.OfType<TextBlock>().Class(cls)); style.Setters.Add(new Setter(TextBlock.FontFamilyProperty, new FontFamily(XyuiTypographyTokens.FontUi))); style.Setters.Add(new Setter(TextBlock.FontSizeProperty, size)); style.Setters.Add(new Setter(TextBlock.FontWeightProperty, weight switch { 500 => FontWeight.Medium, 600 => FontWeight.SemiBold, _ => FontWeight.Normal })); Brush(style, TextBlock.ForegroundProperty, brush); styles.Add(style); }
}
