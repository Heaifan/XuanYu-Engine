using XuanYu.Editor.UI;
using XuanYu.Editor;
using XuanYu.Core.Picking;
using XuanYu.Core.Spatial;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.World;

public sealed class SelectionContractTests
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
            Assert.Contains(vm.InspectorFields, item => item.Value.Contains(DisplayEntity(entity.Key)));
            Assert.Equal(10, vm.RenderSnapshot.Entities.Count);
        }

        vm.SelectedHierarchyItem = entities[0];

        Assert.Equal(11, publishes);
        Assert.Equal(entities[0].Key, vm.SelectionKey);
    }

    [Fact]
    public void Viewport_miss_is_routed_as_a_selection_clear_request()
    {
        var coordinator = new SelectionCoordinator(
            new EditorStateOwner(() => true), new SceneStateOwner(), new ProjectTreePresenter());

        var miss = new ViewportPickingResult(1, 1, 0,
            new SpatialRaycastResult(null, new(0, 0, 0, 0, 0, 0)));
        var request = coordinator.ResolveViewportSelection(miss);

        Assert.Equal("视口空白", request.Source);
        Assert.Null(request.Node);
    }

    static List<EditorTreeNode> EntityNodes(UiVm vm)
    {
        var nodes = vm.HierarchyItems
            .Where(item => item.Key.StartsWith("EntityId(", StringComparison.Ordinal))
            .ToList();
        Assert.Equal(10, nodes.Count);
        return nodes;
    }

    static string DisplayEntity(string key) =>
        key.Replace("EntityId", "实体编号", StringComparison.Ordinal);
}
