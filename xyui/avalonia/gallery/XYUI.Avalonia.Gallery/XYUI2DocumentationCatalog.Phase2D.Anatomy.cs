namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2DocumentationCatalog
{
    static IReadOnlyList<XYUIDocVariant> Phase2DVariants(string id) => id switch
    {
        "XYUI-2-19" => [
            new("Default", "常态，展示色块、HEX/Alpha 文本与下拉箭头", "常规颜色属性展示"),
            new("Open", "展开态，弹出色域网格、色相/透明度滑块与精确数值框", "颜色编辑中"),
            new("Disabled", "禁用态，半透明且阻断点击与面板弹出", "不可编辑状态")
        ],
        "XYUI-2-20" => [
            new("Unchecked", "未激活常态，展示关闭状态开关", "布尔值为 false"),
            new("Checked", "激活状态，展示开启状态开关", "布尔值为 true"),
            new("ReadOnly", "只读模式，展示当前布尔值并阻断切换", "固定配置展示")
        ],
        "XYUI-2-21" => [
            new("Default", "常态数值显示，右侧展示微调步进器", "常规数值编辑"),
            new("Scrubbing", "标签拖动微调中，光标捕获连续步进", "连续无极微调"),
            new("ReadOnly", "只读模式，保留数值与后缀，阻断修改", "参数只读呈现")
        ],
        "XYUI-2-22" => [
            new("Wide", "宽屏单行横排，所有轴均匀横向平铺", "宽容器属性面板"),
            new("Medium", "中屏布局，标签置顶，轴保持横向平铺", "中等宽度面板"),
            new("Compact", "紧凑布局，标签置顶，轴竖向逐行堆叠", "窄侧边栏与小窗口")
        ],
        "XYUI-2-23" => [
            new("Closed", "收起常态，展示当前选中项与下拉箭头", "离散枚举展示"),
            new("Open", "展开状态，弹出枚举候选项列表", "选择操作进行中"),
            new("Disabled", "禁用态，半透明且阻断选择交互", "只读或不可选状态")
        ],
        "XYUI-2-24" => [
            new("Empty", "空引用状态，展示未选择占位与添加指引", "未设置目标资产"),
            new("Resolved", "已解析有效引用，展示名称与类型标识", "引用正常生效"),
            new("Missing", "引用丢失状态，显露警告提示资产缺失", "引用的资产已被删除"),
            new("TypeMismatch", "类型不匹配状态，显露兼容性警告", "拖入不匹配类型")
        ],
        _ => []
    };

    static IReadOnlyList<XYUIDocProperty> Phase2DProperties(string id) => id switch
    {
        "XYUI-2-19" => [
            new("Color", "Color", "#326F8A", "当前颜色真值（Avalonia.Media.Color）"),
            new("Mode", "XYColorPickerMode", "RGBA", "RGB 或 RGBA 显示模式"),
            new("IsOpen", "bool", "false", "拾色器弹出面板展开状态")
        ],
        "XYUI-2-20" => [
            new("Label", "string", "属性", "属性标签显示文本"),
            new("Value", "bool", "false", "布尔开关真值"),
            new("IsReadOnly", "bool", "false", "是否只读阻断修改"),
            new("LabelColumnWidth", "double", "160", "标签列固定基准宽度")
        ],
        "XYUI-2-21" => [
            new("Label", "string", "属性", "属性名称与拖拽微调入口"),
            new("Value", "double", "0", "与内部 XYNumberField 共用唯一真值"),
            new("Minimum / Maximum", "double", "0 / 100", "数值上下限约束"),
            new("Step", "double", "1", "键盘上下键与步进器步长"),
            new("Suffix", "string?", "null", "单位后缀（如 kg, m/s）")
        ],
        "XYUI-2-22" => [
            new("Label", "string", "向量", "属性标签文本"),
            new("Dimension", "XYVectorDimension", "Vector3", "向量维度（Vector2 / 3 / 4）"),
            new("X / Y / Z / W", "double", "0", "各轴独立分量真值"),
            new("Minimum / Maximum", "double", "-100000 / 100000", "各轴数值约束")
        ],
        "XYUI-2-23" => [
            new("Label", "string", "枚举", "属性标签文本"),
            new("ItemsSource", "IEnumerable", "[]", "枚举候选集合数据源"),
            new("SelectedItem", "object?", "null", "当前选中的枚举对象"),
            new("SelectedIndex", "int", "-1", "当前选中项索引")
        ],
        "XYUI-2-24" => [
            new("Label", "string", "引用", "属性标签文本"),
            new("Reference", "XYReferenceValue?", "null", "包含 Name / Type / Id 的引用对象"),
            new("ExpectedType", "string?", "null", "期望约束的资产类型"),
            new("ReferenceState", "XYReferenceState", "Empty", "当前引用状态机状态"),
            new("ReferencePickerContent", "Control?", "null", "自定义选择器面板内容")
        ],
        _ => []
    };
}
