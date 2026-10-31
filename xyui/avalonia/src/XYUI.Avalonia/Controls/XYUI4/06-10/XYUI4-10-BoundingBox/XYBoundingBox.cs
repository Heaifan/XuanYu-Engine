using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYBoundingBox : Control
{
    public static readonly StyledProperty<Rect> BoundsRectProperty = AvaloniaProperty.Register<XYBoundingBox, Rect>(nameof(BoundsRect));
    public static readonly StyledProperty<IBrush?> BorderBrushProperty = AvaloniaProperty.Register<XYBoundingBox, IBrush?>(nameof(BorderBrush));
    public static readonly StyledProperty<IBrush?> HandleBrushProperty = AvaloniaProperty.Register<XYBoundingBox, IBrush?>(nameof(HandleBrush));
    public static readonly StyledProperty<double> HandleSizeProperty = AvaloniaProperty.Register<XYBoundingBox, double>(nameof(HandleSize), 7);
    public static readonly StyledProperty<bool> ShowRotationHandleProperty = AvaloniaProperty.Register<XYBoundingBox, bool>(nameof(ShowRotationHandle), true);
    public static readonly StyledProperty<bool> ShowPivotProperty = AvaloniaProperty.Register<XYBoundingBox, bool>(nameof(ShowPivot), true);
    public static readonly StyledProperty<double> AngleProperty = AvaloniaProperty.Register<XYBoundingBox, double>(nameof(Angle));
    public static readonly StyledProperty<Point> PivotProperty = AvaloniaProperty.Register<XYBoundingBox, Point>(nameof(Pivot));

    static XYBoundingBox() => AffectsRender<XYBoundingBox>(BoundsRectProperty, BorderBrushProperty, HandleBrushProperty, HandleSizeProperty, ShowRotationHandleProperty, ShowPivotProperty, AngleProperty, PivotProperty);

    public XYBoundingBox()
    {
        Classes.Add("xyui-4-component"); Classes.Add("xyui-bounding-box"); IsHitTestVisible = true;
        PointerPressed += OnPointerPressed; PointerMoved += OnPointerMoved; PointerReleased += OnPointerReleased;
    }

    public string CanonicalId => "XYUI-4-4.10";
    public Rect BoundsRect { get => GetValue(BoundsRectProperty); set => SetValue(BoundsRectProperty, value); }
    public IBrush? BorderBrush { get => GetValue(BorderBrushProperty); set => SetValue(BorderBrushProperty, value); }
    public IBrush? HandleBrush { get => GetValue(HandleBrushProperty); set => SetValue(HandleBrushProperty, value); }
    public double HandleSize { get => GetValue(HandleSizeProperty); set => SetValue(HandleSizeProperty, Math.Clamp(value, 6, 8)); }
    public bool ShowRotationHandle { get => GetValue(ShowRotationHandleProperty); set => SetValue(ShowRotationHandleProperty, value); }
    public bool ShowPivot { get => GetValue(ShowPivotProperty); set => SetValue(ShowPivotProperty, value); }
    public double Angle { get => GetValue(AngleProperty); set => SetValue(AngleProperty, value); }
    public Point Pivot { get => GetValue(PivotProperty); set => SetValue(PivotProperty, value); }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (BorderBrush is null || HandleBrush is null || BoundsRect.Width <= 0 || BoundsRect.Height <= 0) return;
        using (context.PushTransform(Matrix.CreateTranslation(BoundsRect.Center.X, BoundsRect.Center.Y)))
        using (context.PushTransform(Matrix.CreateRotation(Angle * Math.PI / 180)))
        using (context.PushTransform(Matrix.CreateTranslation(-BoundsRect.Center.X, -BoundsRect.Center.Y)))
        {
            context.DrawRectangle(Brushes.White, new Pen(BorderBrush, 1.5), BoundsRect);
            foreach (var point in Handles(BoundsRect)) DrawHandle(context, point);
            if (ShowRotationHandle) { var top = new Point(BoundsRect.Center.X, BoundsRect.Top); context.DrawLine(new Pen(BorderBrush, 1), top, new Point(top.X, top.Y - 20)); DrawHandle(context, new Point(top.X, top.Y - 24)); }
            if (ShowPivot) DrawHandle(context, Pivot);
        }
    }

    void DrawHandle(DrawingContext context, Point point)
    {
        var size = HandleSize; context.DrawRectangle(HandleBrush, new Pen(BorderBrush, 1), new Rect(point.X - size / 2, point.Y - size / 2, size, size));
    }

    static IEnumerable<Point> Handles(Rect r) => [new(r.Left, r.Top), new(r.Center.X, r.Top), new(r.Right, r.Top), new(r.Left, r.Center.Y), new(r.Right, r.Center.Y), new(r.Left, r.Bottom), new(r.Center.X, r.Bottom), new(r.Right, r.Bottom)];
}
