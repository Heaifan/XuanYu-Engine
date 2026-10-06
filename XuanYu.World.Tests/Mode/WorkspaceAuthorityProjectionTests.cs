using Avalonia.VisualTree;
using Avalonia.Controls;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Tests.UiRuntime;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.Mode;

[Collection("UiRuntime")]
public sealed class WorkspaceAuthorityProjectionTests
{
    readonly UiHeadlessFixture _fixture;
    public WorkspaceAuthorityProjectionTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Tab_mode_change_projects_to_workspace_switcher()
    {
        RunUi((vm, switcher, context, footer) =>
        {
            Assert.True(vm.ToggleEditorMode());
            Assert.Equal(EditorWorkspaceId.MapEditor, vm.CurrentWorkspace.Id);
            Assert.Equal("要素编辑", switcher.CurrentWorkspace);
            Assert.Equal("feature-editing", switcher.SelectedWorkspaceId);
            Assert.Contains("地图编辑", vm.FooterMode);
            Assert.Contains("地图编辑", footer.Text);
            Assert.True(ContextRoot(context).IsVisible);
        });
    }

    [Fact]
    public void Dropdown_projects_accepted_mode_and_workspace_transitions()
    {
        RunUi((vm, switcher, context, footer) =>
        {
            switcher.SelectWorkspace("feature-editing");
            Assert.True(vm.IsRegionEditMode);
            Assert.Equal("要素编辑", switcher.CurrentWorkspace);
            Assert.Contains("要素编辑", vm.FooterMode);
            Assert.Contains("要素编辑", footer.Text);
            Assert.True(ContextRoot(context).IsVisible);

            switcher.SelectWorkspace("management");
            Assert.True(vm.IsManageMode);
            Assert.True(vm.IsRegionWorkspace);
            Assert.Equal("管理模式", switcher.CurrentWorkspace);
            Assert.Contains("编辑目标：要素编辑", vm.FooterMode);
            Assert.Contains("编辑目标：要素编辑", footer.Text);
            Assert.False(ContextRoot(context).IsVisible);
        });
    }

    [Fact]
    public void Workspace_command_projects_without_changing_mode_or_repeating_no_op()
    {
        RunUi((vm, switcher, context, footer) =>
        {
            Assert.True(vm.ToggleEditorMode());
            vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
            var logCount = vm.LogItems.Count;
            vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);

            Assert.True(vm.IsEditMode);
            Assert.True(vm.IsRegionWorkspace);
            Assert.Equal("feature-editing", switcher.SelectedWorkspaceId);
            Assert.Contains("要素编辑", vm.FooterMode);
            Assert.Contains("要素编辑", footer.Text);
            Assert.True(ContextRoot(context).IsVisible);
            Assert.Equal(logCount, vm.LogItems.Count);
        });
    }

    static Border ContextRoot(ContextToolBar context) => context.GetVisualDescendants()
        .OfType<Border>().Single(x => x.Name == "ContextRoot");

    void RunUi(Action<UiVm, XYWorkspaceSwitcher, ContextToolBar, XYBadge> action)
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var selector = new WorkspaceSelector { DataContext = vm };
            var context = new ContextToolBar { DataContext = vm };
            var status = new RuntimeStatusModule { DataContext = vm };
            var root = new StackPanel(); root.Children.Add(selector); root.Children.Add(context); root.Children.Add(status);
            host.Show(root, 640, 120);
            var footer = status.GetVisualDescendants().OfType<XYBadge>().Last();
            action(vm, selector.GetVisualDescendants().OfType<XYWorkspaceSwitcher>().Single(), context, footer);
        });
    }
}
