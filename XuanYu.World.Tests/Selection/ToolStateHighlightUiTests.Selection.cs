using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.World;

public sealed partial class ToolStateHighlightUiTests
{
    [Fact]
    public void Selection_tools_do_not_show_transform_gizmos()
    {
        var vm = new UiVm(null, () => true);
        Assert.False(vm.RenderSnapshot.ShowMoveGizmo);
        Assert.False(vm.RenderSnapshot.ShowRotateGizmo);
        Assert.False(vm.RenderSnapshot.ShowScaleGizmo);

        vm.SelectedHierarchyItem = EntityNode(vm, 1);
        vm.SelectToolCommand.Execute("选择");
        Assert.False(vm.RenderSnapshot.ShowMoveGizmo);
        Assert.False(vm.RenderSnapshot.ShowRotateGizmo);
        Assert.False(vm.RenderSnapshot.ShowScaleGizmo);

        vm.SelectToolCommand.Execute("框选");
        Assert.False(vm.RenderSnapshot.ShowMoveGizmo);
        Assert.False(vm.RenderSnapshot.ShowRotateGizmo);
        Assert.False(vm.RenderSnapshot.ShowScaleGizmo);
    }
}
