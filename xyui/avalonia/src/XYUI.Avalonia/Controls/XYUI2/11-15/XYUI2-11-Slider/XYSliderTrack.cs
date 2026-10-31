using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public sealed class XYSliderTrack : Control
{
    public static readonly StyledProperty<double> ValueProperty = AvaloniaProperty.Register<XYSliderTrack, double>(nameof(Value));
    public static readonly StyledProperty<double> MinimumProperty = AvaloniaProperty.Register<XYSliderTrack, double>(nameof(Minimum));
    public static readonly StyledProperty<double> MaximumProperty = AvaloniaProperty.Register<XYSliderTrack, double>(nameof(Maximum), 100);
    public static readonly StyledProperty<IBrush?> RailInactiveProperty = AvaloniaProperty.Register<XYSliderTrack, IBrush?>(nameof(RailInactive));
    public static readonly StyledProperty<IBrush?> RailActiveProperty = AvaloniaProperty.Register<XYSliderTrack, IBrush?>(nameof(RailActive));
    public static readonly StyledProperty<IBrush?> ThumbBackgroundProperty = AvaloniaProperty.Register<XYSliderTrack, IBrush?>(nameof(ThumbBackground));
    public static readonly StyledProperty<IBrush?> ThumbBorderProperty = AvaloniaProperty.Register<XYSliderTrack, IBrush?>(nameof(ThumbBorder));
    public double Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
    public double Minimum { get => GetValue(MinimumProperty); set => SetValue(MinimumProperty, value); }
    public double Maximum { get => GetValue(MaximumProperty); set => SetValue(MaximumProperty, value); }
    public IBrush? RailInactive { get => GetValue(RailInactiveProperty); set => SetValue(RailInactiveProperty, value); }
    public IBrush? RailActive { get => GetValue(RailActiveProperty); set => SetValue(RailActiveProperty, value); }
    public IBrush? ThumbBackground { get => GetValue(ThumbBackgroundProperty); set => SetValue(ThumbBackgroundProperty, value); }
    public IBrush? ThumbBorder { get => GetValue(ThumbBorderProperty); set => SetValue(ThumbBorderProperty, value); }
    public bool IsDragging { get => _dragging; set { _dragging = value; InvalidateVisual(); } }
    bool _dragging;
    static XYSliderTrack() => AffectsRender<XYSliderTrack>(ValueProperty, MinimumProperty, MaximumProperty, RailInactiveProperty, RailActiveProperty, ThumbBackgroundProperty, ThumbBorderProperty);

    public override void Render(DrawingContext context)
    {
        base.Render(context); var size = _dragging ? 16d : 14d; var radius = size / 2; var y = Bounds.Height / 2; var start = radius; var end = Math.Max(start, Bounds.Width - radius); var ratio = Maximum > Minimum ? Math.Clamp((Value - Minimum) / (Maximum - Minimum), 0, 1) : 0; var center = start + (end - start) * ratio; var rail = new Rect(start, y - 2, Math.Max(0, end - start), 4);
        context.FillRectangle(RailInactive ?? Brushes.Transparent, rail, 2); context.FillRectangle(RailActive ?? Brushes.Transparent, new Rect(start, y - 2, Math.Max(0, center - start), 4), 2); context.DrawEllipse(ThumbBackground ?? Brushes.Transparent, new Pen(ThumbBorder ?? Brushes.Transparent, 1), new Point(center, y), radius, radius);
    }
}
