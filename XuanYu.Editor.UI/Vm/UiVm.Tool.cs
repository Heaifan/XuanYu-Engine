namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    void SelectTool(string name)
    {
        CancelInteraction("切换工具");
        if (_editorState.ChangeTool(new ChangeEditorToolCommand(name)) is null)
        {
            RaiseToolChanged();
            return;
        }

        RaiseToolChanged();
        FooterMessage = $"当前工具：{ActiveTool}。视口等待输入。";
        FooterState = "状态：就绪";
        LogTool(ActiveTool);
        OnPropertyChanged(nameof(LogSummary));
    }

    bool IsTool(EditorToolId tool) => _editorState.ToolSnapshot.ActiveTool == tool;

    void RaiseToolChanged()
    {
        OnPropertyChanged(nameof(ActiveTool)); OnPropertyChanged(nameof(FooterMode));
        OnPropertyChanged(nameof(IsSelectTool)); OnPropertyChanged(nameof(IsBoxSelectTool));
        OnPropertyChanged(nameof(IsMoveTool)); OnPropertyChanged(nameof(IsRotateTool));
        OnPropertyChanged(nameof(IsScaleTool)); OnPropertyChanged(nameof(IsFocusTool));
        OnPropertyChanged(nameof(IsPanTool)); OnPropertyChanged(nameof(IsOrbitTool));
        OnPropertyChanged(nameof(IsSnapTool));
    }
}
