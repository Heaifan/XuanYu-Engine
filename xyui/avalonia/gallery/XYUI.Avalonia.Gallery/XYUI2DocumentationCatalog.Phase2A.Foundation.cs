namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2DocumentationCatalog
{
    static IReadOnlyList<XYUIDocFoundationItem> Phase2AFoundationMappings(string id) => id switch
    {
        "XYUI-2-01" => [
            new("Action Edge", "XY.Accent.Strong", "Primary 底部 3 DIP 高亮强调条"),
            new("Weakened Edge", "XY.Divider.Default", "Secondary 底部弱化边框线"),
            new("Danger Chrome", "XY.Semantic.Error.*", "Danger 变体边框与警示背景色"),
            new("Focus Visual", "XY.Brush.Border.Color.Focus", "键盘导航焦点外框")
        ],
        "XYUI-2-02" => [
            new("Ghost Rest", "Colors.Transparent", "静态常态下不绘制背景，与工具栏融合"),
            new("Hover Reveal", "XY.Brush.State.Color.Hover", "指针悬停显露容器轮廓底色"),
            new("Selected Edge", "XY.Accent.Strong", "外部选中时激活底部 Action Edge"),
            new("Icon Metric", "XY.Size.Icon.Medium", "标准 16 DIP 居中矢量图标")
        ],
        "XYUI-2-03" => [
            new("Rest Surface", "XY.Surface.Raised", "未选中态默认凸起面板底色"),
            new("Active Surface", "XY.Brush.Surface.Selected", "激活态选中底色"),
            new("Persistent Edge", "XY.Accent.Strong", "激活态常驻 3 DIP 高亮强调条"),
            new("Disabled State", "XY.State.Disabled.*", "禁用态全色系衰减")
        ],
        "XYUI-2-04" => [
            new("Shared Chrome", "XY.Surface.Raised", "主区与菜单槽共享一层平整底色"),
            new("Zone Divider", "XY.Divider.Default", "中间 18 DIP 高度垂直分割线"),
            new("Hover State", "XY.Brush.State.Color.Hover", "各命中区独立响应悬停底色"),
            new("Focus Ring", "XY.Brush.Border.Color.Focus", "外层统一焦点框，两区不争抢焦点")
        ],
        "XYUI-2-05" => [
            new("Unified Zone", "PART_OpenZone", "横跨全钮的单一命中区，无内部隔断"),
            new("Chevron Icon", "XY.Brush.Text.Secondary", "右侧装饰 Chevron 图标基线色"),
            new("Chevron Off", "XY.Brush.State.Disabled.Text", "禁用态装饰槽与图标衰减色"),
            new("Focus Ring", "XY.Brush.Border.Color.Focus", "统一全钮键盘焦点框")
        ],
        "XYUI-2-06" => [
            new("Box Size", "XY.Size.Checkbox", "标准 14 DIP 方形复选框尺寸"),
            new("Box Corner", "XY.Checkbox.Radius", "标准 2 DIP 复选框圆角"),
            new("Check Glyph", "XY.Brush.Accent.Strong", "14 DIP 矢量 Check 勾选描边与 Indeterminate 横杠"),
            new("Input Surface", "XY.Brush.Surface.Input", "未选中常态输入底色")
        ],
        _ => []
    };

    static IReadOnlyList<XYUIDocState> Phase2AStates(string id) => id switch
    {
        "XYUI-2-01" => [
            new("Normal", "默认静止状态，展现对应变体底色与 Action Edge"),
            new(":pointerover", "指针悬停，Primary Edge 由 3 DIP 增厚至 4 DIP"),
            new(":pressed", "按压瞬态，底色加深反馈"),
            new(":disabled", "禁用态，背景透明度衰减，阻断点击")
        ],
        "XYUI-2-02" => [
            new("Ghost", "透明基线状态，无背景与边框"),
            new(":pointerover", "悬停显露浅灰交互底色"),
            new(":selected", "外部状态驱动选中，显露 Surface.Selected 与 Action Edge"),
            new(":disabled", "禁用态，图标透明度衰减至 38%")
        ],
        "XYUI-2-03" => [
            new("Unchecked", "未开启状态，无高亮 Edge"),
            new(":checked", "开启状态，持久显露 Persistent Edge 与高亮边框"),
            new(":disabled", "禁用态，状态冻结不可点击")
        ],
        "XYUI-2-04" => [
            new("Main Hover", "主命中区悬停，仅主区底色变化"),
            new("Menu Hover", "菜单槽悬停，仅右槽底色变化"),
            new(":disabled", "双区同步禁用，Divider 衰减")
        ],
        "XYUI-2-05" => [
            new("Normal", "默认常态，整钮平整"),
            new(":pointerover", "指针悬停，全钮与装饰槽同步变色"),
            new("Disabled", "禁用态，整钮与 Chevron 同步衰减")
        ],
        "XYUI-2-06" => [
            new("Unchecked", "未勾选态，空白方框"),
            new(":checked", "勾选态，呈现强色 Check 矢量"),
            new(":indeterminate", "混合态，呈现强色水平短横杠"),
            new(":disabled", "禁用态，方框与勾选标记同步弱化")
        ],
        _ => []
    };
}
