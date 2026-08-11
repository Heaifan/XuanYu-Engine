using System.Windows.Input;
using XuanYu.Editor.Mode;

namespace XuanYu.Editor.UI;

// EDITOR-A-R3：Mode 是 Workspace 的上层；UiVm 只桥接，不复制 World/Camera/Selection。
public sealed partial class UiVm
{
    readonly EditorModeManager _modeManager = new();

    public ICommand ToggleEditorModeCommand { get; }
    public EditorModeId CurrentMode => _modeManager.CurrentMode;
    public bool IsManageMode => CurrentMode == EditorModeId.Manage;
    public bool IsEditMode => CurrentMode == EditorModeId.Edit;
    public bool IsMapEditMode => IsEditMode && IsMapWorkspace;
    public bool IsRegionEditMode => IsEditMode && IsRegionWorkspace;
    public string CurrentModeDisplayName => IsManageMode ? "管理模式" : CurrentWorkspaceDisplayName;
    public string WorkspaceSelectorHeader => IsManageMode ? $"编辑目标：{SelectedEditTargetDisplayName}" : CurrentWorkspaceDisplayName;
    public string SelectedEditTargetDisplayName => IsMapWorkspace ? "地图" : "区域";
    public string MapWorkspaceOptionText => IsManageMode ? "地图" : "地图编辑";
    public string RegionWorkspaceOptionText => IsManageMode ? "区域" : "区域编辑";
    public string ModeActionText => IsManageMode ? "进入编辑" : "返回管理";

    public bool ToggleEditorMode()
    {
        CancelActiveInput("切换编辑模式");
        if (IsRegionDrawingTool || IsRegionDrawingDraftActive) CancelRegionDrawingFromEscape();
        var transition = _modeManager.Toggle();
        if (!transition.Changed) return false;
        SelectTool("选择", logTool: false);
        LeftTabIndex = IsManageMode ? 0 : IsMapWorkspace ? 2 : 3;
        RaiseModeBindings();
        _logBus.Info(EditorLogSource.Editor, EditorLogCategory.Command,
            $"已切换为：{CurrentModeDisplayName}", "保留 World、Camera、Selection、Assets 与唯一 Main Viewport。");
        RefreshLogBindings(); OnPropertyChanged(nameof(LogSummary));
        return true;
    }

    void RaiseModeBindings()
    {
        OnPropertyChanged(nameof(CurrentMode)); OnPropertyChanged(nameof(IsManageMode));
        OnPropertyChanged(nameof(IsEditMode)); OnPropertyChanged(nameof(IsMapEditMode));
        OnPropertyChanged(nameof(IsRegionEditMode)); OnPropertyChanged(nameof(CurrentModeDisplayName));
        OnPropertyChanged(nameof(WorkspaceSelectorHeader)); OnPropertyChanged(nameof(SelectedEditTargetDisplayName));
        OnPropertyChanged(nameof(MapWorkspaceOptionText));
        OnPropertyChanged(nameof(RegionWorkspaceOptionText)); OnPropertyChanged(nameof(ModeActionText));
        OnPropertyChanged(nameof(IsMapEditorMode));
    }
}
