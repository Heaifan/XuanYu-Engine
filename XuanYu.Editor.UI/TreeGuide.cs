using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace XuanYu.Editor.UI;

public sealed class TreeGuide : Control
{
    public static readonly StyledProperty<IReadOnlyList<TreeGuideSegment>> SegmentsProperty =
        AvaloniaProperty.Register<TreeGuide, IReadOnlyList<TreeGuideSegment>>(nameof(Segments), []);

    public IReadOnlyList<TreeGuideSegment> Segments
    {
        get => GetValue(SegmentsProperty);
        set => SetValue(SegmentsProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var count = Math.Max(1, Segments.Count);
        return new Size(count * 20, 28);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var pen = new Pen(new SolidColorBrush(Color.Parse("#C7D7EA")), 1);
        foreach (var segment in Segments)
        {
            var x = segment.Depth * 20 + 10;
            var mid = Bounds.Height / 2;
            var right = segment.Depth * 20 + 20;
            switch (segment.Kind)
            {
                case TreeGuideSegmentKind.Full:
                    context.DrawLine(pen, new Point(x, 0), new Point(x, Bounds.Height));
                    break;
                case TreeGuideSegmentKind.Tee:
                    context.DrawLine(pen, new Point(x, 0), new Point(x, Bounds.Height));
                    context.DrawLine(pen, new Point(x, mid), new Point(right, mid));
                    break;
                case TreeGuideSegmentKind.Elbow:
                    context.DrawLine(pen, new Point(x, 0), new Point(x, mid));
                    context.DrawLine(pen, new Point(x, mid), new Point(right, mid));
                    break;
            }
        }
    }
}
