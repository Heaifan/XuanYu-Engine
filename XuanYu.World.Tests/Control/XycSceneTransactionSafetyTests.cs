using System.Reflection;
using XuanYu.Core.Identity;
using XuanYu.Core.Scene;
using XuanYu.Editor.UI;
using XuanYu.World;
using XuanYu.World.Scene;
namespace XuanYu.World.Tests.Xyc;
public sealed class XycSceneTransactionSafetyTests
{
    [Fact]
    public void Snapshot_observer_failure_after_world_commit_completes_history_and_stage()
    {
        var vm = NewVm();
        var created = vm.CreateXycTestCube(vm.SceneId, vm.SceneRevision);
        var scene = Scene(vm);
        Action<SceneRenderSnapshot> failAfterCommit = _ => throw new InvalidOperationException("injected observer failure");
        scene.RenderSnapshotChanged += failAfterCommit;
        try
        {
            var after = vm.SetXycTestPosition(vm.SceneId, vm.SceneRevision,
                created.TestEntityId!, new XuanYu.Editor.UI.XycControl.Vector3(17, 3, 0));
            Assert.Equal("2", after.Stage);
            Assert.Equal(2, after.UndoCount);
            Assert.Equal(0, after.RedoCount);
            Assert.False(after.HasActiveInteraction);
            Assert.Equal(new XuanYu.Core.Math.Vector3d(17, 3, 0),
                scene.Entities.Single(x => x.EntityKey == EntityId.FromInt(1)).Transform.Position);
        }
        finally { scene.RenderSnapshotChanged -= failAfterCommit; }
    }
    [Fact]
    public void Wrong_scene_id_and_invalid_redo_leave_world_and_history_unchanged()
    {
        var vm = NewVm();
        var created = vm.CreateXycTestCube(vm.SceneId, vm.SceneRevision);
        var before = vm.InspectXycTestScene();
        var wrongScene = Assert.Throws<XuanYu.Editor.UI.XycControl.XycSceneOperationException>(
            () => vm.CreateXycTestCube("wrong-scene", vm.SceneRevision));
        Assert.Equal("SCENE_MISMATCH", wrongScene.Code);
        var invalidRedo = Assert.Throws<XuanYu.Editor.UI.XycControl.XycSceneOperationException>(
            () => vm.RedoXycTestOperation(vm.SceneId, vm.SceneRevision, created.TestEntityId!));
        Assert.Equal("INVALID_STAGE", invalidRedo.Code);
        AssertSameState(before, vm.InspectXycTestScene());
        Assert.Single(Scene(vm).Entities);
    }
    [Fact]
    public void Active_transform_interaction_rejects_scene_write_without_mutation()
    {
        var vm = NewVm();
        var created = vm.CreateXycTestCube(vm.SceneId, vm.SceneRevision);
        vm.SelectToolCommand.Execute("移动");
        Assert.True(vm.BeginViewportPointer(17, 0, 0, true, true));
        var before = vm.InspectXycTestScene();
        var error = Assert.Throws<XuanYu.Editor.UI.XycControl.XycSceneOperationException>(
            () => vm.SetXycTestPosition(vm.SceneId, vm.SceneRevision, created.TestEntityId!,
                new XuanYu.Editor.UI.XycControl.Vector3(8, 0, 0)));
        Assert.Equal("HISTORY_CONFLICT", error.Code);
        AssertSameState(before, vm.InspectXycTestScene());
        vm.InteractionCommand.Execute("Cancel");
    }
    [Fact]
    public void Active_drawing_transaction_rejects_scene_write_without_mutation()
    {
        var vm = NewVm();
        var created = vm.CreateXycTestCube(vm.SceneId, vm.SceneRevision);
        typeof(UiVm).GetMethod("BeginDrawingTransaction", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(vm, ["区域面"]);
        var before = vm.InspectXycTestScene();
        Assert.True(vm.IsDrawingTransactionActive);
        var error = Assert.Throws<XuanYu.Editor.UI.XycControl.XycSceneOperationException>(
            () => vm.SetXycTestPosition(vm.SceneId, vm.SceneRevision, created.TestEntityId!,
                new XuanYu.Editor.UI.XycControl.Vector3(8, 0, 0)));
        Assert.Equal("HISTORY_CONFLICT", error.Code);
        AssertSameState(before, vm.InspectXycTestScene());
        typeof(UiVm).GetMethod("EndDrawingTransaction", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(vm, null);
    }
    static void AssertSameState(XuanYu.Editor.UI.XycControl.XycSceneState expected, XuanYu.Editor.UI.XycControl.XycSceneState actual)
    {
        Assert.Equal(expected.SceneId, actual.SceneId);
        Assert.Equal(expected.SceneRevision, actual.SceneRevision);
        Assert.Equal(expected.EntityCount, actual.EntityCount);
        Assert.Equal(expected.EntityIds, actual.EntityIds);
        Assert.Equal(expected.HierarchyEntityIds, actual.HierarchyEntityIds);
        Assert.Equal(expected.RenderEntityIds, actual.RenderEntityIds);
        Assert.Equal(expected.UndoCount, actual.UndoCount);
        Assert.Equal(expected.RedoCount, actual.RedoCount);
        Assert.Equal(expected.Stage, actual.Stage);
        Assert.Equal(expected.HasActiveInteraction, actual.HasActiveInteraction);
    }
    static UiVm NewVm() => new(null, () => true, seedInitialScene: false);
    static SceneStateOwner Scene(UiVm vm) => (SceneStateOwner)typeof(UiVm)
        .GetField("_sceneState", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(vm)!;
}
