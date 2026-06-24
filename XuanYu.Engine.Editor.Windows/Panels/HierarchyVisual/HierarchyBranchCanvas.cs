using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace FluidWarfare.Editor.Windows.Panels.HierarchyVisual;

/// <summary>
/// 绘制经典文件树的连续虚线树干。
/// 每行自行绘制祖先竖线和当前折线，不创建额外 Border。
/// </summary>
public sealed class HierarchyBranchCanvas : Control
{
    public static readonly StyledProperty<HierarchyBranchInfo?> BranchInfoProperty =
        AvaloniaProperty.Register<HierarchyBranchCanvas, HierarchyBranchInfo?>(
            nameof(BranchInfo));

    public static readonly StyledProperty<double> IndentProperty =
        AvaloniaProperty.Register<HierarchyBranchCanvas, double>(
            nameof(Indent),
            18.0);

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
        if (info is null || info.Depth <= 0 || Bounds.Height <= 0)
            return;

        var brush = new SolidColorBrush(Color.FromRgb(0x62, 0x6F, 0x7C));
        var dash = new DashStyle([1.0, 2.0], 0);
        var pen = new Pen(brush, 1.0, dashStyle: dash);

        DrawAncestorLines(context, pen, info);
        DrawCurrentBranch(context, pen, info);
    }

    private void DrawAncestorLines(
        DrawingContext context,
        Pen pen,
        HierarchyBranchInfo info)
    {
        for (var level = 1; level < info.Depth; level++)
        {
            if (level >= info.AncestorHasNextSibling.Length ||
                !info.AncestorHasNextSibling[level])
            {
                continue;
            }

            var x = PixelSnap((level - 1) * Indent + Indent / 2);
            context.DrawLine(pen, new Point(x, 0), new Point(x, Bounds.Height));
        }
    }

    private void DrawCurrentBranch(
        DrawingContext context,
        Pen pen,
        HierarchyBranchInfo info)
    {
        var x = PixelSnap((info.Depth - 1) * Indent + Indent / 2);
        var y = PixelSnap(Bounds.Height / 2);
        var endX = PixelSnap(info.Depth * Indent);

        var verticalEnd = info.IsLastSibling ? y : Bounds.Height;
        context.DrawLine(pen, new Point(x, 0), new Point(x, verticalEnd));
        context.DrawLine(pen, new Point(x, y), new Point(endX, y));
    }

    private static double PixelSnap(double value) =>
        Math.Floor(value) + 0.5;
}
