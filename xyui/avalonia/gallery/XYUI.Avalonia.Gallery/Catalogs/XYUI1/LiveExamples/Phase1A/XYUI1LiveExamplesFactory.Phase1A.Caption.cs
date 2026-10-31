using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1LiveExamplesFactory
{
    static Control CreateCaptionExamples()
    {
        var panel = new StackPanel { Spacing = 12 };
        var s1 = new StackPanel { Spacing = 4 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · 时间戳与审计上下文" });
        s1.Children.Add(new XYCaption { Text = "上次修改于 2026-09-04 23:50 · 提交者: LeadArchitect" });
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 4 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · 表单字段下方辅助指引与度量单位" });
        var unitStack = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        unitStack.Children.Add(new XYText { Text = "视口剔除距离: 2500" });
        unitStack.Children.Add(new XYCaption { Text = "(以米为单位，超出范围将被裁切)" });
        s2.Children.Add(unitStack);
        panel.Children.Add(s2);
        return panel;
    }
}
