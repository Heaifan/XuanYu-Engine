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
        var panel = new StackPanel { Spacing = 10 };
        var row1 = new StackPanel { Spacing = 4 };
        row1.Children.Add(new XYCaption { Text = "场景 1: 正文阅读流与 Foreground 覆盖" });
        row1.Children.Add(new XYText { Text = "玄域引擎世界坐标系采用右手定则 (Z 轴向上)，网格数据导入时自动校验切线空间。" });
        var linkText = new XYText { Text = "技术状态：0x07AF · 依赖全部加载" };
        XY.SetForeground(linkText, "XY.Text.Link");
        row1.Children.Add(linkText);
        panel.Children.Add(row1);

        var row2 = new StackPanel { Spacing = 4 };
        row2.Children.Add(new XYCaption { Text = "场景 2: 尺寸继承 (Comfortable) 与禁用对比" });
        var sizedStack = new StackPanel { Spacing = 4 };
        XY.SetSize(sizedStack, XYSize.Comfortable);
        sizedStack.Children.Add(new XYText { Text = "舒适大尺寸正文 (Comfortable Size)" });
        sizedStack.Children.Add(new XYText { Text = "禁用的文本内容 (Disabled State)", IsEnabled = false });
        row2.Children.Add(sizedStack);
        panel.Children.Add(row2);
        return panel;
    }

    static Control CreateLabelExamples()
    {
        var panel = new StackPanel { Spacing = 10 };
        panel.Children.Add(new XYCaption { Text = "场景: Inspector 表单字段名 (Key-Value) 与禁用对比" });
        var grid = new Grid { RowDefinitions = new RowDefinitions("Auto,Auto,Auto"), ColumnDefinitions = new ColumnDefinitions("110,*") };
        var l1 = new XYLabel { Text = "实体标识 (ID)" };
        var v1 = new XYText { Text = "entity_terrain_chunk_42" };
        Grid.SetRow(l1, 0); Grid.SetColumn(l1, 0); Grid.SetRow(v1, 0); Grid.SetColumn(v1, 1);
        var l2 = new XYLabel { Text = "采样精度" };
        var v2 = new XYText { Text = "64 位双精度浮点" };
        Grid.SetRow(l2, 1); Grid.SetColumn(l2, 0); Grid.SetRow(v2, 1); Grid.SetColumn(v2, 1);
        var l3 = new XYLabel { Text = "已锁定层级", IsEnabled = false };
        var v3 = new XYText { Text = "只读属性 (无法修改)", IsEnabled = false };
        Grid.SetRow(l3, 2); Grid.SetColumn(l3, 0); Grid.SetRow(v3, 2); Grid.SetColumn(v3, 1);
        grid.Children.AddRange(new[] { (Control)l1, v1, l2, v2, l3, v3 });
        panel.Children.Add(grid);
        return panel;
    }

    static Control CreateCaptionExamples()
    {
        var panel = new StackPanel { Spacing = 8 };
        panel.Children.Add(new XYCaption { Text = "场景 1: 时间戳与审计上下文" });
        panel.Children.Add(new XYCaption { Text = "上次修改于 2026-09-04 23:50 · 提交者: LeadArchitect" });
        panel.Children.Add(new XYCaption { Text = "场景 2: 表单下方辅助格式指引与单位" });
        var unitStack = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        unitStack.Children.Add(new XYText { Text = "视口剔除距离: 2500" });
        unitStack.Children.Add(new XYCaption { Text = "(以米为单位，超出范围将被裁切)" });
        panel.Children.Add(unitStack);
        return panel;
    }
}
