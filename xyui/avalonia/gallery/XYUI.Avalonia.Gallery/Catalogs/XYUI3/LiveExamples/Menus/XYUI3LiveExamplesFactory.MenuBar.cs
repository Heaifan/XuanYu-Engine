using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3LiveExamplesFactory
{
    static Control CreateMenuBarLiveExamples() =>
        WrapCard(new Views.XYUIMenuLiveSampleView(), "桌面一级紧凑菜单栏 · 声明式 AXAML 与 ICommand 真实绑定");

    static Control CreateMenuBarComposition()
    {
        var host = new Border { Classes = { "xyui-surface-panel" }, Padding = new(8, 4), CornerRadius = new(4) };
        var dock = new DockPanel();
        var chip = new Border
        {
            Classes = { "xyui-surface-panel" },
            Padding = new(8, 3),
            CornerRadius = new(3),
            Child = new TextBlock { Text = "工作区：地图编辑", Classes = { "xyui-text-caption" } }
        };
        DockPanel.SetDock(chip, Dock.Right);
        dock.Children.Add(chip);

        var bar = new XYMenuBar(
            new XYMenuBarItem { Header = "项目" },
            new XYMenuBarItem { Header = "地图", IsActive = true },
            new XYMenuBarItem { Header = "图层" },
            new XYMenuBarItem { Header = "渲染" })
        {
            Classes = { "compact" },
            ShowDivider = false
        };
        dock.Children.Add(bar);
        host.Child = dock;

        var panel = new StackPanel { Spacing = 8 };
        panel.Children.Add(host);
        panel.Children.Add(new TextBlock
        {
            Text = "紧凑顶栏规范：Classes=\"compact\" 配合 ShowDivider=\"False\"，高度 34 DIP，支持键盘 Left/Right 切换相邻项。",
            Classes = { "xyui-text-caption" },
            TextWrapping = global::Avalonia.Media.TextWrapping.Wrap
        });
        return WrapCard(panel, "主窗口顶栏集成 · 紧凑模式与状态指示");
    }
}
