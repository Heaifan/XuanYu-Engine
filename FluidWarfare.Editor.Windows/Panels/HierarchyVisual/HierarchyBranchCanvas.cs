using Avalonia;
using Avalonia.Media;
using Avalonia.Controls;

namespace FluidWarfare.Editor.Windows.Panels.HierarchyVisual;

/// <summary>
/// 自绘连续树干线控件。覆盖节点行左侧整个缩进区域。
/// 一次性绘制所有祖先竖线和当前折线，避免多 Border 拼线产生 1px 缝隙。
/// </summary>
public class HierarchyBranchCanvas : Control
{
    public static readonly StyledProperty<HierarchyBranchInfo?> BranchInfoProperty =
        AvaloniaProperty.Register<HierarchyBranchCanvas, HierarchyBranchInfo?>(nameof(BranchInfo));

    public static readonly StyledProperty<double> IndentProperty =
        AvaloniaProperty.Register<HierarchyBranchCanvas, double>(nameof(Indent), 18.0);

    public HierarchyBranchInfo? BranchInfo
    {
        get => GetValue(BranchInfoProperty);
        set => SetValue(BranchInfoProperty, value);
    }

    public double Indent
    {
        get => GetValue(IndentProperty);
        set => SetValue(IndentProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var info = BranchInfo;
        if (info is null || info.Depth <= 0) return;

        var pen = new Pen(new SolidColorBrush(Color.FromRgb(0x52, 0x60, 0x6D)), 1);
        var h = Bounds.Height;
        var halfIndent = Indent / 2;
        var branchCenter = halfIndent - 0.5;
        var lineLen = halfIndent - 1;

        // 祖先竖线：从行顶贯穿到底
        for (var d = 0; d < info.Depth; d++)
        {
            if (d < info.AncestorHasNextSibling.Length && info.AncestorHasNextSibling[d])
            {
                var x = d * Indent + branchCenter;
                context.DrawLine(pen, new Point(x, 0), new Point(x, h));
            }
        }

        // 当前节点折线
        var cx = (info.Depth - 1) * Indent + branchCenter;
        var cy = h / 2;

        // 竖线段
        if (info.IsLastSibling)
            context.DrawLine(pen, new Point(cx, 0), new Point(cx, cy)); // └ 从顶到中心
        else
            context.DrawLine(pen, new Point(cx, 0), new Point(cx, h)); // ├ 从顶到底

        // 横线段
        context.DrawLine(pen, new Point(cx, cy), new Point(cx + lineLen, cy));
    }
}
