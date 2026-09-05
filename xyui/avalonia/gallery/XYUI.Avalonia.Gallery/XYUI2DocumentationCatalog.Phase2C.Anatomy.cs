namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2DocumentationCatalog
{
    static IReadOnlyList<XYUIDocVariant> Phase2CVariants(string id) => id switch
    {
        "XYUI-2-13" => [
            new("Closed", "收起常态，展示选中项文本或占位符", "静态固定候选展示"),
            new("Open", "展开状态，弹出固定候选项列表", "用户选择操作中"),
            new("Disabled", "禁用态，半透明且阻断点击", "不可选状态")
        ],
        "XYUI-2-14" => [
            new("Standard", "标准多行文本输入，支持自动增长", "常规备注与描述"),
            new("Editor", "代码/文档编辑器模式，显露顶部元数据条", "结构化脚本与配置"),
            new("ReadOnly", "只读模式，支持划选复制，禁止键入", "展示输出与诊断日志")
        ],
        "XYUI-2-15" => [
            new("Default", "空闲常态，展示放大镜与占位文本", "等待输入关键词"),
            new("WithContent", "有内容态，右侧显露快速清除按钮", "键入内容后快速重置"),
            new("FilterActive", "筛选已激活，右侧筛选图标高亮指示", "复合筛选条件生效中")
        ],
        "XYUI-2-16" => [
            new("Masked", "默认密文遮罩态，以圆点掩码呈现", "常规密码安全展示"),
            new("Revealed", "按住临时明文态，显露输入原始内容", "按住眼睛临时核对密码"),
            new("Disabled", "禁用态，阻断输入与明文查看", "权限不足或只读")
        ],
        "XYUI-2-17" => [
            new("SegmentActive", "分段焦点态，高亮当前编辑的年月日段", "键盘微调与精准数字键入"),
            new("CalendarOpen", "日历面板展开态，以月度网格呈现日期", "可视化日期选择"),
            new("DatePopupOpen", "增减调节面板展开态，提供分段步进", "快速加减日期分段")
        ],
        "XYUI-2-18" => [
            new("Standard", "时分秒三段展示，带完整秒微调", "精细时间输入"),
            new("Compact", "时分两段展示，隐藏秒分段", "日常任务与日程时间"),
            new("Scrubbing", "按住左右拖拽微调态，光标捕获连续步进", "无极连续时间调节")
        ],
        _ => []
    };

    static IReadOnlyList<XYUIDocProperty> Phase2CProperties(string id) => id switch
    {
        "XYUI-2-13" => [
            new("ItemsSource", "IEnumerable", "[]", "固定候选项源集合，不可键入新增"),
            new("SelectedItem", "object?", "null", "当前选中的对象项目"),
            new("SelectedIndex", "int", "-1", "当前选中项索引"),
            new("Placeholder", "string?", "null", "未选中任何项时的占位水印"),
            new("IsDropDownOpen", "bool", "false", "候选列表弹出层展开状态")
        ],
        "XYUI-2-14" => [
            new("Mode", "XYTextAreaMode", "Standard", "文本区域模式（Standard 普通 / Editor 编辑器）"),
            new("Placeholder", "string?", "null", "无文本时的水印提示"),
            new("AutoGrow", "bool", "true", "是否随内容行数自适应高度"),
            new("EditorType", "string", "文本", "Editor 模式下顶部栏左侧显示的类型标签"),
            new("LineCount / CharacterCount", "int", "1 / 0", "只读行数与字符数统计")
        ],
        "XYUI-2-15" => [
            new("Text", "string", "", "当前键入的搜索文本，支持回车发起搜索"),
            new("Placeholder", "string?", "null", "搜索占位文本提示"),
            new("FilterContent", "Control?", "null", "自定义筛选弹出面板内容"),
            new("FilterActive", "bool", "false", "是否有激活的高级筛选条件"),
            new("IsFilterOpen", "bool", "false", "高级筛选面板弹出层显隐状态")
        ],
        "XYUI-2-16" => [
            new("Password", "string", "", "密码字符串，包装原生 Text 属性"),
            new("Placeholder", "string?", "null", "密码框水印提示"),
            new("IsRevealed", "bool", "false", "当前是否处于按住显露明文状态")
        ],
        "XYUI-2-17" => [
            new("SelectedDate", "DateOnly", "2026-08-12", "当前选中的公历日期真值"),
            new("MinDate / MaxDate", "DateOnly?", "null", "日期上下限约束，越界自动 Clamp"),
            new("ActiveSegment", "XYDateSegment", "Day", "当前聚焦的日期分段（Year / Month / Day）")
        ],
        "XYUI-2-18" => [
            new("Time", "TimeOnly", "14:30:25", "当前时间真值"),
            new("ShowSeconds", "bool", "true", "是否展示与编辑秒分段"),
            new("ActiveSegment", "XYTimeSegment", "Minute", "当前聚焦的时间分段（Hour / Minute / Second）")
        ],
        _ => []
    };
}
