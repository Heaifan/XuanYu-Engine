namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2DocumentationCatalog
{
    static IReadOnlyList<XYUIDocProperty> Properties(string id) => id switch
    {
        "XYUI-2-06" => [new("IsChecked", "bool?", "false", "支持 Unchecked / Checked / Mixed"), new("IsThreeState", "bool", "false", "启用 Mixed 状态")],
        "XYUI-2-07" => [new("GroupName", "string", "", "同组互斥"), new("IsChecked", "bool", "false", "当前选项")],
        "XYUI-2-08" => [new("IsChecked", "bool", "false", "真实切换 Track / Thumb")],
        "XYUI-2-09" => [new("Text", "string", "", "单行文本"), new("Placeholder", "string?", "null", "占位提示"), new("IsReadOnly", "bool", "false", "只读")],
        "XYUI-2-10" => [new("Value", "double", "0", "统一数值真值"), new("Minimum", "double", "0", "下限"), new("Maximum", "double", "100", "上限"), new("Step", "double", "1", "普通步长"), new("LargeStep", "double", "10", "Shift 步长"), new("SmallStep", "double", "0.1", "Ctrl 步长"), new("Suffix", "string?", "null", "仅显示后缀"), new("DecimalPlaces", "int", "2", "显示小数位")],
        "XYUI-2-11" => [new("Value", "double", "0", "Slider 与 NumberField 的唯一真值"), new("Minimum", "double", "0", "下限"), new("Maximum", "double", "100", "上限"), new("Step", "double", "1", "普通步长"), new("LargeStep", "double", "10", "Shift 步长"), new("SmallStep", "double", "0.1", "Ctrl 步长"), new("DecimalPlaces", "int", "2", "显示小数位"), new("Suffix", "string?", "null", "仅显示后缀"), new("IsNumberFieldVisible", "bool", "true", "显示精确输入")],
        "XYUI-2-12" => [new("ItemsSource", "IEnumerable", "[]", "可编辑候选"), new("SelectedItem", "object?", "null", "当前候选"), new("IsCustomValueAllowed", "bool", "false", "允许自定义值")],
        "XYUI-2-13" => [new("ItemsSource", "IEnumerable", "[]", "固定候选"), new("SelectedIndex", "int", "-1", "当前候选索引"), new("SelectedItem", "object?", "null", "当前候选"), new("Placeholder", "string?", "null", "未选择时的提示")],
        "XYUI-2-14" => [new("Text", "string", "", "多行文本"), new("Placeholder", "string?", "null", "占位提示"), new("Mode", "XYTextAreaMode", "Standard", "标准 / 编辑模式"), new("AutoGrow", "bool", "true", "内容驱动增长"), new("MinHeight", "double", "54", "最小高度"), new("MaxHeight", "double", "Auto", "达到后内部滚动"), new("EditorType", "string", "文本", "编辑标题栏类型"), new("IsError", "bool", "false", "错误边框状态")],
        "XYUI-2-15" => [new("Text", "string", "", "搜索文本"), new("Placeholder", "string?", "null", "占位提示"), new("FilterContent", "Control?", "null", "真实筛选面板内容"), new("IsFilterOpen", "bool", "false", "筛选面板是否打开"), new("FilterActive", "bool", "false", "独立的筛选激活态"), new("IsSearching", "bool", "false", "搜索进行中"), new("IsNoResult", "bool", "false", "无结果态")],
        "XYUI-2-16" => [new("Password", "string", "", "密码文本别名"), new("Placeholder", "string?", "null", "占位提示"), new("IsRevealed", "bool", "false", "按住时临时显示")],
        "XYUI-2-17" => [new("SelectedDate", "DateOnly", "2026-08-12", "当前日期"), new("MinDate", "DateOnly?", "null", "可选最小日期"), new("MaxDate", "DateOnly?", "null", "可选最大日期"), new("DateChanged", "event", "—", "日期变化事件")],
        "XYUI-2-18" => [new("Time", "TimeOnly", "14:30:25", "当前时间"), new("ShowSeconds", "bool", "true", "显示秒分段"), new("TimeChanged", "event", "—", "时间变化事件")],
        "XYUI-2-19" => [new("Color", "Color", "#326F8A", "颜色真值"), new("Mode", "XYColorPickerMode", "RGBA", "RGB / RGBA 显示模式"), new("IsOpen", "bool", "false", "颜色面板是否打开"), new("ColorChanged", "event", "—", "颜色变化事件")],
        "XYUI-2-20" => [new("Label", "string", "属性", "属性名称"), new("Value", "bool", "false", "布尔真值"), new("IsReadOnly", "bool", "false", "只读状态"), new("ValueChanged", "event", "—", "值变化事件")],
        "XYUI-2-21" => [new("Label", "string", "属性", "属性名称与微调入口"), new("Value", "double", "0", "与 XYNumberField 共用真值"), new("Minimum", "double", "0", "下限"), new("Maximum", "double", "100", "上限"), new("Step", "double", "1", "统一步长"), new("Suffix", "string?", "null", "单位后缀"), new("IsReadOnly", "bool", "false", "只读状态")],
        "XYUI-2-22" => [new("Label", "string", "向量", "属性名称"), new("Dimension", "XYVectorDimension", "Vector3", "Vector2 / Vector3 / Vector4"), new("X/Y/Z/W", "double", "0", "各轴共用 XYNumberField"), new("IsReadOnly", "bool", "false", "只读状态")],
        "XYUI-2-23" => [new("Label", "string", "枚举", "属性名称"), new("ItemsSource", "IEnumerable", "[]", "固定候选"), new("SelectedItem", "object?", "null", "当前候选"), new("SelectedIndex", "int", "-1", "当前索引"), new("IsReadOnly", "bool", "false", "只读状态")],
        "XYUI-2-24" => [new("Label", "string", "引用", "属性名称"), new("Reference", "XYReferenceValue?", "null", "Name / Type / ID"), new("ReferenceState", "XYReferenceState", "Empty", "Empty / Resolved / Missing / TypeMismatch"), new("ExpectedType", "string?", "null", "兼容类型"), new("ReferencePickerContent", "Control?", "null", "真实引用选择器内容")],
        _ => []
    };
}
