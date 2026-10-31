namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static XYUI1ComponentDocument BuildBackForwardNavigationDoc(string id, string type) => new(
        id, "前进后退导航", "BackForwardNavigation",
        "紧凑的线性历史前进后退导航，高度 34 DIP，按钮 28 DIP，集成当前位置展示与长按/右键历史弹出菜单。",
        "用于在用户最近访问的视图、地图、数据集或对象位置之间前进后退；到起点禁用 Back，到终点禁用 Forward，新导航截断旧前进历史。",
        () => XYUI3GalleryCatalog.CreatePreview(id),
        ["<c:XYBackForwardNavigation />"],
        [new("Compact Bar", "34 DIP 高度紧凑条，包含前进/后退按钮、分割线与当前位置展示", "Standard Bar"), new("History Dropdown", "右键前进/后退按钮弹出线性历史菜单，点击直达历史项", "Context Menu")],
        [new("Normal", "可用导航状态，支持点击与 Alt+Left / Alt+Right 快捷键"), new("Disabled", "到达历史起点 (CanGoBack=false) 或终点 (CanGoForward=false) 时禁用对应动作"), new("Branch Truncated", "新导航插入时自动清空原先的前进分支历史")],
        Properties(id),
        [new("XY.Surface.Panel", "Panel", "导航条底色"), new("XY.Brush.Accent.Default", "Accent", "当前位置与高亮"), new("XY.Border.Color.Subtle", "Subtle", "分隔线与边框")],
        type)
    {
        CanonicalIdentity = "3.19 · BackForwardNavigation / 前进后退导航",
        Category = "XYUI-3 · 导航与切换",
        Acceptance = "UI + INTERACTION IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE · AWAITING USER INTERACTION ACCEPTANCE",
        QuickStartXaml = """
<StackPanel Spacing="12">
    <!-- 基础前进后退导航栏 (带当前位置展示) -->
    <c:XYBackForwardNavigation />
</StackPanel>
""",
        CoreRules =
        [
            new("线性历史单一事实源", "基于线性列表与 CurrentIndex 驱动，不维护不可同步的独立 Back/Forward 栈。"),
            new("边界自动禁用", "到达历史最前位置禁用 BackButton，到达最新位置禁用 ForwardButton，禁止越界操作。"),
            new("新导航截断旧分支", "在历史中间节点发起新导航时，彻底截断当前位置之后的所有旧 Forward 记录。"),
            new("34 DIP 高密布局", "按钮高度 28 DIP，条高 34 DIP，当前位置文字自动 Ellipsis 截断防止挤压按钮。")
        ],
        DoDonts =
        [
            new("导航边界", "DO: 起点禁用 Back，终点禁用 Forward。", "DON'T: 允许越界触发无效历史变更。", "保证用户不会进入未定义历史状态。"),
            new("与 Undo/Redo 隔离", "DO: 仅记录视口与页面跳转历史。", "DON'T: 将业务数据撤销与页面导航历史混在一起。", "导航历史与数据编辑撤销属于完全不同的两套历史。"),
            new("尺寸稳定", "DO: 当前位置文字超长使用 Ellipsis 截断。", "DON'T: 文字撑开导致左右操作按钮跳出视口。", "导航入口必须保持在绝对可预测的位置。")
        ],
        LiveExamplesFactory = () => XYUI3LiveExamplesFactory.CreateLiveExamples(id)!,
        CompositionFactory = () => XYUI3LiveExamplesFactory.CreateComposition(id)!
    };
}
