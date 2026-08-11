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

    void SwitchWorkspace(object? value)
    {
        if (!TryWorkspaceId(value, out var target) || CurrentWorkspace.Id == target) return;
        CancelActiveInput("切换工作区");
        var transition = _workspaceManager.Switch(target);
        if (!transition.Changed) return;
        SelectTool("选择", logTool: false);
        OnPropertyChanged(nameof(CurrentWorkspace));
        OnPropertyChanged(nameof(CurrentWorkspaceDisplayName));
        OnPropertyChanged(nameof(IsMapWorkspace));
        OnPropertyChanged(nameof(IsRegionWorkspace));
        _logBus.Info(EditorLogSource.Editor, EditorLogCategory.Command,
            $"工作区已切换为：{CurrentWorkspaceDisplayName}", "保留 World、Camera、Selection 与唯一 Main Viewport。");
        RefreshLogBindings();
        OnPropertyChanged(nameof(LogSummary));
    }

    static bool TryWorkspaceId(object? value, out EditorWorkspaceId id)
    {
        if (value is EditorWorkspaceId workspace) { id = workspace; return true; }
        return Enum.TryParse(value?.ToString(), out id);
    }
}
