using System;
using Avalonia;
using Avalonia.Controls;

namespace XYUI.Avalonia.Gallery.Views;

public class GalleryVariantRow : Panel
{
    public double NameWidth { get; set; } = 180;
    public double Spacing { get; set; } = 12;
    public double Breakpoint { get; set; } = 520;

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Children.Count == 0) return new Size(0, 0);
        if (Children.Count == 1)
        {
            Children[0].Measure(availableSize);
            return Children[0].DesiredSize;
        }
        if (availableSize.Width < Breakpoint)
        {
            double totalH = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Measure(new Size(availableSize.Width, double.PositiveInfinity));
                totalH += Children[i].DesiredSize.Height;
                if (i > 0) totalH += 4;
            }
            return new Size(availableSize.Width, totalH);
        }
        Children[0].Measure(new Size(NameWidth, double.PositiveInfinity));
        if (Children.Count == 2)
        {
            var cw = Math.Max(0, availableSize.Width - NameWidth - Spacing);
            Children[1].Measure(new Size(cw, double.PositiveInfinity));
            return new Size(availableSize.Width, Math.Max(Children[0].DesiredSize.Height, Children[1].DesiredSize.Height));
        }
        var contentWidth = Math.Max(0, availableSize.Width - NameWidth - (Children.Count - 1) * Spacing);
        var colWidth = contentWidth / (Children.Count - 1);
        double maxH = Children[0].DesiredSize.Height;
        for (int i = 1; i < Children.Count; i++)
        {
            Children[i].Measure(new Size(colWidth, double.PositiveInfinity));
            maxH = Math.Max(maxH, Children[i].DesiredSize.Height);
        }
        return new Size(availableSize.Width, maxH);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Children.Count == 0) return finalSize;
        if (Children.Count == 1)
        {
            Children[0].Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            return finalSize;
        }
        if (finalSize.Width < Breakpoint)
        {
            double y = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                var h = Children[i].DesiredSize.Height;
                Children[i].Arrange(new Rect(0, y, finalSize.Width, h));
                y += h + 4;
            }
            return finalSize;
        }
        Children[0].Arrange(new Rect(0, 0, NameWidth, finalSize.Height));
        var contentWidth = Math.Max(0, finalSize.Width - NameWidth - (Children.Count - 1) * Spacing);
        var colWidth = contentWidth / (Children.Count - 1);
        double x = NameWidth + Spacing;
        for (int i = 1; i < Children.Count; i++)
        {
            Children[i].Arrange(new Rect(x, 0, colWidth, finalSize.Height));
            x += colWidth + Spacing;
        }
        return finalSize;
    }
}

public class VariantRow : GalleryVariantRow {}
