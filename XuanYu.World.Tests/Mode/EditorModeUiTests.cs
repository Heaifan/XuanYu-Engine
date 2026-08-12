using System.Reflection;
using XuanYu.Editor.Mode;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Tests;

namespace XuanYu.World.Tests.Mode;

public sealed class EditorModeUiTests
{
    [Fact]
    public void Default_ui_starts_manage_with_map_target()
    {
        var vm = Create();
        Assert.True(vm.IsManageMode); Assert.Equal(EditorWorkspaceId.MapEditor, vm.CurrentWorkspace.Id);
        Assert.Equal("管理模式", vm.CurrentEditorModeText);
    }

    [Fact]
    public void Tab_contract_enters_and_leaves_map_edit()
    {
        var vm = Create();
        Assert.True(vm.ToggleEditorMode()); Assert.True(vm.IsMapEditMode);
        Assert.Equal("地图编辑", vm.CurrentEditorModeText);
        Assert.True(vm.ToggleEditorMode()); Assert.True(vm.IsManageMode);
    }

    [Fact]
    public void Region_target_enters_region_edit()
    {
        var vm = Create(); vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        vm.ToggleEditorMode();
        Assert.True(vm.IsRegionEditMode); Assert.Equal("区域编辑", vm.CurrentEditorModeText);
        Assert.Equal(3, vm.LeftTabIndex);
    }

    [Fact]
    public void Escape_cancels_operation_without_leaving_edit_mode()
    {
        var vm = Create(); vm.ToggleEditorMode(); vm.SelectToolCommand.Execute("移动");
        vm.CancelInteractionFromEscape();
        Assert.True(vm.IsEditMode); Assert.Equal(EditorModeId.Edit, vm.CurrentMode);
    }

    [Fact]
    public void Entering_edit_resets_tool_to_select()
    {
        var vm = Create(); vm.SelectToolCommand.Execute("移动"); vm.ToggleEditorMode();
        Assert.True(vm.IsSelectTool); Assert.Equal("选择", vm.ActiveTool);
    }

    [Fact]
    public void Mode_round_trip_preserves_camera_selection_map_and_world()
    {
        var vm = Create(); vm.UpdateViewportFrame(1600, 900); vm.RunCommand.Execute("视角-前");
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(item => item.Key == "EntityId(5)");
        var camera = vm.NavigationCamera; var map = vm.MapSession; var world = World(vm); var selection = vm.SelectionKey;
        vm.ToggleEditorMode(); vm.ToggleEditorMode();
        Assert.Equal(camera, vm.NavigationCamera); Assert.Same(map, vm.MapSession); Assert.Same(world, World(vm));
        Assert.Equal(selection, vm.SelectionKey);
    }

    [Fact]
    public void Region_edit_cancels_region_drawing_and_draft()
    {
        var vm = RegionDrawingTestVm.Create(); vm.SelectToolCommand.Execute("区域绘制");
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.MapEditor);
        Assert.False(vm.IsRegionDrawingTool); Assert.False(vm.IsRegionDrawingDraftActive);
    }

    [Fact]
    public void Edit_workspace_switch_stays_edit_and_same_target_is_no_op()
    {
        var vm = Create(); vm.ToggleEditorMode();
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        Assert.True(vm.IsRegionEditMode); Assert.True(vm.IsSelectTool);
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        Assert.True(vm.IsRegionEditMode); Assert.Equal("区域编辑", vm.CurrentEditorModeText);
    }

    static UiVm Create() => new(null, () => true);
    static object World(UiVm vm) => typeof(UiVm).GetField("_sceneState",
        BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(vm)!;
}
