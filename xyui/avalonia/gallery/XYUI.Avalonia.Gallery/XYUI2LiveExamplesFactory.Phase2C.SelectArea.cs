using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2LiveExamplesFactory
{
    static Control SelectExamples()
    {
        var col1 = new StackPanel { Spacing = 8, Width = 320, HorizontalAlignment = HorizontalAlignment.Left };
        col1.Children.Add(new XYSelect { Width = 300, SelectedIndex = 0, ItemsSource = new[] { "English", "简体中文", "日本語" } });
        col1.Children.Add(new XYSelect { Width = 300, SelectedIndex = 2, ItemsSource = new[] { "低画质 (Low)", "中画质 (Medium)", "高画质 (High)", "极致画质 (Ultra)" } });

        var col2 = new StackPanel { Spacing = 8, Width = 320, HorizontalAlignment = HorizontalAlignment.Left };
        col2.Children.Add(new XYSelect { Width = 300, Placeholder = "请选择目标部署环境..." });
        col2.Children.Add(new XYSelect { Width = 300, SelectedIndex = 0, ItemsSource = new[] { "超长环境配置名称 · Vulkan High-Performance Desktop Render Profile" } });
        col2.Children.Add(new XYSelect { Width = 300, SelectedIndex = 1, IsEnabled = false, ItemsSource = new[] { "开发构建", "发布构建" } });

        return SceneHost(
            Scene("场景 1 · 核心固定候选 (不可自由输入，Enter/Space 展开或提交，Up/Down 导航)", col1),
            Scene("场景 2 · 占位提示、超长字符 Ellipsis 与禁用保护", col2));
    }

    static Control TextAreaExamples()
    {
        var col1 = new StackPanel { Spacing = 8, Width = 360, HorizontalAlignment = HorizontalAlignment.Left };
        col1.Children.Add(new XYTextArea { Width = 340, Text = "第一行任务说明\n第二行执行细节\n第三行补充说明", Placeholder = "请输入描述..." });
        col1.Children.Add(new XYTextArea { Width = 340, Placeholder = "占位提示：请在此描述遇到的渲染问题……" });

        var col2 = new StackPanel { Spacing = 8, Width = 460, HorizontalAlignment = HorizontalAlignment.Left };
        col2.Children.Add(new XYTextArea { Width = 440, Mode = XYTextAreaMode.Editor, EditorType = "JSON", Text = "{\n  \"engine\": \"XuanYu\",\n  \"version\": \"v0.2.28\",\n  \"multithreading\": true\n}" });
        col2.Children.Add(new XYTextArea { Width = 440, IsReadOnly = true, Text = "只读系统日志：[INFO] Vulkan Device Created Successfully." });

        return SceneHost(
            Scene("场景 1 · 标准多行文本 (AutoGrow 自动增长 / 首焦全选 / 再次点击 Caret)", col1),
            Scene("场景 2 · 编辑器模式 (顶部元数据栏 · 类型/行数/字符数统计 / ReadOnly 划选复制)", col2));
    }
}
