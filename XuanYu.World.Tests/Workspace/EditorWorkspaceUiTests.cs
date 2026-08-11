using System.Reflection;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;

namespace XuanYu.World.Tests.Workspace;

// EDITOR-A-R2：UiVm 是唯一 UI 桥接点；切换只改变 Workspace 上下文。
public sealed class EditorWorkspaceUiTests
{
    [Fact]
    public void Default_ui_workspace_is_map_editor()
    {
        var vm = Create();
        Assert.Equal(EditorWorkspaceId.MapEditor, vm.CurrentWorkspace.Id);
        Assert.Equal("地图编辑", vm.CurrentWorkspaceDisplayName);
        Assert.True(vm.IsMapWorkspace); Assert.False(vm.IsRegionWorkspace);
    }

    [Fact]
    public void Map_to_region_and_back_changes_only_workspace_context()
    {
        var vm = Create();
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        Assert.Equal(EditorWorkspaceId.RegionEditor, vm.CurrentWorkspace.Id);
        Assert.Equal("区域编辑", vm.CurrentWorkspaceDisplayName);
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.MapEditor);
        Assert.True(vm.IsMapWorkspace); Assert.False(vm.IsRegionWorkspace);
    }

    [Fact]
    public void Repeating_current_workspace_is_a_no_op()
    {
        var vm = Create();
        vm.SelectToolCommand.Execute("移动");
        var camera = vm.NavigationCamera; var logs = vm.LogItems.Count;
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.MapEditor);
        Assert.Equal("移动", vm.ActiveTool); Assert.Equal(camera, vm.NavigationCamera);
        Assert.Equal(logs, vm.LogItems.Count);
    }

    [Fact]
    public void Changed_workspace_switch_returns_to_select_tool()
    {
        var vm = Create();
        vm.SelectToolCommand.Execute("移动");
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        Assert.Equal("选择", vm.ActiveTool); Assert.True(vm.IsSelectTool);
    }

    [Fact]
    public void Camera_map_session_and_world_owner_survive_round_trip()
    {
        var vm = Create(); vm.UpdateViewportFrame(1600, 900);
        vm.RunCommand.Execute("视角-前");
        var camera = vm.NavigationCamera; var map = vm.MapSession; var world = WorldOwner(vm);
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.MapEditor);
        Assert.Equal(camera, vm.NavigationCamera); Assert.Same(map, vm.MapSession);
        Assert.Same(world, WorldOwner(vm));
    }

    [Fact]
    public void Selection_survives_workspace_round_trip()
    {
        var vm = Create();
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(item => item.Key == "EntityId(5)");
        var selection = vm.SelectionKey;
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.MapEditor);
        Assert.Equal(selection, vm.SelectionKey); Assert.True(vm.HasSelection);
    }

    [Fact]
    public void Region_workspace_does_not_activate_region_drawing_or_draft()
    {
        var vm = Create();
        vm.SelectToolCommand.Execute("区域绘制");
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        Assert.False(vm.IsRegionDrawingTool); Assert.False(vm.IsRegionDrawingDraftActive);
        Assert.Equal(0, vm.RegionDrawingDraftVertexCount);
    }

    static UiVm Create() => new(null, () => true);
    static object WorldOwner(UiVm vm) => typeof(UiVm).GetField("_sceneState",
        BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(vm)!;
}
