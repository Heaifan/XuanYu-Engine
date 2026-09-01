using XYUI.Avalonia.Catalog;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static readonly IReadOnlySet<string> BatchIds = new HashSet<string> { "XYUI-3-3.01", "XYUI-3-3.02", "XYUI-3-3.03", "XYUI-3-3.04", "XYUI-3-3.05", "XYUI-3-3.06", "XYUI-3-3.07", "XYUI-3-3.08", "XYUI-3-3.09", "XYUI-3-3.10", "XYUI-3-3.11", "XYUI-3-3.12", "XYUI-3-3.13", "XYUI-3-3.14", "XYUI-3-3.15", "XYUI-3-3.16", "XYUI-3-3.17", "XYUI-3-3.18", "XYUI-3-3.19", "XYUI-3-3.20", "XYUI-3-3.21", "XYUI-3-3.22", "XYUI-3-3.23", "XYUI-3-3.24" };
    public static IReadOnlyList<XYUI1ComponentDocument> Build() => XyuiCatalogSource.Load().Where(x => BatchIds.Contains(x.SourceItemId)).Select(Create).ToArray();
    static XYUI1ComponentDocument Create(XyuiCatalogEntry entry)
    {
        var type = entry.AvaloniaType.Split('.').Last(); if (string.IsNullOrWhiteSpace(type)) type = ComponentName(entry.SourceItemId); var details = Details(entry.SourceItemId);
        var acceptance = entry.SourceItemId == "XYUI-3-3.04" ? "UI CLOSED · USER VISUAL ACCEPTED · HIERARCHY LOGIC REWORKED · AWAITING USER INTERACTION VERIFICATION" :
            entry.SourceItemId is "XYUI-3-3.09" or "XYUI-3-3.10" or "XYUI-3-3.11" or "XYUI-3-3.12" or "XYUI-3-3.17" or "XYUI-3-3.18" or "XYUI-3-3.19" or "XYUI-3-3.20" ? "UI + INTERACTION IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE · AWAITING USER INTERACTION ACCEPTANCE" : "UI IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE";
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
        "XYUI-3-3.01" => ("文字主导的桌面一级菜单栏，以轻 Hover 与底部 Accent 状态线建立导航层级。", "用于编辑器窗口顶部的文件、编辑、视图、窗口、帮助导航；点击或 Enter/Down 打开菜单，Left/Right 切换项，Esc 关闭。", ["<c:XYMenuBar>文件 / 编辑 / 视图 / 窗口 / 帮助</c:XYMenuBar>"], [new("底部状态线型", "低干扰菜单栏", "Light / Dark")], [new("Default", "纯文字"), new("Hover", "浅色背景"), new("Active", "Accent 文字与底线")]),
        "XYUI-3-3.02" => ("标准桌面菜单面板，提供稳定的 Leading、Label、Shortcut、Chevron 四列。", "用于承载相关命令、快捷键、勾选、单选和子菜单入口；点击或 Enter/Space 激活，Up/Down 导航，Esc 关闭。", ["<c:XYMenu><c:XYMenuItem Label=\"打开\" Shortcut=\"Ctrl+O\" /></c:XYMenu>"], [new("标准桌面菜单型", "高密度命令面板", "Overlay Surface")], [new("Checked", "显示勾选标记"), new("Radio", "显示单选状态"), new("Disabled", "降低对比度"), new("Destructive", "低饱和危险文字")]),
        "XYUI-3-3.03" => ("带对象标题头的菜单组合，主体直接复用 XYMenu 与 XYMenuItem。", "用于 Region、Entity、Dataset 等对象的就地操作预览；绑定目标后右键打开，Esc 或轻量关闭收起。", ["<c:XYContextMenu ContextType=\"ENTITY\" ContextName=\"Infantry_023\" />"], [new("对象标题型", "类型 + 对象名称", "Entity / Region / Dataset")], [new("Header", "两层对象标题"), new("Danger group", "底部独立分组")]),
        "XYUI-3-3.05" => ("长期驻留的模块导航项集合，以左侧 Accent Bar 和浅蓝背景保持当前功能区识别。", "用于编辑器主模块、工具区和管理入口之间的长期位置切换；选中状态不因操作完成而消失。", ["<c:XYNavigationMenu Groups=\"...\" />"], [new("Compact V2", "32 DIP 导航项 / 20 DIP 分组标题", "Light / Dark")], [new("Selected", "浅蓝背景 + 左侧 Accent Bar")]),
        "XYUI-3-3.06" => ("承载一级导航与当前模块上下文的可折叠侧边容器。", "用于编辑器长期驻留的主侧栏；折叠后保留一级导航并转换为 NavigationRail。", ["<c:XYSidebar PrimaryItems=\"...\" ContextItems=\"...\" />"], [new("Compact V2", "212 DIP 展开 / 54 DIP Rail", "Light / Dark")], [new("Expanded", "Header + 主导航 + 上下文"), new("Collapsed", "转换为 Rail")]),
        "XYUI-3-3.07" => ("以窄型常驻图标轨承载一级导航，并为上下文内容保留按需恢复的入口。", "用于空间不足或用户主动折叠 Sidebar 的编辑器布局。", ["<c:XYNavigationRail Items=\"...\" />"], [new("Compact V2", "54 DIP Rail / 40 DIP Item", "Light / Dark")], [new("Selected", "浅蓝背景 + Accent Bar"), new("Context", "按需 Flyout")]),
        "XYUI-3-3.08" => ("用轻分隔、轻背景与底部 Accent Line 表达平级内容页的当前状态。", "用于地图基础、地图环境、数据集和区域编辑等同一区域的页面切换；不承担 TabBar 的溢出与排序管理。", ["<c:XYTabs Items=\"...\" />"], [new("Compact V2", "34 DIP Tab / 10 DIP PaddingX", "Light / Dark")], [new("Selected", "底部 3 DIP Accent Line"), new("Modified", "3 DIP 圆点"), new("Closable", "弱化关闭入口")]),
        "XYUI-3-3.09" => ("真实 XYTabs 视口与固定 Previous、Next、Overflow、New 操作槽组成的紧凑页签栏。", "用于文档和编辑页面的多页签入口；支持按钮与滚轮横向滚动、溢出菜单选页和新增请求事件。", ["<c:XYTabBar />"], [new("Compact SVG", "38 DIP Bar / 32 DIP Arrow", "Light / Dark")], [new("Selected", "沿用 XYTab"), new("Overflow", "Popup tab list")]),
        "XYUI-3-3.10" => ("在真实 XYTab 左侧增加轻量 Drag Grip 的停靠页签视觉。", "用于 Hierarchy、Inspector、Console、Assets 等编辑器面板；支持选择、关闭与同栏拖动排序，不包含 Dock Engine。", ["<c:XYDockTabs />"], [new("Reorderable", "38 DIP DockTab", "Light / Dark")], [new("Selected", "Raised Surface + Single Accent"), new("Modified", "沿用 XYTab")]),
        "XYUI-3-3.11" => ("纯文字紧凑路径，以矢量 Chevron、轻 Hover、中间折叠和当前项强调表达层级。", "用于玄域项目、地图、数据集、资源等层级路径；支持鼠标与键盘调用、当前位置切换和折叠或下拉请求事件。", ["<c:XYBreadcrumb />"], [new("Compact Text Trail", "34 DIP / 26 DIP Item", "Light / Dark")], [new("Collapsed", "Dropdown Request"), new("Current", "Semibold Active Text")]),
        "XYUI-3-3.12" => ("弱默认 Guide 与强化 Selected Ancestor Chain 共同组成的紧凑树导航。", "用于项目、Hierarchy、Dataset 和资源树；支持展开收起、单选和方向键导航，虚拟化与拖放留给后续专门能力。", ["<c:XYTreeNavigation />"], [new("Compact Guided Tree", "28 DIP Row / 16 DIP Indent", "Light / Dark")], [new("Selected", "Accent Bar + Selected Surface"), new("Expanded", "Visible descendant state")]),
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
        "XYUI-3-3.24" => ("响应式导航抽屉，复用 Sidebar、NavigationMenu、SearchField 的真实能力。", "用于窄屏临时导航、上下文抽屉和边缘 Peek；具备遮罩、Esc、失焦和卸载关闭生命周期。", ["<c:XYNavigationDrawer />"], [new("Full Sidebar", "280 DIP", "Light / Dark"), new("Context", "Adaptive Drawer", "Light / Dark")], [new("Open", "Overlay + Backdrop"), new("Closed", "焦点恢复")]),
        _ => ("层级连接型子菜单，使用统一菜单面板和可复用菜单行。", "用于导出、主题、布局等需要二级命令的层级入口；激活父项或 Right 打开，Left/Esc 收起。", ["<c:XYSubMenu ParentMenu=\"XYMenu\" ChildMenu=\"XYMenu\" />"], [new("Open Right", "右侧连接", "默认"), new("Open Left", "左侧镜像", "静态 Variant")], [new("Active Trigger", "父项保持强调"), new("Connector", "40 DIP 连接线与锚点")])
    };

    static string[] BottomNavigationUsage() => ["""
// C#：在宿主 View / ViewModel 中创建并配置完整的底部导航。
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;
using System.Linq;

var items = new[]
{
    new XYBottomNavigationItem("map", "地图", XyuiVectorIcon.Locate, Badge: null, IsEnabled: true),
    new XYBottomNavigationItem("data", "数据", XyuiVectorIcon.Code, Badge: "2"),
    new XYBottomNavigationItem("logs", "日志", XyuiVectorIcon.Section, Badge: "3"),
    new XYBottomNavigationItem("mine", "我的", XyuiVectorIcon.Info)
};
var state = new XYNavigationState(
    items.Select(x => new XYNavigationEntry(x.Id, x.Label, x.Icon)), selectedId: "map");
var primary = new XYButton
{
    Content = new XYIcon { Icon = XyuiVectorIcon.Add, Size = XyuiIconSize.Small },
    Variant = XyuiButtonVariant.Primary
};
var navigation = new XYBottomNavigation(state, items, primary) { SafeAreaBottom = 0 };

bool CanOpenLogs() => true;       // 替换为宿主权限 / 数据就绪判断
void ShowView(string id) { }      // 将内容区切换到 id
void CreateDocument() { }         // 执行宿主的新建流程

navigation.DestinationRequested += (_, request) =>
{
    if (request.Destination.Id == "logs" && !CanOpenLogs()) { request.Reject(); return; }
    request.Accept();
};
navigation.DestinationChanged += (_, id) => ShowView(id);
navigation.PrimaryActionRequested += (_, _) => CreateDocument();

navigation.SelectDestination("data");       // 触发请求，Accept 后提交
navigation.CommitDestination("map");        // 已确认时直接提交
var current = navigation.CurrentDestinationId; // 读取当前 Id
var allItems = navigation.Items;                // 读取目的地与 Badge
"""];
}
