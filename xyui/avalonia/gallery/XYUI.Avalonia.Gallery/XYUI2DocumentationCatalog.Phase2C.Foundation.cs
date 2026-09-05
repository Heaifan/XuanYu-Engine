namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2DocumentationCatalog
{
    static IReadOnlyList<XYUIDocFoundationItem> Phase2CFoundationMappings(string id) => id switch
    {
        "XYUI-2-13" => [
            new("Surface Chrome", "XY.Brush.Surface.Input", "输入框底色与圆角"),
            new("Chevron Icon", "XY.Brush.Text.Secondary", "下拉指示箭头图标颜色"),
            new("Focus Edge", "XY.Brush.Accent.Strong", "聚焦时底部 3 DIP 强色高亮"),
            new("Popup Surface", "XY.Brush.Surface.Raised", "浮层阴影与列表背景")
        ],
        "XYUI-2-14" => [
            new("Editor Header", "XY.Brush.Surface.PanelAlt", "Editor 模式顶部元数据栏背景"),
            new("Metadata Text", "XY.Brush.Text.Secondary", "行数与字符数计数器弱化文本色"),
            new("Input Body", "XY.Brush.Surface.Input", "文本区域背景色"),
            new("Focus Edge", "XY.Brush.Accent.Strong", "聚焦时底部高亮指示线")
        ],
        "XYUI-2-15" => [
            new("Search Icon", "XY.Brush.Text.Secondary", "左侧放大镜矢量图标"),
            new("Clear Button", "XY.Brush.State.Color.Hover", "清空操作悬停背景色"),
            new("Filter Active", "XY.Brush.Accent.Strong", "筛选生效时的视觉强色标记"),
            new("Filter Surface", "XY.Brush.Surface.Raised", "高级筛选条件弹层面板背景")
        ],
        "XYUI-2-16" => [
            new("Mask Glyph", "XY.Brush.Text.Primary", "圆点密码掩码前景色"),
            new("Eye Icon", "XY.Brush.Text.Secondary", "右侧查看眼睛图标颜色"),
            new("Eye Active", "XY.Brush.Accent.Strong", "按住显露明文时的视觉高亮"),
            new("Input Chrome", "XY.Brush.Surface.Input", "密码输入框背景色")
        ],
        "XYUI-2-17" => [
            new("Segment Focus", "XY.Brush.Surface.Selected", "当前激活分段高亮背景"),
            new("Calendar Button", "XY.Brush.Text.Secondary", "右侧日历入口图标颜色"),
            new("Calendar Cell", "XY.Brush.State.Color.Hover", "日历面板日期单元格悬停底色"),
            new("Date Selected", "XY.Brush.Accent.Strong", "日历面板当前选中日期的强调色")
        ],
        "XYUI-2-18" => [
            new("Segment Focus", "XY.Brush.Surface.Selected", "当前激活时间分段高亮背景"),
            new("Clock Button", "XY.Brush.Text.Secondary", "右侧时钟入口图标颜色"),
            new("Scrub Indicator", "XY.Brush.Accent.Strong", "拖拽微调时的微型左右箭头指示"),
            new("Popup Surface", "XY.Brush.Surface.Raised", "调整时间面板背景")
        ],
        _ => []
    };

    static IReadOnlyList<XYUIDocState> Phase2CStates(string id) => id switch
    {
        "XYUI-2-13" => [
            new("Closed", "收起常态，当前无公开 :closed 伪类"),
            new("Open (.xyui-select-open)", "下拉列表展开态，通过类名切换"),
            new(":pressed", "按压瞬态，表面底色微调"),
            new(":disabled", "禁用态，阻断点击与展开")
        ],
        "XYUI-2-14" => [
            new("Standard", "标准多行编辑态"),
            new(":editor", "编辑器模式，顶部显露元数据栏"),
            new(":focus", "获得焦点态，底部强调线显露"),
            new("ReadOnly", "只读态，允许划选复制但不可编辑"),
            new(":error", "错误态，红边警示")
        ],
        "XYUI-2-15" => [
            new("Default", "默认常态"),
            new(":focus", "聚焦编辑态"),
            new("FilterActive (.xyui-search-filter-active)", "筛选激活态"),
            new("Searching (.xyui-search-searching)", "正在搜索中"),
            new("NoResult (.xyui-search-no-result)", "无匹配结果"),
            new(":disabled", "禁用态")
        ],
        "XYUI-2-16" => [
            new("Masked", "默认密文遮罩态"),
            new("Holding (.xyui-password-holding)", "按住临时显露明文态"),
            new(":focus", "获得焦点态"),
            new(":disabled", "禁用态")
        ],
        "XYUI-2-17" => [
            new("Idle", "常规日期展示态"),
            new("Editing (.xyui-date-editing)", "分段编辑微调中"),
            new("CalendarOpen", "月度日历面板展开态"),
            new("DatePopupOpen", "调整日期弹层展开态"),
            new(":disabled", "禁用态")
        ],
        "XYUI-2-18" => [
            new("Idle", "常规时间展示态"),
            new("Editing (.xyui-time-editing)", "分段编辑中"),
            new("Scrubbing (.xyui-time-scrubbing)", "鼠标拖拽连续微调中"),
            new("TimePopupOpen", "调整时间弹层展开态"),
            new(":disabled", "禁用态")
        ],
        _ => []
    };
}
