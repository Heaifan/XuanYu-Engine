using System;
using Avalonia;
using Avalonia.Controls;

namespace XYUI.Avalonia.Gallery.Views;

public class GalleryDoDontRow : Panel
{
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
            Children[0].Measure(new Size(availableSize.Width, double.PositiveInfinity));
            Children[1].Measure(new Size(availableSize.Width, double.PositiveInfinity));
            return new Size(availableSize.Width, Children[0].DesiredSize.Height + 8 + Children[1].DesiredSize.Height);
        }
        var halfWidth = Math.Max(0, (availableSize.Width - Spacing) / 2);
        Children[0].Measure(new Size(halfWidth, double.PositiveInfinity));
        Children[1].Measure(new Size(halfWidth, double.PositiveInfinity));
        return new Size(availableSize.Width, Math.Max(Children[0].DesiredSize.Height, Children[1].DesiredSize.Height));
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
            Children[0].Arrange(new Rect(0, 0, finalSize.Width, Children[0].DesiredSize.Height));
            Children[1].Arrange(new Rect(0, Children[0].DesiredSize.Height + 8, finalSize.Width, Children[1].DesiredSize.Height));
            return finalSize;
        }
        var halfWidth = Math.Max(0, (finalSize.Width - Spacing) / 2);
        Children[0].Arrange(new Rect(0, 0, halfWidth, finalSize.Height));
        Children[1].Arrange(new Rect(halfWidth + Spacing, 0, halfWidth, finalSize.Height));
        return finalSize;
    }
}

public class DoDontRow : GalleryDoDontRow {}
