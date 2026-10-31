using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1LiveExamplesFactory
{
    static Control CreateSelectableTextExamples()
    {
        var panel = new StackPanel { Spacing = 14 };

        var s1 = new StackPanel { Spacing = 6 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · 实体 GUID 与诊断哈希 (支持划选与一键复制)" });
        var col1 = new StackPanel { Spacing = 6 };
        col1.Children.Add(new XYLabel { Text = "构建管线 Hash (Technical Variant · 选区 0~8)" });
        col1.Children.Add(new XYSelectableText { Text = "7f12a8d4c92b8e4f1a603c9d", Variant = XyuiSelectableTextVariant.Technical, SelectionStart = 0, SelectionEnd = 8 });
        col1.Children.Add(new XYLabel { Text = "实体名称 (Default Variant · 可点击复制)" });
        col1.Children.Add(new XYSelectableText { Text = "entity_terrain_sector_42" });
        s1.Children.Add(col1);
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 6 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · 禁用态可选择文本 (IsEnabled=False)" });
        var disText = new XYSelectableText { Text = "archived_object_reference_id_000", IsEnabled = false };
        s2.Children.Add(disText);
        panel.Children.Add(s2);

        return panel;
    }

    static Control CreateEmptyTextExamples()
    {
        var panel = new StackPanel { Spacing = 14 };

        var s1 = new StackPanel { Spacing = 6 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · 空面板与无选中对象提示 (纯文本反馈)" });
        var box1 = new Border
        {
            Background = new global::Avalonia.Media.SolidColorBrush(global::Avalonia.Media.Color.FromArgb(16, 128, 128, 128)),
            Padding = new global::Avalonia.Thickness(16),
            CornerRadius = new global::Avalonia.CornerRadius(4),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Child = new XYEmptyText { Text = "未选中任何游戏对象。请在场景层级树中选择一个实体。" }
        };
        s1.Children.Add(box1);
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 6 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · 搜索无匹配结果占位" });
        var box2 = new Border
        {
            Background = new global::Avalonia.Media.SolidColorBrush(global::Avalonia.Media.Color.FromArgb(16, 128, 128, 128)),
            Padding = new global::Avalonia.Thickness(16),
            CornerRadius = new global::Avalonia.CornerRadius(4),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Child = new XYEmptyText { Text = "未找到符合过滤条件的着色器资源。" }
        };
        s2.Children.Add(box2);
        panel.Children.Add(s2);

        return panel;
    }
}
