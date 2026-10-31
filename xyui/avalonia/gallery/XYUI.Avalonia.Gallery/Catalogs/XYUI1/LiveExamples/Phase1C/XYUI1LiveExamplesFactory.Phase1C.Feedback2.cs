using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1LiveExamplesFactory
{
    static Control CreateWarningTextExamples()
    {
        var panel = new StackPanel { Spacing = 14 };

        var s1 = new StackPanel { Spacing = 6 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · 潜在性能损耗与风险预警 (Normal)" });
        var row1 = new StackPanel { Spacing = 4 };
        row1.Children.Add(new XYLabel { Text = "实时全局光照质量" });
        row1.Children.Add(new Border { Background = new global::Avalonia.Media.SolidColorBrush(global::Avalonia.Media.Color.FromArgb(16, 128, 128, 128)), Padding = new global::Avalonia.Thickness(8, 4), CornerRadius = new global::Avalonia.CornerRadius(4), Child = new XYText { Text = "Ultra (Path Tracing) ▾" } });
        row1.Children.Add(new XYWarningText { Text = "可能显著增加显存与 GPU 算力开销，需确认目标机型配置。" });
        s1.Children.Add(row1);
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 6 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · 未保存提醒与禁用态对比" });
        var row2 = new StackPanel { Spacing = 4 };
        row2.Children.Add(new XYWarningText { Text = "材质实例属性已变更，尚未保存到资产磁盘文件。" });
        row2.Children.Add(new XYWarningText { Text = "已被临时屏蔽的次级警告提示 (Disabled State)。", IsEnabled = false });
        s2.Children.Add(row2);
        panel.Children.Add(s2);

        return panel;
    }

    static Control CreateShortcutHintExamples()
    {
        var panel = new StackPanel { Spacing = 14 };

        var s1 = new StackPanel { Spacing = 6 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · 单键、双键与三键键盘组合展示" });
        var g1 = new Grid { RowDefinitions = new RowDefinitions("Auto,Auto,Auto,Auto"), ColumnDefinitions = new ColumnDefinitions("160,Auto") };

        var c0 = new XYText { Text = "重命名 (Rename)" }; var k0 = new XYShortcutHint { Shortcut = "F2" };
        Grid.SetRow(c0, 0); Grid.SetColumn(c0, 0); Grid.SetRow(k0, 0); Grid.SetColumn(k0, 1);
        var c1 = new XYText { Text = "保存工程 (Save)" }; var k1 = new XYShortcutHint { Shortcut = "Ctrl+S" };
        Grid.SetRow(c1, 1); Grid.SetColumn(c1, 0); Grid.SetRow(k1, 1); Grid.SetColumn(k1, 1);
        var c2 = new XYText { Text = "复制副本 (Duplicate)" }; var k2 = new XYShortcutHint { Shortcut = "Ctrl+D" };
        Grid.SetRow(c2, 2); Grid.SetColumn(c2, 0); Grid.SetRow(k2, 2); Grid.SetColumn(k2, 1);
        var c3 = new XYText { Text = "命令面板 (Palette)" }; var k3 = new XYShortcutHint { Shortcut = "Ctrl+Shift+P" };
        Grid.SetRow(c3, 3); Grid.SetColumn(c3, 0); Grid.SetRow(k3, 3); Grid.SetColumn(k3, 1);

        g1.Children.AddRange(new[] { (Control)c0, k0, c1, k1, c2, k2, c3, k3 });
        s1.Children.Add(g1);
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 6 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · 禁用态快捷键 (IsEnabled=False)" });
        var row2 = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 16 };
        row2.Children.Add(new XYText { Text = "不可用命令 (Disabled Action)", VerticalAlignment = VerticalAlignment.Center });
        row2.Children.Add(new XYShortcutHint { Shortcut = "Ctrl+Shift+Z", IsEnabled = false });
        s2.Children.Add(row2);
        panel.Children.Add(s2);

        return panel;
    }
}
