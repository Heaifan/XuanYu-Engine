namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static XYUI1ComponentDocument BuildViewSwitcherDoc(string id, string type) => new(
        id, "视图切换器", "ViewSwitcher",
        "基于同一业务数据切换观察方式的内容视图切换器，支持分段 (Segmented)、下拉 (Dropdown) 与 Primary + More 变体。",
        "用于同一页面/同一对象内切换呈现形式（如画布/表格/预览/日志，或 2D/3D/线框）；内容不变，只改变观察视角。",
        () => XYUI3GalleryCatalog.CreatePreview(id),
        ["<c:XYViewSwitcher State=\"{Binding ViewState}\" Variant=\"Segmented\" />"],
        [new("Segmented", "36 DIP 外框与 30 DIP 分段项，底部 Accent 指示条", "Viewport Toolbar"), new("Dropdown", "34 DIP 紧凑下拉，包含右侧 ChevronDown 与 Check 标识", "Compact Viewport"), new("PrimaryMore", "高优先级视图并排，低优先级视图自动收拢进 More 菜单", "Responsive Viewport")],
        [new("Current", "当前激活视图带有 Accent 底边条或菜单 Check 标记"), new("Disabled", "数据源不支持特定观察模式时视图项禁用"), new("Hover", "鼠标悬浮项浅色背景反馈")],
        Properties(id),
        [new("XY.Surface.Panel", "Panel", "分段外框与下拉背景"), new("XY.Brush.Accent.Default", "Accent", "底部分段指示条与选中勾选"), new("XY.Border.Color.Subtle", "Subtle", "分段外框与更多分割线")],
        type)
    {
        CanonicalIdentity = "3.21 · ViewSwitcher / 视图切换器",
        Category = "XYUI-3 · 导航与切换",
        Acceptance = "UI + INTERACTION IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE · AWAITING USER INTERACTION ACCEPTANCE",
        QuickStartXaml = """
<StackPanel Spacing="12">
    <!-- 分段视图切换器 (同一内容不同观察模式) -->
    <c:XYViewSwitcher State="{Binding ViewState}" Variant="Segmented" />
    <!-- 下拉视图切换器 (空间紧凑场景) -->
    <c:XYViewSwitcher State="{Binding ViewState}" Variant="Dropdown" />
</StackPanel>
""",
        CoreRules =
        [
            new("内容不变观察模式切换", "ViewSwitcher 专职“同一对象怎么看”，业务数据源保持不变，严格与 Tabs 生命周期区分。"),
            new("共享 ViewState 架构", "所有变体（Segmented / Dropdown / PrimaryMore）共享同一 XYViewState，状态双向同步。"),
            new("Priority 优先级分流", "PrimaryMore 变体按 Priority 排序，高优直接排布，低优自动归入 More 溢出菜单。"),
            new("30 DIP 等高槽位", "分段项内部高度严格为 30 DIP，外框 36 DIP，底部提供独立的 Accent 激活边条。")
        ],
        DoDonts =
        [
            new("职责区分", "DO: 改变同一内容的呈现模式使用 ViewSwitcher。", "DON'T: 用 ViewSwitcher 来打开多个独立文档。", "独立文档生命周期属于 Tabs / TabBar。"),
            new("状态同步", "DO: 多个视口变体统一绑定同一个 XYViewState。", "DON'T: 各自独立维护当前 View 字符串。", "避免分段与下拉状态不一致。"),
            new("空间适配", "DO: 视图项超过 4 个或空间狭窄时使用 PrimaryMore 或 Dropdown。", "DON'T: 强行横向排布大量分段按钮导致挤压变形。", "响应式分流保持工具栏紧凑。")
        ],
        LiveExamplesFactory = () => XYUI3LiveExamplesFactory.CreateLiveExamples(id)!,
        CompositionFactory = () => XYUI3LiveExamplesFactory.CreateComposition(id)!
    };
}
