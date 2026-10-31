using System;
using Avalonia;
using Avalonia.Controls;

namespace XYUI.Avalonia.Gallery.Views;

public class GalleryTokenRow : Panel
{
    public double TokenWidth { get; set; } = 200;
    public double ValueWidth { get; set; } = 140;
    public double Spacing { get; set; } = 12;
    public double Breakpoint { get; set; } = 520;

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Children.Count == 0) return new Size(0, 0);
        if (availableSize.Width < Breakpoint || Children.Count < 3)
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
        Children[0].Measure(new Size(TokenWidth, double.PositiveInfinity));
        Children[1].Measure(new Size(ValueWidth, double.PositiveInfinity));
        var rem = Math.Max(0, availableSize.Width - TokenWidth - ValueWidth - 2 * Spacing);
        Children[2].Measure(new Size(rem, double.PositiveInfinity));
        var maxH = Math.Max(Children[0].DesiredSize.Height, Math.Max(Children[1].DesiredSize.Height, Children[2].DesiredSize.Height));
        return new Size(availableSize.Width, maxH);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Children.Count == 0) return finalSize;
        if (finalSize.Width < Breakpoint || Children.Count < 3)
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
        Children[0].Arrange(new Rect(0, 0, TokenWidth, finalSize.Height));
        Children[1].Arrange(new Rect(TokenWidth + Spacing, 0, ValueWidth, finalSize.Height));
        var rem = Math.Max(0, finalSize.Width - TokenWidth - ValueWidth - 2 * Spacing);
        Children[2].Arrange(new Rect(TokenWidth + ValueWidth + 2 * Spacing, 0, rem, finalSize.Height));
        return finalSize;
    }
}

public class TokenRow : GalleryTokenRow {}
