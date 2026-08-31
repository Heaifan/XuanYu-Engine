using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3GalleryCatalog
{
    public static Control CreatePreview(string id) => id switch
    {
        "XYUI-3-3.01" => MenuBarPreview(), "XYUI-3-3.02" => MenuPreview(),
        "XYUI-3-3.03" => ContextPreview(), "XYUI-3-3.04" => SubMenuPreview(), "XYUI-3-3.05" => NavigationMenuPreview(), "XYUI-3-3.06" => SidebarPreview(), "XYUI-3-3.07" => RailPreview(), "XYUI-3-3.08" => TabsPreview(),
        "XYUI-3-3.09" => TabBarPreview(), "XYUI-3-3.10" => DockTabsPreview(), "XYUI-3-3.11" => BreadcrumbPreview(), "XYUI-3-3.12" => TreeNavigationPreview(), "XYUI-3-3.13" => PaginationPreview(), "XYUI-3-3.14" => StepsPreview(), "XYUI-3-3.15" => ToolbarPreview(), "XYUI-3-3.16" => ToolGroupPreview(), "XYUI-3-3.17" => CommandBarPreview(), "XYUI-3-3.18" => CommandPalettePreview(), "XYUI-3-3.19" => BackForwardPreview(), "XYUI-3-3.20" => WorkspacePreview(),
        _ => new TextBlock { Text = "未注册组件" }
    };
    static Control MenuBarPreview()
    {
        var panel = new StackPanel { Spacing = 14 }; panel.Children.Add(new XYMenuBar(
            new XYMenuBarItem { Label = "文件", Menu = MenuPreview("文件") }, new XYMenuBarItem { Label = "编辑", IsHovered = true, Menu = MenuPreview("编辑") },
            new XYMenuBarItem { Label = "视图", IsActive = true, Menu = MenuPreview("视图") }, new XYMenuBarItem { Label = "窗口", Menu = MenuPreview("窗口") }, new XYMenuBarItem { Label = "帮助", Menu = MenuPreview("帮助") }));
        panel.Children.Add(new XYCaption { Text = "编辑 = Hover · 视图 = Active" }); return panel;
    }
    static XYMenu MenuPreview(string title = "") => new XYMenu(
        Item("新建", "Ctrl+N"), Item("打开", "Ctrl+O", hover: true), Item("保存", "Ctrl+S", hover: true),
        XYMenu.Separator(), Item("显示网格", checkedItem: true, check: XyuiMenuCheckKind.Check), Item("正交视图", checkedItem: true, check: XyuiMenuCheckKind.Radio), Item("投影模式", submenu: true),
        XYMenu.Separator(), Item("关闭所有窗口", enabled: false), XYMenu.Separator(), Item("删除", destructive: true));
    static Control ContextPreview()
    {
        var target = new Border { Padding = new Thickness(10, 6), Child = new XYCaption { Text = "右键对象：Infantry_023" } };
        var context = new XYContextMenu { ContextType = "ENTITY", ContextName = "Infantry_023", Menu = new XYMenu(
            Item("定位", hover: true), Item("编辑"), Item("重命名"), XYMenu.Separator(), Item("复制"), Item("移动到", submenu: true), XYMenu.Separator(), Item("删除", destructive: true)) };
        context.AttachTo(target);
        return new StackPanel { Spacing = 8, Children = { target, context } };
    }
    static Control NavigationMenuPreview() => new XYNavigationMenu(
        XYNavigationMenu.Group("工作区", new XYNavigationItem { Id = "map", Label = "地图", Icon = XyuiVectorIcon.Locate, IsSelected = true },
            new XYNavigationItem { Id = "environment", Label = "环境", Icon = XyuiVectorIcon.Eye }, new XYNavigationItem { Id = "data", Label = "数据", Icon = XyuiVectorIcon.Code }, new XYNavigationItem { Id = "resources", Label = "资源", Icon = XyuiVectorIcon.Browse }),
        XYNavigationMenu.Group("工具", new XYNavigationItem { Id = "scripts", Label = "脚本", Icon = XyuiVectorIcon.Code }, new XYNavigationItem { Id = "debug", Label = "调试", Icon = XyuiVectorIcon.Search }),
        XYNavigationMenu.Group("", new XYNavigationItem { Id = "settings", Label = "设置", Icon = XyuiVectorIcon.Section })) { Width = 246, SelectedId = "map" };
    static Control SidebarPreview() { var sidebar = new XYSidebar { PrimaryItems = Primary(), ContextItems = Context() }; sidebar.Build(); return sidebar; }
    static Control RailPreview() => new XYNavigationRail(new XYNavigationState(Primary().Select(x => new XYNavigationEntry(x.Id, x.Label, x.Icon)), "map"), RailContexts()) { Width = 54 };
    static Control TabsPreview() => new XYTabs(new XYTab { Label = "地图基础" }, new XYTab { Label = "地图环境", IsSelected = true }, new XYTab { Label = "数据集", IsModified = true }, new XYTab { Label = "区域编辑" });
    static IReadOnlyList<XYNavigationItem> Primary() => [new() { Id = "map", Label = "地图", Icon = XyuiVectorIcon.Locate, IsSelected = true }, new() { Id = "environment", Label = "环境", Icon = XyuiVectorIcon.Eye }, new() { Id = "data", Label = "数据", Icon = XyuiVectorIcon.Code }];
    static IReadOnlyList<XYNavigationItem> Context() => [new() { Id = "base", Label = "地图基础", Icon = XyuiVectorIcon.Section }, new() { Id = "world", Label = "地图环境", Icon = XyuiVectorIcon.Section }, new() { Id = "dataset", Label = "数据集", Icon = XyuiVectorIcon.Section }];
    static IReadOnlyDictionary<string, IReadOnlyList<XYNavigationEntry>> RailContexts() => new Dictionary<string, IReadOnlyList<XYNavigationEntry>>
    {
        ["map"] = [new("base", "地图基础", XyuiVectorIcon.Section), new("world", "地图环境", XyuiVectorIcon.Section)],
        ["environment"] = [new("world", "环境设置", XyuiVectorIcon.Section)], ["data"] = [new("dataset", "数据集管理", XyuiVectorIcon.Section)]
    };
    static Control SubMenuPreview() => new StackPanel { Spacing = 12, Children = { new XYSubMenu { ParentMenu = Parent(), ChildMenu = Child() }, new XYCaption { Text = "Open Left · 静态镜像 Variant" }, new XYSubMenu { OpenLeft = true, ParentMenu = Parent(), ChildMenu = Child() }, new XYSubMenuHierarchyDebugPreview() } };
    static XYMenu Parent() => new(Item("打开"), Item("保存"), Item("导出", hover: true, submenu: true), XYMenu.Separator(), Item("关闭"));
    static XYMenu Child() => new(Item("导出地图数据"), Item("导出图片"), Item("导出配置"), XYMenu.Separator(), Item("高级导出", submenu: true));
    static XYMenuItem Item(string label, string shortcut = "", bool enabled = true, bool checkedItem = false,
        XyuiMenuCheckKind check = XyuiMenuCheckKind.None, bool destructive = false, bool hover = false, bool submenu = false) => new()
    { Label = label, Shortcut = shortcut, IsEnabled = enabled, IsChecked = checkedItem, CheckKind = check, IsDestructive = destructive, IsHovered = hover, HasSubMenu = submenu };
}
