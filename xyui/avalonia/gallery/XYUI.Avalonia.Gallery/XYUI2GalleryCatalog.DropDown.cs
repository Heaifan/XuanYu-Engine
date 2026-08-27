using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

// XYUI-2-05 DropDownButton 真实场景样例：导出/筛选/排序等整钮开菜单语义；
// Hover/Pressed 为真实交互态，caption 指示审核者对整钮操作（无独立第二点击区）。
public static partial class XYUI2GalleryCatalog
{
    static Control[] DropDowns() =>
    [
        DropDownCell("导出", "Default · 点击整钮打开导出菜单", false),
        DropDownCell("筛选", "Hover · 悬停后主体与右侧槽同步", false),
        DropDownCell("排序", "Pressed · 按住整钮（含右槽）", false),
        DropDownCell("构建配置", "Focus · 键盘焦点轮廓", false),
        DropDownCell("保存", "Disabled", true),
    ];

    static Control DropDownCell(string content, string caption, bool disabled) => new StackPanel
    {
        Spacing = 4,
        Children =
        {
            new XYDropDownButton { Content = content, IsEnabled = !disabled },
            new XYCaption { Text = caption },
        },
    };
}
