using System.Reflection;
using XuanYu.Core.Identity;
using XuanYu.Core.Scene;
using XuanYu.Core.World;
using XuanYu.Editor.UI;

namespace XuanYu.Core.Tests.World;

public sealed class WorldPartitionUiTests
{
    [Fact]
    public void Scene_region_migration_keeps_entity_id_and_render_identity()
    {
        var scene = new SceneStateOwner();
        scene.EnsureEntityCount(10);
        var entity = scene.Entities[4];
        scene.SetActiveEntity(entity.EntityKey, publish: false);

        Assert.True(scene.MoveEntityToRegion(entity.EntityKey, RegionKey.FromGrid(2, 1)));

        Assert.Equal(entity.EntityKey, scene.RenderSnapshot.Entity.EntityKey);
        Assert.Equal(RegionKey.FromGrid(2, 1), scene.GetRegion(entity.EntityKey));
        Assert.Contains(scene.RenderSnapshot.Entities, item => item.EntityKey == entity.EntityKey);
    }

    [Fact]
    public void Selection_and_inspector_survive_region_migration_by_key()
    {
        var vm = new UiVm(null, () => true);
        var entity = EntityNodes(vm)[4];

        vm.SelectedHierarchyItem = entity;
        SceneOf(vm).MoveEntityToRegion(EntityId.FromInt(5), RegionKey.FromGrid(3, 2));

        Assert.Equal(entity.Key, vm.SelectedNodeKey);
        Assert.Equal(entity.Key, vm.SelectionKey);
        Assert.Equal(entity.Key, vm.SelectedHierarchyItem!.Key);
        Assert.Contains("Region(3,2,0)", vm.SelectionPath);
        Assert.Contains(vm.InspectorFields, item => item.Contains("Region(3,2,0)"));
        Assert.Equal(entity.Key, vm.RenderSnapshot.Entity.EntityKey.ToString());
    }

    [Fact]
    public void Hierarchy_reuses_entity_node_identity_after_region_projection_refresh()
    {
        var vm = new UiVm(null, () => true);
        var before = EntityNodes(vm)[0];

        SceneOf(vm).MoveEntityToRegion(EntityId.FromInt(1), RegionKey.FromGrid(4, 0));
        var after = EntityNodes(vm).Single(item => item.Key == before.Key);

        Assert.Same(before, after);
        Assert.Contains("Region(4,0,0)", after.Path);
        Assert.Contains(vm.HierarchyItems, item => item.Key == "Region(4,0,0)" && item.IsRegion);
    }

    static List<EditorTreeNode> EntityNodes(UiVm vm) =>
        vm.HierarchyItems.Where(item => item.Key.StartsWith("EntityId(", StringComparison.Ordinal)).ToList();

    static SceneStateOwner SceneOf(UiVm vm)
    {
        var field = typeof(UiVm).GetField("_sceneState", BindingFlags.Instance | BindingFlags.NonPublic);
        return (SceneStateOwner)field!.GetValue(vm)!;
    }
}
