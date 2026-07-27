using System.Linq;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.World;

public sealed class WorldToolStateHighlightUiTests
{
    [Fact]
    public void Rotate_tool_switches_highlight_to_rotate_gizmo()
    {
        var vm = new UiVm(null, () => true);
        vm.SelectedHierarchyItem = EntityNode(vm, 1);
        vm.SelectToolCommand.Execute("移动");
        Assert.Equal("移动", vm.ActiveTool);
        Assert.True(vm.IsMoveTool);
        Assert.False(vm.IsRotateTool);
        Assert.False(vm.IsScaleTool);
        Assert.True(vm.RenderSnapshot.ShowMoveGizmo);

        vm.SelectToolCommand.Execute("旋转");
        Assert.Equal("旋转", vm.ActiveTool);
        Assert.True(vm.IsRotateTool);
        Assert.False(vm.IsMoveTool);
        Assert.False(vm.IsScaleTool);
        Assert.True(vm.RenderSnapshot.ShowRotateGizmo);
        Assert.False(vm.RenderSnapshot.ShowMoveGizmo);

        var active = new[] { vm.IsSelectTool, vm.IsMoveTool, vm.IsRotateTool, vm.IsScaleTool }.Count(b => b);
        Assert.Equal(1, active);
    }

    [Fact]
    public void Scale_tool_switches_highlight_to_scale_gizmo()
    {
        var vm = new UiVm(null, () => true);
        vm.SelectedHierarchyItem = EntityNode(vm, 1);
        vm.SelectToolCommand.Execute("移动");
        Assert.Equal("移动", vm.ActiveTool);
        Assert.True(vm.IsMoveTool);
        Assert.False(vm.IsScaleTool);
        Assert.False(vm.IsRotateTool);
        Assert.True(vm.RenderSnapshot.ShowMoveGizmo);

        vm.SelectToolCommand.Execute("缩放");
        Assert.Equal("缩放", vm.ActiveTool);
        Assert.True(vm.IsScaleTool);
        Assert.False(vm.IsMoveTool);
        Assert.False(vm.IsRotateTool);
        Assert.True(vm.RenderSnapshot.ShowScaleGizmo);
        Assert.False(vm.RenderSnapshot.ShowMoveGizmo);

        var active = new[] { vm.IsSelectTool, vm.IsMoveTool, vm.IsRotateTool, vm.IsScaleTool }.Count(b => b);
        Assert.Equal(1, active);
    }

    static EditorTreeNode EntityNode(UiVm vm, int id) =>
        vm.HierarchyItems.Single(item => item.Key == $"EntityId({id})");
}
