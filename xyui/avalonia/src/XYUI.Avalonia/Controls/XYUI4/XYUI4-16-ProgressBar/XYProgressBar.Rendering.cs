using Avalonia;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYProgressBar
{
    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var width = Bounds.Width; var height = Math.Min(Bounds.Height, HeightFor(Size));
        if (width <= 0 || height <= 0 || Track is null || Fill is null) return;
        var radius = height / 2; var y = (Bounds.Height - height) / 2;
        var track = new Rect(0, y, width, height);
        context.DrawRectangle(Track, null, track, radius, radius);
        using (context.PushClip(track))
        {
            var fill = IsIndeterminate ? IndeterminateRect(width, height, y) : new Rect(0, y, width * ProgressFraction, height);
            if (fill.Width > 0) context.DrawRectangle(Fill, null, fill, radius, radius);
        }
    }

    Rect IndeterminateRect(double width, double height, double y)
    {
        var segment = Math.Max(24, width * 0.28);
        return new Rect((width + segment) * _indeterminateOffset - segment, y, segment, height);
    }
}
