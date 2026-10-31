namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    IReadOnlyList<InspectorFieldRow> BuildDebugContextItems() =>
    [
        new("当前选择", HasSelection ? SelectionTitle : "未选择对象"),
        new("当前工具", ActiveTool),
        new("拾取状态", HasSelection ? "已命中" : "无命中"),
        new("日志策略", "高频事件不进入底部日志")
    ];

    IReadOnlyList<InspectorFieldRow> BuildDebugToolItems() =>
    [
        new("捕获", _editorState.InteractionSnapshot.HasCapture ? "已捕获" : "未捕获"),
        new("拖动", _editorState.InteractionSnapshot.HasCapture ? "进行中" : "未开始"),
        new("预览", _editorState.InteractionSnapshot.HasCapture ? "有" : "无"),
        new("诊断", "未启用")
    ];
}
