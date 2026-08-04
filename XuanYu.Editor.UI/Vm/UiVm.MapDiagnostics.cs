namespace XuanYu.Editor.UI;

// MAP-A-R2-D3-F1：地图命令低频诊断日志（复用既有日志总线，不建第二套 Logger）。
// 只记录命令/提交/撤销/重做节点，禁止记录每帧/Hover/Getter/相同快照重复消费。
public sealed partial class UiVm
{
    void LogMapCommandReceived(string command)
    {
        _logBus.Info(EditorLogSource.Editor, EditorLogCategory.Command,
            $"地图命令收到：Command={command}；WidthText={MapWidthText}；DepthText={MapDepthText}；BaseHeightText={MapBaseHeightText}",
            "地图面板按钮低频命令。");
        RefreshLogBindings();
    }

    void LogMapPropertiesStarted(string mapId, string before, double beforeHeight,
        string candidate, double candidateHeight, long stateId, long sequence)
    {
        _logBus.Info(EditorLogSource.Editor, EditorLogCategory.Command,
            $"地图属性提交开始：MapId={mapId}；Before={before}；BeforeHeight={beforeHeight}；" +
            $"Candidate={candidate}；CandidateHeight={candidateHeight}；StateId={stateId}；ChangeSequence={sequence}",
            "地图属性原子提交开始。");
        RefreshLogBindings();
    }

    void LogMapPropertiesSucceeded(string mapId, string after, double afterHeight,
        long stateId, long sequence, bool canUndo, bool canRedo)
    {
        _logBus.Info(EditorLogSource.Editor, EditorLogCategory.Command,
            $"地图属性提交成功：MapId={mapId}；After={after}；AfterHeight={afterHeight}；" +
            $"StateId={stateId}；ChangeSequence={sequence}；CanUndo={canUndo}；CanRedo={canRedo}",
            "地图属性原子提交成功，历史已入栈。");
        RefreshLogBindings();
    }

    void LogMapPropertiesFailed(string code, string message, string current,
        long stateId, long sequence)
    {
        _logBus.Info(EditorLogSource.Editor, EditorLogCategory.Command,
            $"地图属性提交失败：Code={code}；Message={message}；Current={current}；" +
            $"StateId={stateId}；ChangeSequence={sequence}；StateUnchanged=True",
            "失败零污染：地图/历史/序号均未变化。");
        RefreshLogBindings();
    }

    void LogMapHistoryResult(string action, bool success, string before, string after,
        long stateId, long sequence, bool canUndo, bool canRedo, string? code)
    {
        var message = success
            ? $"地图{action}成功：Before={before}；After={after}；StateId={stateId}；" +
              $"ChangeSequence={sequence}；CanUndo={canUndo}；CanRedo={canRedo}"
            : $"地图{action}失败：Code={code}；状态未改变";
        _logBus.Info(EditorLogSource.Editor, EditorLogCategory.Command, message, "地图历史低频操作。");
        RefreshLogBindings();
    }
}
