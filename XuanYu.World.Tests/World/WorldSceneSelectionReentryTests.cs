using XuanYu.Core.Gizmo;
using XuanYu.Core.History;
using XuanYu.Core.Math;
using XuanYu.Core.Transform;
using XuanYu.Editor.UI;

using XuanYu.World.Scene;
using XuanYu.Editor.Transform;
namespace XuanYu.World.Tests.World;

public sealed class WorldSceneSelectionReentryTests
{
    [Fact]
    public void Selecting_entity_one_then_two_publishes_once_per_commit()
    {
        var vm = TestVm();
        var entities = EntityNodes(vm);
        var publishes = 0;
        vm.RenderSnapshotChanged += _ => publishes++;

        vm.SelectedHierarchyItem = entities[0];
        Assert.Equal(1, publishes);
        Assert.Equal(entities[0].Key, vm.SelectionKey);

        vm.SelectedHierarchyItem = entities[1];
        Assert.Equal(2, publishes);
        Assert.Equal(entities[1].Key, vm.SelectionKey);
        Assert.Equal(10, vm.RenderSnapshot.Entities.Count);
    }

    [Fact]
    public void Selecting_same_entity_is_noop()
    {
        var vm = TestVm();
        var entity = EntityNodes(vm)[1];
        vm.SelectedHierarchyItem = entity;
        var publishes = 0;
        vm.RenderSnapshotChanged += _ => publishes++;

        vm.SelectedHierarchyItem = EntityNodes(vm)[1];

        Assert.Equal(0, publishes);
        Assert.Equal(entity.Key, vm.SelectionKey);
    }

    [Fact]
    public void Rapid_entity_switching_keeps_selection_and_active_entity_aligned()
    {
        var vm = TestVm();
        var entities = EntityNodes(vm);

        vm.SelectedHierarchyItem = entities[0];
        vm.SelectedHierarchyItem = entities[1];
        vm.SelectedHierarchyItem = entities[2];
        vm.SelectedHierarchyItem = EntityNodes(vm)[0];

        Assert.Equal(entities[0].Key, vm.SelectionKey);
        Assert.Equal(entities[0].Key, vm.RenderSnapshot.Entity.EntityKey.ToString());
        Assert.Equal(10, vm.RenderSnapshot.Entities.Count);
    }

    [Fact]
    public void Select_b_then_move_undo_redo_keeps_entity_identity()
    {
        var vm = TestVm();
        var b = EntityNodes(vm)[1];
        vm.SelectedHierarchyItem = b;
        var session = new TransformSession();
        var history = new EditorHistoryOwner();

        Assert.True(session.Begin(17, vm.RenderSnapshot.Entity, MoveGizmoAxis.X));
        session.TryPreview(17, new Vector3d(4, 0, 0));
        Assert.True(session.TryCommit(17, SceneOf(vm), out var commit));
        history.Push(new TransformHistoryEntry(commit.EntityKey, commit.Before, commit.After));
        Assert.True(history.TryUndo(out var undo));
        Assert.True(SceneOf(vm).RestoreTransform(undo.EntityKey, undo.Before));
        Assert.True(history.TryRedo(out var redo));
        Assert.True(SceneOf(vm).RestoreTransform(redo.EntityKey, redo.After));

        Assert.Equal(b.Key, redo.EntityKey.ToString());
        Assert.Equal(b.Key, vm.RenderSnapshot.Entity.EntityKey.ToString());
    }

    static UiVm TestVm() => new(null, () => true);

    static List<EditorTreeNode> EntityNodes(UiVm vm) =>
        vm.HierarchyItems.Where(item => item.Key.StartsWith("EntityId(", StringComparison.Ordinal)).ToList();

    static SceneStateOwner SceneOf(UiVm vm)
    {
        var field = typeof(UiVm).GetField("_sceneState",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        return (SceneStateOwner)field!.GetValue(vm)!;
    }
}
