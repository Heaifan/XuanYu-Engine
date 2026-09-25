using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class WorkspaceSelector : UserControl
{
    const string ManagementId = "management";
    const string FeatureId = "feature-editing";
    readonly XYWorkspaceSwitcher _switcher;

    public WorkspaceSelector()
    {
        InitializeComponent();
        _switcher = new(new XYWorkspaceState(ManagementId),
            new XYWorkspaceItem(ManagementId, "管理模式"),
            new XYWorkspaceItem(FeatureId, "要素编辑"),
            new XYWorkspaceItem("scene-editing", "场景编辑（暂未开放）", false),
            new XYWorkspaceItem("debug", "调试（暂未开放）", false));
        _switcher.Name = "WorkspaceSelector";
        _switcher.Trigger.Name = "WorkspaceSelectorButton";
        XYDiagnostic.SetDebugId(_switcher, "XYE.TOP.WORKSPACE");
        XYDiagnostic.SetDebugId(_switcher.Trigger, "XYE.TOP.WORKSPACE.SELECTOR");
        _switcher.WorkspaceChangeRequested += OnWorkspaceChangeRequested;
        SwitcherHost.Children.Add(_switcher);
    }

    void OnWorkspaceChangeRequested(object? sender, XYWorkspaceChangeRequest request)
    {
        if (DataContext is not UiVm vm) { request.Reject(); return; }
        if (request.Workspace.Id == ManagementId)
        {
            if (vm.IsEditMode && !vm.ToggleEditorMode()) { request.Reject(); return; }
            if (vm.IsManageMode) request.Accept(); else request.Reject();
            return;
        }
        if (request.Workspace.Id != FeatureId) { request.Reject(); return; }
        vm.SelectFeatureWorkspaceCommand.Execute(null);
        if (vm.IsEditMode && vm.IsRegionWorkspace) request.Accept(); else request.Reject();
    }
}
