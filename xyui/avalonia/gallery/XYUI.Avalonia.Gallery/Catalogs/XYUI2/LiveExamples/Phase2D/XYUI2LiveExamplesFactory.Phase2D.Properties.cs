using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2LiveExamplesFactory
{
    static Control NumberPropertyExamples()
    {
        var col1 = new StackPanel { Spacing = 8, Width = 540, HorizontalAlignment = HorizontalAlignment.Left };
        col1.Children.Add(new XYNumberProperty { Width = 520, Label = "最大速度", Value = 8.42, Minimum = 0, Maximum = 100, Step = .1, DecimalPlaces = 2, Suffix = "m/s" });
        col1.Children.Add(new XYNumberProperty { Width = 520, Label = "质量", Value = 12.50, Minimum = 0, Maximum = 1000, Step = .5, DecimalPlaces = 2, Suffix = "kg" });
        col1.Children.Add(new XYNumberProperty { Width = 520, Label = "透明度", Value = 55, Minimum = 0, Maximum = 100, Step = 1, DecimalPlaces = 0, Suffix = "%" });

        var col2 = new StackPanel { Spacing = 8, Width = 540, HorizontalAlignment = HorizontalAlignment.Left };
        col2.Children.Add(new XYNumberProperty { Width = 520, Label = "微调参数 (可拖拽标签)", Value = 4.5, Minimum = 0, Maximum = 10, Step = .1, DecimalPlaces = 1 });
        col2.Children.Add(new XYNumberProperty { Width = 520, Label = "只读数值属性", Value = 12, IsReadOnly = true, Suffix = "px" });
        col2.Children.Add(new XYNumberProperty { Width = 520, Label = "禁用数值属性", Value = 0, IsEnabled = false });

        return SceneHost(
            Scene("场景 1 · 核心数值编辑 (复用 XYNumberField，支持步进按钮与键盘上下键)", col1),
            Scene("场景 2 · 标签微调与状态保护 (按住 Label 水平拖动无极调整数值)", col2));
    }

    static Control VectorPropertyExamples()
    {
        var col1 = new StackPanel { Spacing = 8, Width = 640, HorizontalAlignment = HorizontalAlignment.Left };
        col1.Children.Add(new XYVectorProperty { Width = 620, Label = "Auto · 宽屏 Vector3", Dimension = XYVectorDimension.Vector3, Layout = XYVectorPropertyLayout.Auto, X = 12.5, Y = 0, Z = -4.8, Step = .1, DecimalPlaces = 2 });
        col1.Children.Add(new XYVectorProperty { Width = 280, Label = "Auto · 紧凑 Vector3", Dimension = XYVectorDimension.Vector3, Layout = XYVectorPropertyLayout.Auto, X = 0, Y = 90, Z = 0, Step = 1, DecimalPlaces = 0 });
        col1.Children.Add(new XYVectorProperty { Width = 300, Label = "Inline · 紧凑 Vector3", Dimension = XYVectorDimension.Vector3, Layout = XYVectorPropertyLayout.Inline, X = 0, Y = -1.42, Z = 0, Step = .1, DecimalPlaces = 2 });

        var col2 = new StackPanel { Spacing = 8, Width = 640, HorizontalAlignment = HorizontalAlignment.Left };
        col2.Children.Add(new XYVectorProperty { Width = 620, Label = "Stacked · 宽屏 Vector3", Dimension = XYVectorDimension.Vector3, Layout = XYVectorPropertyLayout.Stacked, X = 1, Y = 0, Z = 0, Step = .1, DecimalPlaces = 2 });
        var areaDGroup = new StackPanel { Spacing = 8 };
        XY.SetDensity(areaDGroup, XYDensity.Compact);
        areaDGroup.Children.Add(new XYVectorProperty { Width = 300, Label = "Area D · Position", Dimension = XYVectorDimension.Vector3, Layout = XYVectorPropertyLayout.Inline, X = 0, Y = -1.42, Z = 0, Step = .1, DecimalPlaces = 2 });
        areaDGroup.Children.Add(new XYVectorProperty { Width = 300, Label = "Area D · Rotation", Dimension = XYVectorDimension.Vector3, Layout = XYVectorPropertyLayout.Inline, X = 0, Y = 0, Z = 0, Step = .1, DecimalPlaces = 2 });
        areaDGroup.Children.Add(new XYVectorProperty { Width = 300, Label = "Area D · Scale", Dimension = XYVectorDimension.Vector3, Layout = XYVectorPropertyLayout.Inline, X = 1, Y = 1, Z = 1, Step = .1, DecimalPlaces = 2 });
        col2.Children.Add(areaDGroup);

        return SceneHost(
            Scene("布局策略 · Auto / Inline (各轴独立复用 XYNumberField，独立编辑与 Scrub)", col1),
            Scene("布局策略 · Stacked / XuanYu Editor Area D Transform Inspector", col2));
    }
}
