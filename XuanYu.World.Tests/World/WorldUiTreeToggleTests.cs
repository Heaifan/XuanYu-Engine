namespace XuanYu.World.Tests.World;

using System.Reflection;
using XuanYu.Editor.UI;

public sealed class WorldUiTreeToggleTests
{
    static HashSet<string> CollapsedProjectKeys(UiVm vm) =>
        (HashSet<string>)typeof(UiVm).GetField("_collapsedProjectKeys", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(vm)!;

    static HashSet<string> CollapsedHierarchyKeys(UiVm vm) =>
        (HashSet<string>)typeof(UiVm).GetField("_collapsedHierarchyKeys", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(vm)!;

    [Fact]
    public void ToggleProjectNode_twice_restores_original_visibility()
    {
        var vm = new UiVm(null, () => true);
        var root = vm.ProjectItems[0];

        Assert.True(root.CanToggle);
        var before = vm.ProjectItems.Count;

        vm.ToggleProjectNode(root);
        Assert.Single(vm.ProjectItems);
        Assert.Contains(root.Key, CollapsedProjectKeys(vm));

        vm.ToggleProjectNode(root);
        Assert.Empty(CollapsedProjectKeys(vm));
        Assert.Equal(before, vm.ProjectItems.Count);
    }

    [Fact]
    public void Re_selecting_same_collapsed_project_node_can_expand_it()
    {
        var vm = new UiVm(null, () => true);
        var root = vm.ProjectItems[0];

        vm.ToggleProjectNode(root);
        Assert.Single(vm.ProjectItems);

        vm.SelectedProjectItem = root;
        Assert.True(vm.ProjectItems.Count > 1);
    }

    [Fact]
    public void ToggleHierarchyNode_twice_restores_original_visibility()
    {
        var vm = new UiVm(null, () => true);
        var root = vm.HierarchyItems[0];

        Assert.True(root.CanToggle);
        var before = vm.HierarchyItems.Count;

        vm.ToggleHierarchyNode(root);
        Assert.True(vm.HierarchyItems.Count < before);
        Assert.Contains(root.Key, CollapsedHierarchyKeys(vm));

        vm.ToggleHierarchyNode(root);
        Assert.Empty(CollapsedHierarchyKeys(vm));
        Assert.Equal(before, vm.HierarchyItems.Count);
    }
}
