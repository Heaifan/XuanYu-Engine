using XuanYu.Editor.UI;
using XuanYu.Editor.UI.XycControl;

namespace XuanYu.World.Tests.Xyc;

public sealed class XycMultiEntityHistoryTests
{
    [Fact]
    public void Undo_rejects_non_top_entity_without_moving_history()
    {
        var (vm, a, b, state) = CreateAndMoveBoth();
        var before = vm.InspectXycTestScene();
        Assert.Equal(b, before.UndoTop!.EntityId);
        var error = Assert.Throws<XycSceneOperationException>(() =>
            vm.UndoXycTestOperation(vm.SceneId, state.SceneRevision, a));
        Assert.Equal("HISTORY_CONFLICT", error.Code);
        var after = vm.InspectXycTestScene();
        Assert.Equal(before.UndoCount, after.UndoCount);
        Assert.Equal(before.RedoCount, after.RedoCount);
        Assert.Equal(before.UndoTop, after.UndoTop);
        Assert.Equal(before.EntityIds, after.EntityIds);
    }

    [Fact]
    public void Interleaved_undo_and_redo_follow_global_history_order()
    {
        var (vm, a, b, state) = CreateAndMoveBoth();
        Assert.Equal(b, state.UndoTop!.EntityId);
        state = vm.UndoXycTestOperation(vm.SceneId, state.SceneRevision, b);
        Assert.Equal(a, state.UndoTop!.EntityId);
        state = vm.UndoXycTestOperation(vm.SceneId, state.SceneRevision, a);
        Assert.Equal(new Vector3(0, 0, 0), vm.InspectXycSceneEntity(a).Transform.Position);
        Assert.Equal(new Vector3(0, 0, 0), vm.InspectXycSceneEntity(b).Transform.Position);
        state = vm.RedoXycTestOperation(vm.SceneId, state.SceneRevision, a);
        Assert.Equal(a, state.UndoTop!.EntityId);
        state = vm.RedoXycTestOperation(vm.SceneId, state.SceneRevision, b);
        Assert.Equal(new Vector3(20, 0, 0), vm.InspectXycSceneEntity(a).Transform.Position);
        Assert.Equal(new Vector3(0, 30, 0), vm.InspectXycSceneEntity(b).Transform.Position);
    }

    [Fact]
    public void New_write_after_undo_clears_redo_branch()
    {
        var (vm, a, b, state) = CreateAndMoveBoth();
        state = vm.UndoXycTestOperation(vm.SceneId, state.SceneRevision, b);
        state = vm.SetXycTestPosition(vm.SceneId, state.SceneRevision, a, new Vector3(21, 0, 0));
        Assert.Equal(0, state.RedoCount);
        Assert.Null(state.RedoTop);
    }

    static (UiVm Vm, string A, string B, XycSceneState State) CreateAndMoveBoth()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var state = vm.CreateXycTestCube(vm.SceneId, vm.SceneRevision);
        state = vm.CreateXycTestCube(vm.SceneId, state.SceneRevision);
        var ids = state.SessionEntityIds!;
        state = vm.SetXycTestPosition(vm.SceneId, state.SceneRevision, ids[0], new Vector3(20, 0, 0));
        state = vm.SetXycTestPosition(vm.SceneId, state.SceneRevision, ids[1], new Vector3(0, 30, 0));
        return (vm, ids[0], ids[1], state);
    }
}
