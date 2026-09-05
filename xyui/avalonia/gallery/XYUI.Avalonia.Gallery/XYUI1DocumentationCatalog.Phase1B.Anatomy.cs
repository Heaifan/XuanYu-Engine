namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1DocumentationCatalog
{
    static IReadOnlyList<XYUIDocVariant> Phase1BAnatomy(string id) => id switch
    {
        "XYUI-1-07" => [
            new("Content", "短技术文本与标识符", "Inline identifier / path"),
            new("Typography", "等宽字体排版规格", "xy:XY.Font.Mono / XY.Type.Code"),
            new("Foreground", "次级文本色弱化视觉干扰", "XY.Brush.Text.Tertiary"),
            new("Surface", "浅色技术底色容器", "XY.Brush.Surface.PanelAlt"),
            new("Code Mark", "矢量代码角标 (VERIFY-BASELINE)", "8 DIP Vector Code Mark"),
            new("Variants", "无额外变体，专用于技术标识", "None")
        ],
        "XYUI-1-08" => [
            new("Structure", "对齐等宽数据容器 (Aligned Monospace Data)", "Grid-based data container"),
            new("Columns", "共享列宽：Label(Auto) | Value(Auto) | Unit(Auto)", "Canonical shared columns"),
            new("Value Column", "右对齐数值列保持等宽基准", "XY.Font.Mono / Right Aligned"),
            new("Unit Column", "紧凑单位标注列", "UI Semibold / Left Aligned"),
            new("Variants", "无额外变体，专用于结构化数值流", "None")
        ],
        "XYUI-1-09" => [
            new("Geometry", "固定 22 DIP 高度规范骨架", "22 DIP Canonical Geometry"),
            new("Left Pointer", "左指针尖角 (VERIFY-CANONICAL)", "11 DIP Pointer Width"),
            new("Default", "默认次级面板底色标签", "Variant=\"Default\""),
            new("Accent", "品牌强调色标签", "Variant=\"Accent\"")
        ],
        "XYUI-1-10" => [
            new("Structure", "Badge 骨架 + 语义状态指示复合单元", "Badge anatomy + Status signal"),
            new("Semantic States", "5 项业务状态 (Success/Warning/Error/Info/Neutral)", "XyuiStatusState enum"),
            new("Control State", "独立控件状态：禁用 (IsEnabled=False)", "Not an enum state"),
            new("Tokens", "共享状态色彩 Token", "XY.Brush.State.*")
        ],
        "XYUI-1-11" => [
            new("Role", "极紧凑状态信号圆点 (无文本)", "Compact status signal"),
            new("Shape", "8 DIP 圆形边界与内填充", "Circle dot geometry"),
            new("Semantic States", "5 项业务状态 (Success/Warning/Error/Info/Neutral)", "XyuiStatusState enum"),
            new("Tokens", "与 StatusBadge 共享色源", "XyuiStatusStateTokens")
        ],
        "XYUI-1-12" => [
            new("Viewport", "24 × 24 统一逻辑视口与均匀缩放", "Logical 24x24 / Uniform"),
            new("Centering", "严格几何居中包围盒", "Centering alignment"),
            new("Size Precedence", "显式 Size 优先级高于继承 xy:XY.Size", "Explicit > Inherited"),
            new("Stroke Scaling", "线宽随尺寸变体联动 (1.25~2.00 DIP)", "Stroke scaling")
        ],
        "XYUI-1-13" => [
            new("Composition", "真实复合 XYIcon 与 TextPresenter", "XYIcon + Text composition"),
            new("Alignment", "图标与文字严格垂直居中", "VerticalAlignment=Center"),
            new("Gap", "标准水平间距 Space1 (4 DIP)", "XyuiSpatialTokens.Space1"),
            new("Hierarchy", "文字主色 (Primary) 与图标次级色 (Secondary)", "Foreground hierarchy")
        ],
        _ => []
    };
}
