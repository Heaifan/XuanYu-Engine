namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static XYUI1ComponentDocument BuildDockTabsDoc(string id, string type) => new(
        id, "停靠标签栏", "DockTabs",
        "编辑器停靠面板专用页签，高度 38 DIP，在页签左侧集成轻量 Drag Grip 拖动把手，支持同栏拖动重排。",
        "用于 Hierarchy、Inspector、Console、Assets 等面板的页签组织；支持同栏拖动重排与插入指示，不包含完整 Dock Engine。",
        () => XYUI3GalleryCatalog.CreatePreview(id),
        ["<c:XYDockTabs><c:XYDockTab><c:XYTab Label=\"层级视口\" IsSelected=\"True\" /></c:XYDockTab></c:XYDockTabs>"],
        [new("Reorderable", "38 DIP DockTab，包含 12 DIP Drag Grip 与垂直分割线", "Panel Dock")],
        [new("Selected", "微凸起 Raised Surface + 单条 Accent，无双底边"), new("Dragging", "拖拽时透明度降低至 0.65"), new("Drop Indicator", "目标插入点呈现 2 DIP Accent 插入线")],
        Properties(id),
        [new("XY.Surface.PanelAlt", "PanelAlt", "停靠栏容器底色"), new("XY.Surface.Raised", "Raised", "选中项微凸起背景"), new("XY.Brush.Accent.Default", "Accent", "插入指示线与选中态")],
        type)
    {
        CanonicalIdentity = "3.10 · DockTabs / 停靠标签栏",
        Category = "XYUI-3 · 导航与切换",
        Acceptance = "UI + INTERACTION IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE · AWAITING USER INTERACTION ACCEPTANCE",
        QuickStartXaml = """
<c:XYDockTabs>
    <c:XYDockTab>
        <c:XYTab Label="层级视口" IsSelected="True" />
    </c:XYDockTab>
    <c:XYDockTab>
        <c:XYTab Label="属性检查器" />
    </c:XYDockTab>
    <c:XYDockTab>
        <c:XYTab Label="控制台" />
    </c:XYDockTab>
</c:XYDockTabs>
""",
        CoreRules =
        [
            new("同栏拖拽排序", "支持指针在 DragGrip 把手按下并横向拖动，动态计算相邻目标并展现插入指示线。"),
            new("严格消除双底边", "选中页签采用 Raised Surface 凸起与贴合底线，外围容器底边为 0，杜绝双底线。"),
            new("诚实能力边界", "当前仅负责面板内页签选择与同栏重排，严禁手绘虚假 Drop Zone 冒充全局 Dock Engine。")
        ],
        DoDonts =
        [
            new("把手与文本同轴", "DO: DragGrip 图标与 Tab 文字保持绝对垂直居中同轴。", "DON'T: 把手居上或文字居下产生上下偏斜。", "把手与文字不同轴会严重影响专业感。"),
            new("拖动视觉反馈", "DO: 拖动过程中提供半透明 (Opacity 0.65) 与明确的 Drop Indicator 蓝线。", "DON'T: 拖动时没有任何位置预览直接生硬瞬间交换。", "缺乏指示会让用户无法预判落点。"),
            new("选型精准性", "DO: 停靠面板用 XYDockTabs，文档视口用 XYTabBar，单组页签用 XYTabs。", "DON'T: 把 DockTabs 当成普通工作区页签滥用。", "不同容器承担不同的空间与拖拽生命周期。")
        ],
        LiveExamplesFactory = () => XYUI3LiveExamplesFactory.CreateLiveExamples(id)!,
        CompositionFactory = () => XYUI3LiveExamplesFactory.CreateComposition(id)!
    };
}
