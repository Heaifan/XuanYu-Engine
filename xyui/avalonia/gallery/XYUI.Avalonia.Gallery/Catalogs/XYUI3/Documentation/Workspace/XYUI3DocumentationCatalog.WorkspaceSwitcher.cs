namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static XYUI1ComponentDocument BuildWorkspaceSwitcherDoc(string id, string type) => new(
        id, "工作区切换器", "WorkspaceSwitcher",
        "顶栏级紧凑工作区切换器，用于在世界编辑、地图数据、战争模拟等整套工作环境之间切换，支持下拉菜单与管理入口。",
        "用于切换应用完整的上下文环境（包括面板布局、Dock 布局、工具栏等）；当前工作区唯一，采用 Request-Commit 确认机制。",
        () => XYUI3GalleryCatalog.CreatePreview(id),
        ["<c:XYWorkspaceSwitcher State=\"{Binding WorkspaceState}\" />"],
        [new("Compact Dropdown", "34 DIP 高度 Trigger，拉起与 Trigger 同宽的下拉菜单", "Top Bar Switcher"), new("Workspace Management", "底部包含固定的管理工作区入口与分割线", "Dropdown Footer")],
        [new("Selected", "当前激活工作区项带有右侧 Check 标识与 Selected 高亮"), new("Disabled", "IsEnabled=false 时项视觉置灰，拦截点击与按键切换且不触发请求"), new("Icon Identity", "支持可选 Icon 身份修饰，增强工作区辨识度"), new("Popup Open", "展开同宽浮层，支持 Esc 键与失焦退出")],
        Properties(id),
        [new("XY.Surface.Panel", "Panel", "工作区菜单浮层底色"), new("XY.Brush.Accent.Default", "Accent", "选中工作区勾选指示"), new("XY.Border.Color.Subtle", "Subtle", "触发器与分隔线边框")],
        type)
    {
        CanonicalIdentity = "3.20 · WorkspaceSwitcher / 工作区切换器",
        Category = "XYUI-3 · 导航与切换",
        Acceptance = "UI + INTERACTION IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE · AWAITING USER INTERACTION ACCEPTANCE",
        QuickStartXaml = """
<StackPanel Spacing="12">
    <!-- 顶栏紧凑工作区切换器 (同宽下拉菜单) -->
    <c:XYWorkspaceSwitcher State="{Binding WorkspaceState}" />
</StackPanel>
""",
        CoreRules =
        [
            new("完整环境切换语义", "WorkspaceSwitcher 切换整套工作环境（Dock/Sidebar/Toolbar），禁止降级为普通页面 Tab。"),
            new("Request-Commit 事务状态", "工作区切换必须通过 WorkspaceChangeRequested 请求并显式 Accept() 后才 Commit 状态。"),
            new("同宽下拉设计", "下拉浮层宽度与 Trigger 按钮精确对齐，当前工作区项右侧必须包含 Check 标记。"),
            new("独立管理入口", "下拉菜单底部固定提供管理工作区入口，与工作区列表之间采用分割线隔离。")
        ],
        DoDonts =
        [
            new("语义定位", "DO: 仅用于顶层工作环境身份切换。", "DON'T: 替代页面内部的 Tabs 或视图观察模式。", "避免顶层环境与内部内容发生概念混淆。"),
            new("状态保护", "DO: 监听 WorkspaceChangeRequested 并在有未保存数据时弹窗确认。", "DON'T: 强行静默切换导致未保存工作区状态丢失。", "工作区切换影响全局面板状态。"),
            new("下拉对齐", "DO: 下拉菜单宽度严格与触发器宽度保持一致。", "DON'T: 随菜单项文字长度动态伸缩宽度导致跳动。", "同宽下拉保持顶栏整洁稳定。")
        ],
        LiveExamplesFactory = () => XYUI3LiveExamplesFactory.CreateLiveExamples(id)!,
        CompositionFactory = () => XYUI3LiveExamplesFactory.CreateComposition(id)!
    };
}
