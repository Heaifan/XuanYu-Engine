using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3LiveExamplesFactory
{
    static Control CreateTreeNavigationLiveExamples()
    {
        var mapRoot = new XYTreeNode { Label = "地图系统", Icon = XyuiVectorIcon.Locate, IsExpanded = true };
        var nodeBase = new XYTreeNode { Label = "基础要素", IsSelected = true, Icon = XyuiVectorIcon.Section };
        var nodeEnv = new XYTreeNode { Label = "环境配置", Icon = XyuiVectorIcon.Eye, IsExpanded = true, Badge = "警告", Status = XyuiStatusState.Warning };
        var nodeTerrain = new XYTreeNode { Label = "地形高度图", Icon = XyuiVectorIcon.Section };
        var nodeClimate = new XYTreeNode { Label = "气候与光照", Icon = XyuiVectorIcon.Section, IsEnabled = false };
        nodeEnv.Children.Add(nodeTerrain);
        nodeEnv.Children.Add(nodeClimate);
        mapRoot.Children.Add(nodeBase);
        mapRoot.Children.Add(nodeEnv);

        var dataRoot = new XYTreeNode { Label = "数据集合", Icon = XyuiVectorIcon.Code, Badge = "12", Status = XyuiStatusState.Info };

        var tree = new XYTreeNavigation(mapRoot, dataRoot) { Width = 280 };

        var statusText = new TextBlock { Text = "当前选中：基础要素 (层级深度由 Children 自动推导)", Classes = { "xyui-text-caption" } };
        var eventLog = new TextBlock { Text = "操作指南：气候与光照已禁用 (Disabled)；环境配置呈现清晰 Warning 徽标 (警告)；数据集合带 12 计数徽标。", Classes = { "xyui-text-caption" } };

        tree.SelectionChanged += (_, node) => statusText.Text = $"当前选中：{node.Label} (层级深度: {node.Depth}, Badge: {node.Badge ?? "无"}, Status: {node.Status})";

        var infoPanel = new StackPanel { Spacing = 8, Width = 300, Children = { statusText, eventLog } };
        var host = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 20, Children = { tree, infoPanel } };
        return WrapCard(host, "高密度树状导航 · 嵌套 Children 驱动、Disabled/Badge/Status 真实状态");
    }

    static Control CreateTreeNavigationComposition()
    {
        var col1 = new StackPanel
        {
            Spacing = 6, Width = 260,
            Children =
            {
                new TextBlock { Text = "高密度树结构 (28 DIP 行高)", Classes = { "xyui-text-label" } },
                new TextBlock { Text = "16 DIP 缩进，弱化引导线，左侧 Accent Bar。层级来自 Children 嵌套关系，不从手写 Depth 构造。", Classes = { "xyui-text-caption" }, TextWrapping = global::Avalonia.Media.TextWrapping.Wrap },
                new Border
                {
                    Classes = { "xyui-surface-panel" },
                    Padding = new(8),
                    Child = new TextBlock { Text = "├─ 场景根节点 (Children 驱动)\n│  ├─ 静态几何体 (自动 Depth 1)\n│  └─ 动态角色层 (自动 Depth 1)", Classes = { "xyui-text-code" } }
                }
            }
        };

        var col2 = new StackPanel
        {
            Spacing = 6, Width = 280,
            Children =
            {
                new TextBlock { Text = "要素树与属性视口联动模式", Classes = { "xyui-text-label" } },
                new TextBlock { Text = "树导航作为左侧层级事实源，驱动中央画布对象选中与右侧属性栏同步更新。", Classes = { "xyui-text-caption" }, TextWrapping = global::Avalonia.Media.TextWrapping.Wrap },
                new Border
                {
                    Classes = { "xyui-surface-panel" },
                    Padding = new(8),
                    Child = new TextBlock { Text = "[ 树节点选中 ] ➔ 驱动 ➔ [ Inspector 属性更新 ]", Classes = { "xyui-text-caption" } }
                }
            }
        };

        var grid = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 20, Children = { col1, col2 } };
        return WrapCard(grid, "高密度工程场景 · 树导航在编辑器工作流中的协同角色");
    }
}
