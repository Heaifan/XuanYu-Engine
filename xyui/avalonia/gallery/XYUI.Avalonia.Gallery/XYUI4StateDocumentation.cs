namespace XYUI.Avalonia.Gallery;

static partial class XYUI4StateDocumentation
{
    public static string? Overview(string id) => id switch
    {
        "XYUI-4-4.02" => "SelectedState 表达用户明确选择并持续保留的对象身份。",
        "XYUI-4-4.03" => "ActiveState 表达对象或工具当前正在生效的持续激活状态。",
        "XYUI-4-4.04" => "SelectionContextFocus 表达选择上下文中独立于 Selected 的键盘焦点。",
        "XYUI-4-4.05" => "MultiSelection 表达同一 Selection Set 中的 Primary 与 Secondary 对象。",
        "XYUI-4-4.06" => "SelectionGroup 表达多个对象在交互语义上属于同一组。",
        "XYUI-4-4.07" => "MarqueeSelection 表达矩形候选区域，不直接提交业务选择。",
        "XYUI-4-4.08" => "LassoSelection 表达自由路径候选区域，不直接提交业务选择。",
        "XYUI-4-4.09" => "SelectionOutline 表达已提交选择结果与对象边界的分离轮廓。",
        "XYUI-4-4.10" => "BoundingBox 表达当前 Transform 工具上下文中的对象边界与操作手柄。",
        "XYUI-4-4.12" => "DropIndicator 表达当前 Drag Payload 对目标的 Valid、Invalid 或 Conditional 放置语义。",
        "XYUI-4-4.13" => "InsertionIndicator 表达 Drag Item 最终插入的精确位置、层级与 Before / Into / After 关系。",
        _ => null
    };

    public static string? WhenToUse(string id) => id switch
    {
        "XYUI-4-4.02" => "用于 Tree、List、Navigation、Inspector、Canvas Object 和地图对象。",
        "XYUI-4-4.03" => "用于当前工具、编辑模式或持续生效的操作上下文。",
        "XYUI-4-4.04" => "用于可键盘导航的按钮、列表项、菜单项和输入入口。",
        "XYUI-4-4.05" => "用于日志、列表、画布和地图对象的 Ctrl/Shift 多选。",
        "XYUI-4-4.06" => "用于图层组、地图对象组和复杂矢量对象的整体上下文。",
        "XYUI-4-4.07" => "用于画布、地图和图层面板中的包含式或穿越式矩形选择。",
        "XYUI-4-4.08" => "用于需要自由轮廓命中候选对象的画布和地图交互。",
        "XYUI-4-4.09" => "用于已选对象的稳定边界反馈，不改变对象原始 Fill。",
        "XYUI-4-4.10" => "用于 Resize、Rotate、Pivot 和 Group Transform 操作上下文。",
        "XYUI-4-4.12" => "用于 Container、Slot、Tree Parent、Canvas Region 和 Inspector Target 的放置反馈。",
        "XYUI-4-4.13" => "用于 List、Tree、Layer、Card、Asset 和 Timeline 的稳定重排位置反馈。",
        _ => null
    };

    public static string[] Usages(string id) => id switch
    {
        "XYUI-4-4.02" => ["<ListBoxItem Classes=\"xyui-interactive xyui-selectable\" IsSelected=\"True\" />"],
        "XYUI-4-4.03" => ["<ToggleButton Classes=\"xyui-active\" IsChecked=\"True\" />"],
        "XYUI-4-4.04" => ["<Button Classes=\"xyui-interactive xyui-focusable\" Content=\"对象\" />"],
        "XYUI-4-4.05" => ["<ListBox SelectionMode=\"Multiple\" Classes=\"xyui-multi-selection\" />"],
        "XYUI-4-4.06" => ["<Border Classes=\"xyui-selection-group\"><ListBox SelectionMode=\"Multiple\" /></Border>"],
        "XYUI-4-4.07" => ["<c:XYMarqueeSelection IsCrossing=\"False\" />"],
        "XYUI-4-4.08" => ["<c:XYLassoSelection Points=\"{Binding LassoPoints}\" />"],
        "XYUI-4-4.09" => ["<c:XYSelectionOutline Points=\"{Binding SelectionPoints}\" />"],
        "XYUI-4-4.10" => ["<c:XYBoundingBox BoundsRect=\"{Binding TransformBounds}\" />"],
        "XYUI-4-4.12" => ["<c:XYDropIndicator IsActive=\"True\" State=\"Valid\" />"],
        "XYUI-4-4.13" => ["<c:XYInsertionIndicator IsActive=\"True\" Mode=\"Before\" />"],
        _ => []
    };

