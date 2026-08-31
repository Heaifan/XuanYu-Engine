using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3GalleryCatalog
{
    static Control TabBarPreview()
    {
        var labels = new[] { "地图基础", "地图环境", "数据集", "Region.cs", "World.cs", "Scene.cs", "Camera.cs", "Light.cs", "材质", "纹理", "脚本", "日志" };
        var tabs = labels.Select((label, index) => new XYTab { Label = label, IsSelected = index == 1, IsModified = index == 2, IsClosable = index is > 1 and < 11 }).ToArray();
        var bar = new XYTabBar(tabs) { Width = 520 };
        var serial = labels.Length;
        bar.NewRequested += (_, _) => { var tab = new XYTab { Label = $"新页签-{++serial}", IsClosable = true }; bar.Tabs.Add(tab, true); bar.EnsureVisible(tab); };
        return bar;
    }

    static Control DockTabsPreview() => new XYDockTabs(
        Dock("Hierarchy"), Dock("Inspector", selected: true),
        Dock("Console", modified: true), Dock("Assets")) { Width = 676 };

    static XYDockTab Dock(string label, bool selected = false, bool modified = false) =>
        new(new XYTab { Label = label, IsSelected = selected, IsModified = modified, IsClosable = selected });

    static Control BreadcrumbPreview() => new XYBreadcrumb(
        new XYBreadcrumbItem { Label = "玄域项目" },
        new XYBreadcrumbItem { Label = "地图" },
        new XYBreadcrumbItem { IsCollapsed = true, HiddenPathOptions = ["中国", "华南"] },
        new XYBreadcrumbItem { Label = "行政区" },
        new XYBreadcrumbItem { Label = "广东省", IsCurrent = true, HasDropdown = true, DropdownOptions = ["广东省", "广西", "福建", "湖南"] }) { Width = 696 };

    static Control TreeNavigationPreview() => new XYTreeNavigation(
        Node("玄域项目", 0, XyuiVectorIcon.Section, children: true, expanded: true),
        Node("地图", 1, XyuiVectorIcon.Locate, children: true, expanded: true, active: 1),
        Node("数据集", 2, XyuiVectorIcon.Section, children: true, expanded: true, active: 2),
        Node("行政区", 3, XyuiVectorIcon.Section, selected: true, active: 3),
        Node("广东省", 3, XyuiVectorIcon.StatusDot, active: 2),
        Node("资源", 1, XyuiVectorIcon.Browse, children: true),
        Node("模型", 2, XyuiVectorIcon.Section), Node("材质", 2, XyuiVectorIcon.Section))
    { Width = 392, Height = 404, Padding = new Thickness(12) };

    static XYTreeNode Node(string label, int depth, XyuiVectorIcon icon, bool children = false, bool expanded = false, bool selected = false, int active = 0) =>
        new() { Label = label, Depth = depth, Icon = icon, HasChildren = children, IsExpanded = expanded, IsSelected = selected, ActiveGuideDepth = active };

    static Control PaginationPreview() => new StackPanel { Spacing = 14, Children = { new XYPagination { CurrentPage = 3, TotalPages = 24, TotalItems = 468, ShowTotalItems = true }, new XYPagination { CurrentPage = 1, TotalPages = 24 }, new XYPaginationFooter() } };
    static Control StepsPreview() { var states = new[] { ("创建项目", XYStepState.Completed), ("地图设置", XYStepState.Completed), ("数据配置", XYStepState.Current), ("验证", XYStepState.Pending), ("完成", XYStepState.Pending) }; var horizontal = states.Select(x => new XYStepNode(x.Item1, x.Item2)).ToArray(); var vertical = states.Select(x => new XYStepNode(x.Item1, x.Item2)).ToArray(); return new StackPanel { Spacing = 20, Children = { new XYSteps(horizontal), new XYSteps(vertical) { Orientation = XYStepsOrientation.Vertical } } }; }
    static Control ToolbarPreview() => new XYToolbar(new XYToolbarTool { Label = "选择", Icon = XyuiVectorIcon.Locate }, new XYToolbarTool { Label = "移动", Icon = XyuiVectorIcon.Locate, IsSelected = true }, new XYToolbarTool { Label = "旋转", Icon = XyuiVectorIcon.StatusDot }, new XYToolbarTool { Label = "缩放", Icon = XyuiVectorIcon.Section }, new XYSeparator { Variant = XyuiSeparatorVariant.VerticalSplit, Height = 24 }, new XYToolbarTool { Label = "区域" }, new XYToolbarTool { Label = "道路" }) { Width = 740 };
    static Control ToolGroupPreview() => new XYToolbar(new XYToolGroup(new XYToolbarTool { Label = "选择" }, new XYToolbarTool { Label = "移动", IsSelected = true }, new XYToolbarTool { Label = "旋转" }, new XYToolbarTool { Label = "缩放" }), new XYSeparator { Variant = XyuiSeparatorVariant.VerticalSplit, Height = 24 }, new XYToolGroup(new XYToolbarTool { Label = "区域" }, new XYToolbarTool { Label = "道路" }), new XYToolGroup(new XYToolbarTool { Label = "移动", IsSelected = true }) { IsCollapsed = true }) { Width = 740 };
}
