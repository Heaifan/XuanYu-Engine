using Avalonia;
using Avalonia.Media;
using XYUI.Avalonia.Vector;
using AvaloniaVector = Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYIcon
{
    public override void Render(DrawingContext context)
    {
        if (IconGeometry is null || Bounds.Width <= 0 || Bounds.Height <= 0) return;
        var scale = Math.Min(Bounds.Width, Bounds.Height) / XyuiVectorIcons.LogicalIconSize;
        var offset = new AvaloniaVector((Bounds.Width - XyuiVectorIcons.LogicalIconSize * scale) / 2,
            (Bounds.Height - XyuiVectorIcons.LogicalIconSize * scale) / 2);
        using (context.PushTransform(Matrix.CreateTranslation(offset)))
        using (context.PushTransform(Matrix.CreateScale(scale, scale)))
        {
            var pen = Stroke is null ? null : new Pen(Stroke, StrokeThickness / scale);
            context.DrawGeometry(Fill, pen, IconGeometry);
        }
    }
}
