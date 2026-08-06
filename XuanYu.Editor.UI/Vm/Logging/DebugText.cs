namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D4-F1：调试页静态键值行（结构化 Label/Value，替换拼接字符串；
// 渲染端用 ReadonlyKeyValueRow 单行双列 + 省略 + Tooltip）。
public static class DebugText
{
    public static readonly InspectorFieldRow[] ContextItems =
    [
        new("当前选择", "玄域示例项目"),
        new("当前工具", "选择"),
        new("拾取状态", "无命中"),
        new("日志策略", "高频事件不进入底部日志")
    ];

    public static readonly InspectorFieldRow[] ObjectItems =
    [
        new("类型", "项目"),
        new("对象 ID", "project.sample"),
        new("选中来源", "左侧项目树")
    ];

    public static readonly InspectorFieldRow[] ToolItems =
    [
        new("捕获", "未捕获"),
        new("拖动", "未开始"),
        new("预览", "无"),
        new("诊断", "未启用")
    ];

    public static readonly InspectorFieldRow[] InputItems =
    [
        new("鼠标", "空闲"),
        new("键盘", "无快捷键"),
        new("PointerMoved", "摘要记录"),
        new("Hover", "快照覆盖")
    ];
}
