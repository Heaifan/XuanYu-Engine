namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static XYUI1ComponentDocument BuildSubMenuDoc(string id, string type) => new(
        id, "子菜单", "SubMenu",
        "层级连接型子菜单，通过 40 DIP 连接器与锚点建立父子关联，承载二级及必要的三级细分命令。",
        "用于一级菜单、右键菜单内部展开子项（如导出、视图模式、高级设置）；支持 OpenRight/OpenLeft 与级联收折。",
        () => XYUI3GalleryCatalog.CreatePreview(id),
        ["<c:XYMenu x:Name=\"RootMenu\" Items=\"{Binding RootItems}\" />", "<c:XYSubMenu ParentMenu=\"{Binding RootMenu}\" ChildMenu=\"{Binding Child}\" Trigger=\"{Binding Trigger}\" />"],
        [new("Open Right (标准)", "从父项右侧水平展开，带 40 DIP 连接线与 Accent 锚点", "默认变体"), new("Open Left (镜像)", "当贴近屏幕右边缘时自动向左镜像展开", "空间自适应")],
        [new("Active Trigger", "父菜单项在子菜单展开期间始终保持 Active 状态"), new("Opening / Open", "子菜单面板展开并捕获方向键焦点"), new("Closing", "光标离开或 Esc 时平滑收拢并恢复父项")],
        Properties(id),
        [new("XY.Surface.Overlay", "Overlay", "子菜单面板背景"), new("XY.Brush.Accent.Default", "Accent", "连接锚点色"), new("XY.Border.Color.Subtle", "Subtle", "连接器连线色")],
        type)
    {
        CanonicalIdentity = "3.04 · SubMenu / 子菜单",
        Category = "XYUI-3 · 导航与切换",
        Acceptance = "UI IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE",
        QuickStartXaml = """
<c:XYMenu x:Name="RootMenu" Items="{Binding RootItems}" />
<c:XYSubMenu ParentMenu="{Binding RootMenu}"
             ChildMenu="{Binding ChildMenu}"
             Trigger="{Binding Trigger}"
             OpenLeft="False" />
""",
        CoreRules =
        [
            new("固定根宿主", "RootMenu 只由根宿主承载一次；XYSubMenu 仅管理 Trigger 到 ChildMenu 的级联浮层关系。"),
            new("层级可视连线", "父子菜单间具备 40 DIP 专用连接器与 Accent 锚点，视觉可明确追溯命令源头。"),
            new("父级状态锚定", "光标滑入子菜单时，父项必须保持 Active 强调态，严禁产生 Hover 丢失闪烁。"),
            new("双向展开与级联", "支持 OpenLeft 镜像展开；父级关闭时，其所有后代子菜单必须递归协同收拢。")
        ],
        DoDonts =
        [
            new("层级控制", "DO: 子菜单最多嵌套 2~3 层，保持命令层级清晰浅显。", "DON'T: 无限制深层嵌套至 4 层以上。", "深层级会导致光标移动易脱轨、操作负担急剧上升。"),
            new("自适应方向", "DO: 贴近屏幕边界时切换为 OpenLeft 避免被视口裁剪。", "DON'T: 固定向右展开导致超出屏幕右侧不可见。", "屏幕外溢出是菜单可用性的严重缺陷。"),
            new("指示器统一", "DO: 统一使用 HasSubMenu = true 渲染右箭头 Chevron。", "DON'T: 在 Label 文字中硬编码「>」或「▶」字符。", "硬编码字符无法满足不同语言与高 DPI 矢量对齐。")
        ],
        LiveExamplesFactory = () => XYUI3LiveExamplesFactory.CreateLiveExamples(id)!,
        CompositionFactory = () => XYUI3LiveExamplesFactory.CreateComposition(id)!
    };
}
