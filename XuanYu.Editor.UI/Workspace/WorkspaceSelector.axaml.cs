using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class WorkspaceSelector : UserControl
{
    const string FeatureId = "feature-editing";
    readonly XYWorkspaceSwitcher _switcher;

    public WorkspaceSelector()
    {
        InitializeComponent();
        _switcher = new(new XYWorkspaceState(FeatureId),
            new XYWorkspaceItem(FeatureId, "要素编辑"),
            new XYWorkspaceItem("scene-editing", "场景编辑（暂未开放）", false),
            new XYWorkspaceItem("debug", "调试（暂未开放）", false));
        _switcher.WorkspaceChangeRequested += OnWorkspaceChangeRequested;
        SwitcherHost.Children.Add(_switcher);
    }

    void OnWorkspaceChangeRequested(object? sender, XYWorkspaceChangeRequest request)
    {
        if (request.Workspace.Id != FeatureId || DataContext is not UiVm vm) { request.Reject(); return; }
        vm.SelectFeatureWorkspaceCommand.Execute(null);
        request.Accept();
    }
}
