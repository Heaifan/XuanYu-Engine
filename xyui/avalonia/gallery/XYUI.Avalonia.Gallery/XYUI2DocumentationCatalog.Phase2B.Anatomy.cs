namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2DocumentationCatalog
{
    static IReadOnlyList<XYUIDocVariant> Phase2BVariants(string id) => id switch
    {
        "XYUI-2-07" => [
            new("Unchecked", "未选中态，空心圆环外框", "同组其他项激活时"),
            new("Checked", "选中态，内部呈现 6 DIP 实心强色圆点", "当前唯一选中的选项"),
            new("Disabled", "禁用态，圆环与文字衰减，阻断点击", "选项不可用")
        ],
        "XYUI-2-08" => [
            new("Off", "关闭态，灰色轨道与左置圆形滑块", "功能处于未激活状态"),
            new("On", "开启态，强色轨道与右置圆形滑块", "功能处于实时激活状态"),
            new("Disabled", "禁用态，轨道与滑块弱化衰减", "配置锁定不可切换")
        ],
        "XYUI-2-09" => [
            new("Default", "默认常态，呈现微弱边框与占位提示", "等待用户点击输入"),
            new("Focus", "聚焦编辑态，激活底部 3 DIP 焦点指示线", "文本处于编辑状态"),
            new("ReadOnly", "只读态，允许划选与拷贝文本，不可编辑", "展示只读信息"),
            new("Error", "错误态，红边警示边框与背景微调", "格式或内容校验未通过")
        ],
        "XYUI-2-10" => [
            new("Default", "常规数值展示态，格式化小数位并追加单位后缀", "参数检查与展示"),
            new("Scrubbing", "按住拖拽微调态，鼠标指针捕获并实时更新数值", "交互式无极微调"),
            new("StepperHover", "悬停态，右侧显露步进微调三角箭头", "单步微调入口")
        ],
        "XYUI-2-11" => [
            new("Standard", "带右侧 104 DIP 紧凑输入框的标准滑块组合", "常规属性面板"),
            new("TrackOnly", "隐藏数值输入框，仅保留滑动轨道", "紧凑工具条或音量条")
        ],
        "XYUI-2-12" => [
            new("Closed", "收起常态，展示当前选中项或占位提示", "静态展示选择结果"),
            new("Open", "展开下拉态，Popup 呈现真实列表与滚动条", "正在浏览并选择候选项"),
            new("Filtering", "输入过滤态，输入关键字即时模糊筛选候选集", "快速定位目标项")
        ],
        _ => []
    };

    static IReadOnlyList<XYUIDocProperty> Phase2BProperties(string id) => id switch
    {
        "XYUI-2-07" => [
            new("GroupName", "string", "null", "同组互斥标识，相同名称的单选钮严格互斥"),
            new("IsChecked", "bool?", "false", "当前选中状态，true 为选中"),
            new("Content", "object?", "null", "单选标签文本或自定义内容")
        ],
        "XYUI-2-08" => [
            new("IsChecked", "bool?", "false", "开关开闭状态，true 为 On，false 为 Off"),
            new("Content", "object?", "null", "右侧描述标签，点击标签整行可直接切换开关")
        ],
        "XYUI-2-09" => [
            new("Text", "string", "", "单行文本内容，首次获得编辑焦点自动全选"),
            new("Placeholder", "string?", "null", "文本为空时的占位水印提示"),
            new("IsReadOnly", "bool", "false", "只读模式，允许聚焦划选复制但不可编辑"),
            new("IsError", "bool", "false", "错误警示状态，为 true 时激活 :error 伪类")
        ],
        "XYUI-2-10" => [
            new("Value", "double", "0", "数值真值，严格被 Clamp 在 Minimum 与 Maximum 之间"),
            new("Minimum / Maximum", "double", "0 / 100", "数值上下限约束区间"),
            new("Step", "double", "1", "普通微调步长与精度步长"),
            new("LargeStep / SmallStep", "double", "10 / 0.1", "Shift 加速步长与 Ctrl 精细微调步长"),
            new("DecimalPlaces", "int", "2", "显示格式化小数位数"),
            new("Suffix", "string?", "null", "数值尾部单位显示后缀（如 px, %, mm）"),
            new("IsScrubEnabled", "bool", "true", "是否启用按住数值鼠标拖拽微调协议")
        ],
        "XYUI-2-11" => [
            new("Value", "double", "0", "滑块与右侧 NumberField 唯一共享真值"),
            new("Minimum / Maximum", "double", "0 / 100", "连续区间范围边界"),
            new("Step", "double", "1", "微调离散步长"),
            new("IsNumberFieldVisible", "bool", "true", "是否在右侧展示 104 DIP 紧凑精确数值框")
        ],
        "XYUI-2-12" => [
            new("ItemsSource", "IEnumerable", "[]", "下拉候选项源集合，过滤时不被篡改"),
            new("SelectedItem", "object?", "null", "当前选中对象项"),
            new("Text", "string", "", "编辑文本框当前内容，键入触发即时动态过滤"),
            new("Placeholder", "string?", "null", "未选择且文本为空时的占位提示"),
            new("IsDropDownOpen", "bool", "false", "下拉 Popup 弹出层展开状态"),
            new("IsCustomValueAllowed", "bool", "false", "是否允许键入不在列表内的自定义值")
        ],
        _ => []
    };
}
