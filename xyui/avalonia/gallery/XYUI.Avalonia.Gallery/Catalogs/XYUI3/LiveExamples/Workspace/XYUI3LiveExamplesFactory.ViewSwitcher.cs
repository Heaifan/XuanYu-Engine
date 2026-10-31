using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3LiveExamplesFactory
{
    static Control CreateViewSwitcherLiveExamples()
    {
        var views = new[]
        {
            new XYViewDefinition("map", "Map / 地图画布", XyuiVectorIcon.Locate, Priority: 3),
            new XYViewDefinition("data", "Data / 属性表格", XyuiVectorIcon.Section, Priority: 2),
            new XYViewDefinition("hierarchy", "Hierarchy / 层级视图", XyuiVectorIcon.Eye, Priority: 1),
            new XYViewDefinition("log", "Log / 诊断日志", XyuiVectorIcon.Code, Priority: -1)
        };
        var state = new XYViewState(views, "map");
        var segmented = new XYViewSwitcher(state, XYViewSwitcherVariant.Segmented);
        var dropdown = new XYViewSwitcher(state, XYViewSwitcherVariant.Dropdown);
        var more = new XYViewSwitcher(state, XYViewSwitcherVariant.PrimaryMore);

        foreach (var sw in new[] { segmented, dropdown, more })
            sw.ViewChangeRequested += (_, req) => req.Accept();

        var viewCard = new Border { Classes = { "xyui-surface-panel-alt" }, Padding = new(14, 10), CornerRadius = new(4) };
        var viewText = new TextBlock { Classes = { "xyui-text-caption" } };
        viewCard.Child = viewText;

        void SyncView()
        {
            var modeDesc = state.CurrentViewId switch
            {
                "map" => "【地图画布】模式 · 渲染 2D/3D 地形高度网格、光影与相机视口",
                "data" => "【属性表格】模式 · 列表呈现实体数值属性、材质索引与变换矩阵",
                "hierarchy" => "【层级视图】模式 · 树形大纲查看父子从属关系与挂载组件",
                _ => "【诊断日志】模式 · 输出视口渲染线程耗时、着色器编译与调用批次"
            };
            viewText.Text = $"观察目标: Core_Fortress_01 · {modeDesc}\n(提示: 核心数据源保持绝对不变，三种变体双向同步当前观察模式)";
        }
        state.Changed += (_, _) => SyncView();
        SyncView();

        var col = new StackPanel
        {
            Spacing = 12,
            Children =
            {
                new TextBlock { Text = "分段式变体 (Segmented · 30 DIP 项高 + 底部 Accent 指示条):", Classes = { "xyui-text-label" } },
                segmented,
                new TextBlock { Text = "下拉式与高低频变体 (Dropdown / Primary + More 共享同一 ViewState):", Classes = { "xyui-text-label" } },
                new StackPanel { Orientation = Orientation.Horizontal, Spacing = 16, Children = { dropdown, more } },
                viewCard
            }
        };
        return WrapCard(col, "同一内容不同视图模式切换 · Segmented / Dropdown / PrimaryMore 共享状态联动");
    }

    static Control CreateViewSwitcherComposition()
    {
        var views = new[]
        {
            new XYViewDefinition("canvas", "画布", XyuiVectorIcon.Locate),
            new XYViewDefinition("wireframe", "线框", XyuiVectorIcon.Section),
            new XYViewDefinition("stats", "统计", XyuiVectorIcon.Eye)
        };
        var state = new XYViewState(views, "canvas");
        var switcher = new XYViewSwitcher(state, XYViewSwitcherVariant.Segmented);
        switcher.ViewChangeRequested += (_, req) => req.Accept();

        var title = new TextBlock { Text = "视口工作区 · [战役沙盘_01]", Classes = { "xyui-text-label" }, VerticalAlignment = VerticalAlignment.Center };
        var header = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto"),
            Height = 36,
            Children = { title, switcher }
        };
        Grid.SetColumn(switcher, 2);

        var viewport = new Border
        {
            Height = 100,
            Classes = { "xyui-surface-panel-alt" },
            CornerRadius = new(4),
            Child = new TextBlock { Text = "同一场景对象视口 · 切换观察模式不卸载已加载资源", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Classes = { "xyui-text-caption" } }
        };

        var panel = new StackPanel { Spacing = 8, Children = { header, viewport } };
        return WrapCard(panel, "视口工具栏观察模式集成 · 区分 Tabs 文档生命周期与视图表现切换");
    }
}
