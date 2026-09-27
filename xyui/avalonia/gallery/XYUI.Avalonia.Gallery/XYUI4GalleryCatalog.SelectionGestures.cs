using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI4GalleryCatalog
{
    static Control MarqueePreview() => new StackPanel
    {
        Spacing = 8,
        Children = { Marquee("Window selection", false), Marquee("Crossing selection", true),
            new TextBlock { Text = "框选只表达候选范围；结果仍由 Selection Set 决定。" } }
    };

    static Control MarqueeLiveExample() => new StackPanel
    {
        Spacing = 8,
        Children = { Marquee("包含模式", false), Marquee("穿越模式", true),
            new TextBlock { Text = "Fill opacity 仍保留 XYUI4-GAP-003，不在组件内硬编码。" } }
    };

    static XYMarqueeSelection Marquee(string label, bool crossing) => new()
    {
        Width = 260, Height = 48, IsCrossing = crossing
    };

    static Control LassoPreview() => new StackPanel
    {
        Spacing = 8,
        Children = { Lasso("Lasso candidate"),
            new TextBlock { Text = "低平滑度闭合路径，起点与终点用于候选区域命中。" } }
    };

    static Control LassoLiveExample() => new StackPanel
    {
        Spacing = 8,
        Children = { Lasso("Lasso live"), new TextBlock { Text = "Lasso 只负责路径呈现，不直接修改业务选择。" } }
    };

    static XYLassoSelection Lasso(string label) => new()
    {
        Width = 260, Height = 92,
        Points = new[] { new Point(20, 42), new Point(46, 16), new Point(112, 10), new Point(206, 28), new Point(232, 68), new Point(146, 78), new Point(54, 70) }
    };
}
