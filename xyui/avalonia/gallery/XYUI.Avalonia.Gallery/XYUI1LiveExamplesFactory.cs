using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1LiveExamplesFactory
{
    public static Control? Create(string id) => id switch
    {
        "XYUI-1-01" => CreateTextExamples(),
        "XYUI-1-02" => CreateLabelExamples(),
        "XYUI-1-03" => CreateCaptionExamples(),
        "XYUI-1-04" => CreateHeadingExamples(),
        "XYUI-1-05" => CreateSectionTitleExamples(),
        "XYUI-1-06" => CreateLinkExamples(),
        _ => null
    };

    static Control CreateTextExamples()
    {
        var panel = new StackPanel { Spacing = 12 };
        var s1 = new StackPanel { Spacing = 4 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · 正文阅读流与局部强调" });
        s1.Children.Add(new XYText { Text = "玄域引擎世界坐标系采用右手定则 (Z 轴向上)，网格导入时自动校验切线空间。" });
        var linkText = new XYText { Text = "技术状态：0x07AF · 依赖全部加载" };
        XY.SetForeground(linkText, "XY.Text.Link");
        s1.Children.Add(linkText);
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 4 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · 尺寸继承与禁用对比" });
        var sized = new StackPanel { Spacing = 4 };
        XY.SetSize(sized, XYSize.Comfortable);
        sized.Children.Add(new XYText { Text = "舒适大尺寸正文 (Comfortable Size)" });
        sized.Children.Add(new XYText { Text = "禁用的文本内容 (Disabled State)", IsEnabled = false });
        s2.Children.Add(sized);
        panel.Children.Add(s2);
        return panel;
    }

    static Control CreateLabelExamples()
    {
        var panel = new StackPanel { Spacing = 12 };
        var s1 = new StackPanel { Spacing = 4 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · 属性表单键值对排版 (Key-Value)" });
        var g1 = new Grid { RowDefinitions = new RowDefinitions("Auto,Auto"), ColumnDefinitions = new ColumnDefinitions("110,*") };
        var l1 = new XYLabel { Text = "实体标识 (ID)" }; var v1 = new XYText { Text = "entity_terrain_chunk_42" };
        Grid.SetRow(l1, 0); Grid.SetColumn(l1, 0); Grid.SetRow(v1, 0); Grid.SetColumn(v1, 1);
        var l2 = new XYLabel { Text = "采样精度" }; var v2 = new XYText { Text = "64 位双精度浮点" };
        Grid.SetRow(l2, 1); Grid.SetColumn(l2, 0); Grid.SetRow(v2, 1); Grid.SetColumn(v2, 1);
        g1.Children.AddRange(new[] { (Control)l1, v1, l2, v2 });
        s1.Children.Add(g1);
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 4 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · 锁定与禁用字段表达" });
        var g2 = new Grid { ColumnDefinitions = new ColumnDefinitions("110,*") };
        var l3 = new XYLabel { Text = "已锁定层级", IsEnabled = false };
        var v3 = new XYText { Text = "只读属性 (无法直接修改)", IsEnabled = false };
        Grid.SetColumn(l3, 0); Grid.SetColumn(v3, 1);
        g2.Children.AddRange(new[] { (Control)l3, v3 });
        s2.Children.Add(g2);
        panel.Children.Add(s2);
        return panel;
    }

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
