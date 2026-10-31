using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3LiveExamplesFactory
{
    static Control CreateBottomNavigationLiveExamples()
    {
        var items = new[]
        {
            new XYBottomNavigationItem("map", "地图", XyuiVectorIcon.Locate),
            new XYBottomNavigationItem("data", "数据", XyuiVectorIcon.Code),
            new XYBottomNavigationItem("experiment", "实验", XyuiVectorIcon.Clear),
            new XYBottomNavigationItem("logs", "日志", XyuiVectorIcon.Section, "1"),
            new XYBottomNavigationItem("settings", "设置", XyuiVectorIcon.Info)
        };
        var state = new XYNavigationState(items.Select(i => new XYNavigationEntry(i.Id, i.Label, i.Icon)), "map");
        var standard = new XYBottomNavigation(state, items) { Width = 360 };
        standard.DestinationRequested += (_, req) => req.Accept();

        var primaryItems = items.Where(i => i.Id != "experiment").ToArray();
        var primaryState = new XYNavigationState(primaryItems.Select(i => new XYNavigationEntry(i.Id, i.Label, i.Icon)), "map");
        var primaryBtn = new XYButton { Content = new XYIcon { Icon = XyuiVectorIcon.Add, Size = XyuiIconSize.Small }, Variant = XyuiButtonVariant.Primary };
        var primaryNav = new XYBottomNavigation(primaryState, primaryItems, primaryBtn) { Width = 360 };
        primaryNav.DestinationRequested += (_, req) => req.Accept();

        var feedback = new TextBlock { Text = "当前目的地: map (地图) · 5 槽位等宽锁定 (每槽 72 DIP @ 360 DIP)", Classes = { "xyui-text-caption" } };
        var tip = new TextBlock { Text = "说明: 中央 Primary Action 为 48×48 DIP 独立圆形按钮，向上浮出 16 DIP；点击仅触发新建事件，不修改目的地选中状态。", Classes = { "xyui-text-caption" } };
        state.Changed += (_, _) => feedback.Text = $"标准导航目的地已切换至: [{state.SelectedId}] · 仅图标区域显示 Accent Well";
        primaryState.Changed += (_, _) => feedback.Text = $"主操作导航目的地已切换至: [{primaryState.SelectedId}]";
        primaryNav.PrimaryActionRequested += (_, _) => feedback.Text = "操作反馈: 触发中央独立 Primary Action [新建] (目的地保持不变)";

        var col = new StackPanel
        {
            Spacing = 10,
            Children =
            {
                new TextBlock { Text = "360 DIP 标准等宽排布 (Selected 仅高亮图标区 · 包含数字角标):", Classes = { "xyui-text-label" } },
                standard,
                new TextBlock { Text = "中央独立主操作按钮 (48×48 DIP 圆形操作，向上浮出 16 DIP，独立解耦):", Classes = { "xyui-text-label" } },
                primaryNav, tip, feedback
            }
        };
        return WrapCard(col, "移动端等宽底部导航 · Selected Icon Well、独立主操作与数字角标");
    }

    static Control CreateBottomNavigationComposition()
    {
        var items = new[] { new XYBottomNavigationItem("map", "地图", XyuiVectorIcon.Locate), new XYBottomNavigationItem("data", "数据", XyuiVectorIcon.Code), new XYBottomNavigationItem("logs", "日志", XyuiVectorIcon.Section) };
        var nav0 = new XYBottomNavigation(items) { Width = 260, SafeAreaBottom = 0 };
        var nav24 = new XYBottomNavigation(items) { Width = 260, SafeAreaBottom = 24 };
        Control MockPhone(string title, XYBottomNavigation nav, string desc)
        {
            var grid = new Grid { RowDefinitions = new RowDefinitions("*,Auto") };
            grid.Children.Add(new StackPanel { Margin = new(10, 12), Spacing = 4, Children = { new TextBlock { Text = title, Classes = { "xyui-text-label" } }, new TextBlock { Text = desc, Classes = { "xyui-text-caption" } } } });
            Grid.SetRow(nav, 1); grid.Children.Add(nav);
            return new Border { Width = 262, Height = 140, Classes = { "xyui-surface-panel-alt" }, CornerRadius = new(8), BorderThickness = new(1), Child = grid };
        }
        var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 16, Children = { MockPhone("直边设备 (SafeAreaBottom=0)", nav0, "内容高度 64 DIP · 槽位贴底"), MockPhone("全面屏手势区 (SafeAreaBottom=24)", nav24, "内容 64 DIP + 安全区 24 DIP = 总高 88 DIP") } };
        return WrapCard(row, "移动端安全区避让对比 · 64 DIP 内容高度与 0 vs 24 DIP 底部避让内衬");
    }
}
