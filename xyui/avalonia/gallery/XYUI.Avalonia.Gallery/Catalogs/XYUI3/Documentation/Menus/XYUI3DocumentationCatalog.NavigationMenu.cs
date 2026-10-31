namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static XYUI1ComponentDocument BuildNavigationMenuDoc(string id, string type) => new(
        id, "导航菜单", "NavigationMenu",
        "长期驻留的应用功能区导航项集合，以左侧 3 DIP Accent Bar 明确持久化当前位置，不同于瞬时命令按钮或平级页签。",
        "用于编辑器主模块（地图、环境、数据、资源、脚本、设置）切换；选中态长期保持，支持分组标题与细分隔线。",
        () => XYUI3GalleryCatalog.CreatePreview(id),
        ["<c:XYNavigationMenu NavigationState=\"{Binding State}\" />"],
        [new("Compact 导航型", "32 DIP 紧凑导航项，左侧 3 DIP Accent Bar，20 DIP 分组标题", "Vertical Layout"), new("Badge 徽标型", "右侧展示状态或计数徽标 (消费真实 Badge / Status)", "XYStatusBadge 适配")],
        [new("Default", "透明底色，文字与图标正常呈现"), new("Hover", "轻微悬浮背景变化"), new("Selected", "浅色 Selected 背景 + 左侧 3 DIP Accent Bar + Accent 图标/文字"), new("With Badge", "展示计数 (如资源 12) 或状态 (如调试 Warning)，Selected 态下文字与徽标均清晰可读"), new("Disabled", "图标与文字对比度衰减，不可导航")],
        Properties(id),
        [new("XY.Surface.Selected", "Selected", "选中态浅蓝背景"), new("XY.Brush.Accent.Default", "Accent", "左侧选中状态指示条"), new("XY.Border.Color.Subtle", "Subtle", "分组间细分隔线")],
        type)
    {
        CanonicalIdentity = "3.05 · NavigationMenu / 导航菜单",
        Category = "XYUI-3 · 导航与切换",
        Acceptance = "UI IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE",
        QuickStartXaml = """
<c:XYNavigationMenu NavigationState="{Binding NavigationState}"
                    Width="246" />

<!-- 导航项消费 (支持 Badge 与 Status) -->
<c:XYNavigationItem Id="resources"
                    Label="引擎资源"
                    Icon="Browse"
                    Badge="12"
                    Status="Info" />
""",
        CoreRules =
        [
            new("持久目的地语义", "NavigationMenu 表达「我现在位于哪个功能区域」，Selected 状态必须长期驻留，不会因点击完成而消失。"),
            new("单一事实源驱动", "所有选中态必须严格来自 XYNavigationState.SelectedId / 路由事实源，严禁 Gallery 私自维护选中变量。"),
            new("三方语义隔离", "Navigation ≠ Button List (执行一次性命令) ≠ Menu (瞬时弹出后消失) ≠ Tabs (平级文档页签)。")
        ],
        DoDonts =
        [
            new("选中表现", "DO: 使用左侧 3 DIP 细 Accent Bar 表达稳定当前位置。", "DON'T: 使用全块大面积高饱和背景覆盖整个导航项。", "高饱和大色块会严重抢夺中央编辑区域的视觉焦点。"),
            new("状态权威", "DO: 通过统一的 NavigationState 监听 Changed 驱动内容切换。", "DON'T: 在 Click 事件中手动强改各 Item 的 IsSelected。", "破坏单一事实源会导致多组件状态不同步。"),
            new("组件选型", "DO: 将功能模块定位使用 NavigationMenu，动作执行使用 Button。", "DON'T: 将新建实体、撤销等命令动作塞入 NavigationMenu。", "混淆导航与动作会导致用户对界面行为产生认知混乱。")
        ],
        LiveExamplesFactory = () => XYUI3LiveExamplesFactory.CreateLiveExamples(id)!,
        CompositionFactory = () => XYUI3LiveExamplesFactory.CreateComposition(id)!
    };
}
