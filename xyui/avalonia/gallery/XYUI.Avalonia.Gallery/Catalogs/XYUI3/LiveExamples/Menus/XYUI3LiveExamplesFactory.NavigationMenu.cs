using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3LiveExamplesFactory
{
    static Control CreateNavigationMenuLiveExamples()
    {
        var entries = new[]
        {
            new XYNavigationEntry("map", "地图编辑", XyuiVectorIcon.Locate),
            new XYNavigationEntry("environment", "环境配置", XyuiVectorIcon.Eye),
            new XYNavigationEntry("resources", "引擎资源", XyuiVectorIcon.Browse, Badge: "12", Status: XyuiStatusState.Neutral),
            new XYNavigationEntry("debug", "调试诊断", XyuiVectorIcon.Section, Badge: "Warning", Status: XyuiStatusState.Warning),
            new XYNavigationEntry("scripts", "逻辑脚本", XyuiVectorIcon.Code),
            new XYNavigationEntry("settings", "首选项", XyuiVectorIcon.Section)
        };
        var state = new XYNavigationState(entries, "map");

        var statusText = new TextBlock
        {
            Text = $"当前目的地：{state.SelectedId} ({state.Selected?.Label})",
            Classes = { "xyui-text-label" }
        };
        var activeArea = new Border
        {
            Classes = { "xyui-surface-panel" },
            Padding = new(16, 12),
            CornerRadius = new(4),
            Child = new TextBlock { Text = "正在显示：地图编辑主功能区（Selected 状态长期保持）", Classes = { "xyui-text-body" } }
        };

        state.Changed += (_, _) =>
        {
            statusText.Text = $"当前目的地：{state.SelectedId} ({state.Selected?.Label})";
            activeArea.Child = new TextBlock
            {
                Text = $"正在显示：{state.Selected?.Label} 功能区（由 state.SelectedId 权威驱动）",
                Classes = { "xyui-text-body" }
            };
        };

        var navMenu = new XYNavigationMenu(state) { Width = 230 };
        var left = new StackPanel { Spacing = 8, Children = { navMenu } };
        var right = new StackPanel { Spacing = 8, Width = 320, Children = { statusText, activeArea } };

        var split = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 16, Children = { left, right } };
        return WrapCard(split, "导航菜单 · 真实 XYNavigationState 驱动目的地切换 (含 Badge/Status)");
    }

    static Control CreateNavigationMenuComposition()
    {
        var col1 = new StackPanel
        {
            Spacing = 6, Width = 180,
            Children =
            {
                new TextBlock { Text = "1. NavigationMenu (导航)", Classes = { "xyui-text-label" } },
                new TextBlock { Text = "长期保持选中态，支持消费 Badge / Status 状态。", Classes = { "xyui-text-caption" }, TextWrapping = global::Avalonia.Media.TextWrapping.Wrap },
                new XYNavigationItem { Label = "地图", Icon = XyuiVectorIcon.Locate, IsSelected = true },
                new XYNavigationItem { Label = "资源", Icon = XyuiVectorIcon.Browse, Badge = "12" },
                new XYNavigationItem { Label = "调试", Icon = XyuiVectorIcon.Section, Badge = "Warning", Status = XyuiStatusState.Warning }
            }
        };

        var col2 = new StackPanel
        {
            Spacing = 6, Width = 180,
            Children =
            {
                new TextBlock { Text = "2. Button List (操作)", Classes = { "xyui-text-label" } },
                new TextBlock { Text = "执行一次性瞬时命令，执行后恢复，不保持长期当前位置。", Classes = { "xyui-text-caption" }, TextWrapping = global::Avalonia.Media.TextWrapping.Wrap },
                new XYButton { Content = "创建实体", Variant = XyuiButtonVariant.Secondary }
            }
        };

        var col3 = new StackPanel
        {
            Spacing = 6, Width = 180,
            Children =
            {
                new TextBlock { Text = "3. Tabs (平级文档)", Classes = { "xyui-text-label" } },
                new TextBlock { Text = "切换同一区域内的平级多文档/视口，底部 Accent 线。", Classes = { "xyui-text-caption" }, TextWrapping = global::Avalonia.Media.TextWrapping.Wrap },
                new XYTab { Label = "Map.cs", IsSelected = true }
            }
        };

        var grid = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 16, Children = { col1, col2, col3 } };
        return WrapCard(grid, "核心语义对比 · Navigation ≠ Button List ≠ Tabs");
    }
}
