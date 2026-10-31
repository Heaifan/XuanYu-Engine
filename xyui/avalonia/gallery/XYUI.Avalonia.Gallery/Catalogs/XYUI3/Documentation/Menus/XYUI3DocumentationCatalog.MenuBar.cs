namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static XYUI1ComponentDocument BuildMenuBarDoc(string id, string type) => new(
        id, "菜单栏", "MenuBar",
        "文字主导的桌面一级菜单栏，以轻量 Hover 与底部 Accent 状态线建立导航层级，承载全局一级命令入口。",
        "用于桌面编辑器窗口顶部（文件、编辑、视图、窗口、帮助）；支持鼠标悬停切换、点击打开、键盘 Left/Right 导航与 Esc 关闭。",
        () => XYUI3GalleryCatalog.CreatePreview(id),
        ["<c:XYMenuBar Classes=\"compact\"><c:XYMenuBarItem Header=\"文件\" /></c:XYMenuBar>"],
        [
            new("底部状态线型", "32 DIP 菜单项，Active 态展示底部 Accent 状态线", "Desktop Header"),
            new("Compact 紧凑型", "34 DIP 顶栏模式（Classes=\"compact\" ShowDivider=\"False\"）", "Editor TopBar")
        ],
        [
            new("Default", "纯文字透明底，依靠自身 Padding 紧凑排列"),
            new("Hover", "浅色轻量圆角背景，不改变整体布局"),
            new("Active", "Accent 文字与底部状态线强调"),
            new("Focus", "XY.Focus.Control 独立无障碍焦点外框"),
            new("Disabled", "45% 低透明度置灰，不可交互")
        ],
        Properties(id),
        [new("XY.Surface.Panel", "Panel", "菜单栏背景"), new("XY.Border.Color.Subtle", "Subtle", "底部分隔线"), new("XY.Brush.Accent.Default", "Accent", "Active 底部状态线")],
        type)
    {
        CanonicalIdentity = "3.01 · MenuBar / 菜单栏",
        Category = "XYUI-3 · 导航与切换",
        Acceptance = "UI IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE",
        QuickStartXaml = """
<c:XYMenuBar Classes="compact" ShowDivider="False">
    <c:XYMenuBarItem Header="文件">
        <c:XYMenu>
            <c:XYMenuItem Header="新建项目" Command="{Binding RunCommand}" CommandParameter="New" />
            <c:XYMenuItem Header="打开工程" Command="{Binding RunCommand}" CommandParameter="Open" />
            <c:XYMenuItem Header="保存场景" Command="{Binding RunCommand}" CommandParameter="Save" />
        </c:XYMenu>
    </c:XYMenuBarItem>
    <c:XYMenuBarItem Header="编辑">
        <c:XYMenu>
            <c:XYMenuItem Header="撤销" Command="{Binding UndoCommand}" />
            <c:XYMenuItem Header="重做 (只读锁定)" Command="{Binding DisabledCommand}" />
        </c:XYMenu>
    </c:XYMenuBarItem>
    <c:XYMenuBarItem Header="工作区" ShowChevron="True">
        <c:XYMenu>
            <c:XYMenuItem Header="地图编辑" CheckKind="Radio" IsChecked="{Binding IsMapEditor, Mode=TwoWay}" />
            <c:XYMenuItem Header="区域编辑" CheckKind="Radio" IsChecked="{Binding IsRegionEditor, Mode=TwoWay}" />
        </c:XYMenu>
    </c:XYMenuBarItem>
</c:XYMenuBar>
""",
        CoreRules =
        [
            new("声明式 AXAML 结构", "支持 XYMenuBar 声明式直挂 XYMenuBarItem，内部直挂 XYMenu，无需后台代码手动添加。"),
            new("紧凑顶栏规范", "桌面编辑器顶栏采用 Classes=\"compact\" 与 ShowDivider=\"False\"，高 34 DIP 透明底保持高信息密度。"),
            new("状态指示与全键盘", "Active 态呈现 Accent 状态线；支持 Left/Right 切换相邻项、Enter/Down 打开、Esc 关闭的完整键盘生命周期。")
        ],
        DoDonts =
        [
            new("顶栏密度", "DO: 在编辑器顶栏使用 Classes=\"compact\"。", "DON'T: 使用固定宽度或厚重边框卡片包装菜单栏。", "菜单栏是一级导航，过度装饰会喧宾夺主。"),
            new("声明式嵌套", "DO: 在 AXAML 中直接声明式嵌套 XYMenu 与 XYMenuItem。", "DON'T: 在 Code-Behind 用代码动态实例化菜单结构。", "声明式结构更直观易读。"),
            new("组件选型", "DO: 横向横跨多命令类别使用 XYMenuBar。", "DON'T: 将单个按钮的上下文菜单手写为单项 MenuBar。", "单一动作下拉应选用 XYDropDownButton。")
        ],
        LiveExamplesFactory = () => XYUI3LiveExamplesFactory.CreateLiveExamples(id)!,
        CompositionFactory = () => XYUI3LiveExamplesFactory.CreateComposition(id)!
    };
}
