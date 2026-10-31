using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3LiveExamplesFactory
{
    static Control CreateNavigationDrawerLiveExamples()
    {
        var state = new XYNavigationState([
            new("map", "地图编辑", XyuiVectorIcon.Locate),
            new("data", "数据配置", XyuiVectorIcon.Code),
            new("settings", "系统设置", XyuiVectorIcon.Section)
        ], "map");
        var drawer = new XYNavigationDrawer(state);

        var feedback = new TextBlock { Text = "抽屉状态: 已收起 · 点击 [☰ 打开抽屉] 展开模态浮层", Classes = { "xyui-text-caption" } };
        drawer.Closed += (_, _) => feedback.Text = $"抽屉状态: 已收起 · 当前目的地: [{state.SelectedId}] (焦点已归还至触发器)";
        state.Changed += (_, _) => feedback.Text = $"抽屉内目的地切换至: [{state.SelectedId}] · 主视口已同步";

        var btnOpen = new XYButton { Content = "☰ 打开抽屉", Variant = XyuiButtonVariant.Primary };
        btnOpen.Click += (_, _) => { drawer.Open(); feedback.Text = "抽屉状态: 已展开 · 半透明遮罩覆盖视口，点击遮罩或按 Esc 即可收起"; };

        var pageMock = new Border { Classes = { "xyui-surface-panel-alt" }, Height = 130, CornerRadius = new(4), Padding = new(14), Child = new StackPanel { Spacing = 6, Children = { new TextBlock { Text = "世界编辑器 · 地图数据工作区", Classes = { "xyui-text-label" } }, new TextBlock { Text = "抽屉展开时遮罩覆盖整个视口，底层内容清晰可见但阻断交互。", Classes = { "xyui-text-caption" } } } } };
        var col = new Border
        {
            Classes = { "xyui-navigation-drawer-host" }, Width = 640, Height = 270,
            Child = new StackPanel { Spacing = 10, Children = { new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12, Children = { btnOpen, drawer, feedback } }, pageMock } }
        };
        return WrapCard(col, "响应式模态抽屉导航 · 侧边栏结构镜像、全视口遮罩与键盘生命周期");
    }

    static Control CreateNavigationDrawerComposition()
    {
        var state = new XYNavigationState([new("map", "地图", XyuiVectorIcon.Locate), new("data", "数据", XyuiVectorIcon.Code), new("settings", "设置", XyuiVectorIcon.Section)], "map");
        var drawer = new XYNavigationDrawer(state);

        var burger = new XYButton { Content = "☰ 导航", Variant = XyuiButtonVariant.Secondary };
        burger.Click += (_, _) => drawer.Open();

        var topBar = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto"), Height = 36,
            Children = { burger, new TextBlock { Text = "窄屏编辑器视口 (呼出抽屉时半透明遮罩覆盖视口)", VerticalAlignment = VerticalAlignment.Center, Classes = { "xyui-text-caption" }, Margin = new(12, 0) } }
        };
        Grid.SetColumn((TextBlock)topBar.Children[1], 1);

        var canvasMock = new Border { Classes = { "xyui-surface-panel-alt" }, Height = 170, CornerRadius = new(4), Padding = new(16), Child = new TextBlock { Text = "【视口工作区】\n展开抽屉时此处被半透明遮罩覆盖，点击右侧遮罩或按 Esc 键即可收起。", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Classes = { "xyui-text-caption" } } };

        var mockViewport = new Border
        {
            Classes = { "xyui-navigation-drawer-host" }, Width = 640, Height = 250,
            Child = new StackPanel { Spacing = 8, Children = { topBar, drawer, canvasMock } }
        };
        return WrapCard(mockViewport, "窄屏视口汉堡菜单与抽屉协同 · 完整 Viewport 模态遮罩与收拢形态");
    }
}
