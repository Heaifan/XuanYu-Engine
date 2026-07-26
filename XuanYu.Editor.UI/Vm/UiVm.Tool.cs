namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    void SelectTool(string name)
    {
        if (!CanChangeToolNow(name)) return;
        if (name is "框选" or "缩放")
        {
            FooterState = "状态：就绪";
            FooterMessage = $"{name}尚未实装，当前仍保持：{ActiveTool}。";
            _logBus.Info(EditorLogSource.Input, EditorLogCategory.Tool,
                $"{name}工具尚未实装，当前工具仍为：{ActiveTool}", "未实装工具点击不切换真实工具。");
            RefreshLogBindings();
            OnPropertyChanged(nameof(LogSummary));
            RaiseToolChanged();
            return;
        }

        if (_editorState.ChangeTool(new ChangeEditorToolCommand(name)) is null)
        {
            RaiseToolChanged();
            return;
        }

        RaiseToolChanged();
        PublishSceneRenderSnapshot();
        FooterMessage = $"当前工具：{ActiveTool}。视口等待输入。";
        FooterState = "状态：就绪";
        LogTool(ActiveTool);
        OnPropertyChanged(nameof(LogSummary));
    }

    bool IsTool(EditorToolId tool) => _editorState.ToolSnapshot.ActiveTool == tool;

    void ToggleSnap()
    {
        var result = _editorState.ToggleSnap(new ToggleEditorSnapCommand());
        RaiseToolChanged();
        FooterMessage = $"{result.Snapshot.SnapText}；当前工具：{ActiveTool}。";
        FooterState = "状态：就绪";
        LogCommand(result.Snapshot.SnapText);
        OnPropertyChanged(nameof(LogSummary));
    }

    void RaiseToolChanged()
    {
        OnPropertyChanged(nameof(ActiveTool)); OnPropertyChanged(nameof(FooterMode));
        OnPropertyChanged(nameof(IsSelectTool)); OnPropertyChanged(nameof(IsBoxSelectTool));
        OnPropertyChanged(nameof(IsMoveTool)); OnPropertyChanged(nameof(IsRotateTool));
        OnPropertyChanged(nameof(IsScaleTool)); OnPropertyChanged(nameof(IsSnapEnabled));
        OnPropertyChanged(nameof(SnapMode));
    }
}
