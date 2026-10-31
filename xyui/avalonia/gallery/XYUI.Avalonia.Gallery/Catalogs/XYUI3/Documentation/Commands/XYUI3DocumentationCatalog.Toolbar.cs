namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static XYUI1ComponentDocument BuildToolbarDoc(string id, string type) => new(
        id, "工具栏", "Toolbar",
        "极简连续的高密度工具栏，以图标优先、紧凑间距与细分隔线组织高频工具，严格与一次性执行的命令栏区分。",
        "用于选择、平移、旋转、缩放、画刷等视口与场景高频模式工具的常驻切换与互斥激活。",
        () => XYUI3GalleryCatalog.CreatePreview(id),
        ["<c:XYToolbar IsCompact=\"True\"><c:XYToolbarTool Label=\"移动\" Icon=\"Locate\" IsSelected=\"True\" /></c:XYToolbar>"],
        [new("Compact Icon-first", "34 DIP 高度，28 DIP 图标工具，2 DIP 极窄间距", "Viewport Header"), new("Labeled Toolbar", "展开文字标签模式，适合宽幅控制栏", "Document Editor")],
        [new("Selected", "Selected Surface 微凸表面与 Accent 指示边框"), new("Hover", "浅色悬浮背景反馈"), new("Disabled", "工具不可用时整体半透明弱化")],
        Properties(id),
        [new("XY.Surface.Panel", "Panel", "工具栏容器底色"), new("XY.Brush.Accent.Default", "Accent", "活动工具激活标记"), new("XY.Border.Color.Subtle", "Subtle", "工具组间细分隔线")],
        type)
    {
        CanonicalIdentity = "3.15 · Toolbar / 工具栏",
        Category = "XYUI-3 · 导航与切换",
        Acceptance = "UI + INTERACTION IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE · AWAITING USER INTERACTION ACCEPTANCE",
        QuickStartXaml = """
<c:XYToolbar IsCompact="True">
    <c:XYToolbarTool Label="选择" Icon="Locate" />
    <c:XYToolbarTool Label="移动" Icon="Locate" IsSelected="True" />
    <c:XYToolbarTool Label="旋转" Icon="StatusDot" />
    <c:XYToolbarTool Label="缩放" Icon="Section" />
    <c:XYSeparator Variant="VerticalSplit" Height="24" />
    <c:XYToolbarTool Label="区域选择" />
    <c:XYToolbarTool Label="道路绘制" />
</c:XYToolbar>
""",
        CoreRules =
        [
            new("图标优先与高密排布", "默认启用 IsCompact=True，以 28 DIP 图标按钮与 2 DIP 极窄间距呈现，保持高视口留白。"),
            new("互斥模式工具管理", "同一操作维度内的工具切换互斥，当前激活工具呈现明确的 Selected 表面与微 Accent 边框。"),
            new("与 CommandBar 严格分界", "Toolbar 承载视口模式与绘制工具切换；CommandBar 承载保存、导出、删除等动作指令。"),
            new("细分隔线语义分区", "使用 XYSeparator (VerticalSplit) 区分不同逻辑区段，杜绝无边界工具杂乱堆叠。")
        ],
        DoDonts =
        [
            new("工具 vs 动作", "DO: 将视口平移、框选、画刷等常驻模式放在 Toolbar。", "DON'T: 将保存、导出、提交等一次性指令放进 Toolbar。", "一次性动作执行属于 CommandBar 职责。"),
            new("激活状态", "DO: 激活工具提供清晰的高亮微凸视觉。", "DON'T: 激活态与未激活态视觉一致，让用户猜当前模式。", "编辑器的工具模式必须一眼可辨。"),
            new("间距规范", "DO: 严格使用 2~4 DIP 紧凑间距。", "DON'T: 随意拉大至 16+ DIP 导致工具栏松散失衡。", "桌面游戏引擎工具栏追求极高空间利用率。")
        ],
        LiveExamplesFactory = () => XYUI3LiveExamplesFactory.CreateLiveExamples(id)!,
        CompositionFactory = () => XYUI3LiveExamplesFactory.CreateComposition(id)!
    };
}
