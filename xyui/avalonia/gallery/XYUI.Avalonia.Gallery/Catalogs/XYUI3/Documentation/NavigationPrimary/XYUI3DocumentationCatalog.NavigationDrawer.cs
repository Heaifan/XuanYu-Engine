namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static XYUI1ComponentDocument BuildNavigationDrawerDoc(string id, string type) => new(
        id, "抽屉导航", "NavigationDrawer",
        "响应式临时导航容器，在窄屏或空间不足时以模态抽屉形式临时展开，复用 Sidebar/NavigationMenu 完整导航结构。",
        "用于移动端、平板或桌面窗口缩小至紧凑阈值时的全局导航；具备半透明遮罩、Esc 键退出、失焦退出与卸载安全关闭。",
        () => XYUI3GalleryCatalog.CreatePreview(id),
        ["<c:XYNavigationDrawer NavigationState=\"{Binding NavigationState}\" Variant=\"FullSidebar\" />"],
        [new("Full Sidebar", "280 DIP 宽度抽屉，完整映射 Header + 一级导航 + 上下文树 + 置底 Footer", "Modal Overlay"), new("Context Drawer", "紧凑抽屉，仅承载当前模块的二级上下文导航与局部工具", "Context Peek")],
        [new("Open", "抽屉滑出，半透明遮罩可见，捕获键盘焦点"), new("Closed", "抽屉收起，遮罩淡出，恢复主视口焦点"), new("Light Dismiss", "点击半透明遮罩背景或按 Esc 键立即安全关闭")],
        Properties(id),
        [new("XY.Surface.Overlay", "Overlay", "抽屉模态面板底色"), new("XY.Brush.Accent.Default", "Accent", "抽屉内选中导航指示"), new("XY.Border.Color.Subtle", "Subtle", "抽屉边缘分割与右侧投影")],
        type)
    {
        CanonicalIdentity = "3.24 · NavigationDrawer / 抽屉导航",
        Category = "XYUI-3 · 导航与切换",
        Acceptance = "UI + INTERACTION IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE · AWAITING USER INTERACTION ACCEPTANCE",
        QuickStartXaml = """
<StackPanel Spacing="12">
    <!-- 响应式导航抽屉 (模态遮罩 + 侧边栏结构镜像) -->
    <c:XYNavigationDrawer NavigationState="{Binding NavigationState}"
                          Variant="FullSidebar" />
</StackPanel>
""",
        CoreRules =
        [
            new("共享导航状态事实源", "NavigationDrawer 严格复用 XYNavigationState，抽屉内的选中项与桌面主导航完全双向同步。"),
            new("完整模态生命周期", "具备 Open()、Close()、Esc 键监听、Backdrop 点击关闭以及组件卸载时自动关闭的安全生命周期。"),
            new("抽屉非 Sidebar 换皮", "Drawer 具备独立的弹出层、遮罩与焦点管理，不是简单将 Sidebar 设为可见/不可见。"),
            new("移动与桌面协同", "在窄屏下与 BottomNavigation 配合，BottomNavigation 管核心一级，Drawer 管二级或全局菜单。")
        ],
        DoDonts =
        [
            new("状态同步", "DO: 抽屉内导航与外部主导航共用同一个 XYNavigationState。", "DON'T: 在抽屉内部维护一套孤立的选中状态。", "抽屉关闭后主页面必须保持同步。"),
            new("模态安全", "DO: 点击半透明遮罩或按 Esc 键均能立即关闭抽屉。", "DON'T: 缺少遮罩点击关闭导致抽屉卡在屏幕上。", "临时导航必须具备无阻碍退出路径。"),
            new("内容精炼", "DO: 承载清晰的一二级导航与置底设置。", "DON'T: 在抽屉内部嵌套过于复杂的复杂编辑表单。", "抽屉的核心职责是导航，而非复杂编辑。")
        ],
        LiveExamplesFactory = () => XYUI3LiveExamplesFactory.CreateLiveExamples(id)!,
        CompositionFactory = () => XYUI3LiveExamplesFactory.CreateComposition(id)!
    };
}
