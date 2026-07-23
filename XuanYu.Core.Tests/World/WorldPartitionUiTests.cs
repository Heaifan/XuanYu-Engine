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

        scene.CommitPositionWithResult(entity.EntityKey, new XuanYu.Core.Math.Vector3d(2001, 1001, 0));

        Assert.Equal(entity.EntityKey, scene.RenderSnapshot.Entity.EntityKey);
        Assert.Equal(RegionKey.FromGrid(2, 1), scene.GetRegion(entity.EntityKey));
        Assert.Contains(scene.RenderSnapshot.Entities, item => item.EntityKey == entity.EntityKey);
    }

    [Fact]
    public void Selection_and_inspector_survive_region_migration_by_key()
    {
        var vm = new UiVm(null, () => true);
        var entity = EntityNodes(vm).Single(item => item.Key == "EntityId(5)");

        vm.SelectedHierarchyItem = entity;
        SceneOf(vm).CommitPositionWithResult(EntityId.FromInt(5), new XuanYu.Core.Math.Vector3d(16, 11, 0));

        Assert.Equal(entity.Key, vm.SelectedNodeKey);
        Assert.Equal(entity.Key, vm.SelectionKey);
        Assert.Equal(entity.Key, vm.SelectedHierarchyItem!.Key);
        Assert.Contains("区域 3,2,0", vm.SelectionPath);
        Assert.Contains(vm.InspectorFields, item => item.Contains("区域 3,2,0"));
        Assert.Equal(entity.Key, vm.RenderSnapshot.Entity.EntityKey.ToString());
    }

    [Fact]
    public void Hierarchy_reuses_entity_node_identity_after_region_projection_refresh()
    {
        var vm = new UiVm(null, () => true);
        var before = EntityNodes(vm)[0];

        SceneOf(vm).CommitPositionWithResult(EntityId.FromInt(1), new XuanYu.Core.Math.Vector3d(21, 0, 0));
        var after = EntityNodes(vm).Single(item => item.Key == before.Key);

        Assert.Same(before, after);
        Assert.Contains("区域 4,0,0", after.Path);
        Assert.Contains(vm.HierarchyItems, item => item.Key == "Region(4,0,0)" && item.IsRegion);
    }

    [Fact]
    public void Destroy_removes_entity_node_and_prunes_hierarchy_cache()
    {
        var vm = new UiVm(null, () => true);
        var key = EntityNodes(vm)[0].Key;

        Assert.True(SceneOf(vm).DestroyEntity(EntityId.FromInt(1)));
        _ = vm.HierarchyItems;

        Assert.DoesNotContain(vm.HierarchyItems, item => item.Key == key);
        Assert.False(HierarchyCacheOf(vm).ContainsKey(key));
    }

    static List<EditorTreeNode> EntityNodes(UiVm vm) =>
        vm.HierarchyItems.Where(item => item.Key.StartsWith("EntityId(", StringComparison.Ordinal)).ToList();

    static SceneStateOwner SceneOf(UiVm vm)
    {
        var field = typeof(UiVm).GetField("_sceneState", BindingFlags.Instance | BindingFlags.NonPublic);
        return (SceneStateOwner)field!.GetValue(vm)!;
    }

    static Dictionary<string, EditorTreeNode> HierarchyCacheOf(UiVm vm)
    {
        var field = typeof(UiVm).GetField("_hierarchyNodeCache", BindingFlags.Instance | BindingFlags.NonPublic);
        return (Dictionary<string, EditorTreeNode>)field!.GetValue(vm)!;
    }
}
