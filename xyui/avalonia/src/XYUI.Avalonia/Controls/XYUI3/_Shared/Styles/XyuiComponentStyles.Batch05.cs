using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiComponentStyles
{
    static void Batch05(Styles styles)
    {
        var bar = new Style(x => x.OfType<XYCommandBar>().Class("xyui-command-bar")); Brush(bar, Border.BackgroundProperty, "XY.Brush.Surface.App"); bar.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(0))); styles.Add(bar);
        var barSurface = new Style(x => x.OfType<Border>().Class("xyui-command-bar-surface")); Brush(barSurface, Border.BackgroundProperty, "XY.Brush.Surface.Toolbar"); Brush(barSurface, Border.BorderBrushProperty, "XY.Brush.Border.Color.Default"); barSurface.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1))); styles.Add(barSurface);
        var command = new Style(x => x.OfType<XYCommandItem>().Class("xyui-command-item")); command.Setters.Add(new Setter(Button.HeightProperty, 28d)); command.Setters.Add(new Setter(Button.PaddingProperty, new Thickness(8, 0))); command.Setters.Add(new Setter(TemplatedControl.FontSizeProperty, 12.5d)); styles.Add(command);
        var palette = new Style(x => x.OfType<XYCommandPalette>().Class("xyui-command-palette")); Brush(palette, Border.BackgroundProperty, "XY.Brush.Surface.Overlay"); Brush(palette, Border.BorderBrushProperty, "XY.Brush.Border.Color.Subtle"); palette.Setters.Add(new Setter(Border.WidthProperty, 440d)); palette.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(10))); palette.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(5))); styles.Add(palette);
        var paletteSurface = new Style(x => x.OfType<Border>().Class("xyui-palette-surface")); Brush(paletteSurface, Border.BackgroundProperty, "XY.Brush.Surface.Overlay"); Brush(paletteSurface, Border.BorderBrushProperty, "XY.Brush.Border.Color.Subtle"); paletteSurface.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1))); paletteSurface.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(5))); styles.Add(paletteSurface);
        var result = new Style(x => x.OfType<Button>().Class("xyui-palette-result")); result.Setters.Add(new Setter(Button.HeightProperty, 30d)); result.Setters.Add(new Setter(Button.PaddingProperty, new Thickness(10, 0))); styles.Add(result);
        var selected = new Style(x => x.OfType<Button>().Class("xyui-palette-result-selected")); Brush(selected, Button.BackgroundProperty, "XY.Brush.Surface.Selected"); Brush(selected, Button.ForegroundProperty, "XY.Brush.Accent.Strong"); styles.Add(selected);
        var back = new Style(x => x.OfType<XYBackForwardNavigation>().Class("xyui-back-forward")); back.Setters.Add(new Setter(Border.HeightProperty, 34d)); back.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(4, 3))); styles.Add(back);
        var action = new Style(x => x.OfType<XYIconButton>().Class("xyui-back-forward-action")); action.Setters.Add(new Setter(Button.WidthProperty, 28d)); action.Setters.Add(new Setter(Button.HeightProperty, 28d)); action.Setters.Add(new Setter(Button.PaddingProperty, new Thickness(0))); styles.Add(action);
        var backSurface = new Style(x => x.OfType<Border>().Class("xyui-back-forward-surface")); Brush(backSurface, Border.BackgroundProperty, "XY.Brush.Surface.Panel"); Brush(backSurface, Border.BorderBrushProperty, "XY.Brush.Border.Color.Default"); backSurface.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1))); styles.Add(backSurface);
        var workspace = new Style(x => x.OfType<XYWorkspaceSwitcher>().Class("xyui-workspace-switcher")); workspace.Setters.Add(new Setter(Border.WidthProperty, 224d)); styles.Add(workspace);
        var workspaceItem = new Style(x => x.OfType<Button>().Class("xyui-workspace-item")); workspaceItem.Setters.Add(new Setter(Button.HeightProperty, 32d)); workspaceItem.Setters.Add(new Setter(Button.PaddingProperty, new Thickness(10, 0))); styles.Add(workspaceItem);
        var workspacePopup = new Style(x => x.OfType<Border>().Class("xyui-workspace-popup")); Brush(workspacePopup, Border.BackgroundProperty, "XY.Brush.Surface.Overlay"); Brush(workspacePopup, Border.BorderBrushProperty, "XY.Brush.Border.Color.Subtle"); workspacePopup.Setters.Add(new Setter(Border.WidthProperty, 224d)); workspacePopup.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(8))); workspacePopup.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(4))); styles.Add(workspacePopup);
    }
}
