namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public int TransformHistoryCount => _historyOwner.Count;
    public int TransformRedoCount => _historyOwner.RedoCount;
    public bool TryCommitInspectorTransformValue(string group, string axis, string text)
    {
        if (_editorState.InteractionSnapshot.Phase != EditorInteractionPhase.Idle)
        {
            FooterMessage = "活动变换会话中不能提交检查器数值。";
            return false;
        }
        if (!TrySelectedEntityKey(out var key) || !_sceneState.TryGetEntity(key, out var entity))
        {
            FooterMessage = "检查器提交失败：未选择有效实体。";
            return false;
        }
        if (!TryParseInspectorNumber(text, out var value))
        {
            FooterMessage = "检查器提交失败：请输入有效数字。";
            return false;
        }
        if (!TryBuildInspectorTransform(entity.Transform, group, axis, value, out var next, out var error))
        {
            FooterMessage = error;
            return false;
        }
        var commit = _sceneState.CommitTransformWithResult(key, next);
        if (!commit.Changed)
        {
            FooterMessage = "检查器数值未变化。";
            return false;
        }
        RecordTransformHistory(commit);
        FooterMessage = $"检查器已提交：{group} {axis} = {FormatNumber(value)}。";
        OnPropertyChanged(nameof(InspectorFields));
        OnPropertyChanged(nameof(DebugObjectItems));
        OnPropertyChanged(nameof(TransformHistoryCount));
        OnPropertyChanged(nameof(TransformRedoCount));
        PublishSceneRenderSnapshot();
        return true;
    }
}
