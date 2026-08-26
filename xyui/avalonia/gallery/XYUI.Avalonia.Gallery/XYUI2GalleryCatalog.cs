using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

// XYUI-2 真实 Runtime 预览：Batch 01 与 SplitButton 收口样例。
public static class XYUI2GalleryCatalog
{
    public static Control CreatePreview(string id) => id switch
    {
        "XYUI-2-01" => Host("XY.Button · Primary / Secondary / Danger / Disabled", Buttons()),
        "XYUI-2-02" => Host("XY.IconButton · Default Ghost / Hover(交互) / Selected / Disabled", IconButtons()),
        "XYUI-2-03" => Host("XY.ToggleButton · OFF / ON(Persistent Edge) / Disabled", Toggles()),
        "XYUI-2-04" => Host("XY.SplitButton · Compact Icon Well / Default / Hover / Pressed / Disabled", Splits()),
        _ => new TextBlock { Text = "未实装组件（Batch 02+）" }
    };

    static StackPanel Host(string title, Control[] samples)
    {
        var panel = new WrapPanel { Orientation = Orientation.Horizontal };
        foreach (var sample in samples)
        {
            sample.Margin = new Thickness(0, 0, 8, 0);
            panel.Children.Add(sample);
        }

        return new StackPanel { Spacing = 8, Children = { new XYCaption { Text = title }, panel } };
    }

    static Control[] Buttons() =>
    [
        new XYButton { Content = "新建" },
        new XYButton { Content = "取消", Variant = XyuiButtonVariant.Secondary },
        new XYButton { Content = "删除", Variant = XyuiButtonVariant.Danger },
        new XYButton { Content = "保存", IsEnabled = false },
    ];

    static Control[] IconButtons()
    {
        var selected = GhostIcon(XyuiVectorIcon.Code, "查看代码");
        selected.IsSelected = true;
        return
        [
            GhostIcon(XyuiVectorIcon.Search, "搜索"),
            GhostIcon(XyuiVectorIcon.Copy, "复制"),
            selected,
            DisabledIcon(XyuiVectorIcon.Info),
        ];
    }

    static XYIconButton GhostIcon(XyuiVectorIcon icon, string name) => new XYIconButton
    {
        Content = new XYIcon { Icon = icon, Size = XyuiIconSize.Medium },
    }.Named(name);

    static XYIconButton DisabledIcon(XyuiVectorIcon icon) => new XYIconButton
    {
        Content = new XYIcon { Icon = icon, Size = XyuiIconSize.Medium },
        IsEnabled = false,
    }.Named("信息（禁用）");

    static Control[] Toggles() =>
    [
        new XYToggleButton { Content = "网格吸附" },
        new XYToggleButton { Content = "正交模式", IsChecked = true },
        new XYToggleButton { Content = "显示参考网格", IsEnabled = false },
    ];

    // Hover/Pressed 为真实交互态；caption 指示审核者在对应区域操作。
    static Control[] Splits() =>
    [
        SplitCell("新建", "Default", false),
        SplitCell("导入", "MainHover · 悬停主区", false),
        SplitCell("保存", "MenuHover · 悬停右侧图标槽", false),
        SplitCell("运行", "Pressed Main · 按住主区", false),
        SplitCell("更多", "Pressed Menu · 按住图标槽", false),
        SplitCell("发布", "Disabled", true),
    ];

    static Control SplitCell(string content, string caption, bool disabled) => new StackPanel
    {
        Spacing = 4,
        Children =
        {
            new XYSplitButton { Content = content, IsEnabled = !disabled },
            new XYCaption { Text = caption },
        },
    };
}
