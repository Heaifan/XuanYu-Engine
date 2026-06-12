using Avalonia;
using Avalonia.Media;
using Avalonia.Controls;

namespace FluidWarfare.Editor.Windows.Panels.HierarchyVisual;

/// <summary>
/// 树干分支线绘制控件。不创建额外子控件，直接在 OnRender 中绘制直线。
/// 同时供世界树和项目内容树使用。
/// </summary>
public class HierarchyBranchGuide : Control
{
    public static readonly StyledProperty<HierarchyBranchInfo?> BranchInfoProperty =
        AvaloniaProperty.Register<HierarchyBranchGuide, HierarchyBranchInfo?>(nameof(BranchInfo));

    public static readonly StyledProperty<double> IndentProperty =
        AvaloniaProperty.Register<HierarchyBranchGuide, double>(nameof(Indent), 16.0);

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
        var halfIndent = Indent / 2;
        var branchCenter = halfIndent;
        var lineLen = halfIndent;

        // 画祖先竖线
        for (var d = 0; d < info.Depth; d++)
        {
            var x = d * Indent + branchCenter;
            if (d < info.AncestorHasNextSibling.Length && info.AncestorHasNextSibling[d])
            {
                context.DrawLine(pen, new Point(x, 0), new Point(x, Bounds.Height));
            }
        }

        // 当前节点横线
        var cx = info.Depth * Indent;
        var cy = Bounds.Height / 2;

        if (info.IsLastSibling)
        {
            // └─
            context.DrawLine(pen, new Point(cx + branchCenter, cy), new Point(cx + branchCenter + lineLen, cy));
            context.DrawLine(pen, new Point(cx + branchCenter, cy), new Point(cx + branchCenter, Bounds.Height));
        }
        else
        {
            // ├─
            context.DrawLine(pen, new Point(cx + branchCenter, 0), new Point(cx + branchCenter, Bounds.Height));
            context.DrawLine(pen, new Point(cx + branchCenter, cy), new Point(cx + branchCenter + lineLen, cy));
        }
    }
}
