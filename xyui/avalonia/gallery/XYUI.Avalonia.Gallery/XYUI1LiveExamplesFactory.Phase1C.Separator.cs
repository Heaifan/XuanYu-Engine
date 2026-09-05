using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1LiveExamplesFactory
{
    static Control CreateSeparatorExamples()
    {
        var panel = new StackPanel { Spacing = 14 };

        var s1 = new StackPanel { Spacing = 6 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · Inspector 属性面板分组 (Section / Panel)" });
        var box1 = new Border { Background = new global::Avalonia.Media.SolidColorBrush(global::Avalonia.Media.Color.FromArgb(16, 128, 128, 128)), Padding = new global::Avalonia.Thickness(12, 8), CornerRadius = new global::Avalonia.CornerRadius(4) };
        var inner1 = new StackPanel { Spacing = 6 };
        inner1.Children.Add(new XYLabel { Text = "Transform 变换参数" });
        inner1.Children.Add(new XYSeparator { Variant = XyuiSeparatorVariant.Section });
        inner1.Children.Add(new XYText { Text = "Position: (0.0, 12.5, -4.0)" });
        inner1.Children.Add(new XYSeparator { Variant = XyuiSeparatorVariant.Section });
        inner1.Children.Add(new XYText { Text = "Rotation: (0.0, 0.0, 0.0)" });
        box1.Child = inner1;
        s1.Children.Add(box1);
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 6 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · Toolbar 工具栏垂直分割 (VerticalSplit)" });
        var tb = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Height = 28 };
        tb.Children.Add(new XYText { Text = "选择模式", VerticalAlignment = VerticalAlignment.Center });
        tb.Children.Add(new XYText { Text = "移动", VerticalAlignment = VerticalAlignment.Center });
        tb.Children.Add(new XYText { Text = "旋转", VerticalAlignment = VerticalAlignment.Center });
        tb.Children.Add(new XYSeparator { Variant = XyuiSeparatorVariant.VerticalSplit });
        tb.Children.Add(new XYText { Text = "局部坐标", VerticalAlignment = VerticalAlignment.Center });
        tb.Children.Add(new XYText { Text = "世界坐标", VerticalAlignment = VerticalAlignment.Center });
        s2.Children.Add(tb);
        panel.Children.Add(s2);

        var s3 = new StackPanel { Spacing = 6 };
        s3.Children.Add(new XYCaption { Text = "场景 3 · 列表行项分割 (ListRow Inset 16 DIP)" });
        var list = new StackPanel { Spacing = 4 };
        list.Children.Add(new XYText { Text = "资产 1：Textures/Environment/skybox_hdr" });
        list.Children.Add(new XYSeparator { Variant = XyuiSeparatorVariant.ListRow });
        list.Children.Add(new XYText { Text = "资产 2：Models/Characters/hero_mesh" });
        list.Children.Add(new XYSeparator { Variant = XyuiSeparatorVariant.ListRow });
        list.Children.Add(new XYText { Text = "资产 3：Shaders/PBR/standard_metallic" });
        s3.Children.Add(list);
        panel.Children.Add(s3);

        return panel;
    }
}
