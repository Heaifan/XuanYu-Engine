namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2DocumentationCatalog
{
    static IReadOnlyList<XYUIDocGuideItem> Phase2BHowToUse(string id) => id switch
    {
        "XYUI-2-07" => [
            new("适用场景 (Use when)", "在一组互斥的选项中做出单项选择，如「渲染模式（实时/最终）」、「坐标空间（局部/世界）」。"),
            new("禁用场景 (Avoid when)", "不要用于可并存的多个特性开关（应使用 XYCheckbox）；不要用于二元开关（应使用 XYSwitch）。"),
            new("分组规范 (GroupName)", "必须显式为同组内的所有 RadioButton 指定相同的 GroupName 属性，确保互斥正确建立。"),
            new("布局推荐 (Layout)", "推荐垂直堆叠排版，或横向紧凑对齐，选项间保持 Space2 以上间隔。")
        ],
        "XYUI-2-08" => [
            new("适用场景 (Use when)", "控制具有即时生效特性的二元功能开闭，如「硬件加速」、「实时自动保存」、「物理模拟」。"),
            new("三态对比 (Comparison)", "Switch 表达即时生效；Checkbox 表达多选与待提交表单；ToggleButton 表达持续视图/工具模式。"),
            new("触控/点击 (Hit Area)", "用户点击开关滑块或整行文字标签均可顺畅触发切换，无需精确瞄准滑块本体。"),
            new("禁用场景 (Avoid when)", "不要在需要用户在弹窗底部点击「确定」后才生效的表单中使用。")
        ],
        "XYUI-2-09" => [
            new("编辑焦点规范 (Focus Rule)", "首次聚焦自动全选：Tab 聚焦或首次点击文本框时自动选中全部文本，方便立即覆盖输入。"),
            new("二次点击定位 (Second Click)", "处于聚焦状态下的后续指针点击，将正常落入点击位置的光标 Caret，方便局部修改。"),
            new("只读与禁用 (ReadOnly vs Disabled)", "ReadOnly 允许用户选中并复制资源 ID 或诊断文本；Disabled 完全阻断交互与划选。"),
            new("校验反馈 (Validation)", "当键入非法内容时，置 IsError=true，边框立即呈现错误警示色。")
        ],
        "XYUI-2-10" => [
            new("微调拖拽协议 (Scrubbing)", "按住数值文本区域水平拖动，值按 DecimalPlaces 精度步进；按 Escape 可立即撤回初始值。"),
            new("键盘加速 (Key Modifiers)", "按 Up/Down 微调；按住 Shift 加速为 LargeStep；按住 Ctrl 精细微调为 SmallStep。"),
            new("边界自愈 (Clamping)", "任何超出 [Minimum, Maximum] 区间的非法输入在失焦或 Enter 提交时自动被 Clamp。"),
            new("格式与单位 (Format)", "利用 DecimalPlaces 规范浮点精度，利用 Suffix 标注物理单位（如 %, px, rad）。")
        ],
        "XYUI-2-11" => [
            new("适用场景 (Use when)", "用于视觉调节直观的连续参数，如「透明度」、「音量增益」、「LOD 裁切距离」。"),
            new("双向联动 (Twin Sync)", "拖动滑块与在右侧精确数值框键入数值完全等价，二者数据与事件时刻保持同步。"),
            new("边界回归 (Bounds)", "支持最小 0% 到最大 100% 的精准滑动与填充，轨道与手柄对齐无偏差。"),
            new("紧凑降级 (Compact)", "若空间受限，可设 IsNumberFieldVisible=false，仅展示滑动轨道。")
        ],
        "XYUI-2-12" => [
            new("模糊输入过滤 (Filter)", "直接在输入框中键入字母，下拉列表实时过滤包含该字符的候选项，无需重新点击搜索。"),
            new("键盘流转 (Keyboard Flow)", "聚焦后按 Down 展开，继续按 Up/Down 切换高亮，Enter 选中并关闭，Escape 取消关闭。"),
            new("自定义值策略 (Custom Value)", "若仅允许从固定候选选取，保持 IsCustomValueAllowed=false，非法键入将红边警示。"),
            new("生命周期规范 (Lifecycle)", "宿主窗口失去焦点或应用切换后台时，内置 Popup 自动安全收起，防止悬挂浮层。")
        ],
        _ => []
    };
}
