using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3GalleryCatalog
{
    static Control TabBarPreview() => new XYTabBar(
        new XYTab { Label = "地图基础", IsClosable = false },
        new XYTab { Label = "地图环境", IsSelected = true },
        new XYTab { Label = "数据集", IsModified = true, IsClosable = false },
        new XYTab { Label = "Region.cs", IsClosable = false },
        new XYTab { Label = "World.cs", IsClosable = false }) { Width = 736 };

    static Control DockTabsPreview() => new XYDockTabs(
        Dock("Hierarchy"), Dock("Inspector", selected: true),
        Dock("Console", modified: true), Dock("Assets")) { Width = 676 };

    static XYDockTab Dock(string label, bool selected = false, bool modified = false) =>
        new(new XYTab { Label = label, IsSelected = selected, IsModified = modified, IsClosable = selected });

    static Control BreadcrumbPreview() => new XYBreadcrumb(
        new XYBreadcrumbItem { Label = "玄域项目" },
        new XYBreadcrumbItem { Label = "地图" },
        new XYBreadcrumbItem { IsCollapsed = true },
        new XYBreadcrumbItem { Label = "行政区" },
        new XYBreadcrumbItem { Label = "广东省", IsCurrent = true, HasDropdown = true }) { Width = 696 };

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
}
