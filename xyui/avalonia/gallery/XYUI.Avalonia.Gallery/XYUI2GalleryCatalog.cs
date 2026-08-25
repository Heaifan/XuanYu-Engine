using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

// XYUI-2 Batch 01 真实 Runtime 预览：每个组件展示 Default / 变体 / Selected(或 ON) / Disabled。
public static class XYUI2GalleryCatalog
{
    public static Control CreatePreview(string id) => id switch
    {
        "XYUI-2-01" => Host("XY.Button · Primary / Secondary / Danger / Disabled", Buttons()),
        "XYUI-2-02" => Host("XY.IconButton · Default Ghost / Hover(交互) / Selected / Disabled", IconButtons()),
        "XYUI-2-03" => Host("XY.ToggleButton · OFF / ON(Persistent Edge) / Disabled", Toggles()),
        "XYUI-2-04" => Host("XY.SplitButton · Default / MainHover / MenuHover / Disabled（可悬停验证）", Splits()),
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

    // Split（R2 · Soft Partition）：四状态明确展示 Default / MainHover / MenuHover / Disabled。
    // Hover 为交互态无法静态定格，每个 sample 下方用 caption 标注对应状态，由审核者实际悬停验证；
    // 主区与菜单区各自独立 Hover：悬停按钮主体=MainHover，悬停右侧箭头=MenuHover。
    static Control[] Splits() =>
    [
        SplitCell("新建", "Default", false),
        SplitCell("导入", "MainHover · 悬停主区", false),
        SplitCell("保存选项", "MenuHover · 悬停右侧箭头", false),
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

internal static class XYIconButtonNamingExtensions
{
    // Canonical AccessibleName = Required：Ghost 图标按钮必须携带自动化名称。
    public static XYIconButton Named(this XYIconButton button, string name)
    {
        AutomationProperties.SetName(button, name);
        return button;
    }
}
