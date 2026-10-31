using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1LiveExamplesFactory
{
    static Control CreateSearchHighlightExamples()
    {
        var panel = new StackPanel { Spacing = 14 };

        var s1 = new StackPanel { Spacing = 6 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · 搜索结果高亮呈现 (RESULT-PRESENTATION)" });
        var col1 = new StackPanel { Spacing = 6 };
        col1.Children.Add(new XYCaption { Text = "查询词（由宿主提供说明）: terrain" });
        col1.Children.Add(new XYSearchHighlight { Text = "World_terrain_chunk_loader_node" });
        col1.Children.Add(new XYSearchHighlight { Text = "terrain_heightfield_generator_v2" });
        s1.Children.Add(col1);
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 6 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · 诊断匹配与禁用态对比" });
        var col2 = new StackPanel { Spacing = 6 };
        col2.Children.Add(new XYSearchHighlight { Text = "命中项：RenderPipelinePass_Deferred" });
        col2.Children.Add(new XYSearchHighlight { Text = "已忽略的匹配结果 (Disabled State)", IsEnabled = false });
        s2.Children.Add(col2);
        panel.Children.Add(s2);

        return panel;
    }

    static Control CreateTruncatedTextExamples()
    {
        var panel = new StackPanel { Spacing = 14 };

        var s1 = new StackPanel { Spacing = 6 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · 受限宿主宽度对比 (140 DIP vs 240 DIP)" });
        var list = new StackPanel { Spacing = 8 };

        var box1 = new Border { Width = 140, Background = new global::Avalonia.Media.SolidColorBrush(global::Avalonia.Media.Color.FromArgb(16, 128, 128, 128)), Padding = new global::Avalonia.Thickness(8, 4), CornerRadius = new global::Avalonia.CornerRadius(4), HorizontalAlignment = HorizontalAlignment.Left };
        var t1 = new XYTruncatedText { Text = "Textures/Environment/Atmosphere/skybox_hdr_v3_compressed.dds", Mode = XyuiTruncatedTextMode.End };
        ToolTip.SetTip(box1, new XYTooltip { Content = new XYCaption { Text = "完整路径: Textures/Environment/Atmosphere/skybox_hdr_v3_compressed.dds" } });
        box1.Child = t1;
        list.Children.Add(box1);

        var box2 = new Border { Width = 240, Background = new global::Avalonia.Media.SolidColorBrush(global::Avalonia.Media.Color.FromArgb(16, 128, 128, 128)), Padding = new global::Avalonia.Thickness(8, 4), CornerRadius = new global::Avalonia.CornerRadius(4), HorizontalAlignment = HorizontalAlignment.Left };
        var t2 = new XYTruncatedText { Text = "Textures/Environment/Atmosphere/skybox_hdr_v3_compressed.dds", Mode = XyuiTruncatedTextMode.End };
        box2.Child = t2;
        list.Children.Add(box2);

        s1.Children.Add(list);
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 6 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · Middle 模式诚实降级展示 (XYUI1-GAP-002)" });
        var box3 = new Border { Width = 180, Background = new global::Avalonia.Media.SolidColorBrush(global::Avalonia.Media.Color.FromArgb(16, 128, 128, 128)), Padding = new global::Avalonia.Thickness(8, 4), CornerRadius = new global::Avalonia.CornerRadius(4), HorizontalAlignment = HorizontalAlignment.Left };
        box3.Child = new XYTruncatedText { Text = "entity_terrain_sector_42_diagnostics_trace", Mode = XyuiTruncatedTextMode.Middle };
        s2.Children.Add(box3);
        panel.Children.Add(s2);

        return panel;
    }
}
