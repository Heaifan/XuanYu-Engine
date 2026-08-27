using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2GalleryCatalog
{
    static Control[] TextFields() => [
        new XYTextField { Width = 200, Text = "Northern Region" },
        new XYTextField { Width = 200, Placeholder = "输入名称" },
        new XYTextField { Width = 200, Text = "Northern Region" },
        new XYTextField { Width = 200, Text = "只读属性", IsReadOnly = true },
        new XYTextField { Width = 200, Text = "禁用输入", IsEnabled = false },
        new XYTextField { Width = 200, Text = "Invalid Path", IsError = true }];
    static Control[] NumberFields() => [
        NumberSample("Default", new XYNumberField { Width = 220, Value = 125, Maximum = 200 }),
        NumberSample("Hover / Stepper", new XYNumberField { Width = 220, Value = 50, Step = 5 }),
        NumberSample("Focus", new XYNumberField { Width = 220, Value = 25 }),
        NumberSample("Disabled", new XYNumberField { Width = 220, Value = 50, IsEnabled = false }),
        NumberSample("Min / Max", new XYNumberField { Width = 220, Value = 0, Minimum = 0, Maximum = 100 }),
        NumberSample("Suffix", new XYNumberField { Width = 220, Value = 72, Suffix = "%" }),
        NumberSample("Scrub · 按住数值左右拖动", new XYNumberField { Width = 220, Value = 50, IsScrubEnabled = true })];
    static Control NumberSample(string caption, XYNumberField field) => new StackPanel { Spacing = 4, Children = { new XYCaption { Text = caption }, field } };
    static Control[] Sliders() => [new XYSlider { Value = 35 }, new XYSlider { Value = 70, Maximum = 100 }, new XYSlider { Value = 2, Maximum = 4, Step = .25 }];
    static Control[] ComboBoxes() => [new XYComboBox { Placeholder = "North...", ItemsSource = new[] { "Northern Region", "Northern Coast", "Northwest Hub" } }, new XYComboBox { SelectedIndex = 1, ItemsSource = new[] { "Steel", "Glass", "Wood" } }];
    static Control[] Selects() => [new XYSelect { SelectedIndex = 0, ItemsSource = new[] { "实体", "线框", "材质预览" } }, new XYSelect { SelectedIndex = 1, ItemsSource = new[] { "世界", "局部" } }];
    static Control[] TextAreas() => [new XYTextArea { Text = "普通多行文本\n第二行内容", Mode = XYTextAreaMode.Standard }, new XYTextArea { Text = "编辑器内容\n第二行\n第三行", Mode = XYTextAreaMode.Editor }];
}
