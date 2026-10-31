using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3LiveExamplesFactory
{
    static Control CreateToolbarLiveExamples()
    {
        var toolSelect = new XYToolbarTool { Label = "框选", Icon = XyuiVectorIcon.Locate, ToolId = "tool-select" };
        var toolMove = new XYToolbarTool { Label = "移动", Icon = XyuiVectorIcon.Locate, ToolId = "tool-move", IsSelected = true };
        var toolRotate = new XYToolbarTool { Label = "旋转", Icon = XyuiVectorIcon.StatusDot, ToolId = "tool-rotate" };
        var toolScale = new XYToolbarTool { Label = "缩放", Icon = XyuiVectorIcon.Section, ToolId = "tool-scale" };
        var toolBrush = new XYToolbarTool { Label = "地形画刷", Icon = XyuiVectorIcon.Code, ToolId = "tool-brush" };
        var toolSample = new XYToolbarTool { Label = "标高取样", Icon = XyuiVectorIcon.Eye, ToolId = "tool-sample" };

        var toolbar = new XYToolbar(
            toolSelect, toolMove, toolRotate, toolScale,
            new XYSeparator { Variant = XyuiSeparatorVariant.VerticalSplit, Height = 24 },
            toolBrush, toolSample) { IsCompact = true };

        var feedback = new TextBlock { Text = "当前活动工具: tool-move (移动) · 34 DIP 紧凑图标优先模式", Classes = { "xyui-text-caption" } };

        void BindTool(XYToolbarTool t) => t.SelectionRequested += (_, _) => feedback.Text = $"当前活动工具: {t.ToolId} ({t.Label})";
        BindTool(toolSelect); BindTool(toolMove); BindTool(toolRotate); BindTool(toolScale); BindTool(toolBrush); BindTool(toolSample);

        var labeledBar = new XYToolbar(
            new XYToolbarTool { Label = "全选", Icon = XyuiVectorIcon.Locate, IsSelected = true },
            new XYToolbarTool { Label = "对齐", Icon = XyuiVectorIcon.Section },
            new XYToolbarTool { Label = "分布", Icon = XyuiVectorIcon.StatusDot }) { IsCompact = false };

        var root = new StackPanel
        {
            Spacing = 14,
            Children =
            {
                new TextBlock { Text = "紧凑图标模式 (IsCompact=True，支持点击互斥切换与高亮激活):", Classes = { "xyui-text-label" } },
                toolbar, feedback,
                new TextBlock { Text = "文字标签模式 (IsCompact=False，适合展开式工具栏):", Classes = { "xyui-text-label" } },
                labeledBar
            }
        };
        return WrapCard(root, "高密度视口工具栏 · 图标优先、极窄间距与激活互斥");
    }

    static Control CreateToolbarComposition()
    {
        var bar = new XYToolbar(
            new XYToolbarTool { Label = "漫游", Icon = XyuiVectorIcon.Locate, IsSelected = true },
            new XYToolbarTool { Label = "框选", Icon = XyuiVectorIcon.Section },
            new XYSeparator { Variant = XyuiSeparatorVariant.VerticalSplit, Height = 20 },
            new XYToolbarTool { Label = "网格吸附", Icon = XyuiVectorIcon.Code },
            new XYToolbarTool { Label = "标尺", Icon = XyuiVectorIcon.Eye }) { IsCompact = true };

        var coords = new TextBlock { Text = "X: 124.50  Y: -48.20  Z: 0.00 | 网格: 1.0 DIP", Classes = { "xyui-text-code" }, VerticalAlignment = VerticalAlignment.Center };
        var header = new Grid { ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto"), Children = { bar, coords } };
        Grid.SetColumn(coords, 2);

        var canvasMock = new Border
        {
            Classes = { "xyui-surface-panel-alt" },
            Height = 120,
            Child = new TextBlock { Text = "[ 3D 场景主视口画布 · 工具栏常驻顶栏提供无感快捷模式切换 ]", Classes = { "xyui-text-caption" }, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }
        };

        var panel = new StackPanel { Spacing = 8, Width = 680, Children = { header, canvasMock } };
        return WrapCard(panel, "3D 编辑视口顶栏 · 工具栏与视口状态协同");
    }
}
