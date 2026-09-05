using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2LiveExamplesFactory
{
    static Control TextFieldExamples()
    {
        var col1 = new StackPanel { Spacing = 8, Width = 320, HorizontalAlignment = HorizontalAlignment.Left };
        col1.Children.Add(new XYTextField { Text = "XuanYu_GameProject_01", Placeholder = "项目名称" });
        col1.Children.Add(new XYTextField { Placeholder = "请输入资源包全路径 (留空占位)" });

        var col2 = new StackPanel { Spacing = 8, Width = 320, HorizontalAlignment = HorizontalAlignment.Left };
        col2.Children.Add(new XYTextField { Text = "guid-7f12a8d4c92b8e4f1a603c9d", IsReadOnly = true });
        col2.Children.Add(new XYTextField { Text = "invalid_path_chars::#@", IsError = true });
        col2.Children.Add(new XYTextField { Text = "locked_asset_metadata", IsEnabled = false });

        return SceneHost(
            Scene("场景 1 · 资产元数据编辑 (首次获得焦点自动全选 / 再次点击正常光标定位)", col1),
            Scene("场景 2 · 状态对比 (ReadOnly 可划选复制 / Error 红框警示 / Disabled 禁用)", col2));
    }

    static Control NumberFieldExamples()
    {
        var col1 = new StackPanel { Spacing = 8, Width = 320, HorizontalAlignment = HorizontalAlignment.Left };
        col1.Children.Add(new XYNumberField { Value = 12.50, Step = 0.5, DecimalPlaces = 2, Suffix = "m" });
        col1.Children.Add(new XYNumberField { Value = -45.00, Step = 1.0, DecimalPlaces = 1, Suffix = "°" });
        col1.Children.Add(new XYNumberField { Value = 1.00, Step = 0.1, DecimalPlaces = 2, Suffix = "x" });

        var col2 = new StackPanel { Spacing = 8, Width = 320, HorizontalAlignment = HorizontalAlignment.Left };
        col2.Children.Add(new XYNumberField { Value = 0.85, Minimum = 0, Maximum = 1, Step = 0.05, Suffix = " AO" });
        col2.Children.Add(new XYNumberField { Value = 120, Minimum = 0, Maximum = 500, Step = 10, Suffix = " 粒子" });
        col2.Children.Add(new XYNumberField { Value = 44.1, Suffix = " kHz", IsEnabled = false });

        var tip = new XYCaption { Text = "微调交互：鼠标按住数值文字区域水平拖动可连续微调 (Scrub)；按 Up/Down 步进；Escape 回退初始值。" };

        return SceneHost(
            Scene("场景 1 · Transform 空间变换微调 (按住数值区域拖拽微调 / 悬停显露上下步进器)", col1),
            Scene("场景 2 · 边界区间与精度约束 (Clamp 上下限 / Suffix 单位后缀 / 禁用态)", col2),
            tip);
    }
}
