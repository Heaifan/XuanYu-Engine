namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static IReadOnlyList<XYUIDocProperty> Properties(string id) => id switch
    {
        "XYUI-3-3.01" => MenuBarProperties(),
        "XYUI-3-3.02" => MenuProperties(),
        "XYUI-3-3.08" => TabsProperties(),
        "XYUI-3-3.09" => TabBarProperties(),
        "XYUI-3-3.07" => NavigationRailProperties(),
        "XYUI-3-3.23" => BottomNavProperties(),
        _ => []
    };

    static IReadOnlyList<XYUIDocProperty> MenuBarProperties() =>
    [
        P("XYMenuBar.Items", "IList<XYMenuBarItem>", "[]", "一级菜单项集合，AXAML [Content] 声明式挂载。"),
        P("XYMenuBar.ShowDivider", "bool", "true", "是否渲染底部分隔线，顶栏紧凑场景设为 False。"),
        P("XYMenuBar.Classes=\"compact\"", "string", "—", "紧凑顶栏样式（34 DIP 高度，透明背景）。"),
        P("XYMenuBarItem.Header / Label", "string", "\"\"", "一级菜单项标题文本。"),
        P("XYMenuBarItem.Menu", "XYMenu?", "null", "展开的浮动菜单容器，AXAML [Content] 声明式嵌套。"),
        P("XYMenuBarItem.ShowChevron", "bool", "false", "是否在标题右侧显示 Chevron 下拉箭头。"),
        P("XYMenuBarItem.IsActive", "bool", "false", "当前菜单项是否处于激活状态（底部 Accent 线）。")
    ];

    static IReadOnlyList<XYUIDocProperty> MenuProperties() =>
    [
        P("XYMenu.Items", "IList<Control>", "[]", "菜单命令项集合，AXAML [Content] 声明式挂载。"),
        P("XYMenuItem.Command", "object? (ICommand)", "null", "触发执行命令，支持 ICommand 绑定与 Action 委托。"),
        P("XYMenuItem.CommandParameter", "object?", "null", "命令参数，支持同一 ICommand 复用于多个菜单项。"),
        P("XYMenuItem.IsChecked", "bool", "false", "勾选/单选状态，支持 TwoWay 绑定外部 VM 真源。"),
        P("XYMenuItem.CheckKind", "XyuiMenuCheckKind", "None", "枚举状态：None、Check、Radio。"),
        P("XYMenuItem.Shortcut", "string", "\"\"", "右侧稳定对齐快捷键提示文本（如 Ctrl+S）。"),
        P("XYMenuItem.Icon", "XyuiVectorIcon?", "null", "左侧前缀矢量图标。")
    ];

    static IReadOnlyList<XYUIDocProperty> BottomNavProperties() =>
    [
        P("XYBottomNavigationItem.Id", "string", "必填", "目的地唯一标识；SelectDestination 使用它。"),
        P("XYBottomNavigationItem.Label", "string", "必填", "目的地显示文本。"),
        P("XYBottomNavigationItem.Icon", "XyuiVectorIcon", "必填", "来自 XYUI Vector Icon Registry。"),
        P("XYBottomNavigationItem.Badge", "string?", "null", "可选状态提示文本；null 时不显示 Badge。"),
        P("XYBottomNavigationItem.IsEnabled", "bool", "true", "false 时目的地不可点击。"),
        P("NavigationState", "XYNavigationState", "必填", "共享目的地与当前 SelectedId 的状态源。"),
        P("Items", "IReadOnlyList<XYBottomNavigationItem>", "state.Entries", "只读目的地集合，按等宽 Slot 渲染。")
    ];

    static IReadOnlyList<XYUIDocProperty> TabsProperties() =>
    [
        P("XYTabs.SizingMode", "XyuiTabSizingMode", "Equal", "Equal 保持等宽兼容布局；Content 按每个 Tab 的内容宽度排版。"),
        P("XYTabs.Items", "ObservableCollection<XYTab>", "[]", "单组平级页签集合，不承担滚动、溢出与新增操作。")
    ];

    static IReadOnlyList<XYUIDocProperty> TabBarProperties() =>
    [
        P("XYTabBar.SizingMode", "XyuiTabSizingMode", "Equal", "转发到内部 XYTabs；Content 模式与翻页/溢出宿主协同。"),
        P("XYTabBar.Items", "IList<XYTab>", "[]", "多文档页签集合，支持滚动、翻页、溢出与新增。")
    ];

    static IReadOnlyList<XYUIDocProperty> NavigationRailProperties() =>
    [
        P("NavigationState", "XYNavigationState", "必填", "唯一导航事实源，负责 SelectedId、RequestNavigation 与禁用目的地。"),
        P("LayoutVariant", "XyuiNavigationLayoutVariant", "Default", "Default 保持纯图标 Rail；Workspace 使用图标上置、标签常驻的堆叠布局。"),
        P("XYNavigationItem.Label", "string", "必填", "Workspace 变体中始终可见的一级工作区标签。"),
        P("XYNavigationItem.Icon", "XyuiVectorIcon", "必填", "来自 XYUI Vector Icon Registry 的导航图标。"),
        P("XYNavigationItem.IsEnabled", "bool", "true", "false 时不可点击、不可导航，并使用 Disabled 视觉。")
    ];

    static XYUIDocProperty P(string name, string type, string value, string description) =>
        new(name, type, value, description);
}
