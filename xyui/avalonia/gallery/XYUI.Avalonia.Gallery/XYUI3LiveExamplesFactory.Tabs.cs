using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3LiveExamplesFactory
{
    static Control CreateTabsLiveExamples()
    {
        var tab1 = new XYTab { Label = "地图基础.map", IsSelected = true };
        var tab2 = new XYTab { Label = "地形高度.raw", IsModified = true };
        var tab3 = new XYTab { Label = "要素规则.json", IsClosable = true };
        var tab4 = new XYTab { Label = "系统只读.log", IsClosable = false };

        var tabs = new XYTabs(tab1, tab2, tab3, tab4) { SizingMode = XyuiTabSizingMode.Content };

        var statusText = new TextBlock { Text = "当前活动页签：地图基础.map", Classes = { "xyui-text-caption" } };
        var logText = new TextBlock { Text = "操作提示：点击各页签直接切换；选中有 × 的页签可点击关闭，邻项自动接替。", Classes = { "xyui-text-caption" } };

        tabs.SelectionChanged += (_, tab) => statusText.Text = $"当前活动页签：{tab.Label} (IsModified={tab.IsModified})";
        tabs.TabClosed += (_, tab) => logText.Text = $"事件：已关闭 [{tab.Label}]，选中态自动迁移至相邻页签。";

        var toggleModifiedBtn = new XYButton { Content = "切换当前项修改状态 (IsModified)" };
        toggleModifiedBtn.Click += (_, _) =>
        {
            var cur = tabs.Items.FirstOrDefault(x => x.IsSelected);
            if (cur is not null) cur.IsModified = !cur.IsModified;
        };

        var ctrlRow = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12, Children = { toggleModifiedBtn } };
        var container = new StackPanel { Spacing = 12, Children = { tabs, statusText, logText, ctrlRow } };
        return WrapCard(container, "基础标签页 · 真实单选接替、未保存圆点与弱化关闭");
    }

    static Control CreateTabsComposition()
    {
        var col1 = new StackPanel
        {
            Spacing = 6, Width = 180,
            Children =
            {
                new TextBlock { Text = "1. Tabs (单个/基础组)", Classes = { "xyui-text-label" } },
                new TextBlock { Text = "轻量平级多视口切换，只负责单组排版与选择，不带翻页与溢出。", Classes = { "xyui-text-caption" }, TextWrapping = global::Avalonia.Media.TextWrapping.Wrap },
                new XYTabs(new XYTab { Label = "视口 A", IsSelected = true }, new XYTab { Label = "视口 B" })
            }
        };
        var col2 = new StackPanel
        {
            Spacing = 6, Width = 200,
            Children =
            {
                new TextBlock { Text = "2. TabBar (多页签栏)", Classes = { "xyui-text-label" } },
                new TextBlock { Text = "完整多文档容器，集成滚轮平移、前后翻页、溢出菜单与新增按钮。", Classes = { "xyui-text-caption" }, TextWrapping = global::Avalonia.Media.TextWrapping.Wrap },
                new Border { Classes = { "xyui-surface-panel" }, Padding = new(6), Child = new TextBlock { Text = "[ 滚轮 + 翻页 + 溢出 + 新增 ]", Classes = { "xyui-text-code" } } }
            }
        };
        var col3 = new StackPanel
        {
            Spacing = 6, Width = 200,
            Children =
            {
                new TextBlock { Text = "3. DockTabs (停靠标签)", Classes = { "xyui-text-label" } },
                new TextBlock { Text = "停靠面板专用，左侧集成 DragGrip 拖动手柄，支持同栏拖动重排。", Classes = { "xyui-text-caption" }, TextWrapping = global::Avalonia.Media.TextWrapping.Wrap },
                new Border { Classes = { "xyui-surface-panel" }, Padding = new(6), Child = new TextBlock { Text = "[ Drag Grip + 同栏重排 ]", Classes = { "xyui-text-code" } } }
            }
        };
        var grid = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 16, Children = { col1, col2, col3 } };
        return WrapCard(grid, "Tabs 家族职责矩阵 · Tabs ≠ TabBar ≠ DockTabs");
    }
}
