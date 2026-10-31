namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static XYUI1ComponentDocument BuildContextMenuDoc(string id, string type) => new(
        id, "上下文菜单", "ContextMenu",
        "围绕当前目标对象或数据项提供就地操作的右键菜单，带有双层对象识别头，主体复用 XYMenu。",
        "用于视口 Entity、地图 Region、数据集 Dataset、Hierarchy 节点等右键就地操作；支持 AttachTo 绑定与 Esc/轻量关闭。",
        () => XYUI3GalleryCatalog.CreatePreview(id),
        ["<c:XYContextMenu ContextType=\"ENTITY\" ContextName=\"Infantry_023\" />"],
        [new("对象标题型", "顶部包含轻量对象类型 (ContextType) 与对象名称 (ContextName)", "Object Context")],
        [new("Default", "就地浮动面板形态"), new("Header", "弱背景区分的两层对象标题"), new("Danger Group", "底部独立分隔的危险操作组")],
        Properties(id),
        [new("XY.Surface.Overlay", "Overlay", "上下文菜单底色"), new("XY.Shadow.Popup", "Popup", "浮层阴影"), new("XY.Border.Color.Subtle", "Subtle", "顶部分隔线")],
        type)
    {
        CanonicalIdentity = "3.03 · ContextMenu / 上下文菜单",
        Category = "XYUI-3 · 导航与切换",
        Acceptance = "UI IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE",
        QuickStartXaml = """
<c:XYContextMenu ContextType="ENTITY" ContextName="Infantry_023">
    <c:XYContextMenu.Menu>
        <c:XYMenu>
            <c:XYMenuItem Label="定位实体" Icon="Locate" />
            <c:XYMenuItem Label="编辑组件" />
            <c:XYMenu.Separator />
            <c:XYMenuItem Label="删除实体" IsDestructive="True" />
        </c:XYMenu>
    </c:XYContextMenu.Menu>
</c:XYContextMenu>
""",
        CoreRules =
        [
            new("围绕对象语义", "ContextMenu 回答「我现在对这个对象能做什么」，顶部必须明确显示操作对象标识。"),
            new("主体复用规范", "除 ContextHeader 外，命令列表 100% 复用 XYMenu 与 XYMenuItem，严禁制造第二套菜单行实现。"),
            new("手势绑定机制", "通过 AttachTo(Control) 统一挂载 PointerPressed 右键事件与 PlacementMode.Pointer 定位。")
        ],
        DoDonts =
        [
            new("对象识别头", "DO: ContextHeader 保持精简（类型+名称两行），帮助用户核对目标。", "DON'T: 在 Header 塞入长 UUID 或复杂属性表格。", "过度臃肿的信息卡片会破坏快速就地操作体验。"),
            new("手势与关闭", "DO: 支持右键呼出、Esc 关闭及点击空白区 Light Dismiss。", "DON'T: 必须点击关闭按钮才能收起上下文菜单。", "阻断式弹窗严重干扰工作流连贯性。"),
            new("危险命令分区", "DO: 将删除、清空等危险操作用 Separator 隔离并标红。", "DON'T: 将危险操作穿插在常用操作中间。", "极其容易发生无意误触与数据破坏。")
        ],
        LiveExamplesFactory = () => XYUI3LiveExamplesFactory.CreateLiveExamples(id)!,
        CompositionFactory = () => XYUI3LiveExamplesFactory.CreateComposition(id)!
    };
}
