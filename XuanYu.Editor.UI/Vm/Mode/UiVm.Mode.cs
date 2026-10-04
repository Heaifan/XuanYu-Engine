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
    public bool CanUseEditTools => IsEditMode;
    public bool CanToggleSnap => IsMapEditMode;
    public bool IsRegionEditMode => IsEditMode && IsRegionWorkspace && IsRegionContext;
    public string CurrentEditorModeText => IsManageMode ? "管理模式" : CurrentWorkspaceDisplayName;

    public bool ToggleEditorMode()
    {
        CancelMarkerDrawing("模式切换");
        ViewportInput.CancelForModeChange();
        var transition = _modeManager.Toggle();
        if (!transition.Changed) return false;
        SelectTool("选择", logTool: false);
        RaiseModeBindings();
        _logBus.Info(EditorLogSource.Editor, EditorLogCategory.Command,
            $"已切换为：{CurrentEditorModeText}", "保留 World、Camera、Selection、Assets 与唯一 Main Viewport。");
        RefreshLogBindings(); OnPropertyChanged(nameof(LogSummary));
        return true;
    }

    void RaiseModeBindings()
    {
        OnPropertyChanged(nameof(CurrentMode)); OnPropertyChanged(nameof(IsManageMode));
        OnPropertyChanged(nameof(IsEditMode)); OnPropertyChanged(nameof(IsMapEditMode));
        OnPropertyChanged(nameof(IsMapWorkspaceInspectorVisible)); OnPropertyChanged(nameof(IsLegacyInspectorContentVisible));
        OnPropertyChanged(nameof(IsInspectorEmptyAndNoMapWorkspace));
        OnPropertyChanged(nameof(CanUseEditTools)); OnPropertyChanged(nameof(CanToggleSnap));
        OnPropertyChanged(nameof(CanTransformSelectedEntity));
        (SelectToolCommand as RelayCommand)?.RaiseCanExecuteChanged();
        OnPropertyChanged(nameof(IsRegionEditMode)); OnPropertyChanged(nameof(CurrentEditorModeText));
        OnPropertyChanged(nameof(CurrentRegionAuthoringMode));
        OnPropertyChanged(nameof(IsRegionSurfaceAuthoringMode));
        OnPropertyChanged(nameof(IsRoadAuthoringMode));
        OnPropertyChanged(nameof(CanRequestMarkerPlacement));
        OnPropertyChanged(nameof(CanStartMarkerPlacement));
        OnPropertyChanged(nameof(CanStartRegionDrawing));
        OnPropertyChanged(nameof(CanRequestRegionDrawing));
        OnPropertyChanged(nameof(CanRequestRoadDrawing));
        OnPropertyChanged(nameof(IsMapEditorMode));
        RaiseLayerContextBindings();
    }
}
