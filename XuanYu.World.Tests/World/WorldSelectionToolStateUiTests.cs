using System.Reflection;
using XuanYu.Core.Identity;
using XuanYu.Editor.UI;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.World;

public sealed class WorldSelectionToolStateUiTests
{
    [Fact]
    public void Hierarchy_selection_updates_single_inspector_entity()
    {
        var vm = new UiVm(null, () => true);
        var first = EntityNode(vm, 1);
        var second = EntityNode(vm, 2);

        vm.SelectedHierarchyItem = first;
        Assert.Contains("实体编号：实体编号(1)", vm.InspectorFields);
        vm.SelectedHierarchyItem = second;

        Assert.Equal(second.Key, vm.SelectionKey);
        Assert.Equal(second.Key, vm.SelectedHierarchyItem!.Key);
        Assert.Contains("实体编号：实体编号(2)", vm.InspectorFields);
        Assert.DoesNotContain("实体编号：实体编号(1)", vm.InspectorFields);
    }

    [Fact]
    public void Clearing_selection_clears_hierarchy_inspector_and_gizmo()
    {
        var vm = new UiVm(null, () => true);
        vm.SelectedHierarchyItem = EntityNode(vm, 1);
        vm.SelectToolCommand.Execute("移动");

        Assert.True(vm.RenderSnapshot.ShowMoveGizmo);
        vm.SelectedHierarchyItem = null;

        Assert.False(vm.HasSelection);
        Assert.Null(vm.SelectedHierarchyItem);
        Assert.False(vm.RenderSnapshot.ShowMoveGizmo);
        Assert.Contains("名称：玄域示例项目", vm.InspectorFields);
        Assert.Equal("移动", vm.ActiveTool);
    }

    [Fact]
    public void Destroying_selected_entity_clears_invalid_selection()
    {
        var vm = new UiVm(null, () => true);
        vm.SelectedHierarchyItem = EntityNode(vm, 1);

        Assert.True(SceneOf(vm).DestroyEntity(EntityId.FromInt(1)));

        Assert.False(vm.HasSelection);
        Assert.NotEqual("EntityId(1)", vm.SelectionKey);
        Assert.Null(vm.SelectedHierarchyItem);
        Assert.False(vm.RenderSnapshot.ShowMoveGizmo);
    }

    [Fact]
    public void Unimplemented_tools_do_not_enter_fake_active_state()
    {
        var vm = new UiVm(null, () => true);

        vm.SelectToolCommand.Execute("旋转");
        Assert.Equal("选择", vm.ActiveTool);
        vm.SelectToolCommand.Execute("缩放");
        Assert.Equal("选择", vm.ActiveTool);
        vm.SelectToolCommand.Execute("框选");
        Assert.Equal("选择", vm.ActiveTool);
    }

    [Fact]
    public void Move_capture_blocks_tool_switch_camera_and_picking()
    {
        var vm = new UiVm(null, () => true);
        vm.SelectedHierarchyItem = EntityNode(vm, 1);
        vm.SelectToolCommand.Execute("移动");
        vm.InteractionCommand.Execute("Begin");

        vm.SelectToolCommand.Execute("选择");
        Assert.Equal("移动", vm.ActiveTool);
        Assert.False(vm.BeginCameraNavigation(7, 100, 100, false, 800, 600));
        Assert.False(vm.PickViewportPointer(100, 100, 800, 600, 800, 600, 1, 1, true));
        Assert.Equal("EntityId(1)", vm.SelectionKey);
    }

    static EditorTreeNode EntityNode(UiVm vm, int id) =>
        vm.HierarchyItems.Single(item => item.Key == $"EntityId({id})");

    static SceneStateOwner SceneOf(UiVm vm)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        return (SceneStateOwner)typeof(UiVm).GetField("_sceneState", flags)!.GetValue(vm)!;
    }
}
