namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static XYUI1ComponentDocument BuildCommandPaletteDoc(string id, string type) => new(
        id, "命令面板", "CommandPalette",
        "紧凑快速命令检索面板，宽度 440~600 DIP，集成前缀过滤、键盘闭环导航、右侧详情与快捷键直达。",
        "用于全局命令搜索、资产实体直达、工作区导航与引擎设置；拒绝膨胀为大尺寸模态或聊天弹窗。",
        () => XYUI3GalleryCatalog.CreatePreview(id),
        ["<c:XYCommandPalette Width=\"440\" />"],
        [new("Compact Desktop", "440 DIP 紧凑宽度，34 DIP 检索框，左右双栏结构", "Standard Palette"), new("Full Width", "600 DIP 宽幅面板，适合深层资产与路径检索", "Deep Hierarchy")],
        [new("Recent", "输入框为空时展示最近使用命令"), new("Filtered", "输入关键字即时计算并高亮匹配结果"), new("Focused", "键盘上下键选中项高亮，并驱动右侧详情更新"), new("No Results", "检索无匹配时展示正式空状态与提示"), new("Disabled Command", "命令因依赖未就绪处于禁用态，可见但不可执行")],
        Properties(id),
        [new("XY.Surface.Panel", "Panel", "命令面板底色"), new("XY.Brush.Accent.Default", "Accent", "键盘选中行标记"), new("XY.Border.Color.Subtle", "Subtle", "结果与详情分隔线")],
        type)
    {
        CanonicalIdentity = "3.18 · CommandPalette / 命令面板",
        Category = "XYUI-3 · 导航与切换",
        Acceptance = "UI + INTERACTION IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE · AWAITING USER INTERACTION ACCEPTANCE",
        QuickStartXaml = """
<!-- 全局快速命令面板 (可通过快捷键或按钮唤起) -->
<c:XYCommandPalette Width="440">
    <!-- 内部包含 XYSearchField、结果视口、详情栏与作用域下拉菜单 -->
</c:XYCommandPalette>
""",
        CoreRules =
        [
            new("高密尺寸与左右双栏布局", "宽度严格控制在 440~600 DIP，左侧 28 DIP 紧凑结果列表，右侧轻量详情栏，杜绝界面臃肿。"),
            new("多前缀作用域快速过滤", "支持输入前缀（> 命令、@ 对象、# 导航、: 设置）及搜索框前置下拉菜单切换检索维度。"),
            new("全键盘操作闭环", "支持 Up/Down 滚动光标、Enter 确认执行、Esc 退出，确保高频开发者无需脱离键盘。"),
            new("最近命令与实时详情驱动", "无输入时展示 RecentItems 历史，选择项即时驱动右侧标题、描述与快捷键联动更新。")
        ],
        DoDonts =
        [
            new("面板尺寸", "DO: 维持 440~600 DIP 桌面高密尺寸。", "DON'T: 做成全屏大覆盖层或大号 AI 对话框。", "命令面板是极速直达工具，必须保持小巧敏捷。"),
            new("键盘友好", "DO: 全程可用键盘箭头、Enter 与 Esc 操作。", "DON'T: 必须用鼠标点击才能执行。", "脱离键盘会严重阻断高阶用户的操作流。"),
            new("详情同步", "DO: 键盘光标移动时右侧详情即时同步更新。", "DON'T: 右侧详情常年空白或延迟刷新。", "及时反馈让用户在按回车前清楚预判操作后果。")
        ],
        LiveExamplesFactory = () => XYUI3LiveExamplesFactory.CreateLiveExamples(id)!,
        CompositionFactory = () => XYUI3LiveExamplesFactory.CreateComposition(id)!
    };
}
