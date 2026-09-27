namespace XYUI.Avalonia.Gallery;

static class XYUI4StateDocumentation
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
        _ => []
    };

    public static XYUIDocProperty[] Properties(string id) => id switch
    {
        "XYUI-4-4.02" => [P("Class", "xyui-selectable", "Required"), P("IsSelected", "bool", "由组件或业务状态驱动")],
        "XYUI-4-4.03" => [P("Class", "xyui-active", "Persistent Active"), P("Pressed", "Avalonia 原生状态", "瞬时 Active 不独立造状态")],
        "XYUI-4-4.04" => [P("Class", "xyui-focusable", "Required"), P("Focus", "Keyboard / Pointer / Programmatic", "由焦点来源驱动")],
        "XYUI-4-4.05" => [P("SelectionMode", "Multiple", "Required"), P("Primary", "single", "同一集合最多一个")],
        "XYUI-4-4.06" => [P("Members", "Selection Set", "Primary + Secondary"), P("GroupBounds", "conditional", "整体变换时显示")],
        "XYUI-4-4.07" => [P("IsCrossing", "bool", "包含式 / 穿越式"), P("Width / Height", "double", "由视口拖动几何提供")],
        "XYUI-4-4.08" => [P("Points", "IReadOnlyList<Point>", "低平滑度路径"), P("Stroke / Fill", "IBrush?", "由 Foundation Token 提供")],
        "XYUI-4-4.09" => [P("Points", "IReadOnlyList<Point>", "对象边界路径"), P("SeparationBrush", "IBrush?", "GAP-001 未定义时保持空")],
        "XYUI-4-4.10" => [P("BoundsRect", "Rect", "由 Transform 上下文提供"), P("HandleSize", "6–8 DIP", "视觉尺寸"), P("ShowRotationHandle / ShowPivot", "bool", "模式开关")],
        _ => []
    };

    public static XYUIDocToken[] Tokens(string id) => id switch
    {
        "XYUI-4-4.02" => [T("Surface", "XY.Brush.State.Color.Selected"), T("Border", "XY.Brush.Border.Color.Selected"), T("Width", "XY.Border.Width.Selected")],
        "XYUI-4-4.03" => [T("Persistent", "XY.Brush.State.Color.Active"), T("Pressed", "XY.Brush.State.Color.Pressed"), T("LayoutShift", "XY.State.ResizeOnChange")],
        "XYUI-4-4.04" => [T("Ring", "XY.Brush.Border.Color.Focus"), T("Width", "XY.Border.Width.Focus"), T("Offset", "XYUI4-GAP-002")],
        "XYUI-4-4.05" => [T("Primary", "XY.Editor.Selection"), T("Secondary", "XY.Editor.MultiSelection"), T("LayoutShift", "XY.State.ResizeOnChange")],
        "XYUI-4-4.06" => [T("Container", "XY.Editor.Selection"), T("Context", "XY.Editor.MultiSelection"), T("Transform", "4.10 / 4.11")],
        "XYUI-4-4.07" => [T("Border", "XY.Editor.Selection"), T("Crossing", "XY.Editor.MultiSelection"), T("Fill opacity", "XYUI4-GAP-003")],
        "XYUI-4-4.08" => [T("Stroke", "XY.Editor.Selection"), T("Candidate", "XY.Editor.MultiSelection"), T("Fill opacity", "XYUI4-GAP-003")],
        "XYUI-4-4.09" => [T("Accent", "XY.Editor.Selection"), T("Separation", "XYUI4-GAP-001"), T("Glow", "XY.Shadow.None")],
        "XYUI-4-4.10" => [T("Border", "XY.Editor.BoundingBox"), T("Handle", "XY.Editor.Handle"), T("Pivot Border", "XY.Editor.Selection"), T("LayoutShift", "XY.State.ResizeOnChange")],
        _ => []
    };

    public static IReadOnlyList<XYUIDocRule>? Rules(string id) => id switch
    {
        "XYUI-4-4.02" => [new("持久身份", "Selected 在指针离开后继续保持。"), new("状态区分", "Selected 不等同 Active、Focus 或 Checked。")],
        "XYUI-4-4.03" => [new("生命周期", "Persistent Active 必须由工具或模式状态驱动。"), new("瞬时反馈", "Pointer Down / Press 使用 Foundation Pressed，不另造残留状态。")],
        "XYUI-4-4.04" => [new("独立通道", "Focus Ring 不替代 Selected Surface。"), new("键盘可见", "键盘导航时 Focus 必须独立可见。")],
        "XYUI-4-4.05" => [new("主次明确", "Primary 必须强于 Secondary。"), new("集合稳定", "Hover 或 Focus 不得清空 Selection Set。")],
        "XYUI-4-4.06" => [new("组与多选区分", "SelectionGroup 不等同临时 MultiSelection。"), new("整体反馈", "Group Bounds 只在整体操作时出现。")],
        "XYUI-4-4.07" => [new("候选与结果分离", "框选释放后才交给 Selection Set。"), new("布局稳定", "拖动框不得推动周围布局。"), new("模式区分", "Crossing 使用 MultiSelection 语义并保持虚线。")],
        "XYUI-4-4.08" => [new("路径稳定", "低平滑度闭合路径，不引入高频重采样。"), new("候选与结果分离", "套索只呈现候选，不直接修改业务选择。")],
        "XYUI-4-4.09" => [new("结果反馈", "轮廓只呈现已提交选择，不负责命中或变换。"), new("边界分离", "不得用 Glow 或布局变化替代分离轮廓。")],
        "XYUI-4-4.10" => [new("工具上下文", "普通 Selected 不常驻 BoundingBox，只有 Transform 模式显示。"), new("几何分离", "BoundingBox 不改变对象真实 Geometry。"), new("手柄命中", "视觉手柄与实际 SemanticExpanded 命中区域分离。")],
        _ => null
    };

    static XYUIDocProperty P(string n, string t, string d) => new(n, t, d, "组件状态契约");
    static XYUIDocToken T(string n, string v) => new(n, v, "来自 XYUI Foundation 的语义 Token");
}
