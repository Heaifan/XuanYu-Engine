using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3GalleryCatalog
{
    static XYViewDefinition View(string id, string label, XyuiVectorIcon icon, int priority = 0) => new(id, label, icon, Priority: priority);
    static Control ViewSwitcherPreview()
    {
        var views = new[] { View("canvas", "画布", XyuiVectorIcon.Locate, 3), View("table", "表格", XyuiVectorIcon.Section, 2), View("preview", "预览", XyuiVectorIcon.Eye, 1), View("logs", "日志", XyuiVectorIcon.Code, -1) };
        var state = new XYViewState(views, "canvas"); var segmented = new XYViewSwitcher(state); var dropdown = new XYViewSwitcher(state, XYViewSwitcherVariant.Dropdown); var more = new XYViewSwitcher(state, XYViewSwitcherVariant.PrimaryMore);
        foreach (var switcher in new[] { segmented, dropdown, more }) switcher.ViewChangeRequested += (_, request) => request.Accept();
        return new StackPanel { Spacing = 10, Children = { segmented, dropdown, more } };
    }
    static Control TocPreview()
    {
        var sections = new[] { new XYTocSection("intro", "概览", 1), new XYTocSection("setup", "配置", 1), new XYTocSection("map", "地图编辑", 2, "setup"), new XYTocSection("data", "数据集", 2, "setup"), new XYTocSection("api", "API", 1) };
        var state = new XYTableOfContentsState(sections, "data"); var hierarchy = new XYTableOfContents(state); var compact = new XYTableOfContents(state, XYTableOfContentsVariant.Compact);
        return new StackPanel { Spacing = 10, Children = { hierarchy, compact } };
    }
    static Control BottomNavigationPreview()
    {
        var state = new XYNavigationState([new("home", "首页", XyuiVectorIcon.Locate), new("map", "地图", XyuiVectorIcon.Section), new("data", "数据", XyuiVectorIcon.Code)], "map");
        var primary = new XYButton { Content = new XYIcon { Icon = XyuiVectorIcon.Add, Size = XyuiIconSize.Small }, Variant = XyuiButtonVariant.Primary };
        var nav = new XYBottomNavigation(state, [new("home", "首页", XyuiVectorIcon.Locate), new("map", "地图", XyuiVectorIcon.Section), new("data", "数据", XyuiVectorIcon.Code, "3")], primary);
        return new StackPanel { Spacing = 8, Children = { nav, new XYCaption { Text = "等宽目的地 · Primary Action 独立" } } };
    }
    static Control NavigationDrawerPreview()
    {
        var state = new XYNavigationState([new("map", "地图", XyuiVectorIcon.Locate), new("data", "数据", XyuiVectorIcon.Code), new("settings", "设置", XyuiVectorIcon.Section)], "map");
        var drawer = new XYNavigationDrawer(state); drawer.Open();
        return new StackPanel { Spacing = 8, Children = { drawer, new XYCaption { Text = "Full Sidebar · Backdrop / Esc / LightDismiss" } } };
    }
}
