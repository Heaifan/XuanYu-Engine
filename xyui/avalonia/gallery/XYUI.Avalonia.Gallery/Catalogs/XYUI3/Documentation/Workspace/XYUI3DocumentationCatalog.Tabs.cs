namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static XYUI1ComponentDocument BuildTabsDoc(string id, string type) => new(
        id, "标签页", "Tabs",
        "平级文档与视口内容切换的基础标签组件，以文字、修改标记圆点、弱化关闭与底部 3 DIP Accent 构成稳定排版。",
        "用于地图基础、地图环境、数据集等同一编辑区域的平级多视口切换；不承担 TabBar 的横向滚动与溢出管理。",
        () => XYUI3GalleryCatalog.CreatePreview(id),
        ["<c:XYTabs SizingMode=\"Content\"><c:XYTab Label=\"地图基础\" IsSelected=\"True\" /></c:XYTabs>"],
        [new("Compact V2", "34 DIP 高度，10 DIP 水平内边距，底部 3 DIP Accent", "Desktop Standard")],
        [new("Default", "浅色文字，透明背景"), new("Hover", "轻微悬浮背景"), new("Selected", "浅蓝背景 + 底部 Accent Line + 高亮文字"), new("Modified", "文字右侧呈现 3 DIP 未保存圆点"), new("Closable", "选中态浮现弱化关闭 × 按钮")],
        Properties(id),
        [new("XY.Surface.Panel", "Panel", "标签页容器底色"), new("XY.Brush.Accent.Default", "Accent", "选中态底部状态线"), new("XY.Border.Color.Subtle", "Subtle", "标签间细分隔线")],
        type)
    {
        CanonicalIdentity = "3.08 · Tabs / 标签页",
        Category = "XYUI-3 · 导航与切换",
        Acceptance = "UI + INTERACTION IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE · AWAITING USER INTERACTION ACCEPTANCE",
        QuickStartXaml = """
<c:XYTabs SizingMode="Content">
    <c:XYTab Label="地图基础" IsSelected="True" />
    <c:XYTab Label="地图环境" />
    <c:XYTab Label="数据集" IsModified="True" />
    <c:XYTab Label="区域编辑" IsClosable="False" />
</c:XYTabs>
""",
        CoreRules =
        [
            new("同轴居中对齐", "Tab 的文字、3 DIP 修改标记圆点与关闭按钮必须严格垂直居中同轴排版。"),
            new("单底线保证", "Selected 状态仅显示自身的底部 3 DIP Accent Line，严禁与外层容器产生平行双底线。"),
            new("相邻接替机制", "当前活动 Tab 关闭后，必须自动接替相邻 Tab 保持选中，支持全部关闭态。")
        ],
        DoDonts =
        [
            new("对齐规范", "DO: 文字、圆点与关闭按钮共享统一垂直中线。", "DON'T: 各子元素出现 1~2 DIP 的上下漂移错位。", "高密度桌面编辑器排版对错位极其敏感。"),
            new("底线干净度", "DO: 底部 Accent 贴合容器底边，且容器底边 thickness=0。", "DON'T: 给 Tabs 外部再画一层独立下划线。", "两条平行线会严重破坏视觉统一性。"),
            new("职责划分", "DO: 将视口级单页切换使用 XYTabs。", "DON'T: 在 XYTabs 内部硬编码溢出或新增按钮。", "复杂页签管理统一交由 XYTabBar 承担。")
        ],
        LiveExamplesFactory = () => XYUI3LiveExamplesFactory.CreateLiveExamples(id)!,
        CompositionFactory = () => XYUI3LiveExamplesFactory.CreateComposition(id)!
    };
}
