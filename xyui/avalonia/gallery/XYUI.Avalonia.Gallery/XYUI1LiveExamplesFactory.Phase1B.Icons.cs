using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1LiveExamplesFactory
{
    static Control CreateIconExamples()
    {
        var panel = new StackPanel { Spacing = 12 };
        var s1 = new StackPanel { Spacing = 4 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · 尺寸阶梯与线宽联动 (Compact / Default / Comfortable / Touch)" });
        var row1 = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 16, VerticalAlignment = VerticalAlignment.Center };
        row1.Children.Add(new XYIcon { Icon = XyuiVectorIcon.Search, Size = XyuiIconSize.Compact });
        row1.Children.Add(new XYIcon { Icon = XyuiVectorIcon.Search, Size = XyuiIconSize.Default });
        row1.Children.Add(new XYIcon { Icon = XyuiVectorIcon.Search, Size = XyuiIconSize.Comfortable });
        row1.Children.Add(new XYIcon { Icon = XyuiVectorIcon.Search, Size = XyuiIconSize.Touch });
        s1.Children.Add(row1);
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 4 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · 显式覆盖与禁用对比 (Explicit Size > Inherited)" });
        var row2 = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 16, VerticalAlignment = VerticalAlignment.Center };
        var inherited = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, VerticalAlignment = VerticalAlignment.Center };
        XY.SetSize(inherited, XYSize.Comfortable);
        inherited.Children.Add(new XYIcon { Icon = XyuiVectorIcon.Locate });
        inherited.Children.Add(new XYIcon { Icon = XyuiVectorIcon.Locate, Size = XyuiIconSize.Small });
        inherited.Children.Add(new XYIcon { Icon = XyuiVectorIcon.Locate, IsEnabled = false });
        row2.Children.Add(inherited);
        s2.Children.Add(row2);
        panel.Children.Add(s2);
        return panel;
    }

    static Control CreateIconLabelExamples()
    {
        var panel = new StackPanel { Spacing = 12 };
        var s1 = new StackPanel { Spacing = 4 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · 场景与地形资产项 (Icon + Text 垂直居中)" });
        var row1 = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 16 };
        row1.Children.Add(new XYIconLabel { Icon = XyuiVectorIcon.Browse, Label = "Scene" });
        row1.Children.Add(new XYIconLabel { Icon = XyuiVectorIcon.Tag, Label = "Terrain" });
        row1.Children.Add(new XYIconLabel { Icon = XyuiVectorIcon.Eye, Label = "Lighting" });
        s1.Children.Add(row1);
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 4 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · 缓存与区域标记 (Disabled 全状态联动)" });
        var row2 = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 16 };
        row2.Children.Add(new XYIconLabel { Icon = XyuiVectorIcon.Copy, Label = "Local Cache" });
        row2.Children.Add(new XYIconLabel { Icon = XyuiVectorIcon.Info, Label = "Region" });
        row2.Children.Add(new XYIconLabel { Icon = XyuiVectorIcon.Locate, Label = "Disabled Chunk", IsEnabled = false });
        s2.Children.Add(row2);
        panel.Children.Add(s2);
        return panel;
    }
}
