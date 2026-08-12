namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    void SelectTool(string name, bool logTool = true)
    {
        if (!CanChangeToolNow(name)) return;
        var requestedTool = EditorToolText.FromText(name);
        if (requestedTool == EditorToolId.RegionDrawing && !CanStartRegionDrawing)
        {
            FooterState = "状态：不可用";
            FooterMessage = "请先选择一个正常且未锁定的区域数据集，并进入区域编辑。";
            return;
        }
        if (IsRegionDrawingTool && requestedTool != EditorToolId.RegionDrawing)
        {
            var hadDraft = _regionDrawing.IsActive;
            _regionDrawing.Cancel();
            RaiseRegionDrawingBindings();
            if (hadDraft) LogRegionDrawingCanceled();
        }
        if (name is "框选")
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
        if (logTool) LogTool(ActiveTool);
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
        OnPropertyChanged(nameof(IsRegionDrawingTool));
        OnPropertyChanged(nameof(SnapMode));
    }
}
