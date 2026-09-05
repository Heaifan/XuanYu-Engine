namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2DocumentationCatalog
{
    static string Phase2CQuickStart(string id) => id switch
    {
        "XYUI-2-13" => "<xy:XYSelect ItemsSource=\"{Binding Languages}\" SelectedIndex=\"0\" Placeholder=\"选择语言\" />",
        "XYUI-2-14" => "<xy:XYTextArea Mode=\"Editor\" EditorType=\"JSON\" Text=\"{\n  \"\"mode\"\": \"\"balanced\"\"\n}\" />",
        "XYUI-2-15" => "<xy:XYSearchField Placeholder=\"搜索资产与场景...\" SearchRequested=\"OnSearch\" />",
        "XYUI-2-16" => "<xy:XYPasswordField Placeholder=\"请输入密码\" Password=\"{Binding AuthToken}\" />",
        "XYUI-2-17" => "<xy:XYDatePicker SelectedDate=\"{Binding ReleaseDate}\" MinDate=\"2026-01-01\" />",
        "XYUI-2-18" => "<xy:XYTimePicker Time=\"{Binding ScheduledTime}\" ShowSeconds=\"True\" />",
        _ => ""
    };

    static IReadOnlyList<XYUIDocRule> Phase2CCoreRules(string id) => id switch
    {
        "XYUI-2-13" => [
            new("组件定义", "固定候选选择控件，严格从固定集合中选取，不提供自由文本编辑与模糊过滤。"),
            new("交互契约", "点击表面任意位置切换 Popup 展开；Enter/Space 展开或提交；Up/Down 导航；Escape 安全关闭。"),
            new("与 ComboBox 区别", "ComboBox 支持可编辑键入与模糊过滤；Select 纯为离散候选选择器。")
        ],
        "XYUI-2-14" => [
            new("组件定义", "多行文本编辑控件，支持标准文本录入与带元数据栏的代码/配置编辑器模式。"),
            new("编辑协议", "严格遵从统一编辑协议：首焦自动全选，再次点击定位光标 Caret；支持回车换行。"),
            new("自适应排版", "AutoGrow=true 时高度自适应内容；达到 MaxHeight 时启动内部 ScrollViewer 滚动。")
        ],
        "XYUI-2-15" => [
            new("组件定义", "集成搜索语义的文本框，内置搜索图标、快速清空按钮以及可选的高级筛选面板。"),
            new("按键契约", "Enter 触发 SearchRequested 事件；有内容时 Esc 清空文本；筛选面板打开时 Esc 关闭浮层。"),
            new("清空与焦点", "点击清空按钮立即清空 Text，并将焦点保持在输入框内。")
        ],
        "XYUI-2-16" => [
            new("组件定义", "密码安全输入控件，默认展示圆点掩码，提供按住查看明文的临时揭示功能。"),
            new("揭示生命周期", "按住眼睛图标或回车/空格时临时显示明文；指针释放、失去捕获或失焦时强制遮罩。"),
            new("安全性", "严禁持久保持明文显示状态；禁用态完全阻断明文查看。")
        ],
        "XYUI-2-17" => [
            new("组件定义", "基于 DateOnly 的精准日期选择控件，采用年月日独立分段键盘编辑与可视化日历面板。"),
            new("分段流转", "Left/Right 键切换分段，Up/Down 键步进数值，键入数字直接覆盖分段；非法日期自动修正。"),
            new("双重调节", "右侧日历图标弹出月度日历；点击分段弹出增减调节面板；支持 Previous/Next 快速换日。")
        ],
        "XYUI-2-18" => [
            new("组件定义", "基于 TimeOnly 的高精度时间选择器，支持时分秒分段流转、数值微调与弹出调节。"),
            new("Scrubbing 协议", "水平按住数值区域拖动可启动无极微调（每 4 DIP 一步长），右增左减，松开提交。"),
            new("秒段可控", "ShowSeconds 控制秒分段显隐；隐藏时秒段完全不参与排版与键盘切换。")
        ],
        _ => []
    };
}
