using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2GalleryCatalog
{
    static Control[] ColorPickers() =>
    [
        ColorSample("RGB · 默认颜色", new XYColorPicker { Width = 300, Color = Color.FromRgb(50, 111, 138), Mode = XYColorPickerMode.RGB }),
        ColorSample("RGBA · 半透明", new XYColorPicker { Width = 300, Color = Color.FromArgb(140, 50, 111, 138), Mode = XYColorPickerMode.RGBA }),
        ColorSample("浅色高亮", new XYColorPicker { Width = 300, Color = Color.FromArgb(255, 230, 178, 92), Mode = XYColorPickerMode.RGBA }),
        ColorSample("禁用", new XYColorPicker { Width = 300, Color = Color.FromArgb(160, 50, 111, 138), IsEnabled = false }),
        new StackPanel { Spacing = 4, Children = { new XYCaption { Text = "交互提示" }, new TextBlock { Text = "点击色块、颜色值或箭头 → 打开颜色面板\n拖动颜色区域 → 调整饱和度与明度\n拖动色相 / 透明度 → 实时更新颜色\nHEX 支持 #RRGGBB / #RRGGBBAA；非法输入保留原值并提示\nEsc、点击外部或窗口失活 → 关闭面板；禁用 → 阻断交互" } } },
    ];

    static Control[] BoolProperties() =>
    [
        BoolSample("启用网格", true), BoolSample("显示辅助线", false), BoolSample("较长的属性名称用于省略显示", true),
        new XYBoolProperty { Width = 420, Label = "禁用的配置选项", Value = true, IsEnabled = false },
        new XYBoolProperty { Width = 420, Label = "只读的布尔状态", Value = true, IsReadOnly = true },
        new StackPanel { Spacing = 4, Children = { new XYCaption { Text = "交互提示" }, new TextBlock { Text = "点击属性行或真实开关 → 只切换一次\n聚焦属性行后按空格 → 切换一次\n只读 / 禁用 → 保留状态并阻断修改\n多个属性行共用同一标签列与值列" } } },
    ];

    static Control ColorSample(string caption, XYColorPicker picker) => new StackPanel { Spacing = 4, Children = { new XYCaption { Text = caption }, picker } };
    static Control BoolSample(string label, bool value) => new XYBoolProperty { Width = 420, Label = label, Value = value };
}
