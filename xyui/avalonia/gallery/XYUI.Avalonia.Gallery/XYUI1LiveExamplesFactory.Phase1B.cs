using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1LiveExamplesFactory
{
    static Control CreateCodeTextExamples()
    {
        var panel = new StackPanel { Spacing = 12 };
        var s1 = new StackPanel { Spacing = 4 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · 实体 ID 与技术键名" });
        var row1 = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12 };
        row1.Children.Add(new XYCodeText { Text = "entity_terrain_chunk_42" });
        row1.Children.Add(new XYCodeText { Text = "World.RegionKey" });
        s1.Children.Add(row1);
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 4 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · 资源路径与技术标识" });
        var row2 = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12 };
        row2.Children.Add(new XYCodeText { Text = "Assets/Maps/Asia.json" });
        row2.Children.Add(new XYCodeText { Text = "terrain/main-heightfield" });
        s2.Children.Add(row2);
        panel.Children.Add(s2);
        return panel;
    }

    static Control CreateMonoTextExamples()
    {
        var panel = new StackPanel { Spacing = 12 };
        var s1 = new StackPanel { Spacing = 4 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · 空间三维坐标数据流" });
        var mono1 = new XYMonoText();
        mono1.Rows.Add(new("X 坐标", "142.583", "m"));
        mono1.Rows.Add(new("Y 坐标", "45.500", "m"));
        mono1.Rows.Add(new("Z 坐标", "0.000", "m"));
        s1.Children.Add(mono1);
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 4 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · 运行时性能指标 (数字对齐)" });
        var mono2 = new XYMonoText();
        mono2.Rows.Add(new("Frame", "16.67", "ms"));
        mono2.Rows.Add(new("FPS", "61.20"));
        mono2.Rows.Add(new("Memory", "2.41", "GB"));
        s2.Children.Add(mono2);
        panel.Children.Add(s2);
        return panel;
    }

    static Control CreateBadgeExamples()
    {
        var panel = new StackPanel { Spacing = 12 };
        var s1 = new StackPanel { Spacing = 4 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · 资产与工作区常规标签 (Default)" });
        var row1 = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12 };
        row1.Children.Add(new XYBadge { Text = "Local", Variant = XyuiBadgeVariant.Default });
        row1.Children.Add(new XYBadge { Text = "Read Only", Variant = XyuiBadgeVariant.Default });
        s1.Children.Add(row1);
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 4 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · 实验与未保存标记 (Accent / 22 DIP)" });
        var row2 = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12 };
        row2.Children.Add(new XYBadge { Text = "Experimental", Variant = XyuiBadgeVariant.Accent });
        row2.Children.Add(new XYBadge { Text = "Unsaved", Variant = XyuiBadgeVariant.Accent });
        s2.Children.Add(row2);
        panel.Children.Add(s2);
        return panel;
    }
}
