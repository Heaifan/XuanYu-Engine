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
        var bar = new Style(x => x.OfType<XYBottomNavigation>().Class("xyui-bottom-navigation")); Brush(bar, Border.BackgroundProperty, "XY.Brush.Surface.Panel"); Brush(bar, Border.BorderBrushProperty, "XY.Brush.Border.Color.Subtle"); bar.Setters.Add(new Setter(Border.HeightProperty, 64d)); bar.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(0, 1, 0, 0))); bar.Setters.Add(new Setter(Border.HorizontalAlignmentProperty, HorizontalAlignment.Stretch)); styles.Add(bar);
        var destination = new Style(x => x.OfType<XYBottomDestination>().Class("xyui-bottom-navigation-destination")); destination.Setters.Add(new Setter(Border.HeightProperty, 64d)); destination.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Transparent)); styles.Add(destination);
        var well = new Style(x => x.OfType<Border>().Class("xyui-bottom-navigation-icon-well")); well.Setters.Add(new Setter(Border.WidthProperty, 36d)); well.Setters.Add(new Setter(Border.HeightProperty, 28d)); well.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(6))); well.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Transparent)); styles.Add(well);
        var selectedWell = new Style(x => x.OfType<Border>().Class("xyui-bottom-navigation-selected-well")); Brush(selectedWell, Border.BackgroundProperty, "XY.Brush.Surface.Selected"); styles.Add(selectedWell);
        var icon = new Style(x => x.OfType<XYIcon>().Class("xyui-bottom-navigation-icon")); icon.Setters.Add(new Setter(Control.WidthProperty, 18d)); icon.Setters.Add(new Setter(Control.HeightProperty, 18d)); Brush(icon, XYIcon.StrokeProperty, "XY.Brush.Text.Secondary"); styles.Add(icon);
        var selectedIcon = new Style(x => x.OfType<XYBottomDestination>().Class("xyui-bottom-navigation-selected").Descendant().OfType<XYIcon>().Class("xyui-bottom-navigation-icon")); Brush(selectedIcon, XYIcon.StrokeProperty, "XY.Brush.Accent.Default"); styles.Add(selectedIcon);
        var label = new Style(x => x.OfType<TextBlock>().Class("xyui-bottom-navigation-label")); label.Setters.Add(new Setter(TemplatedControl.FontSizeProperty, 11.5d)); label.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center)); Brush(label, TextBlock.ForegroundProperty, "XY.Brush.Text.Secondary"); styles.Add(label);
        var selectedLabel = new Style(x => x.OfType<XYBottomDestination>().Class("xyui-bottom-navigation-selected").Descendant().OfType<TextBlock>().Class("xyui-bottom-navigation-label")); Brush(selectedLabel, TextBlock.ForegroundProperty, "XY.Brush.Accent.Default"); selectedLabel.Setters.Add(new Setter(TemplatedControl.FontWeightProperty, FontWeight.SemiBold)); styles.Add(selectedLabel);
        var badgeCount = new Style(x => x.OfType<XYStatusBadge>().Class("xyui-bottom-navigation-badge")); badgeCount.Setters.Add(new Setter(Border.HeightProperty, 15d)); badgeCount.Setters.Add(new Setter(Layoutable.MinHeightProperty, 15d)); badgeCount.Setters.Add(new Setter(Layoutable.MaxHeightProperty, 15d)); badgeCount.Setters.Add(new Setter(Border.MarginProperty, new Thickness(0, -2, -6, 0))); badgeCount.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(2, 0))); badgeCount.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(8))); badgeCount.Setters.Add(new Setter(TemplatedControl.FontSizeProperty, 9d)); styles.Add(badgeCount);
        var primaryHost = new Style(x => x.OfType<Grid>().Class("xyui-bottom-navigation-primary-host")); primaryHost.Setters.Add(new Setter(Visual.ClipToBoundsProperty, false)); primaryHost.Setters.Add(new Setter(Layoutable.MarginProperty, new Thickness(0, -16, 0, 0))); styles.Add(primaryHost);
        var primary = new Style(x => x.OfType<XYButton>().Class("xyui-bottom-navigation-primary")); Brush(primary, Button.BackgroundProperty, "XY.Brush.Accent.Default"); primary.Setters.Add(new Setter(Button.WidthProperty, 48d)); primary.Setters.Add(new Setter(Button.HeightProperty, 48d)); primary.Setters.Add(new Setter(Button.CornerRadiusProperty, new CornerRadius(24))); primary.Setters.Add(new Setter(Button.PaddingProperty, new Thickness(0))); styles.Add(primary);
        var primaryIcon = new Style(x => x.OfType<XYIcon>().Class("xyui-bottom-navigation-primary-icon")); primaryIcon.Setters.Add(new Setter(Control.WidthProperty, 18d)); primaryIcon.Setters.Add(new Setter(Control.HeightProperty, 18d)); primaryIcon.Setters.Add(new Setter(XYIcon.StrokeProperty, Brushes.White)); styles.Add(primaryIcon);
        var primaryLabel = new Style(x => x.OfType<TextBlock>().Class("xyui-bottom-navigation-primary-label")); primaryLabel.Setters.Add(new Setter(TemplatedControl.FontSizeProperty, 11d)); Brush(primaryLabel, TextBlock.ForegroundProperty, "XY.Brush.Text.Secondary"); styles.Add(primaryLabel);
    }
}
