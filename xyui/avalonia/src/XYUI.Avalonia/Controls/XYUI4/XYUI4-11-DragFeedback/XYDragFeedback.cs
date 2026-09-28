using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public sealed class XYDragFeedback : Control
{
    public static readonly StyledProperty<Rect> SourceRectProperty = AvaloniaProperty.Register<XYDragFeedback, Rect>(nameof(SourceRect));
    public static readonly StyledProperty<Rect> PreviewRectProperty = AvaloniaProperty.Register<XYDragFeedback, Rect>(nameof(PreviewRect));
    public static readonly StyledProperty<bool> IsDraggingProperty = AvaloniaProperty.Register<XYDragFeedback, bool>(nameof(IsDragging));
    public static readonly StyledProperty<double> SourceOpacityProperty = AvaloniaProperty.Register<XYDragFeedback, double>(nameof(SourceOpacity), .55);
    public static readonly StyledProperty<double> PreviewOpacityProperty = AvaloniaProperty.Register<XYDragFeedback, double>(nameof(PreviewOpacity), .9);
    public static readonly StyledProperty<IBrush?> SourceBrushProperty = AvaloniaProperty.Register<XYDragFeedback, IBrush?>(nameof(SourceBrush));
    public static readonly StyledProperty<IBrush?> PreviewBrushProperty = AvaloniaProperty.Register<XYDragFeedback, IBrush?>(nameof(PreviewBrush));
    public static readonly StyledProperty<IBrush?> BorderBrushProperty = AvaloniaProperty.Register<XYDragFeedback, IBrush?>(nameof(BorderBrush));
    public static readonly StyledProperty<string?> PreviewTextProperty = AvaloniaProperty.Register<XYDragFeedback, string?>(nameof(PreviewText));

    static XYDragFeedback() => AffectsRender<XYDragFeedback>(SourceRectProperty, PreviewRectProperty, IsDraggingProperty, SourceOpacityProperty, PreviewOpacityProperty, SourceBrushProperty, PreviewBrushProperty, BorderBrushProperty, PreviewTextProperty);

    public XYDragFeedback() { Classes.Add("xyui-4-component"); Classes.Add("xyui-drag-feedback"); IsHitTestVisible = false; }
    public string CanonicalId => "XYUI-4-4.11";
    public Rect SourceRect { get => GetValue(SourceRectProperty); set => SetValue(SourceRectProperty, value); }
    public Rect PreviewRect { get => GetValue(PreviewRectProperty); set => SetValue(PreviewRectProperty, value); }
    public bool IsDragging { get => GetValue(IsDraggingProperty); set => SetValue(IsDraggingProperty, value); }
    public double SourceOpacity { get => GetValue(SourceOpacityProperty); set => SetValue(SourceOpacityProperty, Math.Clamp(value, 0, 1)); }
    public double PreviewOpacity { get => GetValue(PreviewOpacityProperty); set => SetValue(PreviewOpacityProperty, Math.Clamp(value, 0, 1)); }
    public IBrush? SourceBrush { get => GetValue(SourceBrushProperty); set => SetValue(SourceBrushProperty, value); }
    public IBrush? PreviewBrush { get => GetValue(PreviewBrushProperty); set => SetValue(PreviewBrushProperty, value); }
    public IBrush? BorderBrush { get => GetValue(BorderBrushProperty); set => SetValue(BorderBrushProperty, value); }
    public string? PreviewText { get => GetValue(PreviewTextProperty); set => SetValue(PreviewTextProperty, value); }

    public override void Render(DrawingContext context)
    {
        if (!IsDragging || BorderBrush is null) return;
        var pen = new Pen(BorderBrush, 1.5);
        if (SourceRect.Width > 0 && SourceRect.Height > 0) using (context.PushOpacity(SourceOpacity)) context.DrawRectangle(SourceBrush, pen, SourceRect);
        if (PreviewRect.Width > 0 && PreviewRect.Height > 0) using (context.PushOpacity(PreviewOpacity)) context.DrawRectangle(PreviewBrush, pen, PreviewRect);
    }
}
