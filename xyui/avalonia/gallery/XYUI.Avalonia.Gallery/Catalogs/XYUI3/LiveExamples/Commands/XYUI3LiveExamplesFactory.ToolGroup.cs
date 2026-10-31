using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3LiveExamplesFactory
{
    static Control CreateToolGroupLiveExamples()
    {
        var t1 = new XYToolbarTool { Label = "选择", Icon = XyuiVectorIcon.Locate, ToolId = "g-select" };
        var t2 = new XYToolbarTool { Label = "平移", Icon = XyuiVectorIcon.Locate, ToolId = "g-move", IsSelected = true };
        var t3 = new XYToolbarTool { Label = "旋转", Icon = XyuiVectorIcon.StatusDot, ToolId = "g-rotate" };
        var groupTransform = new XYToolGroup(t1, t2, t3);

        var t4 = new XYToolbarTool { Label = "立方体", Icon = XyuiVectorIcon.Section, ToolId = "g-box" };
        var t5 = new XYToolbarTool { Label = "地表网格", Icon = XyuiVectorIcon.Eye, ToolId = "g-terrain" };
        var groupCreate = new XYToolGroup(t4, t5);

        var t6 = new XYToolbarTool { Label = "地形画刷", Icon = XyuiVectorIcon.Code, ToolId = "g-brush", IsSelected = true };
        var t7 = new XYToolbarTool { Label = "水体生成", Icon = XyuiVectorIcon.Section, ToolId = "g-water" };
        var groupBrush = new XYToolGroup(t6, t7) { IsCollapsed = true };
        ToolTip.SetTip(groupBrush.CollapsedTrigger, "画刷工具组 (点击展开更多 · 消费本组选中工具: 地形画刷)");

        var compactToolbar = new XYToolbar(groupTransform, groupCreate, groupBrush) { IsCompact = true };

        var l1 = new XYToolbarTool { Label = "选择", Icon = XyuiVectorIcon.Locate, ToolId = "l-select" };
        var l2 = new XYToolbarTool { Label = "平移", Icon = XyuiVectorIcon.Locate, ToolId = "l-move", IsSelected = true };
        var l3 = new XYToolbarTool { Label = "旋转", Icon = XyuiVectorIcon.StatusDot, ToolId = "l-rotate" };
        var labeledGroup = new XYToolGroup(l1, l2, l3);
        var labeledToolbar = new XYToolbar(labeledGroup) { IsCompact = false };

        var feedback = new TextBlock { Text = "状态：组 1 活动项为 [平移]；组 3 为折叠态，Trigger 消费本组选中项 [地形画刷] (非全局平移)", Classes = { "xyui-text-caption" } };

        void Track(XYToolbarTool t, XYToolGroup g) => t.SelectionRequested += (_, _) => feedback.Text = $"当前激活工具: {t.ToolId} ({t.Label}) | 本组独立活动项: {g.ActiveToolId ?? "无"}";
        Track(t1, groupTransform); Track(t2, groupTransform); Track(t3, groupTransform);
        Track(t4, groupCreate); Track(t5, groupCreate);
        Track(t6, groupBrush); Track(t7, groupBrush);
        Track(l1, labeledGroup); Track(l2, labeledGroup); Track(l3, labeledGroup);

        var toggleBtn = new XYButton { Content = "切换组 3 折叠/展开 (验证 Trigger 状态与组内独立选中)", Variant = XyuiButtonVariant.Secondary };
        toggleBtn.Click += (_, _) => groupBrush.IsCollapsed = !groupBrush.IsCollapsed;

        var panel = new StackPanel
        {
            Spacing = 12,
            Children =
            {
                new TextBlock { Text = "紧凑图标模式 (IsCompact=True，组内纯图标，折叠 Trigger 独立消费本组 [地形画刷]):", Classes = { "xyui-text-label" } },
                compactToolbar,
                new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10, Children = { toggleBtn } },
                new TextBlock { Text = "文字标签模式 (IsCompact=False，完整展示「选择 / 平移 / 旋转」，无窄槽截断):", Classes = { "xyui-text-label" } },
                labeledToolbar,
                feedback
            }
        };
        return WrapCard(panel, "工具组 · 组内状态独立、紧凑/文字模式传播与折叠 Trigger 语义");
    }

    static Control CreateToolGroupComposition()
    {
        var gNav = new XYToolGroup(new XYToolbarTool { Label = "聚焦", Icon = XyuiVectorIcon.Locate }, new XYToolbarTool { Label = "正交视图", Icon = XyuiVectorIcon.Eye, IsSelected = true });
        var gMod = new XYToolGroup(new XYToolbarTool { Label = "布尔差集", Icon = XyuiVectorIcon.Section }, new XYToolbarTool { Label = "倒角平滑", Icon = XyuiVectorIcon.StatusDot });
        var gExp = new XYToolGroup(new XYToolbarTool { Label = "拓扑分析", Icon = XyuiVectorIcon.Code, IsSelected = true }) { IsCollapsed = true };

        var bar = new XYToolbar(gNav, gMod, gExp) { IsCompact = true };
        var label = new TextBlock { Text = "几何体建模次级操作面板", Classes = { "xyui-text-label" } };
        var root = new StackPanel
        {
            Spacing = 8, Width = 640,
            Children =
            {
                label,
                new Border { Classes = { "xyui-surface-panel-alt" }, Padding = new(8), Child = bar },
                new TextBlock { Text = "注：不同几何阶段工具通过 ToolGroup 形成内聚段落，避免杂乱无章的大平铺。", Classes = { "xyui-text-caption" } }
            }
        };
        return WrapCard(root, "建模工具次级面板 · ToolGroup 阶段化聚合实践");
    }
}
