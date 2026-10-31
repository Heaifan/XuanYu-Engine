using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public sealed class XYSelectionOutline : Control
{
    public static readonly StyledProperty<IBrush?> AccentBrushProperty =
        AvaloniaProperty.Register<XYSelectionOutline, IBrush?>(nameof(AccentBrush));
    public static readonly StyledProperty<IBrush?> SeparationBrushProperty =
        AvaloniaProperty.Register<XYSelectionOutline, IBrush?>(nameof(SeparationBrush));
    public static readonly StyledProperty<IReadOnlyList<Point>> PointsProperty =
        AvaloniaProperty.Register<XYSelectionOutline, IReadOnlyList<Point>>(nameof(Points), Array.Empty<Point>());

    static XYSelectionOutline() => AffectsRender<XYSelectionOutline>(AccentBrushProperty, SeparationBrushProperty, PointsProperty);

    public XYSelectionOutline()
    {
        Classes.Add("xyui-4-component");
        Classes.Add("xyui-selection-outline");
        IsHitTestVisible = false;
    }

    public string CanonicalId => "XYUI-4-4.09";
    public IBrush? AccentBrush { get => GetValue(AccentBrushProperty); set => SetValue(AccentBrushProperty, value); }
    public IBrush? SeparationBrush { get => GetValue(SeparationBrushProperty); set => SetValue(SeparationBrushProperty, value); }
    public IReadOnlyList<Point> Points { get => GetValue(PointsProperty); set => SetValue(PointsProperty, value); }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (Points.Count < 2 || AccentBrush is null) return;
        var geometry = new StreamGeometry();
        using (var figure = geometry.Open())
        {
            figure.BeginFigure(Points[0], true);
            for (var i = 1; i < Points.Count; i++) figure.LineTo(Points[i]);
            figure.EndFigure(true);
        }
        if (SeparationBrush is not null) context.DrawGeometry(null, new Pen(SeparationBrush, 5), geometry);
        context.DrawGeometry(null, new Pen(AccentBrush, 2.5), geometry);
    }
}
