namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2DocumentationCatalog
{
    static IReadOnlyList<XYUIDocFoundationItem> Phase2DFoundationMappings(string id) => id switch
    {
        "XYUI-2-19" => [
            new("Swatch Chrome", "XY.Brush.Surface.Input", "颜色输入框表面与圆角"),
            new("Checkerboard", "XY.Brush.Surface.CheckerLight", "棋盘格半透明底纹指示"),
            new("Panel Chrome", "XY.Brush.Surface.Raised", "颜色调节浮层面板底色与阴影"),
            new("Focus Edge", "XY.Brush.Accent.Strong", "聚焦时底部 3 DIP 高亮指示线")
        ],
        "XYUI-2-20" => [
            new("Label Text", "XY.Brush.Text.Primary", "属性标签文本前景色"),
            new("Switch Track", "XY.Brush.Surface.Panel", "开关轨道底色"),
            new("Switch Active", "XY.Brush.Accent.Strong", "开关激活开启态填充色"),
            new("Row Border", "XY.Brush.Border.Subtle", "属性行分割基线")
        ],
        "XYUI-2-21" => [
            new("Label Text", "XY.Brush.Text.Primary", "属性标签与微调手柄前景色"),
            new("Scrub Active", "XY.Brush.Accent.Strong", "标签拖拽微调中高亮指示"),
            new("Input Chrome", "XY.Brush.Surface.Input", "数值输入框底色"),
            new("Suffix Text", "XY.Brush.Text.Secondary", "单位后缀弱化显示颜色")
        ],
        "XYUI-2-22" => [
            new("Label Text", "XY.Brush.Text.Primary", "向量属性标签前景色"),
            new("Axis Host", "XY.Brush.Surface.Input", "各轴数值输入框底色"),
            new("Axis Label", "XY.Brush.Text.Secondary", "各轴分量前缀标签颜色"),
            new("Focus Edge", "XY.Brush.Accent.Strong", "当前轴聚焦时底部指示线")
        ],
        "XYUI-2-23" => [
            new("Label Text", "XY.Brush.Text.Primary", "枚举属性标签前景色"),
            new("Select Chrome", "XY.Brush.Surface.Input", "下拉选择框表面底色"),
            new("Chevron Icon", "XY.Brush.Text.Secondary", "下拉指示箭头前景色"),
            new("Popup Surface", "XY.Brush.Surface.Raised", "枚举离散候选列表浮层底色")
        ],
        "XYUI-2-24" => [
            new("Reference Chrome", "XY.Brush.Surface.Input", "引用展示框底色与圆角"),
            new("Type Badge", "XY.Brush.Text.Secondary", "引用对象类型标识颜色"),
            new("Missing Alert", "XY.Brush.Semantic.Danger", "引用丢失状态红边警告"),
            new("Mismatch Alert", "XY.Brush.Semantic.Warning", "类型不匹配黄色警告")
        ],
        _ => []
    };

    static IReadOnlyList<XYUIDocState> Phase2DStates(string id) => id switch
    {
        "XYUI-2-19" => [
            new("Default", "常态收起显示"),
            new("Open (.xyui-color-open)", "拾色面板展开态"),
            new(":disabled", "禁用阻断交互态")
        ],
        "XYUI-2-20" => [
            new("Unchecked", "未激活常态"),
            new("Checked", "激活开启态"),
            new("ReadOnly", "只读阻断修改态"),
            new(":disabled", "禁用阻断交互态")
        ],
        "XYUI-2-21" => [
            new("Idle", "常规数值展示态"),
            new("Scrubbing (.xyui-number-property-scrubbing)", "标签拖拽连续微调中"),
            new("ReadOnly", "只读阻断修改态"),
            new(":disabled", "禁用阻断交互态")
        ],
        "XYUI-2-22" => [
            new("Wide (.xyui-vector-wide)", "宽屏横向平铺排列"),
            new("Medium (.xyui-vector-medium)", "中屏标签折行排列"),
            new("Compact (.xyui-vector-compact)", "紧凑纵向堆叠排列"),
            new(":disabled", "禁用阻断交互态")
        ],
        "XYUI-2-23" => [
            new("Closed", "常态收起显示"),
            new("Open (.xyui-select-open)", "下拉列表展开态"),
            new("ReadOnly", "只读阻断修改态"),
            new(":disabled", "禁用阻断交互态")
        ],
        "XYUI-2-24" => [
            new("Empty", "未设置引用"),
            new("Resolved", "引用解析正常生效"),
            new("Missing (.xyui-reference-missing)", "资产丢失警告"),
            new("Mismatch (.xyui-reference-mismatch)", "类型不匹配警告"),
            new(":disabled", "禁用阻断交互态")
        ],
        _ => []
    };
}