    public static XYUIDocVariant[] Variants(string id) => id switch
    {
        "XYUI-4-4.02" => [new("Filled Selection", "低饱和 Selected Surface", "List / Inspector"), new("Edge Anchored", "稳定边缘锚点", "Navigation / Tree")],
        "XYUI-4-4.03" => [new("Persistent", "持续激活", "工具 / 模式"), new("Pressed", "瞬时按压反馈", "Button / Drag")],
        "XYUI-4-4.04" => [new("Outer Ring", "独立焦点环", "Button / List"), new("Border Focus", "输入边框焦点", "Input Field"), new("Dual Ring", "内外双环", "高辨识度入口")],
        "XYUI-4-4.05" => [new("Primary", "完整 Selected 视觉", "主要对象"), new("Secondary", "弱化但清晰的 Selected", "其余对象"), new("Group Bounds", "条件型整体范围", "整体变换")],
        "XYUI-4-4.06" => [new("Group Container", "透明组容器", "图层组 / 对象组"), new("Group Header", "组上下文标题", "组操作")],
        "XYUI-4-4.07" => [new("Window", "包含式框选", "完全位于矩形内"), new("Crossing", "穿越式框选", "与矩形相交")],
        "XYUI-4-4.08" => [new("Candidate Path", "候选路径", "低平滑度"), new("Closed Result", "闭合结果", "交给 Selection Set")],
        "XYUI-4-4.09" => [new("Separation", "分离轮廓", "对象边界与选择反馈"), new("Accent", "选择强调线", "XY.Editor.Selection")],
        "XYUI-4-4.10" => [new("Resize", "八个缩放手柄", "Resize Mode"), new("Rotate", "旋转手柄", "Rotate Mode"), new("Pivot", "原点手柄", "Pivot Edit")],
        "XYUI-4-4.12" => [new("Target Border", "目标边框", "Container / Slot"), new("Semantic Mark", "状态符号", "Valid / Invalid / Conditional")],
        "XYUI-4-4.13" => [new("Anchored Insert Line", "锚定插入线", "List / Tree"), new("Gap Preview", "结构占位预览", "Card / Asset")],
        _ => []
    };

    public static XYUIDocState[] States(string id) => id switch
    {
        "XYUI-4-4.02" => [new("Selected", "指针离开后继续保持"), new("Selected + Hover", "Selected 为主，Hover 仅轻度变化"), new("Selected + Disabled", "保留身份但降低可操作感")],
        "XYUI-4-4.03" => [new("Persistent Active", "当前工具或模式持续生效"), new("Pressed", "按压期间的瞬时反馈")],
        "XYUI-4-4.04" => [new("Keyboard Focus", "键盘导航目标"), new("Selected + Focus", "焦点环独立叠加"), new("Disabled", "不可获得新的交互焦点")],
        "XYUI-4-4.05" => [new("Primary + Secondary", "同一 Selection Set 的层级"), new("Selected + Focus", "Focus 不改变集合")],
        "XYUI-4-4.06" => [new("Group Selected", "整体组上下文"), new("Member Editing", "进入组内编辑成员")],
        "XYUI-4-4.07" => [new("Dragging", "拖动时显示候选框"), new("Committed", "释放后交给选择集合")],
        "XYUI-4-4.08" => [new("Drawing", "指针拖动形成路径"), new("Committed", "释放后交给选择集合")],
        "XYUI-4-4.09" => [new("Selected", "选择结果已提交"), new("Stable", "指针离开后保持")],
        "XYUI-4-4.10" => [new("Resize", "显示八个边角手柄"), new("Rotate", "显示旋转手柄"), new("Pivot", "显示 Pivot 手柄")],
        "XYUI-4-4.12" => [new("None", "没有有效目标"), new("Valid", "Pointer Up 可以提交"), new("Invalid", "Pointer Up 不提交"), new("Conditional", "需要转换或额外动作")],
        "XYUI-4-4.13" => [new("Before", "插入目标之前"), new("Into", "插入目标内部"), new("After", "插入目标之后")],
        _ => []
    };

}
