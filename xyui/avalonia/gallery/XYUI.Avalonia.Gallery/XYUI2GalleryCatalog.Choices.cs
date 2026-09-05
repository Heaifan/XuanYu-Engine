using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2GalleryCatalog
{
    static Control[] Checkboxes()
    {
        var mixed = new XYCheckbox { Content = "批量属性", IsThreeState = true, IsChecked = null };
        return [new XYCheckbox { Content = "显示网格", IsChecked = true }, new XYCheckbox { Content = "启用阴影", IsChecked = true }, new XYCheckbox { Content = "锁定对象" }, mixed, new XYCheckbox { Content = "禁用示例", IsEnabled = false }];
    }

    static Control[] RadioButtons() =>
    [
        new StackPanel { Spacing = 4, Children = { new XYCaption { Text = "渲染模式" }, new XYRadioButton { Content = "实体", GroupName = "render", IsChecked = true }, new XYRadioButton { Content = "线框", GroupName = "render" }, new XYRadioButton { Content = "材质预览", GroupName = "render" } } },
        new StackPanel { Spacing = 4, Children = { new XYCaption { Text = "坐标空间" }, new XYRadioButton { Content = "世界", GroupName = "space", IsChecked = true }, new XYRadioButton { Content = "局部", GroupName = "space" } } },
    ];

    static Control[] Switches() =>
    [
        new XYSwitch { Content = "自动保存", IsChecked = true },
        new XYSwitch { Content = "实时预览", IsChecked = true },
        new XYSwitch { Content = "自动刷新" },
        new XYSwitch { Content = "物理模拟" },
        new XYSwitch { Content = "网络同步", IsEnabled = false },
    ];
}
