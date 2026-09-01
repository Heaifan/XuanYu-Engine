using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiComponentStyles
{
    static void BottomNavigation(Styles styles)
    {
        var bar = new Style(x => x.OfType<XYBottomNavigation>().Class("xyui-bottom-navigation")); Brush(bar, Border.BackgroundProperty, "XY.Brush.Surface.Panel"); Brush(bar, Border.BorderBrushProperty, "XY.Brush.Border.Color.Subtle"); bar.Setters.Add(new Setter(Border.HeightProperty, 66d)); bar.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1))); bar.Setters.Add(new Setter(Border.HorizontalAlignmentProperty, HorizontalAlignment.Stretch)); styles.Add(bar);
        var destination = new Style(x => x.OfType<XYBottomDestination>().Class("xyui-bottom-navigation-destination")); destination.Setters.Add(new Setter(Border.HeightProperty, 58d)); destination.Setters.Add(new Setter(Border.MarginProperty, new Thickness(4))); destination.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(4))); destination.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Transparent)); styles.Add(destination);
        var selected = new Style(x => x.OfType<XYBottomDestination>().Class("xyui-bottom-navigation-selected")); Brush(selected, Border.BackgroundProperty, "XY.Brush.Surface.Selected"); styles.Add(selected);
        var icon = new Style(x => x.OfType<XYIcon>().Class("xyui-bottom-navigation-icon")); icon.Setters.Add(new Setter(Control.WidthProperty, 16d)); icon.Setters.Add(new Setter(Control.HeightProperty, 16d)); Brush(icon, XYIcon.StrokeProperty, "XY.Brush.Text.Secondary"); styles.Add(icon);
        var selectedIcon = new Style(x => x.OfType<XYBottomDestination>().Class("xyui-bottom-navigation-selected").Descendant().OfType<XYIcon>().Class("xyui-bottom-navigation-icon")); Brush(selectedIcon, XYIcon.StrokeProperty, "XY.Brush.Accent.Strong"); styles.Add(selectedIcon);
        var label = new Style(x => x.OfType<TextBlock>().Class("xyui-bottom-navigation-label")); label.Setters.Add(new Setter(TemplatedControl.FontSizeProperty, 10.5d)); label.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center)); Brush(label, TextBlock.ForegroundProperty, "XY.Brush.Text.Secondary"); styles.Add(label);
        var selectedLabel = new Style(x => x.OfType<XYBottomDestination>().Class("xyui-bottom-navigation-selected").Descendant().OfType<TextBlock>().Class("xyui-bottom-navigation-label")); Brush(selectedLabel, TextBlock.ForegroundProperty, "XY.Brush.Accent.Strong"); selectedLabel.Setters.Add(new Setter(TemplatedControl.FontWeightProperty, FontWeight.SemiBold)); styles.Add(selectedLabel);
        var badge = new Style(x => x.OfType<XYStatusDot>().Class("xyui-bottom-navigation-badge")); badge.Setters.Add(new Setter(Border.WidthProperty, 8d)); badge.Setters.Add(new Setter(Border.HeightProperty, 8d)); badge.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(4))); badge.Setters.Add(new Setter(Border.MarginProperty, new Thickness(0, 2, -2, 0))); styles.Add(badge);
        var primaryHost = new Style(x => x.OfType<Grid>().Class("xyui-bottom-navigation-primary-host")); primaryHost.Setters.Add(new Setter(Visual.RenderTransformProperty, new TranslateTransform(0, -12))); styles.Add(primaryHost);
        var primary = new Style(x => x.OfType<XYButton>().Class("xyui-bottom-navigation-primary")); Brush(primary, Button.BackgroundProperty, "XY.Brush.Accent.Default"); primary.Setters.Add(new Setter(Button.WidthProperty, 54d)); primary.Setters.Add(new Setter(Button.HeightProperty, 54d)); primary.Setters.Add(new Setter(Button.CornerRadiusProperty, new CornerRadius(27))); primary.Setters.Add(new Setter(Button.PaddingProperty, new Thickness(0))); styles.Add(primary);
        var primaryIcon = new Style(x => x.OfType<XYIcon>().Class("xyui-bottom-navigation-primary-icon")); primaryIcon.Setters.Add(new Setter(Control.WidthProperty, 22d)); primaryIcon.Setters.Add(new Setter(Control.HeightProperty, 22d)); primaryIcon.Setters.Add(new Setter(XYIcon.StrokeProperty, Brushes.White)); styles.Add(primaryIcon);
        var primaryLabel = new Style(x => x.OfType<TextBlock>().Class("xyui-bottom-navigation-primary-label")); primaryLabel.Setters.Add(new Setter(TemplatedControl.FontSizeProperty, 10.5d)); Brush(primaryLabel, TextBlock.ForegroundProperty, "XY.Brush.Text.Secondary"); styles.Add(primaryLabel);
    }
}
