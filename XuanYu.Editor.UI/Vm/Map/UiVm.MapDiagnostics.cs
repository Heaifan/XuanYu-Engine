namespace XuanYu.Editor.UI;

// MAP-A-R2-D3-F2：地图命令低频诊断日志（复用既有日志总线，字段名/状态值全部中文显示）。
// 只记录命令/提交/撤销/重做节点；禁止记录每帧/Hover/Getter/相同快照重复消费。
public sealed partial class UiVm
{
    void LogMapCommandReceived(string command)
    {
        _logBus.Info(EditorLogSource.Editor, EditorLogCategory.Command,
            $"地图命令收到：命令={command}；宽度输入={MapWidthText}；深度输入={MapDepthText}；基础高度输入={MapBaseHeightText}",
            "地图面板按钮低频命令。");
        RefreshLogBindings();
    }

    void LogMapPropertiesStarted(string mapId, string before, double beforeHeight,
        string candidate, double candidateHeight, long stateId, long sequence)
    {
        _logBus.Info(EditorLogSource.Editor, EditorLogCategory.Command,
            $"地图属性提交开始：地图标识={mapId}；原尺寸={before}；原基础高度={beforeHeight}；" +
            $"候选尺寸={candidate}；候选基础高度={candidateHeight}；历史状态={stateId}；变更序号={sequence}",
            "地图属性原子提交开始。");
        RefreshLogBindings();
    }

    void LogMapPropertiesSucceeded(string mapId, string after, double afterHeight,
        long stateId, long sequence, bool canUndo, bool canRedo)
    {
        _logBus.Info(EditorLogSource.Editor, EditorLogCategory.Command,
            $"地图属性提交成功：地图标识={mapId}；新尺寸={after}；新基础高度={afterHeight}；" +
            $"历史状态={stateId}；变更序号={sequence}；可撤销={FormatBoolean(canUndo)}；可重做={FormatBoolean(canRedo)}",
            "地图属性原子提交成功，历史已入栈。");
        RefreshLogBindings();
    }

    void LogMapPropertiesFailed(string code, string message, string current,
        long stateId, long sequence)
    {
        _logBus.Info(EditorLogSource.Editor, EditorLogCategory.Command,
            $"地图属性提交失败：错误类型={FormatErrorCode(code)}；错误说明={message}；当前尺寸={current}；" +
            $"历史状态={stateId}；变更序号={sequence}；状态保持不变=是",
            "失败零污染：地图/历史/序号均未变化。");
        RefreshLogBindings();
    }

    void LogMapHistoryResult(string action, bool success, string before, string after,
        long stateId, long sequence, bool canUndo, bool canRedo, string? code)
    {
        var message = success
            ? $"地图{action}成功：恢复前={before}；恢复后={after}；历史状态={stateId}；" +
              $"变更序号={sequence}；可撤销={FormatBoolean(canUndo)}；可重做={FormatBoolean(canRedo)}"
            : $"地图{action}失败：错误类型={FormatErrorCode(code ?? "")}；状态未改变";
        _logBus.Info(EditorLogSource.Editor, EditorLogCategory.Command, message, "地图历史低频操作。");
        RefreshLogBindings();
    }
}
