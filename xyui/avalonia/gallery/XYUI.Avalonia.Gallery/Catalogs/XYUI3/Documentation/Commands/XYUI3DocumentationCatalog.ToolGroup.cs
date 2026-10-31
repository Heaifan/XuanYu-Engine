namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static XYUI1ComponentDocument BuildToolGroupDoc(string id, string type) => new(
        id, "工具组", "ToolGroup",
        "Toolbar 内部的语义分组容器，集成细垂直分割线、组内紧凑排布与静态折叠触发器，Toolbar 掌管布局，ToolGroup 专职聚合。",
        "用于将变换工具、网格生成、笔刷等同类工具在 Toolbar 内分组隔离；支持空间受限时静态折叠为单键触发器。",
        () => XYUI3GalleryCatalog.CreatePreview(id),
        ["<c:XYToolGroup><c:XYToolbarTool Label=\"移动\" IsSelected=\"True\" /></c:XYToolGroup>"],
        [new("Inline Group", "带前置 24 DIP 垂直分割线与 2 DIP 工具间距的内联组", "Standard Group"), new("Collapsed Trigger", "收起为单图标按键，图标保持组内当前选中项", "Compact Viewport")],
        [new("Expanded", "内联展示全部组内工具项"), new("Collapsed", "仅展示单一触发图标，点击恢复展开"), new("Active Tool Id", "组内活动工具与 Toolbar 全局同步")],
        Properties(id),
        [new("XY.Surface.Panel", "Panel", "工具组宿主底色"), new("XY.Border.Color.Subtle", "Subtle", "组前置垂直分割线"), new("XY.Brush.Accent.Default", "Accent", "激活工具指示")],
        type)
    {
        CanonicalIdentity = "3.16 · ToolGroup / 工具组",
        Category = "XYUI-3 · 导航与切换",
        Acceptance = "UI + INTERACTION IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE · AWAITING USER INTERACTION ACCEPTANCE",
        QuickStartXaml = """
<c:XYToolbar>
    <!-- 基础变换工具组 -->
    <c:XYToolGroup>
        <c:XYToolbarTool Label="选择" Icon="Locate" />
        <c:XYToolbarTool Label="移动" Icon="Locate" IsSelected="True" />
        <c:XYToolbarTool Label="旋转" Icon="StatusDot" />
    </c:XYToolGroup>
    <!-- 地图网格工具组 -->
    <c:XYToolGroup>
        <c:XYToolbarTool Label="区块网格" Icon="Section" />
        <c:XYToolbarTool Label="标高采样" Icon="Eye" />
    </c:XYToolGroup>
    <!-- 折叠工具组 (继承当前选中工具图标) -->
    <c:XYToolGroup IsCollapsed="True">
        <c:XYToolbarTool Label="地形画刷" Icon="Code" IsSelected="True" />
    </c:XYToolGroup>
</c:XYToolbar>
""",
        CoreRules =
        [
            new("Toolbar 布局与 ToolGroup 分组解耦", "Toolbar 掌管顶栏水平排列与整体尺寸；ToolGroup 仅负责同类工具聚拢、前置分割线与折叠语义。"),
            new("折叠触发器继承 Active 语义", "当 IsCollapsed=True 时，折叠按钮自动消费组内当前 Selected 工具的图标，保持模式可读性。"),
            new("轻量内联展开拒绝复杂弹层", "点击折叠触发器直接展开为内联平铺工具（IsCollapsed=false），不越权构造多级 Flyout 浮动弹窗。"),
            new("严谨微间距与统一分割线", "组内工具严格遵循 2 DIP 紧凑间距，前置分隔线统一为 24 DIP 垂直分割线，不产生双重边界。")
        ],
        DoDonts =
        [
            new("职责范围", "DO: 将 ToolGroup 作为 Toolbar 的子容器使用。", "DON'T: 脱离 Toolbar 孤立放置 ToolGroup。", "ToolGroup 专为工具栏内部语义分区设计。"),
            new("折叠状态", "DO: 折叠触发器准确继承并展示组内选中的工具图标。", "DON'T: 折叠后显示未知问号或固定无意义图标。", "丢失选中工具图标会导致操作模式不透明。"),
            new("交互轻量", "DO: 点击折叠触发器直接切换为展开平铺。", "DON'T: 强行弹出重量级模态对话框。", "高频工具切换必须维持极低交互损耗。")
        ],
        LiveExamplesFactory = () => XYUI3LiveExamplesFactory.CreateLiveExamples(id)!,
        CompositionFactory = () => XYUI3LiveExamplesFactory.CreateComposition(id)!
    };
}
