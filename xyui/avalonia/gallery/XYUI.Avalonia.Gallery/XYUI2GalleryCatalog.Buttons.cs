using Avalonia.Controls;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2GalleryCatalog
{
    static Control[] Buttons() =>
    [
        new XYButton { Content = "新建" }, new XYButton { Content = "取消", Variant = XyuiButtonVariant.Secondary },
        new XYButton { Content = "删除", Variant = XyuiButtonVariant.Danger }, new XYButton { Content = "保存", IsEnabled = false },
    ];
    static Control[] IconButtons()
    {
        var selected = GhostIcon(XyuiVectorIcon.Code, "查看代码"); selected.IsSelected = true;
        return [GhostIcon(XyuiVectorIcon.Search, "搜索"), GhostIcon(XyuiVectorIcon.Copy, "复制"), selected, DisabledIcon(XyuiVectorIcon.Info)];
    }
    static XYIconButton GhostIcon(XyuiVectorIcon icon, string name) => new XYIconButton { Content = new XYIcon { Icon = icon, Size = XyuiIconSize.Medium } }.Named(name);
    static XYIconButton DisabledIcon(XyuiVectorIcon icon) => new XYIconButton { Content = new XYIcon { Icon = icon, Size = XyuiIconSize.Medium }, IsEnabled = false }.Named("信息（禁用）");
    static Control[] Toggles() => [new XYToggleButton { Content = "网格吸附" }, new XYToggleButton { Content = "正交模式", IsChecked = true }, new XYToggleButton { Content = "显示参考网格", IsEnabled = false }];
    static Control[] Splits() => [SplitCell("新建", "Default", false), SplitCell("导入", "MainHover · 悬停主区", false), SplitCell("保存", "MenuHover · 悬停右侧图标槽", false), SplitCell("运行", "Pressed Main · 按住主区", false), SplitCell("更多", "Pressed Menu · 按住图标槽", false), SplitCell("发布", "Disabled", true)];
    static Control SplitCell(string content, string caption, bool disabled) => new StackPanel { Spacing = 4, Children = { new XYSplitButton { Content = content, IsEnabled = !disabled }, new XYCaption { Text = caption } } };
}
