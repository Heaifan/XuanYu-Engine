using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3LiveExamplesFactory
{
    static Control CreateWorkspaceSwitcherLiveExamples()
    {
        var items = new[]
        {
            new XYWorkspaceItem("world-edit", "World Editor / 世界编辑", Icon: XYUI.Avalonia.Vector.XyuiVectorIcon.Locate),
            new XYWorkspaceItem("map-data", "Map Data / 地图数据", Icon: XYUI.Avalonia.Vector.XyuiVectorIcon.Code),
            new XYWorkspaceItem("war-sim", "War Simulation / 战争模拟", Icon: XYUI.Avalonia.Vector.XyuiVectorIcon.Eye),
            new XYWorkspaceItem("debug-analysis", "Debug / 调试分析 (已禁用)", IsEnabled: false, Icon: XYUI.Avalonia.Vector.XyuiVectorIcon.Section)
        };
        var state = new XYWorkspaceState("world-edit");
        var switcher = new XYWorkspaceSwitcher(state, items) { Width = 280 };

        var feedback = new TextBlock { Text = "当前工作区: [World Editor / 世界编辑] · 上下文: 2D/3D场景视口 + 场景树 + 实体检查器", Classes = { "xyui-text-caption" } };
        switcher.WorkspaceChangeRequested += (_, req) => req.Accept();
        switcher.WorkspaceChanged += (_, id) =>
        {
            var desc = id switch
            {
                "world-edit" => "2D/3D场景视口 + 场景树 + 实体检查器",
                "map-data" => "属性网格 + 地图要素表 + 拓扑校验器",
                "war-sim" => "态势地图 + 兵棋推演面板 + 仿真控制台",
                _ => "性能监视器 + 运行时日志 + 内存分析工具"
            };
            feedback.Text = $"当前工作区: [{switcher.CurrentWorkspace}] · 上下文: {desc}";
        };
        switcher.ManageRequested += (_, _) => feedback.Text = "管理操作: 触发 [管理工作区...] 全局配置弹窗";

        var btnWar = new XYButton { Content = "切至战争模拟", Variant = XyuiButtonVariant.Secondary };
        btnWar.Click += (_, _) => switcher.SelectWorkspace("war-sim");
        var btnData = new XYButton { Content = "切至地图数据", Variant = XyuiButtonVariant.Secondary };
        btnData.Click += (_, _) => switcher.SelectWorkspace("map-data");
        var btnDisabled = new XYButton { Content = "切至禁用项 (拦截)", Variant = XyuiButtonVariant.Secondary };
        btnDisabled.Click += (_, _) => { switcher.SelectWorkspace("debug-analysis"); feedback.Text = $"尝试切换至禁用项: 状态保持在 [{switcher.CurrentWorkspace}] (拦截生效)"; };
        var btnOpen = new XYButton { Content = "展开下拉菜单", Variant = XyuiButtonVariant.Secondary };
        btnOpen.Click += (_, _) => switcher.Open();

        var col = new StackPanel
        {
            Spacing = 12,
            Children =
            {
                new TextBlock { Text = "顶栏工作区切换器 (点击触发器展开同宽菜单，当前项带右侧勾选):", Classes = { "xyui-text-label" } },
                switcher,
                feedback,
                new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Children = { btnWar, btnData, btnDisabled, btnOpen } }
            }
        };
        return WrapCard(col, "应用级工作区环境切换 · Request-Commit 事务机制与同宽下拉");
    }

    static Control CreateWorkspaceSwitcherComposition()
    {
        var items = new[]
        {
            new XYWorkspaceItem("world-edit", "World Editor", Icon: XYUI.Avalonia.Vector.XyuiVectorIcon.Locate),
            new XYWorkspaceItem("map-data", "Map Data", Icon: XYUI.Avalonia.Vector.XyuiVectorIcon.Code),
            new XYWorkspaceItem("war-sim", "War Simulation", Icon: XYUI.Avalonia.Vector.XyuiVectorIcon.Eye),
            new XYWorkspaceItem("debug-analysis", "Debug Analysis (Disabled)", IsEnabled: false, Icon: XYUI.Avalonia.Vector.XyuiVectorIcon.Section)
        };
        var switcher = new XYWorkspaceSwitcher(new XYWorkspaceState("world-edit"), items) { Width = 200 };
        switcher.WorkspaceChangeRequested += (_, req) => req.Accept();

        var logo = new TextBlock { Text = "玄域引擎 XuanYu", Classes = { "xyui-text-label" }, VerticalAlignment = VerticalAlignment.Center, Margin = new(0, 0, 16, 0) };
        var status = new TextBlock { Text = "● 渲染线程 60 FPS", Classes = { "xyui-text-caption" }, VerticalAlignment = VerticalAlignment.Center };

        var topBar = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12,
            Children = { logo, switcher, status }
        };

        var panel = new StackPanel
        {
            Spacing = 10,
            Children =
            {
                new Border { Classes = { "xyui-surface-panel-alt" }, Padding = new(12, 8), CornerRadius = new(4), Child = topBar },
                new TextBlock { Text = "说明: 工作区切换器部署于全局顶栏中央，切换工作区会触发整套 Dock/Toolbar 布局重载。", Classes = { "xyui-text-caption" } }
            }
        };
        return WrapCard(panel, "主界面顶栏全局工作区集成 · 品牌标识、环境身份与状态监控同轴呈现");
    }
}
