using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI4GalleryCatalog
{
    static Control SelectionOutlinePreview() => new StackPanel
    {
        Spacing = 8,
        Children = { Outline("Selected region"), new TextBlock { Text = "双层轮廓用于把选择结果与对象原始 Fill 分离。" } }
    };

    static Control SelectionOutlineLiveExample() => new StackPanel
    {
        Spacing = 8,
        Children = { Outline("Selection result"), new TextBlock { Text = "不使用 Glow，不改变对象布局或原始填充。" } }
    };

    static XYSelectionOutline Outline(string label) => new()
    {
        Width = 260, Height = 92,
        Points = new[] { new Point(28, 26), new Point(76, 12), new Point(214, 20), new Point(238, 66), new Point(92, 80), new Point(28, 58) }
    };
}
