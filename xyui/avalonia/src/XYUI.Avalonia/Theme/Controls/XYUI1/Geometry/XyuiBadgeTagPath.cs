using Avalonia;
using Avalonia.Media;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Controls;

internal sealed class XyuiBadgeTagPath : VectorPath
{
    Size _arrangedSize = new(24, XYBadge.BadgeHeight);

    protected override Size ArrangeOverride(Size finalSize)
    {
        _arrangedSize = finalSize;
        InvalidateGeometry();
        return base.ArrangeOverride(finalSize);
    }

    protected override Geometry CreateDefiningGeometry()
    {
        var width = Math.Max(_arrangedSize.Width, XYBadge.PointerWidth + 1);
        var height = XYBadge.BadgeHeight;
        var geometry = new StreamGeometry();
        using var context = geometry.Open();
        context.BeginFigure(new Point(XYBadge.PointerTipInset, height / 2), true);
        context.LineTo(new Point(XYBadge.PointerWidth, 0));
        context.LineTo(new Point(width, 0));
        context.LineTo(new Point(width, height));
        context.LineTo(new Point(XYBadge.PointerWidth, height));
        context.EndFigure(true);
        return geometry;
    }
}
