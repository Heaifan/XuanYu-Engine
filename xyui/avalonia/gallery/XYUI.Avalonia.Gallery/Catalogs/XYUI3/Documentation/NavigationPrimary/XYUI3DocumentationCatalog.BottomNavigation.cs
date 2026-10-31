namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static XYUI1ComponentDocument BuildBottomNavigationDoc(string id, string type) => new(
        id, "底部导航", "BottomNavigation",
        "移动端与窄屏辅助应用的等宽底部目的地导航，标准高度 64 DIP，图标在上、标签在下，支持独立 Primary Action。",
        "用于移动端、触控屏或紧凑预览壳的核心一级目的地切换（3~5 个）；等宽槽位，切换时零抖动，可选中央主操作按钮。",
        () => XYUI3GalleryCatalog.CreatePreview(id),
        ["<c:XYBottomNavigation NavigationState=\"{Binding NavigationState}\" Width=\"400\" />"],
        [new("Standard Destinations", "3~5 个核心目的地等宽槽位，图标在上，标签在下", "Standard Mobile"), new("Primary Action Floating", "中央独立主操作按钮，与导航目的地语义分离", "Action Center")],
        [new("Selected", "选中的目的地图标与文字呈现 Accent 高亮，背景提供浅层 Selected 区域"), new("Badge / Status", "目的地右上角叠加数字角标或未读状态点 (XYStatusDot)"), new("Normal", "普通未选中目的地，文字与图标保持低对比度")],
        Properties(id),
        [new("XY.Surface.Panel", "Panel", "底部导航栏主底色"), new("XY.Brush.Accent.Default", "Accent", "选中目的地图标与文字高亮"), new("XY.Border.Color.Subtle", "Subtle", "导航栏顶部边框线")],
        type)
    {
        CanonicalIdentity = "3.23 · BottomNavigation / 底部导航",
        Category = "XYUI-3 · 导航与切换",
        Acceptance = "UI + INTERACTION IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE · AWAITING USER INTERACTION ACCEPTANCE",
        QuickStartXaml = """
<StackPanel Spacing="12">
    <!-- 标准等宽移动端底部导航 -->
    <c:XYBottomNavigation NavigationState="{Binding NavigationState}"
                          Width="400" />
</StackPanel>
""",
        CoreRules =
        [
            new("等宽槽位无抖动布局", "所有目的地槽位平分整个导航栏宽度，选中态绝不改变槽位尺寸，严防横向跳动。"),
            new("图标在上标签在下", "单项固定为垂直排布（Icon 在上、Label 在下），窄屏下文本截断或紧凑排布，禁止横向溢出。"),
            new("Primary Action 语义分离", "中央主操作按钮（如新建/扫描）仅触发 PrimaryActionRequested 事件，绝不改变导航选中状态。"),
            new("64 DIP 移动端人机工效", "导航栏高度固定为 64 DIP，触控命中区稳定，底部安全区由宿主容器提供。")
        ],
        DoDonts =
        [
            new("槽位等宽", "DO: 所有目的地平分宽度保持操作点绝对稳定。", "DON'T: 选中项放大宽度导致其他槽位被挤压移动。", "触控环境下尺寸跳动极易引起误触。"),
            new("目的地数量", "DO: 严格限制在 3 到 5 个核心目的地。", "DON'T: 塞入 6 个以上项目导致图标文字重叠。", "目的地过多应转为 NavigationDrawer。"),
            new("动作隔离", "DO: 中央快捷操作与页面切换完全解耦。", "DON'T: 把新建动作做成一个选中的页面 Destination。", "模式与动作混淆会破坏导航状态机。")
        ],
        LiveExamplesFactory = () => XYUI3LiveExamplesFactory.CreateLiveExamples(id)!,
        CompositionFactory = () => XYUI3LiveExamplesFactory.CreateComposition(id)!
    };
}
