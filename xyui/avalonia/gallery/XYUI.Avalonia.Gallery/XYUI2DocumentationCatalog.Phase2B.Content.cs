namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2DocumentationCatalog
{
    static string Phase2BQuickStart(string id) => id switch
    {
        "XYUI-2-07" => "<xy:XYRadioButton GroupName=\"render\" Content=\"实时预览\" IsChecked=\"True\" />\n<xy:XYRadioButton GroupName=\"render\" Content=\"最终质量\" />",
        "XYUI-2-08" => "<xy:XYSwitch Content=\"自动保存工程\" IsChecked=\"{Binding AutoSave}\" />",
        "XYUI-2-09" => "<xy:XYTextField Text=\"{Binding ProjectName}\" Placeholder=\"请输入项目名称\" />",
        "XYUI-2-10" => "<xy:XYNumberField Value=\"{Binding Scale}\" Minimum=\"0.1\" Maximum=\"10.0\" Step=\"0.1\" Suffix=\"x\" />",
        "XYUI-2-11" => "<xy:XYSlider Value=\"{Binding Volume}\" Minimum=\"0\" Maximum=\"100\" Suffix=\"%\" />",
        "XYUI-2-12" => "<xy:XYComboBox ItemsSource=\"{Binding RenderEngines}\" SelectedItem=\"{Binding CurrentEngine}\" Placeholder=\"选择渲染后端\" />",
        _ => ""
    };

    static IReadOnlyList<XYUIDocRule> Phase2BCoreRules(string id) => id switch
    {
        "XYUI-2-07" => [
            new("组件定义", "单选按钮，用于同组互斥选项的选择，同组内有且仅有一个可处于激活态。"),
            new("互斥机制", "严格依托 Avalonia 原生 GroupName 属性实现组内互斥，Label 与圆点属于统一可点击区域。"),
            new("相邻区别", "vs XYCheckbox：RadioButton 为互斥单选；Checkbox 为并存多选或树形汇总。"),
            new("禁用场景", "同组选项少于 2 个时禁止使用；二元开关应使用 XYSwitch。")
        ],
        "XYUI-2-08" => [
            new("组件定义", "二元立即生效开关控件，点击立即切换系统或模块状态，无暂存状态。"),
            new("语义边界", "Switch = 立即生效设置；Checkbox = 表单多选/待提交；ToggleButton = 视图/工具模式保持。"),
            new("交互范围", "点击整行（含文字说明与滑块 Track）均可触发状态切换。"),
            new("禁用场景", "禁止在包含确定/取消按钮的待提交表单中使用（表单内应使用 XYCheckbox）。")
        ],
        "XYUI-2-09" => [
            new("组件定义", "单行文本输入控件，继承 XyuiEditableTextBox，内置底部 3 DIP 焦点强调边框。"),
            new("焦点全选规则", "核心契约：第一次获得编辑焦点时自动全选文本；已经处于编辑焦点后再次点击，正常定位光标 Caret。"),
            new("状态区分", "严格区分 IsReadOnly 与 IsEnabled：ReadOnly 可聚焦/可复制文本但不可编辑；Disabled 完全阻断交互。"),
            new("错误视觉", "支持 IsError 属性，当处于错误态时切换边框为语义警示色（:error 伪类）。")
        ],
        "XYUI-2-10" => [
            new("组件定义", "高精度数值输入与微调控件，集成了直接键盘键入、微调步进器与按住鼠标拖拽微调。"),
            new("Scrub 协议", "按住数值文本区域水平拖动（每 4 DIP 按 DecimalPlaces 精度步进）进行快速微调，松开提交，按 Escape 可取消回退。"),
            new("键盘步长", "支持 Up/Down 键调节：普通按 Step，按住 Shift 按 LargeStep，按住 Ctrl 按 SmallStep。"),
            new("数值约束", "严格遵循 Minimum 与 Maximum 边界 Clamp，支持小数位 DecimalPlaces 与单位 Suffix。")
        ],
        "XYUI-2-11" => [
            new("组件定义", "连续范围滑块控件，左侧为可视化滑动轨道，右侧为 104 DIP 紧凑精确数值输入框。"),
            new("单一真值", "滑动轨道与右侧 XYNumberField 始终双向同步唯一的 Value 真值，绝不脱节。"),
            new("边界回归", "保证 0%、50%、100% 极限位置及负数区间数值与轨道填充比例精确对齐。"),
            new("布局自适应", "轨道与滑块 Thumb 适应不同宿主宽度，右侧输入框可通过 IsNumberFieldVisible 显隐。")
        ],
        "XYUI-2-12" => [
            new("组件定义", "可编辑且支持实时模糊过滤的下拉组合框，内置真正的 Popup 弹出层与候选项列表。"),
            new("过滤契约", "在文本框中键入关键词时，下拉列表即时按照包含模式模糊筛选，保留原始 ItemsSource 不被破坏。"),
            new("键盘与弹层", "Down 展开并高亮首项，Up/Down 移动焦点，Enter 选中提交，Escape 立即关闭下拉层。"),
            new("生命周期", "包含宿主窗口失活与 Visual Tree 脱离时自动安全收起 Popup 的生命周期防护。")
        ],
        _ => []
    };
}
