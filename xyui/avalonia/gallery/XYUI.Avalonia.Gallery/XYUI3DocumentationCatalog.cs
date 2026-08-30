using XYUI.Avalonia.Catalog;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static class XYUI3DocumentationCatalog
{
    static readonly IReadOnlySet<string> BatchIds = new HashSet<string> { "XYUI-3-3.01", "XYUI-3-3.02", "XYUI-3-3.03", "XYUI-3-3.04", "XYUI-3-3.05", "XYUI-3-3.06", "XYUI-3-3.07", "XYUI-3-3.08" };
    public static IReadOnlyList<XYUI1ComponentDocument> Build() => XyuiCatalogSource.Load().Where(x => BatchIds.Contains(x.SourceItemId)).Select(Create).ToArray();
    static XYUI1ComponentDocument Create(XyuiCatalogEntry entry)
    {
        var type = entry.AvaloniaType.Split('.').Last(); if (string.IsNullOrWhiteSpace(type)) type = ComponentName(entry.SourceItemId); var details = Details(entry.SourceItemId);
        var acceptance = entry.SourceItemId == "XYUI-3-3.04" ? "UI CLOSED · USER VISUAL ACCEPTED · HIERARCHY LOGIC REWORKED · AWAITING USER INTERACTION VERIFICATION" : "UI IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE";
        return new(entry.SourceItemId, entry.Title.Split('/').Last().Trim(), type, details.Overview, details.WhenToUse,
            () => XYUI3GalleryCatalog.CreatePreview(entry.SourceItemId), details.Usages, details.Variants, details.States,
            [], entry.ApiRefs.Select(x => new XYUIDocToken(x, "Canonical", "Foundation token reference")).ToArray(), type)
        { CanonicalIdentity = entry.CanonicalIdentity, KnownGap = entry.KnownGap, Acceptance = acceptance };
    }
    static string ComponentName(string id) => id switch
    {
        "XYUI-3-3.05" => "XYNavigationMenu", "XYUI-3-3.06" => "XYSidebar",
        "XYUI-3-3.07" => "XYNavigationRail", "XYUI-3-3.08" => "XYTabs", _ => ""
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
        _ => ("层级连接型子菜单，使用统一菜单面板和可复用菜单行。", "用于导出、主题、布局等需要二级命令的层级入口；激活父项或 Right 打开，Left/Esc 收起。", ["<c:XYSubMenu ParentMenu=\"XYMenu\" ChildMenu=\"XYMenu\" />"], [new("Open Right", "右侧连接", "默认"), new("Open Left", "左侧镜像", "静态 Variant")], [new("Active Trigger", "父项保持强调"), new("Connector", "40 DIP 连接线与锚点")])
    };
}
