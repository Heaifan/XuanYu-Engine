namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static XYUI1ComponentDocument BuildBreadcrumbDoc(string id, string type) => new(
        id, "面包屑导航", "Breadcrumb",
        "纯文字紧凑路径导航，高度 34 DIP，以矢量 Chevron 分隔符、中间折叠与当前项强调表达层级结构。",
        "用于玄域工程、地图、区域、要素等层级路径定位；支持祖先层级点击跳转、省略号展开与当前位置高亮。",
        () => XYUI3GalleryCatalog.CreatePreview(id),
        ["<c:XYBreadcrumb><c:XYBreadcrumbItem Label=\"工程\" /><c:XYBreadcrumbItem Label=\"地图\" IsCurrent=\"True\" /></c:XYBreadcrumb>"],
        [new("Compact Text Trail", "34 DIP 面包屑容器，26 DIP 文本项，细 Chevron 分隔", "Text Trail")],
        [new("Ancestor", "可点击跳转的祖先路径，悬浮浅色背景"), new("Current", "粗体 Accent 文字，表达当前所在终点"), new("Collapsed", "中间省略号 (...)，点击呼出被折叠层级下拉")],
        Properties(id),
        [new("XY.Surface.Panel", "Panel", "容器底色"), new("XY.Brush.Text.Primary", "Primary", "当前项强调色"), new("XY.Brush.Text.Secondary", "Secondary", "祖先文本色")],
        type)
    {
        CanonicalIdentity = "3.11 · Breadcrumb / 面包屑导航",
        Category = "XYUI-3 · 导航与切换",
        Acceptance = "UI + INTERACTION IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE · AWAITING USER INTERACTION ACCEPTANCE",
        QuickStartXaml = """
<c:XYBreadcrumb>
    <c:XYBreadcrumbItem Label="玄域工程" />
    <c:XYBreadcrumbItem Label="世界地图" />
    <c:XYBreadcrumbItem Label="华南大区" IsCollapsed="True" />
    <c:XYBreadcrumbItem Label="核心要塞" IsCurrent="True" />
</c:XYBreadcrumb>
""",
        CoreRules =
        [
            new("明确层级路径语义", "Breadcrumb 表达「我当前在层级树中的哪个位置」，严禁与线性向导 (Steps) 混淆。"),
            new("祖先层级快速跳转", "点击祖先项直接回退导航至该层级，当前项（IsCurrent）不可重复点击刷新。"),
            new("超长路径省略机制", "深层路径自动以省略号折叠中间项，点击省略号可在 DropdownPopup 中选择恢复。")
        ],
        DoDonts =
        [
            new("组件认知", "DO: 将空间/资源树的上下级回溯使用 Breadcrumb。", "DON'T: 将「第一步→第二步→第三步」做成 Breadcrumb。", "连续线性向导属于 XYSteps，层级树回溯才是 Breadcrumb。"),
            new("末端指示", "DO: 当前最终节点使用 IsCurrent=True 明确高亮。", "DON'T: 所有层级颜色一致，用户无法识别当前终点。", "终点必须形成清晰的视觉锚点。"),
            new("折叠可用性", "DO: 折叠省略号 (...) 支持点击呼出完整隐藏路径列表。", "DON'T: 仅静态画出三个点而无法交互展开。", "不可交互的假折叠会截断用户的路径选择。")
        ],
        LiveExamplesFactory = () => XYUI3LiveExamplesFactory.CreateLiveExamples(id)!,
        CompositionFactory = () => XYUI3LiveExamplesFactory.CreateComposition(id)!
    };
}
