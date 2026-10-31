using XYUI.Avalonia.Catalog;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static readonly IReadOnlySet<string> BatchIds = new HashSet<string> { "XYUI-3-3.01", "XYUI-3-3.02", "XYUI-3-3.03", "XYUI-3-3.04", "XYUI-3-3.05", "XYUI-3-3.06", "XYUI-3-3.07", "XYUI-3-3.08", "XYUI-3-3.09", "XYUI-3-3.10", "XYUI-3-3.11", "XYUI-3-3.12", "XYUI-3-3.13", "XYUI-3-3.14", "XYUI-3-3.15", "XYUI-3-3.16", "XYUI-3-3.17", "XYUI-3-3.18", "XYUI-3-3.19", "XYUI-3-3.20", "XYUI-3-3.21", "XYUI-3-3.22", "XYUI-3-3.23", "XYUI-3-3.24" };
    public static IReadOnlyList<XYUI1ComponentDocument> Build() => XyuiCatalogSource.Load().Where(x => BatchIds.Contains(x.SourceItemId)).Select(Create).ToArray();
    static XYUI1ComponentDocument Create(XyuiCatalogEntry entry)
    {
        var type = entry.AvaloniaType.Split('.').Last(); if (string.IsNullOrWhiteSpace(type)) type = ComponentName(entry.SourceItemId);
        if (entry.SourceItemId == "XYUI-3-3.01") return BuildMenuBarDoc(entry.SourceItemId, type);
        if (entry.SourceItemId == "XYUI-3-3.02") return BuildMenuDoc(entry.SourceItemId, type);
        if (entry.SourceItemId == "XYUI-3-3.03") return BuildContextMenuDoc(entry.SourceItemId, type);
        if (entry.SourceItemId == "XYUI-3-3.04") return BuildSubMenuDoc(entry.SourceItemId, type);
        if (entry.SourceItemId == "XYUI-3-3.05") return BuildNavigationMenuDoc(entry.SourceItemId, type);
        if (entry.SourceItemId == "XYUI-3-3.06") return BuildSidebarDoc(entry.SourceItemId, type);
        if (entry.SourceItemId == "XYUI-3-3.07") return BuildNavigationRailDoc(entry.SourceItemId, type);
        if (entry.SourceItemId == "XYUI-3-3.08") return BuildTabsDoc(entry.SourceItemId, type);
        if (entry.SourceItemId == "XYUI-3-3.09") return BuildTabBarDoc(entry.SourceItemId, type);
        if (entry.SourceItemId == "XYUI-3-3.10") return BuildDockTabsDoc(entry.SourceItemId, type);
        if (entry.SourceItemId == "XYUI-3-3.11") return BuildBreadcrumbDoc(entry.SourceItemId, type);
        if (entry.SourceItemId == "XYUI-3-3.12") return BuildTreeNavigationDoc(entry.SourceItemId, type);
        if (entry.SourceItemId == "XYUI-3-3.13") return BuildPaginationDoc(entry.SourceItemId, type);
        if (entry.SourceItemId == "XYUI-3-3.14") return BuildStepsDoc(entry.SourceItemId, type);
        if (entry.SourceItemId == "XYUI-3-3.15") return BuildToolbarDoc(entry.SourceItemId, type);
        if (entry.SourceItemId == "XYUI-3-3.16") return BuildToolGroupDoc(entry.SourceItemId, type);
        if (entry.SourceItemId == "XYUI-3-3.17") return BuildCommandBarDoc(entry.SourceItemId, type);
        if (entry.SourceItemId == "XYUI-3-3.18") return BuildCommandPaletteDoc(entry.SourceItemId, type);
        if (entry.SourceItemId == "XYUI-3-3.19") return BuildBackForwardNavigationDoc(entry.SourceItemId, type);
        if (entry.SourceItemId == "XYUI-3-3.20") return BuildWorkspaceSwitcherDoc(entry.SourceItemId, type);
        if (entry.SourceItemId == "XYUI-3-3.21") return BuildViewSwitcherDoc(entry.SourceItemId, type);
        if (entry.SourceItemId == "XYUI-3-3.22") return BuildTableOfContentsDoc(entry.SourceItemId, type);
        if (entry.SourceItemId == "XYUI-3-3.23") return BuildBottomNavigationDoc(entry.SourceItemId, type);
        if (entry.SourceItemId == "XYUI-3-3.24") return BuildNavigationDrawerDoc(entry.SourceItemId, type);
        var details = Details(entry.SourceItemId);
        var acceptance = "UI IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE";
        return new(entry.SourceItemId, entry.Title.Split('/').Last().Trim(), type, details.Overview, details.WhenToUse,
            () => XYUI3GalleryCatalog.CreatePreview(entry.SourceItemId), details.Usages, details.Variants, details.States,
            Properties(entry.SourceItemId), entry.ApiRefs.Select(x => new XYUIDocToken(x, "Canonical", "Foundation token reference")).ToArray(), type)
        { CanonicalIdentity = entry.CanonicalIdentity, KnownGap = entry.KnownGap, Acceptance = acceptance };
    }
    static string ComponentName(string id) => id switch
    {
        "XYUI-3-3.05" => "XYNavigationMenu", "XYUI-3-3.06" => "XYSidebar",
        "XYUI-3-3.07" => "XYNavigationRail", "XYUI-3-3.08" => "XYTabs", "XYUI-3-3.09" => "XYTabBar", "XYUI-3-3.10" => "XYDockTabs", "XYUI-3-3.11" => "XYBreadcrumb", "XYUI-3-3.12" => "XYTreeNavigation", "XYUI-3-3.13" => "XYPagination", "XYUI-3-3.14" => "XYSteps", "XYUI-3-3.15" => "XYToolbar", "XYUI-3-3.16" => "XYToolGroup", "XYUI-3-3.17" => "XYCommandBar", "XYUI-3-3.18" => "XYCommandPalette", "XYUI-3-3.19" => "XYBackForwardNavigation", "XYUI-3-3.20" => "XYWorkspaceSwitcher", "XYUI-3-3.21" => "XYViewSwitcher", "XYUI-3-3.22" => "XYTableOfContents", "XYUI-3-3.23" => "XYBottomNavigation", "XYUI-3-3.24" => "XYNavigationDrawer", _ => ""
    };
    static (string Overview, string WhenToUse, string[] Usages, XYUIDocVariant[] Variants, XYUIDocState[] States) Details(string id) => id switch
    {
        "XYUI-3-3.13" => ("邻近页快速跳转与紧凑数据 Footer 组合的分页导航。", "用于资源搜索、日志历史和数据记录；支持前后页、邻近页、跳页输入与每页数量展示。", ["<c:XYPagination />", "<c:XYPaginationFooter />"], [new("Compact Neighbor", "34 DIP", "Light / Dark")], [new("Current", "Selected Surface + Accent"), new("Disabled", "边界按钮禁用")]),
        "XYUI-3-3.14" => ("用完成、当前、待执行状态表达连续流程的横向或纵向步骤导航。", "用于创建项目、导入资源和配置流程；同一状态数据可切换 Orientation。", ["<c:XYSteps />"], [new("Adaptive", "Horizontal / Vertical", "Light / Dark")], [new("Completed", "Vector status"), new("Current", "Inner indicator"), new("Pending", "Subtle border")]),
        "XYUI-3-3.15" => ("极简连续的编辑器工具栏，直接复用 XYIconButton 等基础动作控件。", "用于选择、移动、旋转、缩放及区域工具的紧凑排列。", ["<c:XYToolbar />"], [new("Compact Toolbar", "38 DIP", "Light / Dark")], [new("Active", "Selected Surface + Accent"), new("Hover", "浅色背景")]),
        "XYUI-3-3.16" => ("Toolbar 内部的工具组，提供分隔、浅 Hover 区域和静态折叠触发器。", "用于将变换工具与区域工具保持同一 Toolbar 层级；不承担 Flyout 生命周期。", ["<c:XYToolGroup />"], [new("Separator Group", "4 DIP Padding", "Light / Dark")], [new("Collapsed", "Trigger 保留 Active 语义"), new("Hover", "浅层组背景")]),
        "XYUI-3-3.17" => ("紧凑的一次性页面或对象命令栏。", "用于新建、导入、保存、验证和删除等命令。", ["<c:XYCommandBar />"], [new("Compact V2", "34 DIP Bar / 28 DIP Command", "Light / Dark")], [new("Primary", "强调主命令"), new("More", "XYMenu Popup")]),
        "XYUI-3-3.18" => ("紧凑快速命令搜索面板。", "用于过滤、键盘选择并执行全局命令。", ["<c:XYCommandPalette />"], [new("Compact V2", "440 DIP / 34 DIP Search", "Light / Dark")], [new("Recent", "空输入显示命令"), new("Selected", "键盘选中")]),
        "XYUI-3-3.19" => ("紧凑的前进后退导航历史。", "用于页面、对象和工作区位置之间的导航。", ["<c:XYBackForwardNavigation />"], [new("Compact V2", "34 DIP Bar / 28 DIP Action", "Light / Dark")], [new("Disabled", "无可用历史")]),
        "XYUI-3-3.20" => ("顶栏级紧凑工作区切换器。", "用于切换地图编辑、数据编辑、战争实验和调试工作区。", ["<c:XYWorkspaceSwitcher />"], [new("Compact V2", "34 DIP Trigger / 32 DIP Item", "Light / Dark")], [new("Selected", "当前工作区"), new("Popup", "同宽下拉")]),
        "XYUI-3-3.21" => ("共享 XYViewState 的视图切换器，支持分段、下拉和 Primary + More。", "用于同一页面内的主视图切换；所有变体共享 request→commit 状态。", ["<c:XYViewSwitcher />"], [new("Segmented", "36 DIP / 30 DIP Item", "Light / Dark"), new("Dropdown", "XYMenu Popup", "Light / Dark")], [new("Current", "当前视图"), new("Disabled", "不可用视图")]),
        "XYUI-3-3.22" => ("限深两级的章节目录导航，不承担 Tree 或 ScrollSpy。", "用于文档和长页章节跳转，层级状态由 XYTableOfContentsState 共享。", ["<c:XYTableOfContents />"], [new("Hierarchical", "Level 1 + Level 2", "Light / Dark"), new("Compact", "XYMenu Popup", "Light / Dark")], [new("Current", "当前章节"), new("Parent", "当前父章节")]),
        "XYUI-3-3.23" => ("移动端等宽目的地导航，Primary Action 与目的地状态分离。", "用于窄屏编辑器或预览壳的一级目的地切换；底部安全区由宿主提供。", BottomNavigationUsage(), [new("Equal Slots", "64 DIP Bar", "Light / Dark")], [new("Selected", "目的地选中"), new("Badge", "复用状态提示")]),
        _ => ("响应式导航抽屉，复用 Sidebar、NavigationMenu、SearchField 的真实能力。", "用于窄屏临时导航、上下文抽屉和边缘 Peek；具备遮罩、Esc、失焦和卸载关闭生命周期。", ["<c:XYNavigationDrawer />"], [new("Full Sidebar", "280 DIP", "Light / Dark"), new("Context", "Adaptive Drawer", "Light / Dark")], [new("Open", "Overlay + Backdrop"), new("Closed", "焦点恢复")])
    };
    static string[] BottomNavigationUsage() => ["""
var items = new[] { new XYBottomNavigationItem("map", "地图", XyuiVectorIcon.Locate) };
var state = new XYNavigationState(items.Select(x => new XYNavigationEntry(x.Id, x.Label, x.Icon)));
var nav = new XYBottomNavigation(state, items);
"""];
}
