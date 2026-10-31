namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static XYUI1ComponentDocument BuildCommandBarDoc(string id, string type) => new(
        id, "命令栏", "CommandBar",
        "紧凑的一次性页面或对象命令栏，支持 Standard 与 Contextual 变体，集成 Primary、Danger 角色与 More 溢出菜单。",
        "用于新建、导入、保存、校验、删除等动作执行；严格与视口常驻模式切换的 Toolbar 分离。",
        () => XYUI3GalleryCatalog.CreatePreview(id),
        ["<c:XYCommandBar><c:XYCommandItem Label=\"保存\" CommandId=\"save\" /></c:XYCommandBar>"],
        [new("Standard Bar", "34 DIP 高度，28 DIP 动作项，右侧带 More 溢出按钮", "View / Page Header"), new("Contextual Bar", "前置「已选择 · 实体标识」与分割线，针对选中目标快速执行", "Selection Inspector")],
        [new("Primary", "高饱和主题色主命令按钮"), new("Normal", "标准次级动作按键"), new("Danger", "破坏性删除指令，自动前置分割线"), new("More Popup", "集成 XYMenu 展开扩展操作")],
        Properties(id),
        [new("XY.Surface.Panel", "Panel", "命令栏底色"), new("XY.Brush.Accent.Default", "Accent", "Primary 命令背景色"), new("XY.Brush.Status.Error", "Error", "Danger 命令强调色")],
        type)
    {
        CanonicalIdentity = "3.17 · CommandBar / 命令栏",
        Category = "XYUI-3 · 导航与切换",
        Acceptance = "UI + INTERACTION IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE · AWAITING USER INTERACTION ACCEPTANCE",
        QuickStartXaml = """
<StackPanel Spacing="12">
    <!-- 标准命令栏：新建 (Primary)、导入、保存、删除 (Danger) 及溢出 -->
    <c:XYCommandBar>
        <c:XYCommandItem Label="新建" CommandId="new" Role="Primary" Icon="Add" />
        <c:XYCommandItem Label="导入" CommandId="import" />
        <c:XYCommandItem Label="保存" CommandId="save" />
        <c:XYCommandItem Label="删除" CommandId="delete" Role="Danger" />
    </c:XYCommandBar>
    <!-- 上下文命令栏：带实体名称提示与专属操作 -->
    <c:XYCommandBar Variant="Contextual" ContextIdentity="核心要塞道路_01">
        <c:XYCommandItem Label="编辑" CommandId="edit" />
        <c:XYCommandItem Label="复制" CommandId="copy" />
        <c:XYCommandItem Label="验证" CommandId="validate" />
    </c:XYCommandBar>
</StackPanel>
""",
        CoreRules =
        [
            new("命令角色视觉分级体系", "Primary 突显主推进命令；Danger 破坏性命令自动添加左侧细分割线隔离；Normal 保持标准低强调度。"),
            new("Contextual 上下文精准映射", "支持 ContextIdentity 显示已选对象名与竖向分割线，为特定实体提供针对性命令集合。"),
            new("More 溢出菜单无缝弹出", "多余命令自动折叠于右侧 More 按钮中，点击展开标准 XYMenu，支持 Esc 键与失焦退出。"),
            new("与 Toolbar 明确职责分工", "CommandBar 专职一次性动作分发，不保持持续激活态；Toolbar 专职模式切换。")
        ],
        DoDonts =
        [
            new("危险命令隔离", "DO: Danger 角色自动前置分割线并置于靠后位置。", "DON'T: 将破坏性删除与主要保存命令紧贴并列。", "视觉区隔可有效防止误触关键数据丢失。"),
            new("上下文明确度", "DO: 对选中对象操作使用 Contextual 变体明确显示实体名称。", "DON'T: 隐式改变全局命令而无任何目标指示。", "实体名称指示让用户操作明确无误。"),
            new("架构混淆", "DO: 视口工具选 Toolbar，操作动作选 CommandBar。", "DON'T: 把移动/旋转等视口模式做成 CommandBar 按钮。", "模式切换与动作执行必须分流。")
        ],
        LiveExamplesFactory = () => XYUI3LiveExamplesFactory.CreateLiveExamples(id)!,
        CompositionFactory = () => XYUI3LiveExamplesFactory.CreateComposition(id)!
    };
}
