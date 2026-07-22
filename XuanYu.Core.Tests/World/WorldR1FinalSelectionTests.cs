using XuanYu.Editor.UI;

namespace XuanYu.Core.Tests.World;

public sealed class WorldR1FinalSelectionTests
{
    [Fact]
    public void Entity_one_to_ten_selection_cycle_stays_single_source()
    {
        var vm = new UiVm(null, () => true);
        var entities = EntityNodes(vm);
        var publishes = 0;
        vm.RenderSnapshotChanged += _ => publishes++;

        foreach (var entity in entities)
        {
            vm.SelectedHierarchyItem = entity;
            Assert.Equal(entity.Key, vm.SelectionKey);
            Assert.Equal(entity.Key, vm.RenderSnapshot.Entity.EntityKey.ToString());
            Assert.Equal(entity.Title, vm.SelectionTitle);
            Assert.Contains(vm.InspectorFields, item => item.Contains(entity.Key));
            Assert.Equal(10, vm.RenderSnapshot.Entities.Count);
        }

        vm.SelectedHierarchyItem = entities[0];

        Assert.Equal(11, publishes);
        Assert.Equal(entities[0].Key, vm.SelectionKey);
    }

    static List<EditorTreeNode> EntityNodes(UiVm vm)
    {
        var nodes = vm.HierarchyItems
            .Where(item => item.Key.StartsWith("EntityId(", StringComparison.Ordinal))
            .ToList();
        Assert.Equal(10, nodes.Count);
        return nodes;
    }
}
