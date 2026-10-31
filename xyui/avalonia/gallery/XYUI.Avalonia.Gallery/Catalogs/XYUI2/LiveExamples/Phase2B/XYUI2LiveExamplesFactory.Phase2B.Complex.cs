using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2LiveExamplesFactory
{
    static Control SliderExamples()
    {
        var col1 = new StackPanel { Spacing = 10, Width = 420, HorizontalAlignment = HorizontalAlignment.Left };
        col1.Children.Add(new XYSlider { Value = 2.4, Minimum = 0, Maximum = 5.0, Step = 0.1, DecimalPlaces = 1, Suffix = " EV" });
        col1.Children.Add(new XYSlider { Value = 75, Minimum = 0, Maximum = 100, Step = 1, Suffix = "%" });

        var col2 = new StackPanel { Spacing = 10, Width = 420, HorizontalAlignment = HorizontalAlignment.Left };
        col2.Children.Add(new XYSlider { Value = 50, Minimum = 0, Maximum = 100, IsNumberFieldVisible = false });
        col2.Children.Add(new XYSlider { Value = 30, Minimum = 0, Maximum = 100, IsEnabled = false });

        return SceneHost(
            Scene("场景 1 · 视口渲染参数调节 (滑动轨道与右侧 104 DIP 数值框双向实时同步)", col1),
            Scene("场景 2 · 紧凑轨道模式与禁用对比 (支持隐藏右侧输入框 / 禁用态锁定)", col2));
    }

    static Control ComboBoxExamples()
    {
        var col1 = new StackPanel { Spacing = 8, Width = 360, HorizontalAlignment = HorizontalAlignment.Left };
        var comboEngines = new XYComboBox
        {
            ItemsSource = new[] { "Vulkan 1.3 (默认管线)", "Direct3D 12 (Windows)", "Metal 3 (macOS)", "OpenGL 4.6 (兼容模式)", "Software Rasterizer (纯软)" },
            SelectedItem = "Vulkan 1.3 (默认管线)"
        };
        col1.Children.Add(comboEngines);

        var col2 = new StackPanel { Spacing = 8, Width = 360, HorizontalAlignment = HorizontalAlignment.Left };
        var comboCustom = new XYComboBox
        {
            ItemsSource = new[] { "ASTC_4x4", "ASTC_8x8", "BC7_UNORM", "ETC2_RGB" },
            Placeholder = "选择或键入过滤..."
        };
        col2.Children.Add(comboCustom);

        var tip = new XYCaption { Text = "交互说明：键入关键字可即时模糊过滤列表；按 Down 展开，按 Up/Down 移动焦点，按 Enter 选中，按 Escape 取消。" };

        return SceneHost(
            Scene("场景 1 · 渲染后端配置 (键入关键字即时模糊筛选 / 键盘上下与回车选中)", col1),
            Scene("场景 2 · 纹理格式候选 (支持模糊过滤与失焦生命周期收起)", col2),
            tip);
    }
}
