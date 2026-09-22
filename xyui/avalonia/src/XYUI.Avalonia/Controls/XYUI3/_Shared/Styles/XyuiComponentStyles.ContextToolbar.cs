using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiComponentStyles
{
    static void ContextToolbar(Styles styles)
    {
        var bar = new Style(x => x.OfType<XYContextToolbar>().Class("xyui-context-toolbar")); Brush(bar, Border.BackgroundProperty, "XY.Brush.Surface.Toolbar"); Brush(bar, Border.BorderBrushProperty, "XY.Brush.Border.Color.Subtle"); bar.Setters.Add(new Setter(Border.HeightProperty, 48d)); bar.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(8, 4))); bar.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(6))); styles.Add(bar);
        var group = new Style(x => x.OfType<XYContextGroup>().Class("xyui-context-group")); Brush(group, Border.BackgroundProperty, "XY.Brush.Surface.PanelAlt"); Brush(group, Border.BorderBrushProperty, "XY.Brush.Border.Color.Subtle"); group.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1))); group.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(7, 3))); group.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(7))); styles.Add(group);
        var board = new Style(x => x.OfType<Border>().Class("xyui-context-board-surface")); Brush(board, Border.BackgroundProperty, "XY.Brush.Surface.Overlay"); Brush(board, Border.BorderBrushProperty, "XY.Brush.Border.Color.Default"); board.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1))); board.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(10))); board.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(7))); styles.Add(board);
        var selectedCategory = new Style(x => x.OfType<XYButton>().Class("xyui-context-category-selected")); Brush(selectedCategory, Button.BackgroundProperty, "XY.Brush.Surface.Selected"); selectedCategory.Setters.Add(new Setter(Button.BorderBrushProperty, Token("XY.Brush.Border.Color.Focus"))); styles.Add(selectedCategory);
        var selectedAction = new Style(x => x.OfType<XYButton>().Class("xyui-context-action-selected")); Brush(selectedAction, Button.BackgroundProperty, "XY.Brush.Surface.Selected"); selectedAction.Setters.Add(new Setter(Button.BorderBrushProperty, Token("XY.Brush.Border.Color.Focus"))); styles.Add(selectedAction);
        TextStyle(styles, "xyui-context-group-header", 11, 600, "XY.Brush.Text.Secondary"); TextStyle(styles, "xyui-context-board-header", 13, 600, "XY.Brush.Text.Primary"); TextStyle(styles, "xyui-context-hint-text", 11, 400, "XY.Brush.Text.Secondary");
        var hint = new Style(x => x.OfType<XYContextHintBar>().Class("xyui-context-hint-bar")); hint.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(0, 1, 0, 0))); Brush(hint, Border.BorderBrushProperty, "XY.Brush.Border.Color.Subtle"); styles.Add(hint);
    }
    static object Token(string key) => new DynamicResourceExtension(key);
}
