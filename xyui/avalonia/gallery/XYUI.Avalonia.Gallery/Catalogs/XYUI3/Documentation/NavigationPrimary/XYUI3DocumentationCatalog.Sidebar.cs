namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static XYUI1ComponentDocument BuildSidebarDoc(string id, string type) => new(
        id, "侧边导航栏", "Sidebar",
        "桌面编辑器长期驻留的侧边复合容器，固定承载 Header、一级导航、模块上下文及置底 Footer，支持平滑折叠。",
        "用于玄域引擎桌面主界面左侧；展开态宽 212~240 DIP，折叠态收拢为 Canonical NavigationRail Width (64 DIP) 紧凑轨，Footer 始终置底吸附。",
        () => XYUI3GalleryCatalog.CreatePreview(id),
        ["<c:XYSidebar NavigationState=\"{Binding State}\" IsCollapsed=\"{Binding IsCollapsed}\" />"],
        [new("Expanded (展开)", "标准 212 DIP 容器：Header + 主导航 + 上下文树/工具 + 置底设置", "桌面标准模式"), new("Collapsed (折叠)", "紧凑 64 DIP 轨 (Canonical NavigationRail Width)：仅保留一级图标、展开触发器与置底入口", "空间紧凑模式")],
        [new("Expanded", "完整展示四段式复合结构"), new("Collapsed", "自适应收拢为 Rail 表现并隐藏文字与上下文区"), new("Footer Anchored", "无论 Context 内容如何滚动，Footer 保持吸底")],
        Properties(id),
        [new("XY.Surface.Panel", "Panel", "侧栏容器主底色"), new("XY.Surface.PanelAlt", "PanelAlt", "侧栏 Header 背景"), new("XY.Border.Color.Subtle", "Subtle", "侧栏边框与分隔线")],
        type)
    {
        CanonicalIdentity = "3.06 · Sidebar / 侧边导航栏",
        Category = "XYUI-3 · 导航与切换",
        Acceptance = "UI IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE",
        QuickStartXaml = """
<c:XYSidebar NavigationState="{Binding NavigationState}"
             IsCollapsed="{Binding IsCollapsed, Mode=TwoWay}"
             ExpandedWidth="240">
    <c:XYSidebar.ContextRegion>
        <c:XYNavigationMenu NavigationState="{Binding ContextNavigationState}" />
    </c:XYSidebar.ContextRegion>
</c:XYSidebar>
""",
        CoreRules =
        [
            new("四段式标准解剖", "Sidebar 固定由 Header + Primary Navigation + Context Region + Sticky Footer 四段组成，非单纯 NavigationMenu 拉宽。"),
            new("置底 Footer 隔离", "Footer (设置/关于) 必须固定吸附于底部 Row 4，严禁随 Context 滚动区发生上下漂移。"),
            new("折叠形态转换", "折叠时转换为紧凑 Rail 表现，隐藏文字与上下文，保留一级核心图标与展开按钮。")
        ],
        DoDonts =
        [
            new("结构清晰度", "DO: 严格保持主导航与当前模块上下文区域的视觉边界与语义解耦。", "DON'T: 把所有层级导航混杂成一个平铺的长列表。", "主导航是全局视角，上下文区是局部视角，不可混淆。"),
            new("Footer 布局", "DO: Footer 必须放置在 Grid 底部独立 Auto 行，中间区域设为 * 伸缩行。", "DON'T: 将 Footer 放在 StackPanel 随内容下推。", "内容不足时 Footer 会上浮，内容过多时会被推至屏幕外。"),
            new("响应式边界", "DO: Sidebar 折叠态正式复用 XYNavigationRail 作为适配形态。", "DON'T: 提前对 3.07 开展独立组件验收。", "3.07 的独立组件文档、完整能力展示与独立人工验收留到 Round 2。")
        ],
        LiveExamplesFactory = () => XYUI3LiveExamplesFactory.CreateLiveExamples(id)!,
        CompositionFactory = () => XYUI3LiveExamplesFactory.CreateComposition(id)!
    };
}
