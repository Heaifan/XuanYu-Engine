namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2DocumentationCatalog
{
    static IReadOnlyList<XYUIDocFoundationItem> Phase2BFoundationMappings(string id) => id switch
    {
        "XYUI-2-07" => [
            new("Radio Halo", "XY.Brush.State.Color.Hover", "悬停时外环扩散光晕"),
            new("Radio Circle", "XY.Brush.Border.Color.Default", "16 DIP 外圆圈边框"),
            new("Active Dot", "XY.Brush.Accent.Strong", "6 DIP 实心强色中心圆点"),
            new("Focus Visual", "XY.Brush.Border.Color.Focus", "键盘导航焦点外圈")
        ],
        "XYUI-2-08" => [
            new("Track Inactive", "XY.Brush.Surface.PanelAlt", "关闭态轨道底色"),
            new("Track Active", "XY.Brush.Accent.Soft", "开启态轨道底色"),
            new("Thumb Metric", "14×14 DIP", "圆形滑块几何尺寸"),
            new("Disabled", "XY.State.Disabled.*", "禁用态全色系衰减")
        ],
        "XYUI-2-09" => [
            new("Input Surface", "XY.Brush.Surface.Input", "输入框常态背景色"),
            new("Focus Edge", "XY.Brush.Accent.Strong", "聚焦时底部 3 DIP 强调指示线"),
            new("Placeholder", "XY.Brush.Text.Tertiary", "占位文本弱化前景色"),
            new("Error Border", "XY.Semantic.Error.Border", "错误态红色提示边框")
        ],
        "XYUI-2-10" => [
            new("Value Host", "PART_ValueHost", "数值文本承载容器与 Scrub 拖拽区"),
            new("Stepper Buttons", "PART_StepperCell", "上下微调步进小三角，悬停显露"),
            new("Unit Suffix", "XY.Brush.Text.Secondary", "紧凑后缀单位颜色"),
            new("Scrub Feedback", "XY.Brush.Accent.Strong", "拖拽时微调光标指示")
        ],
        "XYUI-2-11" => [
            new("Rail Geometry", "XY.Slider.Rail.Height = 4", "4 DIP 轨道槽高度"),
            new("Thumb Geometry", "14×14 DIP (激活 16×16)", "圆形滑块常态与拖拽瞬态尺寸"),
            new("Fill Track", "XY.Brush.Accent.Strong", "已填充区高亮进度条"),
            new("Twin NumberField", "104 DIP Width", "右侧精确数值框标准宽度")
        ],
        "XYUI-2-12" => [
            new("Chrome Host", "XY.Brush.Surface.Input", "主输入框与右侧下拉槽容器"),
            new("Chevron Icon", "XY.Brush.Text.Secondary", "右侧 32 DIP 展开箭头"),
            new("Popup Surface", "XY.Brush.Surface.Raised", "下拉列表阴影与浮层背景"),
            new("Item Selected", "XY.Brush.Surface.Selected", "候选项选中高亮底色")
        ],
        _ => []
    };

    static IReadOnlyList<XYUIDocState> Phase2BStates(string id) => id switch
    {
        "XYUI-2-07" => [
            new("Unchecked", "未勾选态，空心环"),
            new(":checked", "勾选态，呈现强色中心点"),
            new(":pointerover", "悬停态，外圈显露光晕"),
            new(":disabled", "禁用态，状态锁定不可交互")
        ],
        "XYUI-2-08" => [
            new("Off (Unchecked)", "关闭态，滑块居左"),
            new(":checked (On)", "开启态，滑块居右且轨道高亮"),
            new(":disabled", "禁用态，阻断点击")
        ],
        "XYUI-2-09" => [
            new("Default", "默认常态，呈现微弱边框"),
            new(":focus", "获得焦点态，底部强调线显露"),
            new("ReadOnly", "只读态，只读光标与选择能力"),
            new(":error", "错误态，红边警示")
        ],
        "XYUI-2-10" => [
            new("Default", "格式化数值展示"),
            new(":focus", "文本全选编辑态"),
            new("Scrubbing (internal)", "鼠标拖拽连续微调的内部交互态；当前无公开 :scrubbing 伪类"),
            new(":error", "非法文本键入提示")
        ],
        "XYUI-2-11" => [
            new("Default", "滑块静止态"),
            new(":pointerover", "滑块悬停微放大"),
            new("Dragging (internal)", "XYSliderTrack 的内部拖拽态；当前无公开 :dragging 伪类"),
            new(":disabled", "滑块禁用锁定")
        ],
        "XYUI-2-12" => [
            new("Closed", "收起常态；当前无公开 :closed 伪类"),
            new("Open (.xyui-combo-open)", "下拉 Popup 展开态；通过类名切换"),
            new("Filtering (internal)", "即时过滤筛选候选项中；当前无公开 :filtering 伪类"),
            new(":error", "输入无效且不允许自定义值")
        ],
        _ => []
    };
}
