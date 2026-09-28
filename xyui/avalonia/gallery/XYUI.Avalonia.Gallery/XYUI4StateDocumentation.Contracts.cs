namespace XYUI.Avalonia.Gallery;

static partial class XYUI4StateDocumentation
{
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
