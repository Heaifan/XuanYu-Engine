using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class InspectorEntityEditTargetTests
{
    [Fact]
    public void Entity_name_commit_uses_target_captured_before_selection_switch()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: true);
        var first = vm.HierarchyItems.First(item => item.IsEntity);
        var second = vm.HierarchyItems.Last(item => item.IsEntity);
        vm.SelectedHierarchyItem = first;
        vm.BeginInspectorEntityNameEdit();
        vm.SelectedHierarchyItem = second;
        vm.InspectorEntityNameText = "第一个实体";
        Assert.True(vm.CommitInspectorEntityName());
        Assert.Equal("第一个实体", vm.HierarchyItems.Single(item => item.Key == first.Key).Title);
        Assert.Equal(second.Title, vm.HierarchyItems.Single(item => item.Key == second.Key).Title);
    }
}
