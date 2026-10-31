using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public sealed class XYLassoSelection : Control
{
    public static readonly StyledProperty<IBrush?> StrokeProperty =
        AvaloniaProperty.Register<XYLassoSelection, IBrush?>(nameof(Stroke));
    public static readonly StyledProperty<IBrush?> FillProperty =
        AvaloniaProperty.Register<XYLassoSelection, IBrush?>(nameof(Fill));
    public static readonly StyledProperty<IReadOnlyList<Point>> PointsProperty =
        AvaloniaProperty.Register<XYLassoSelection, IReadOnlyList<Point>>(nameof(Points), Array.Empty<Point>());

    static XYLassoSelection() => AffectsRender<XYLassoSelection>(StrokeProperty, FillProperty, PointsProperty);

    public XYLassoSelection()
    {
        Classes.Add("xyui-4-component");
        Classes.Add("xyui-lasso-selection");
        IsHitTestVisible = false;
    }

    public string CanonicalId => "XYUI-4-4.08";
    public IBrush? Stroke { get => GetValue(StrokeProperty); set => SetValue(StrokeProperty, value); }
    public IBrush? Fill { get => GetValue(FillProperty); set => SetValue(FillProperty, value); }
    public IReadOnlyList<Point> Points { get => GetValue(PointsProperty); set => SetValue(PointsProperty, value); }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (Stroke is null || Points.Count < 2) return;
        var geometry = new StreamGeometry();
        using (var figure = geometry.Open())
        {
            figure.BeginFigure(Points[0], true);
            for (var i = 1; i < Points.Count; i++) figure.LineTo(Points[i]);
            figure.EndFigure(true);
        }
        context.DrawGeometry(Fill, new Pen(Stroke, 1.25), geometry);
    }
}
