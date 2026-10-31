using Avalonia;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYSpinner
{
    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var side = Math.Min(Bounds.Width, Bounds.Height);
        if (side <= 0 || Track is null || Arc is null) return;
        var stroke = StrokeFor(Size);
        var radius = Math.Max(1d, side / 2d - stroke / 2d);
        var center = new Point(Bounds.Width / 2d, Bounds.Height / 2d);
        context.DrawEllipse(null, new Pen(Track, stroke), center, radius, radius);
        using (context.PushTransform(Matrix.CreateTranslation(center.X, center.Y)))
        using (context.PushTransform(Matrix.CreateRotation(_angle * Math.PI / 180d)))
        using (context.PushTransform(Matrix.CreateScale(radius, radius)))
            context.DrawGeometry(null, new Pen(Arc, stroke / radius), ArcGeometry);
    }
}
