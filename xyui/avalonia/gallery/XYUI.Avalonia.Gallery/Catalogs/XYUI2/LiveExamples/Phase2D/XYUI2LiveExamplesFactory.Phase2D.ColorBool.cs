using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2LiveExamplesFactory
{
    static Control ColorPickerExamples()
    {
        var col1 = new StackPanel { Spacing = 8, Width = 320, HorizontalAlignment = HorizontalAlignment.Left };
        col1.Children.Add(new XYColorPicker { Width = 300, Color = Color.FromRgb(50, 111, 138), Mode = XYColorPickerMode.RGB });
        col1.Children.Add(new XYColorPicker { Width = 300, Color = Color.FromArgb(140, 50, 111, 138), Mode = XYColorPickerMode.RGBA });

        var col2 = new StackPanel { Spacing = 8, Width = 320, HorizontalAlignment = HorizontalAlignment.Left };
        col2.Children.Add(new XYColorPicker { Width = 300, Color = Color.FromArgb(255, 230, 178, 92), Mode = XYColorPickerMode.RGBA });
        col2.Children.Add(new XYColorPicker { Width = 300, Color = Color.FromArgb(160, 50, 111, 138), IsEnabled = false });

        return SceneHost(
            Scene("场景 1 · 核心选色 (RGB / RGBA 半透明，支持色盘拖拽、滑块与 HEX 联动)", col1),
            Scene("场景 2 · 浅色高亮与禁用保护 (面板支持 Esc / 失焦关闭，阻断修改)", col2));
    }

    static Control BoolPropertyExamples()
    {
        var col1 = new StackPanel { Spacing = 8, Width = 440, HorizontalAlignment = HorizontalAlignment.Left };
        col1.Children.Add(new XYBoolProperty { Width = 420, Label = "启用网格", Value = true });
        col1.Children.Add(new XYBoolProperty { Width = 420, Label = "显示辅助线", Value = false });

        var col2 = new StackPanel { Spacing = 8, Width = 440, HorizontalAlignment = HorizontalAlignment.Left };
        col2.Children.Add(new XYBoolProperty { Width = 420, Label = "只读的布尔状态", Value = true, IsReadOnly = true });
        col2.Children.Add(new XYBoolProperty { Width = 420, Label = "禁用的配置选项", Value = true, IsEnabled = false });

        return SceneHost(
            Scene("场景 1 · 常用场景配置 (支持整行点击与空格键快速切换)", col1),
            Scene("场景 2 · 只读呈现与禁用保护 (保留视觉真值，阻断用户修改)", col2));
    }
}
