using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1LiveExamplesFactory
{
    static Control CreateTooltipExamples()
    {
        var panel = new StackPanel { Spacing = 14 };

        var s1 = new StackPanel { Spacing = 6 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · 工具栏图标悬浮提示生命周期 (Hover Target)" });
        var bar = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12 };

        var btn1 = new Border
        {
            Background = new global::Avalonia.Media.SolidColorBrush(global::Avalonia.Media.Color.FromArgb(20, 128, 128, 128)),
            Padding = new global::Avalonia.Thickness(8),
            CornerRadius = new global::Avalonia.CornerRadius(4),
            Child = new XYIcon { Icon = XyuiVectorIcon.Search, Size = XyuiIconSize.Comfortable }
        };
        ToolTip.SetTip(btn1, new XYTooltip { Content = new XYCaption { Text = "在当前工程资产与对象树中执行全文检索 (Ctrl+F)" } });
        bar.Children.Add(btn1);

        var btn2 = new Border
        {
            Background = new global::Avalonia.Media.SolidColorBrush(global::Avalonia.Media.Color.FromArgb(20, 128, 128, 128)),
            Padding = new global::Avalonia.Thickness(8),
            CornerRadius = new global::Avalonia.CornerRadius(4),
            Child = new XYIcon { Icon = XyuiVectorIcon.Code, Size = XyuiIconSize.Comfortable }
        };
        ToolTip.SetTip(btn2, new XYTooltip { Content = new XYCaption { Text = "打开着色器底层 SPIR-V 字节码诊断查看器" } });
        bar.Children.Add(btn2);
        s1.Children.Add(bar);
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 6 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · 禁用命令目标提示 (Disabled Target)" });
        var disBtn = new Border
        {
            Background = new global::Avalonia.Media.SolidColorBrush(global::Avalonia.Media.Color.FromArgb(10, 128, 128, 128)),
            Padding = new global::Avalonia.Thickness(8),
            CornerRadius = new global::Avalonia.CornerRadius(4),
            Child = new XYText { Text = "烘焙光照贴图 (不可用)", IsEnabled = false }
        };
        ToolTip.SetTip(disBtn, new XYTooltip { Content = new XYCaption { Text = "当前场景未包含烘焙就绪的光照探针与静态网格对象。" } });
        s2.Children.Add(disBtn);
        panel.Children.Add(s2);

        var s3 = new StackPanel { Spacing = 6 };
        s3.Children.Add(new XYCaption { Text = "场景 3 · 提示内容载体视觉外观 (Tooltip Container Chrome)" });
        s3.Children.Add(new XYTooltip { Content = new XYCaption { Text = "提示载体：6 DIP 内边距与 4 DIP 圆角浮层" } });
        panel.Children.Add(s3);

        return panel;
    }

    static Control CreateRichTextExamples()
    {
        var panel = new StackPanel { Spacing = 14 };

        var s1 = new StackPanel { Spacing = 6 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · 编译与构建摘要 (Normal + Strong + Mono)" });
        var r1 = new XYRichText
        {
            Text = "已完成着色器变体编译：",
            StrongText = "18 个着色器",
            MonoText = "耗时 2.4 s (pipeline_chunk_04)"
        };
        s1.Children.Add(r1);
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 6 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · 资源定位与版本状态" });
        var r2 = new XYRichText
        {
            Text = "网格顶点缓冲区就绪：",
            StrongText = "48,200 顶点",
            MonoText = "vertex_buffer_id: 0x8F2A"
        };
        s2.Children.Add(r2);
        panel.Children.Add(s2);

        return panel;
    }
}
