using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Styling;
using XYUI.Avalonia.Typography;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiComponentStyles
{
    static void AddXYUI4(Styles styles) { HoverState(styles); Spinner(styles); LoadingIndicator(styles); ProgressBar(styles); SelectionGestures(styles); SelectionOutline(styles); BoundingBox(styles); DragFeedback(styles); }

    static void SelectionGestures(Styles styles)
    {
        var marquee = new Style(x => x.OfType<XYMarqueeSelection>().Class("xyui-marquee-selection"));
        Brush(marquee, XYMarqueeSelection.BorderBrushProperty, "XY.Brush.Editor.Selection"); styles.Add(marquee);
        var crossing = new Style(x => x.OfType<XYMarqueeSelection>().Class("xyui-marquee-selection-crossing"));
        Brush(crossing, XYMarqueeSelection.BorderBrushProperty, "XY.Brush.Editor.MultiSelection"); styles.Add(crossing);
        var lasso = new Style(x => x.OfType<XYLassoSelection>().Class("xyui-lasso-selection"));
        Brush(lasso, XYLassoSelection.StrokeProperty, "XY.Brush.Editor.Selection"); styles.Add(lasso);
    }

    static void SelectionOutline(Styles styles)
    {
        var outline = new Style(x => x.OfType<XYSelectionOutline>().Class("xyui-selection-outline"));
        Brush(outline, XYSelectionOutline.AccentBrushProperty, "XY.Brush.Editor.Selection"); styles.Add(outline);
    }

    static void BoundingBox(Styles styles)
    {
        var box = new Style(x => x.OfType<XYBoundingBox>().Class("xyui-bounding-box"));
        Brush(box, XYBoundingBox.BorderBrushProperty, "XY.Brush.Editor.BoundingBox");
        Brush(box, XYBoundingBox.HandleBrushProperty, "XY.Brush.Editor.Handle"); styles.Add(box);
    }

    static void HoverState(Styles styles)
    {
        var root = new Style(x => x.OfType<XYHoverState>().Class("xyui-hover-state"));
        root.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Transparent));
        root.Setters.Add(new Setter(Border.BorderBrushProperty, Brushes.Transparent));
        root.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1.5))); styles.Add(root);
        HoverSurface(styles, "xyui-hover-state-surface", Border.BackgroundProperty, "XY.Brush.State.Color.Hover");
        HoverSurface(styles, "xyui-hover-state-border", Border.BorderBrushProperty, "XY.Brush.State.Color.Hover");
        HoverSurface(styles, "xyui-hover-state-outline", Border.BorderBrushProperty, "XY.Brush.Accent.Default");
        HoverSurface(styles, "xyui-hover-state-handle", Border.BorderBrushProperty, "XY.Brush.State.Color.Hover");
        var selected = new Style(x => x.OfType<XYHoverState>().Class("xyui-hover-state-selected"));
        Brush(selected, Border.BackgroundProperty, "XY.Brush.Surface.Selected"); styles.Add(selected);
    }

    static void HoverSurface(Styles styles, string cls, AvaloniaProperty property, string token)
    {
        var style = new Style(x => x.OfType<XYHoverState>().Class(cls).Class("xyui-hover-state-hovered"));
        Brush(style, property, token); styles.Add(style);
    }

    static void Spinner(Styles styles)
    {
        var root = new Style(x => x.OfType<XYSpinner>().Class("xyui-spinner"));
        Brush(root, XYSpinner.TrackProperty, "XY.Brush.Accent.Soft");
        Brush(root, XYSpinner.ArcProperty, "XY.Brush.Accent.Default"); styles.Add(root);
    }

    static void LoadingIndicator(Styles styles)
    {
        var root = new Style(x => x.OfType<XYLoadingIndicator>().Class("xyui-loading-indicator"));
        root.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Transparent));
        root.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(0))); styles.Add(root);
        var label = new Style(x => x.OfType<TextBlock>().Class("xyui-loading-indicator-text"));
        label.Setters.Add(new Setter(TextBlock.FontSizeProperty, XyuiTypographyTokens.FontSizeBody));
        label.Setters.Add(new Setter(TextBlock.ForegroundProperty, new DynamicResourceExtension("XY.Brush.Text.Secondary"))); styles.Add(label);
        var secondary = new Style(x => x.OfType<TextBlock>().Class("xyui-loading-indicator-secondary"));
        secondary.Setters.Add(new Setter(TextBlock.FontSizeProperty, XyuiTypographyTokens.FontSizeCaption));
        secondary.Setters.Add(new Setter(TextBlock.ForegroundProperty, new DynamicResourceExtension("XY.Brush.Text.Secondary"))); styles.Add(secondary);
    }

    static void ProgressBar(Styles styles)
    {
        var root = new Style(x => x.OfType<XYProgressBar>().Class("xyui-progress-bar"));
        Brush(root, XYProgressBar.TrackProperty, "XY.Brush.Accent.Soft");
        Brush(root, XYProgressBar.FillProperty, "XY.Brush.Accent.Default"); styles.Add(root);
    }

    static void DragFeedback(Styles styles)
    {
        var root = new Style(x => x.OfType<XYDragFeedback>().Class("xyui-drag-feedback"));
        Brush(root, XYDragFeedback.SourceBrushProperty, "XY.Brush.State.Color.Dragging");
        Brush(root, XYDragFeedback.PreviewBrushProperty, "XY.Brush.Surface.Selected");
        Brush(root, XYDragFeedback.BorderBrushProperty, "XY.Brush.Editor.Selection"); styles.Add(root);
    }
}
