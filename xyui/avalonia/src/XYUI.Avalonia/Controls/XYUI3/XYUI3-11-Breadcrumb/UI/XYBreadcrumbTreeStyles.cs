using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiComponentStyles
{
    static void BreadcrumbTree(Styles styles)
    {
        var breadcrumb = new Style(x => x.OfType<XYBreadcrumb>().Class("xyui-breadcrumb")); Brush(breadcrumb, Border.BackgroundProperty, "XY.Brush.Surface.Panel"); breadcrumb.Setters.Add(new Setter(Border.HeightProperty, XyuiCompactNavigationTokens.BreadcrumbHeight)); breadcrumb.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(4, 0))); styles.Add(breadcrumb);
        var item = new Style(x => x.OfType<XYBreadcrumbItem>().Class("xyui-breadcrumb-item")); item.Setters.Add(new Setter(Border.HeightProperty, XyuiCompactNavigationTokens.BreadcrumbItemHeight)); item.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(8, 0))); item.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(3))); item.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Transparent)); styles.Add(item);
        var hover = new Style(x => x.OfType<XYBreadcrumbItem>().Class("xyui-breadcrumb-item").Class(":pointerover")); Brush(hover, Border.BackgroundProperty, "XY.Brush.State.Color.Hover"); styles.Add(hover);
        var collapsed = new Style(x => x.OfType<XYBreadcrumbItem>().Class("xyui-breadcrumb-collapsed")); collapsed.Setters.Add(new Setter(Border.WidthProperty, XyuiCompactNavigationTokens.BreadcrumbCollapsedWidth)); collapsed.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(0))); Brush(collapsed, Border.BackgroundProperty, "XY.Brush.Surface.Selected"); styles.Add(collapsed);
        TextStyle(styles, "xyui-breadcrumb-label", 13, 400, "XY.Brush.Text.Secondary");
        var current = new Style(x => x.OfType<XYBreadcrumbItem>().Class("xyui-breadcrumb-current").Descendant().OfType<TextBlock>().Class("xyui-breadcrumb-label")); Brush(current, TextBlock.ForegroundProperty, "XY.Brush.Accent.Strong"); current.Setters.Add(new Setter(TextBlock.FontWeightProperty, FontWeight.SemiBold)); styles.Add(current);
        var separator = new Style(x => x.OfType<XYIcon>().Class("xyui-breadcrumb-separator")); Brush(separator, XYIcon.StrokeProperty, "XY.Brush.Border.Color.Subtle"); styles.Add(separator);
        var breadcrumbIcon = new Style(x => x.OfType<XYIcon>().Class("xyui-breadcrumb-dropdown")); Brush(breadcrumbIcon, XYIcon.StrokeProperty, "XY.Brush.Text.Secondary"); styles.Add(breadcrumbIcon);
        var ellipsis = new Style(x => x.OfType<XYIcon>().Class("xyui-breadcrumb-ellipsis")); Brush(ellipsis, XYIcon.StrokeProperty, "XY.Brush.Text.Secondary"); styles.Add(ellipsis);
        var tree = new Style(x => x.OfType<XYTreeNavigation>().Class("xyui-tree-navigation")); Brush(tree, Border.BackgroundProperty, "XY.Brush.Surface.Panel"); Brush(tree, Border.BorderBrushProperty, "XY.Brush.Border.Color.Default"); tree.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1))); tree.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(5))); tree.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(8))); styles.Add(tree);
        var node = new Style(x => x.OfType<XYTreeNode>().Class("xyui-tree-node")); node.Setters.Add(new Setter(Border.HeightProperty, XyuiCompactNavigationTokens.TreeRowHeight)); styles.Add(node);
        var surface = new Style(x => x.OfType<Border>().Class("xyui-tree-node-surface")); surface.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Transparent)); surface.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(3))); styles.Add(surface);
        var treeHover = new Style(x => x.OfType<XYTreeNode>().Class(":pointerover").Descendant().OfType<Border>().Class("xyui-tree-node-surface")); Brush(treeHover, Border.BackgroundProperty, "XY.Brush.State.Color.Hover"); styles.Add(treeHover);
        var selected = new Style(x => x.OfType<XYTreeNode>().Class("xyui-tree-selected").Descendant().OfType<Border>().Class("xyui-tree-node-surface")); Brush(selected, Border.BackgroundProperty, "XY.Brush.Surface.Selected"); styles.Add(selected);
        var accent = new Style(x => x.OfType<Border>().Class("xyui-tree-accent")); Brush(accent, Border.BackgroundProperty, "XY.Brush.Accent.Default"); styles.Add(accent);
        TextStyle(styles, "xyui-tree-label", 13, 400, "XY.Brush.Text.Primary");
        var selectedText = new Style(x => x.OfType<XYTreeNode>().Class("xyui-tree-selected").Descendant().OfType<TextBlock>().Class("xyui-tree-label")); Brush(selectedText, TextBlock.ForegroundProperty, "XY.Brush.Accent.Strong"); selectedText.Setters.Add(new Setter(TextBlock.FontWeightProperty, FontWeight.Medium)); styles.Add(selectedText);
        var treeIcon = new Style(x => x.OfType<XYIcon>().Class("xyui-tree-icon")); Brush(treeIcon, XYIcon.StrokeProperty, "XY.Brush.Text.Secondary"); styles.Add(treeIcon);
        var treeSelectedIcon = new Style(x => x.OfType<XYTreeNode>().Class("xyui-tree-selected").Descendant().OfType<XYIcon>().Class("xyui-tree-icon")); Brush(treeSelectedIcon, XYIcon.StrokeProperty, "XY.Brush.Accent.Default"); styles.Add(treeSelectedIcon);
        var guide = new Style(x => x.OfType<Border>().Class("xyui-tree-guide")); Brush(guide, Border.BackgroundProperty, "XY.Brush.Border.Color.Subtle"); styles.Add(guide);
    }
}
