using XuanYu.Editor.Workspace;

namespace XuanYu.Editor.UI;

// EDITOR-A-R2：UiVm 持有唯一 Workspace Manager；World、Camera、Selection 仍由既有 Owner 管理。
public sealed partial class UiVm
{
    readonly EditorWorkspaceManager _workspaceManager = new();

    public EditorWorkspaceDefinition CurrentWorkspace => _workspaceManager.CurrentWorkspace;
    public string CurrentWorkspaceDisplayName => CurrentWorkspace.DisplayName;
    public bool IsMapWorkspace => CurrentWorkspace.Id == EditorWorkspaceId.MapEditor;
    public bool IsRegionWorkspace => CurrentWorkspace.Id == EditorWorkspaceId.RegionEditor;
    public bool IsRoadWorkspace => CurrentWorkspace.Id == EditorWorkspaceId.RoadEditor;

    void SwitchWorkspace(object? value)
    {
        if (!TryWorkspaceId(value, out var target) || CurrentWorkspace.Id == target) return;
        if (IsEditMode) CancelActiveInput("切换编辑工作区");
        var transition = _workspaceManager.Switch(target);
        if (!transition.Changed) return;
        ClearLayerSelection();
        if (IsEditMode) { SelectTool("选择", logTool: false); LeftTabIndex = IsMapWorkspace ? 2 : IsRegionWorkspace ? 3 : 4; }
        OnPropertyChanged(nameof(CurrentWorkspace));
        OnPropertyChanged(nameof(CurrentWorkspaceDisplayName));
        OnPropertyChanged(nameof(IsMapWorkspace));
        OnPropertyChanged(nameof(IsRegionWorkspace));
        OnPropertyChanged(nameof(IsRoadWorkspace));
        RaiseLayerContextBindings();
        RaiseModeBindings();
        _logBus.Info(EditorLogSource.Editor, EditorLogCategory.Command,
            $"编辑目标已切换为：{CurrentWorkspaceDisplayName}", "Manage 只改变目标；Edit 保留上下文后切换工作区。");
        RefreshLogBindings();
        OnPropertyChanged(nameof(LogSummary));
    }

    static bool TryWorkspaceId(object? value, out EditorWorkspaceId id)
    {
        if (value is EditorWorkspaceId workspace) { id = workspace; return true; }
        return Enum.TryParse(value?.ToString(), out id);
    }
}
