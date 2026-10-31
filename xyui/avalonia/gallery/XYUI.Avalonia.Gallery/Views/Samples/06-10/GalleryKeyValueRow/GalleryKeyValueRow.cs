using System;
using Avalonia;
using Avalonia.Controls;

namespace XYUI.Avalonia.Gallery.Views;

public class GalleryKeyValueRow : Panel
{
    public double KeyWidth { get; set; } = 210;
    public double Spacing { get; set; } = 16;
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
            return new Size(availableSize.Width, Children[0].DesiredSize.Height + 6 + Children[1].DesiredSize.Height);
        }
        Children[0].Measure(new Size(KeyWidth, double.PositiveInfinity));
        var contentWidth = Math.Max(0, availableSize.Width - KeyWidth - Spacing);
        Children[1].Measure(new Size(contentWidth, double.PositiveInfinity));
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
            Children[1].Arrange(new Rect(0, Children[0].DesiredSize.Height + 6, finalSize.Width, Children[1].DesiredSize.Height));
            return finalSize;
        }
        Children[0].Arrange(new Rect(0, 0, KeyWidth, Children[0].DesiredSize.Height));
        var contentWidth = Math.Max(0, finalSize.Width - KeyWidth - Spacing);
        Children[1].Arrange(new Rect(KeyWidth + Spacing, 0, contentWidth, Children[1].DesiredSize.Height));
        return finalSize;
    }
}

public class GalleryRuleRow : GalleryKeyValueRow
{
    public GalleryRuleRow() { KeyWidth = 210; Spacing = 16; Breakpoint = 520; }
}

public class GalleryStateRow : GalleryKeyValueRow
{
    public GalleryStateRow() { KeyWidth = 180; Spacing = 16; Breakpoint = 520; }
}

public class RuleRow : GalleryRuleRow {}
public class StateRow : GalleryStateRow {}
public class KeyValueRow : GalleryKeyValueRow {}
