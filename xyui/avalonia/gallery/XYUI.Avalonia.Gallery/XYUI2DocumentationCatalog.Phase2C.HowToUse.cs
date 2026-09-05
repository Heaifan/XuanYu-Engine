namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2DocumentationCatalog
{
    static IReadOnlyList<XYUIDocGuideItem> Phase2CHowToUse(string id) => id switch
    {
        "XYUI-2-13" => [
            new("适用场景 (Use when)", "在固定、有限且互斥的离散候选集中进行单项选择，如「语言」、「渲染品质」、「分辨率」。"),
            new("禁用场景 (Avoid when)", "不要用于需要用户模糊搜索或自由键入新项的场景（应使用 XYComboBox）。"),
            new("长文本处理 (Ellipsis)", "当选项文字过长时，内部自动启用 CharacterEllipsis 省略，保持控件尺寸规整。")
        ],
        "XYUI-2-14" => [
            new("适用场景 (Use when)", "用于输入或编辑多行文本，如「资源备注」、「任务描述」、「配置 JSON」或「脚本代码」。"),
            new("模式选择 (Modes)", "简单文本使用 Standard；代码、脚本或需要行号/字符统计的结构化内容使用 Editor。"),
            new("只读与禁用 (ReadOnly)", "只读模式允许划选复制日志与报错信息；禁用态完全阻断交互。")
        ],
        "XYUI-2-15" => [
            new("适用场景 (Use when)", "需要提供资产检索、日志过滤、层级树节点定位等具有搜索与清空语义的场景。"),
            new("高级筛选 (Filter Panel)", "可通过 FilterContent 注入复合筛选面板；筛选条件生效时保持 FilterActive=true。"),
            new("键盘流转 (Keyboard)", "键入后按 Enter 发起搜索；按 Esc 快速清空内容或关闭筛选浮层。")
        ],
        "XYUI-2-16" => [
            new("适用场景 (Use when)", "用于敏感信息录入，如「登录密码」、「API 访问密钥」、「私有凭证」。"),
            new("临时揭示 (Reveal)", "用户按住右侧眼睛图标可临时核对密码，松开立即遮罩，兼顾易用与安全。"),
            new("禁用场景 (Avoid when)", "不要将密码作为普通 TextField 展示，严禁在界面上持久展示明文密码。")
        ],
        "XYUI-2-17" => [
            new("适用场景 (Use when)", "需要输入或选择日期，如「项目排期」、「发布版本日期」、「日志时间范围」。"),
            new("键盘操作 (Keyboard)", "通过 Left/Right 键在年月日分段间流转，直接键入数字替换，或者 Up/Down 增减。"),
            new("双重选择 (Popup)", "可点击右侧日历图标弹出月历直观选择，也可点击分段进入微调面板增减。")
        ],
        "XYUI-2-18" => [
            new("适用场景 (Use when)", "用于时间参数设定，如「日常任务触发点」、「物理引擎时间步长」、「动画关键帧时间」。"),
            new("无极拖拽 (Scrubbing)", "水平按住数值区域拖拽即可按照 4 DIP 一步长无极微调，右拖增加左拖减少。"),
            new("秒段开关 (ShowSeconds)", "日常小时/分钟选择设 ShowSeconds=false 降噪；高精度时间设 ShowSeconds=true。")
        ],
        _ => []
    };
}
