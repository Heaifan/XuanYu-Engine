namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static XYUI1ComponentDocument BuildMenuDoc(string id, string type) => new(
        id, "菜单", "Menu",
        "标准桌面级浮动命令面板，严格提供 Leading、Label、Shortcut、Chevron 稳定四列布局与命令分组。",
        "作为 MenuBar、ContextMenu、SubMenu 的底层命令容器；支持鼠标悬浮、点击执行、Up/Down 键盘导航与 Esc 关闭。",
        () => XYUI3GalleryCatalog.CreatePreview(id),
        ["<c:XYMenu><c:XYMenuItem Header=\"保存\" Command=\"{Binding SaveCommand}\" /></c:XYMenu>"],
        [new("标准桌面型", "浮动 Overlay 面板，紧凑行高 (28~32 DIP)，细边框与投影", "Overlay Surface"), new("嵌入型 (Embedded)", "作为工具箱或右键面板内部嵌入使用", "In-Panel")],
        [
            new("Default", "标准静止态，透明底无多余修饰"),
            new("Hover", "浅色轻量圆角背景，不改变整体布局"),
            new("Pressed", "点击瞬间微暗反馈"),
            new("Focus", "XY.Focus.Control 独立无障碍焦点外框"),
            new("Disabled", "CanExecute=false 时 45% 低透明度置灰并禁止交互"),
            new("Checked", "CheckKind=\"Check\" 勾选态，左侧显示勾选图标"),
            new("Radio Selected", "CheckKind=\"Radio\" 互斥单选态，左侧显示单选圆点"),
            new("Popup Open", "弹出层激活状态，Overlay 阴影与顶层 z-index 保证")
        ],
        Properties(id),
        [new("XY.Surface.Overlay", "Overlay", "菜单浮动面板背景"), new("XY.Shadow.Popup", "Popup", "浮层阴影"), new("XY.Border.Color.Subtle", "Subtle", "面板边框与分隔线")],
        type)
    {
        CanonicalIdentity = "3.02 · Menu / 菜单",
        Category = "XYUI-3 · 导航与切换",
        Acceptance = "UI IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE",
        QuickStartXaml = """
<c:XYMenu>
    <c:XYMenuItem Header="新建" Command="{Binding RunCommand}" CommandParameter="New" Shortcut="Ctrl+N" />
    <c:XYMenuItem Header="打开" Command="{Binding RunCommand}" CommandParameter="Open" Shortcut="Ctrl+O" />
    <c:XYMenuItem Header="保存" Command="{Binding RunCommand}" CommandParameter="Save" Shortcut="Ctrl+S" />
    <c:XYMenuItem Header="重做 (只读锁定)" Command="{Binding DisabledCommand}" />
    <c:XYSeparator Classes="xyui-menu-separator" />
    <c:XYMenuItem Header="构造网格" CheckKind="Check" IsChecked="{Binding IsGridVisible, Mode=TwoWay}" />
    <c:XYMenuItem Header="世界原点" CheckKind="Check" IsChecked="{Binding IsOriginVisible, Mode=TwoWay}" />
    <c:XYSeparator Classes="xyui-menu-separator" />
    <c:XYMenuItem Header="地图编辑" CheckKind="Radio" IsChecked="{Binding IsMapEditor, Mode=TwoWay}" />
    <c:XYMenuItem Header="区域编辑" CheckKind="Radio" IsChecked="{Binding IsRegionEditor, Mode=TwoWay}" />
</c:XYMenu>
""",
        CoreRules =
        [
            new("MVVM 命令与参数", "支持 ICommand 绑定与 CommandParameter 参数复用；自动监听 CanExecuteChanged 驱动 IsEnabled，禁止 View 手动控制颜色。"),
            new("状态真源与单选复选", "Check（勾选）与 Radio（互斥单选）支持 TwoWay 绑定；VM / 外部状态为唯一事实源，Menu 不产生独立业务状态。"),
            new("控件选型边界", "横向顶栏一级分类用 XYMenuBar；局部单一按钮下拉用 XYDropDownButton；菜单内状态切换与互斥单选使用 XYMenuItem Check/Radio。")
        ],
        DoDonts =
        [
            new("命令绑定", "DO: 在 AXAML 中直接绑定 ViewModel 的 ICommand 并传递 CommandParameter。", "DON'T: 在 Code-Behind 用 Click/Invoked 手写模拟业务命令。", "保证 MVVM 单向解耦与可测试性。"),
            new("状态维护", "DO: 业务状态存放在 ViewModel / 状态管理器中，通过 IsChecked 双向同步。", "DON'T: 由 Menu 内部私自缓存或决定业务单选开关。", "状态真源必须集中。"),
            new("可用性指示", "DO: 通过 Command.CanExecute 控制可用性，视觉自动 45% 禁用置灰。", "DON'T: 手动修改 Foreground 字体颜色模拟禁用态。", "保持统一设计规范与状态可达性。")
        ],
        LiveExamplesFactory = () => XYUI3LiveExamplesFactory.CreateLiveExamples(id)!,
        CompositionFactory = () => XYUI3LiveExamplesFactory.CreateComposition(id)!
    };
}
