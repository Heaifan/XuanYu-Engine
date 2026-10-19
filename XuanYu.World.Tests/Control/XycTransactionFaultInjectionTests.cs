using System.Reflection;
using XuanYu.Core.Scene;
using XuanYu.Editor.UI;
using XuanYu.Editor.UI.XycControl;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.Xyc;

public sealed class XycTransactionFaultInjectionTests
{
    [Fact]
    public void Failure_before_world_commit_leaves_entity_and_history_unchanged()
    {
        var vm = NewVm();
        var created = vm.CreateXycTestCube(vm.SceneId, vm.SceneRevision);
        var id = created.SessionEntityIds![0];
        var before = vm.InspectXycTestScene();
        SetFault(vm, "XycBeforeWorldMutationForTest");
        Assert.NotNull(Record.Exception(() => vm.SetXycTestPosition(
            vm.SceneId, before.SceneRevision, id, new Vector3(8, 0, 0))));
        var after = vm.InspectXycTestScene();
        Assert.Equal(before.EntityIds, after.EntityIds);
        Assert.Equal(before.UndoCount, after.UndoCount);
        Assert.Equal(before.RedoCount, after.RedoCount);
        Assert.Equal(new Vector3(0, 0, 0), vm.InspectXycSceneEntity(id).Transform.Position);
    }

    [Fact]
    public void Observer_failure_after_world_commit_keeps_all_projections_and_history_authoritative()
    {
        var vm = NewVm();
        var state = vm.CreateXycTestCube(vm.SceneId, vm.SceneRevision);
        var id = state.SessionEntityIds![0];
        var scene = (SceneStateOwner)typeof(UiVm).GetField("_sceneState", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(vm)!;
        Action<SceneRenderSnapshot> fail = _ => throw new InvalidOperationException("observer fault");
        scene.RenderSnapshotChanged += fail;
        try { state = vm.SetXycTestPosition(vm.SceneId, state.SceneRevision, id, new Vector3(18, 0, 0)); }
        finally { scene.RenderSnapshotChanged -= fail; }
        Assert.Equal(new Vector3(18, 0, 0), vm.InspectXycSceneEntity(id).Transform.Position);
        Assert.Contains(id, state.EntityIds);
        Assert.Contains(id, state.HierarchyEntityIds);
        Assert.Contains(id, state.RenderEntityIds);
        Assert.Equal(id, state.UndoTop!.EntityId);
        Assert.Equal("COMMITTED", state.CommitStatus);
    }

    [Fact]
    public void Apply_failure_after_world_mutation_is_reported_in_doubt()
    {
        var vm = NewVm();
        var state = vm.CreateXycTestCube(vm.SceneId, vm.SceneRevision);
        var id = state.SessionEntityIds![0];
        state = vm.SetXycTestPosition(vm.SceneId, state.SceneRevision, id, new Vector3(8, 0, 0));
        var before = vm.InspectXycTestScene();
        SetFault(vm, "XycWorldAppliedBeforeHistoryCompleteForTest");
        Assert.ThrowsAny<Exception>(() => vm.UndoXycTestOperation(vm.SceneId, state.SceneRevision, id));
        var after = vm.InspectXycTestScene();
        Assert.Equal(before.UndoCount, after.UndoCount);
        Assert.Equal(before.RedoCount, after.RedoCount);
        Assert.Equal(before.UndoTop, after.UndoTop);
        Assert.Equal("IN_DOUBT", after.CommitStatus);
        Assert.Equal(new Vector3(0, 0, 0), vm.InspectXycSceneEntity(id).Transform.Position);
    }

    static void SetFault(UiVm vm, string name)
    {
        var property = typeof(UiVm).GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(property);
        property!.SetValue(vm, (Action)(() => throw new InvalidOperationException("injected " + name)));
    }

    static UiVm NewVm() => new(null, () => true, seedInitialScene: false);
}
