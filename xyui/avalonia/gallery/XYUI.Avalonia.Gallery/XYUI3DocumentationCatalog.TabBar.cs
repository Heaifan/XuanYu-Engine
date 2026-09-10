namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static XYUI1ComponentDocument BuildTabBarDoc(string id, string type) => new(
        id, "标签栏", "TabBar",
        "完整的多页签视口容器，高度 38 DIP，由横向平移区、翻页按钮、溢出菜单与新增页签槽组成。",
        "用于多文档与工作区页签的生命周期管理；支持横向滚轮平移、Previous/Next 翻页、溢出菜单选页与动态新增/关闭。",
        () => XYUI3GalleryCatalog.CreatePreview(id),
        ["<c:XYTabBar SizingMode=\"Content\"><c:XYTab Label=\"Scene_01.map\" IsSelected=\"True\" /></c:XYTabBar>"],
        [new("Compact SVG", "38 DIP Bar，32 DIP 翻页箭头，40 DIP 溢出槽，50 DIP 新增槽", "Desktop Multi-tab")],
        [new("Selected", "复用 XYTab 底部 3 DIP Accent，且底边保持单一"), new("Overflow Active", "溢出按钮激活并展开 Popup 列表"), new("Action Hover", "辅助操作按钮浅色轻量悬浮背景")],
        Properties(id),
        [new("XY.Surface.Panel", "Panel", "标签栏主背景"), new("XY.Surface.PanelAlt", "PanelAlt", "翻页操作槽背景"), new("XY.Border.Color.Default", "Default", "标签栏外围框线 (底边为 0)")],
        type)
    {
        CanonicalIdentity = "3.09 · TabBar / 标签栏",
        Category = "XYUI-3 · 导航与切换",
        Acceptance = "UI + INTERACTION IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE · AWAITING USER INTERACTION ACCEPTANCE",
        QuickStartXaml = """
<c:XYTabBar x:Name="DocumentTabBar" SizingMode="Content">
    <c:XYTab Label="Scene_01.map" IsSelected="True" />
    <c:XYTab Label="Terrain_02.data" IsModified="True" />
    <c:XYTab Label="Lighting.env" />
</c:XYTabBar>
""",
        CoreRules =
        [
            new("单一底线红线 (PRES-3.09-01/02)", "TabBar 底部只允许显示一条 Selected Tab 的 Accent 线，严禁与外层容器产生平行双底线。"),
            new("严格垂直同轴 (PRES-3.09-03/04)", "TabBar 内部文字、Modified 圆点、关闭按钮必须同轴绝对垂直居中，禁止上下错位。"),
            new("交互真实可点 (PRES-3.09-06)", "Previous、Next、Overflow、New 必须具备真实响应，支持滚动平移、浮层选页与增删。")
        ],
        DoDonts =
        [
            new("底线干净度", "DO: TabBar 容器 BorderThickness 底部设为 0，由 Tab Accent 作为唯一底线。", "DON'T: 在外层卡片底部再画一条分隔线造成平行双底线。", "双底边会使整个编辑栏显现杂乱条纹。"),
            new("动态稳定性", "DO: Hover 与 Selected 切换时维持固定高度 (38 DIP)，避免尺寸跳动。", "DON'T: 选中时临时增加边框厚度导致整行抖动。", "高频切换页签时画面抖动会造成严重视觉疲劳。"),
            new("溢出可用性", "DO: 页签超出可视区域时，通过 OverflowButton 弹出完整列表供直接选择。", "DON'T: 仅截断隐藏而无法从菜单检索被遮盖的页签。", "无法访问的被挤出页签会导致工作流受阻。")
        ],
        LiveExamplesFactory = () => XYUI3LiveExamplesFactory.CreateLiveExamples(id)!,
        CompositionFactory = () => XYUI3LiveExamplesFactory.CreateComposition(id)!
    };
}
