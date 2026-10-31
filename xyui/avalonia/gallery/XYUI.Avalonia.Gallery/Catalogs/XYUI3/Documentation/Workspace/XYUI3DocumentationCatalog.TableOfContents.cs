namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static XYUI1ComponentDocument BuildTableOfContentsDoc(string id, string type) => new(
        id, "目录导航", "TableOfContents",
        "限深两级的长页面/长文档内部章节目录导航，提供 Hierarchical 常驻层级与 Compact 折叠浮层变体。",
        "用于文档、设置页、大型报告与 Inspector 的内部章节快速跳转与定位；清晰区分当前章节、父级章节与普通章节。",
        () => XYUI3GalleryCatalog.CreatePreview(id),
        ["<c:XYTableOfContents State=\"{Binding TocState}\" Variant=\"Hierarchical\" />"],
        [new("Hierarchical", "桌面常驻目录，展示 Level 1 + Level 2 结构与左侧连续导引线", "Document Right Rail"), new("Compact", "窄屏折叠目录，触发 34 DIP 下拉弹出层，当前项带右侧勾选", "Mobile / Floating")],
        [new("Current", "当前聚焦章节，显示 Accent 左指示条与高亮文字"), new("Parent Active", "子章节激活时其父章节同时呈现激活态（保持层级上下文）"), new("Normal", "普通待跳转章节，浅色悬停")],
        Properties(id),
        [new("XY.Surface.Panel", "Panel", "目录容器与下拉背景"), new("XY.Brush.Accent.Default", "Accent", "当前章节左侧指示条与勾选"), new("XY.Border.Color.Subtle", "Subtle", "二级目录贯穿导引线")],
        type)
    {
        CanonicalIdentity = "3.22 · TableOfContents / 目录导航",
        Category = "XYUI-3 · 导航与切换",
        Acceptance = "UI + INTERACTION IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE · AWAITING USER INTERACTION ACCEPTANCE",
        QuickStartXaml = """
<StackPanel Spacing="12">
    <!-- 桌面常驻层级目录 (带连续导引线) -->
    <c:XYTableOfContents State="{Binding TocState}" Variant="Hierarchical" />
    <!-- 紧凑折叠目录 (浮层形态) -->
    <c:XYTableOfContents State="{Binding TocState}" Variant="Compact" />
</StackPanel>
""",
        CoreRules =
        [
            new("限深两级原则", "TableOfContents 仅渲染 Level 1 与 Level 2 章节，Level 3 及更深层级自动过滤，避免退化为业务树。"),
            new("父子层级导引线", "二级子章节必须成组缩进，并由左侧一根连续的 xyui-toc-level-guide 导引线贯穿。"),
            new("激活态层级联动", "子章节处于 Current 状态时，对应父章节必须自动附加 xyui-toc-parent-active 样式保持上下文。"),
            new("页面内部锚点跳转", "TOC 专职同一页面内部 Section 跳转，通过 SectionRequested 事务事件确认或拦截跳转。")
        ],
        DoDonts =
        [
            new("深度控制", "DO: 目录深度严格限制在两级以内。", "DON'T: 引入多层无限嵌套把 TOC 当成 TreeView。", "页面目录应保持极简清晰。"),
            new("状态联动", "DO: 当前选中子章节时，父章节保持微高亮指示上下文。", "DON'T: 只有末级高亮导致用户失去层级归属感。", "明确的层级关系帮助长文档定位。"),
            new("区分业务树", "DO: 仅从页面标题或固定 Section 构造 TOC。", "DON'T: 在 TOC 节点上附加复选框、拖拽或增删改操作。", "TOC 是只读导航，不是实体编辑树。")
        ],
        LiveExamplesFactory = () => XYUI3LiveExamplesFactory.CreateLiveExamples(id)!,
        CompositionFactory = () => XYUI3LiveExamplesFactory.CreateComposition(id)!
    };
}
