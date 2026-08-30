using XYUI.Avalonia.Catalog;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static class XYUI3DocumentationCatalog
{
    static readonly IReadOnlySet<string> BatchIds = new HashSet<string> { "XYUI-3-3.01", "XYUI-3-3.02", "XYUI-3-3.03", "XYUI-3-3.04" };
    public static IReadOnlyList<XYUI1ComponentDocument> Build() => XyuiCatalogSource.Load().Where(x => BatchIds.Contains(x.SourceItemId)).Select(Create).ToArray();
    static XYUI1ComponentDocument Create(XyuiCatalogEntry entry)
    {
        var type = entry.AvaloniaType.Split('.').Last(); var details = Details(entry.SourceItemId);
        var acceptance = entry.SourceItemId == "XYUI-3-3.04" ? "UI CLOSED · USER VISUAL ACCEPTED · HIERARCHY LOGIC REWORKED · AWAITING USER INTERACTION VERIFICATION" : "UI IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE";
        return new(entry.SourceItemId, entry.Title.Split('/').Last().Trim(), type, details.Overview, details.WhenToUse,
            () => XYUI3GalleryCatalog.CreatePreview(entry.SourceItemId), details.Usages, details.Variants, details.States,
            [], entry.ApiRefs.Select(x => new XYUIDocToken(x, "Canonical", "Foundation token reference")).ToArray(), type)
        { CanonicalIdentity = entry.CanonicalIdentity, KnownGap = entry.KnownGap, Acceptance = acceptance };
    }
    static (string Overview, string WhenToUse, string[] Usages, XYUIDocVariant[] Variants, XYUIDocState[] States) Details(string id) => id switch
    {
        "XYUI-3-3.01" => ("文字主导的桌面一级菜单栏，以轻 Hover 与底部 Accent 状态线建立导航层级。", "用于编辑器窗口顶部的文件、编辑、视图、窗口、帮助导航；点击或 Enter/Down 打开菜单，Left/Right 切换项，Esc 关闭。", ["<c:XYMenuBar>文件 / 编辑 / 视图 / 窗口 / 帮助</c:XYMenuBar>"], [new("底部状态线型", "低干扰菜单栏", "Light / Dark")], [new("Default", "纯文字"), new("Hover", "浅色背景"), new("Active", "Accent 文字与底线")]),
        "XYUI-3-3.02" => ("标准桌面菜单面板，提供稳定的 Leading、Label、Shortcut、Chevron 四列。", "用于承载相关命令、快捷键、勾选、单选和子菜单入口；点击或 Enter/Space 激活，Up/Down 导航，Esc 关闭。", ["<c:XYMenu><c:XYMenuItem Label=\"打开\" Shortcut=\"Ctrl+O\" /></c:XYMenu>"], [new("标准桌面菜单型", "高密度命令面板", "Overlay Surface")], [new("Checked", "显示勾选标记"), new("Radio", "显示单选状态"), new("Disabled", "降低对比度"), new("Destructive", "低饱和危险文字")]),
        "XYUI-3-3.03" => ("带对象标题头的菜单组合，主体直接复用 XYMenu 与 XYMenuItem。", "用于 Region、Entity、Dataset 等对象的就地操作预览；绑定目标后右键打开，Esc 或轻量关闭收起。", ["<c:XYContextMenu ContextType=\"ENTITY\" ContextName=\"Infantry_023\" />"], [new("对象标题型", "类型 + 对象名称", "Entity / Region / Dataset")], [new("Header", "两层对象标题"), new("Danger group", "底部独立分组")]),
        _ => ("层级连接型子菜单，使用统一菜单面板和可复用菜单行。", "用于导出、主题、布局等需要二级命令的层级入口；激活父项或 Right 打开，Left/Esc 收起。", ["<c:XYSubMenu ParentMenu=\"XYMenu\" ChildMenu=\"XYMenu\" />"], [new("Open Right", "右侧连接", "默认"), new("Open Left", "左侧镜像", "静态 Variant")], [new("Active Trigger", "父项保持强调"), new("Connector", "40 DIP 连接线与锚点")])
    };
}
