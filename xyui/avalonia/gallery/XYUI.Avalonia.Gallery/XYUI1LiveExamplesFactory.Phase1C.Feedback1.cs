using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1LiveExamplesFactory
{
    static Control CreateHelpTextExamples()
    {
        var panel = new StackPanel { Spacing = 14 };

        var s1 = new StackPanel { Spacing = 6 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · 输入设置项下方操作引导 (Normal)" });
        var row1 = new StackPanel { Spacing = 4 };
        row1.Children.Add(new XYLabel { Text = "烘焙贴图输出格式" });
        row1.Children.Add(new Border { Background = new global::Avalonia.Media.SolidColorBrush(global::Avalonia.Media.Color.FromArgb(16, 128, 128, 128)), Padding = new global::Avalonia.Thickness(8, 4), CornerRadius = new global::Avalonia.CornerRadius(4), Child = new XYText { Text = "EXR (32-bit Float) ▾" } });
        row1.Children.Add(new XYHelpText { Text = "支持 EXR、PNG 与 TGA，建议 HDR 环境使用 EXR 格式。" });
        s1.Children.Add(row1);
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 6 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · 禁用态对比 (IsEnabled=False 同步降级)" });
        var row2 = new StackPanel { Spacing = 4 };
        row2.Children.Add(new XYLabel { Text = "网格细分缓存池" });
        row2.Children.Add(new XYHelpText { Text = "当前图形 API 未启用动态几何细分特性。", IsEnabled = false });
        s2.Children.Add(row2);
        panel.Children.Add(s2);

        return panel;
    }

    static Control CreateErrorTextExamples()
    {
        var panel = new StackPanel { Spacing = 14 };

        var s1 = new StackPanel { Spacing = 6 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · 路径与文件校验阻断 (Normal)" });
        var row1 = new StackPanel { Spacing = 4 };
        row1.Children.Add(new XYLabel { Text = "着色器包含路径 (Include Path)" });
        row1.Children.Add(new Border { Background = new global::Avalonia.Media.SolidColorBrush(global::Avalonia.Media.Color.FromArgb(16, 128, 128, 128)), Padding = new global::Avalonia.Thickness(8, 4), CornerRadius = new global::Avalonia.CornerRadius(4), Child = new XYText { Text = "Z:\\Missing\\Engine\\Shaders" } });
        row1.Children.Add(new XYErrorText { Text = "指定的目录不存在或无法访问，阻断编译流程。" });
        s1.Children.Add(row1);
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 6 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · 数值上下限阻断与禁用态对比" });
        var row2 = new StackPanel { Spacing = 4 };
        row2.Children.Add(new XYErrorText { Text = "纹理尺寸 32768 超出当前设备最大允许限制 (16384)。" });
        row2.Children.Add(new XYErrorText { Text = "已忽略的历史校验错误 (Disabled State)。", IsEnabled = false });
        s2.Children.Add(row2);
        panel.Children.Add(s2);

        return panel;
    }
}
