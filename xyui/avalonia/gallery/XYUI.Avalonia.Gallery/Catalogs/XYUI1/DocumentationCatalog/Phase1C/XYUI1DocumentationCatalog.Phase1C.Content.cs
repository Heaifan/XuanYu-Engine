namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1DocumentationCatalog
{
    static string Phase1CQuickStart(string id, string type) => id switch
    {
        "XYUI-1-14" => "<xy:XYSeparator Variant=\"Section\" />\n<!-- 垂直分割: Variant=\"VerticalSplit\" -->",
        "XYUI-1-15" => "<xy:XYHelpText Text=\"修改将在下次启动时生效\" />",
        "XYUI-1-16" => "<xy:XYErrorText Text=\"路径不存在\" />",
        "XYUI-1-17" => "<xy:XYWarningText Text=\"资源尚未保存\" />",
        "XYUI-1-18" => "<xy:XYShortcutHint Shortcut=\"Ctrl+Shift+S\" CombinationMode=\"SeparateKeycaps\" />",
        _ => Phase1DQuickStart(id, type)
    };

    static IReadOnlyList<XYUIDocRule> Phase1CCoreRules(string id) => id switch
    {
        "XYUI-1-14" => [
            new("组件定义", "内容结构分隔线，由 Variant 驱动方向与留白（无独立 Orientation 属性）。"),
            new("适用场景", "区隔列表行、Inspector 字段分组、Toolbar 分块与主视图左右分栏。"),
            new("禁用场景", "不要作为控件外边框（Border）或高亮背景使用；禁止单独加 Orientation 属性。"),
            new("相邻区别", "vs SectionTitle：Separator 仅提供纯净几何分隔线，不包含文本标题与折叠逻辑。")
        ],
        "XYUI-1-15" => [
            new("组件定义", "普通辅助说明与上下文提示组件，由 Caption 字阶与 Secondary 前景驱动。"),
            new("适用场景", "输入项下方、空闲区域操作指导、参数单位格式说明与功能使用提示。"),
            new("禁用场景", "不要用于风险警告（请使用 WarningText）或阻断错误（请使用 ErrorText）。"),
            new("相邻区别", "vs Caption：HelpText 针对具体控件/设置项提供操作引导；Caption 承担泛化次级说明。")
        ],
        "XYUI-1-16" => [
            new("组件定义", "失败、无效与阻断性错误提示组件，由 Semantic.Error 语义色族驱动。"),
            new("适用场景", "校验失败、非法输入路径、超出数值上下限与不可完成的操作结果提示。"),
            new("禁用场景", "不要用“红色文字”定义状态（此为语义色）；不要用于可容忍的操作警告。"),
            new("相邻区别", "vs WarningText：ErrorText 表达操作已被阻断/条件不成立；WarningText 仍可继续操作。"),
            new("已知限制 (GAP)", "XYUI1-GAP-004：Avalonia AutomationPeer 辅助功能映射尚未落地，诚实登记保留。")
        ],
        "XYUI-1-17" => [
            new("组件定义", "风险与注意事项警告组件，由 Semantic.Warning 语义色族驱动（可继续但需注意）。"),
            new("适用场景", "资源未保存、潜在性能损耗、显存超限预警与需要用户二次确认的非阻断状态。"),
            new("禁用场景", "禁止用于致命阻断错误（请使用 ErrorText）；不要用于常规使用帮助。"),
            new("相邻区别", "vs HelpText / ErrorText：WarningText 提示有风险但允许继续，HelpText 为纯净帮助。"),
            new("已知限制 (GAP)", "XYUI1-GAP-004：Avalonia AutomationPeer 辅助功能映射尚未落地，诚实登记保留。")
        ],
        "XYUI-1-18" => [
            new("组件定义", "键盘快捷键符号化提示组件，以独立键帽（SeparateKeycaps）紧凑排布。"),
            new("适用场景", "命令菜单项右侧、工具栏悬停提示、按钮旁键盘辅助说明与全局操作向导。"),
            new("禁用场景", "不要当作可点击的 Button 使用（此为提示标记）；禁止发明 Inline/Compact 等伪模式。"),
            new("相邻区别", "vs Badge / CodeText：具有真实键帽结构与 Separator 符号，使用 Mono 等宽字系。")
        ],
        _ => Phase1DCoreRules(id)
    };
}
