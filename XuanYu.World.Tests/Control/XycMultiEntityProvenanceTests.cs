using System.Reflection;
using XuanYu.Core.History;
using XuanYu.Core.Identity;
using XuanYu.Core.Scene;
using XuanYu.Editor.UI;
using XuanYu.Editor.UI.XycControl;

namespace XuanYu.World.Tests.Xyc;

public sealed class XycMultiEntityProvenanceTests
{
    [Fact]
    public void Undo_rejects_unowned_editor_history_item_even_for_session_entity()
    {
        var (vm, id, state) = CreateAndMove();
        var key = EntityId.FromInt(int.Parse(id[9..^1]));
        var history = (EditorHistoryOwner)typeof(UiVm)
            .GetField("_historyOwner", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(vm)!;
        var current = new CommittedTransform(new XuanYu.Core.Math.Vector3d(20, 0, 0));
        history.PushEntry(new TransformHistoryEntry(key, CommittedTransform.Identity, current));
        state = vm.InspectXycTestScene();
        var before = state;

        var error = Assert.Throws<XycSceneOperationException>(() =>
            vm.UndoXycTestOperation(vm.SceneId, state.SceneRevision, id));

        Assert.Equal("HISTORY_CONFLICT", error.Code);
        var after = vm.InspectXycTestScene();
        Assert.Equal(before.UndoCount, after.UndoCount);
        Assert.Equal(before.UndoTop, after.UndoTop);
        Assert.Equal(new Vector3(20, 0, 0), vm.InspectXycSceneEntity(id).Transform.Position);
    }

    [Fact]
    public void Session_rejects_entity_access_after_document_scene_changes()
    {
        var (vm, id, _) = CreateAndMove();
        var session = typeof(UiVm).GetField("_documentSession",
            BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(vm)!;
        session.GetType().GetMethod("MarkNew")!.Invoke(session, null);
        var state = vm.InspectXycTestScene();

        var error = Assert.Throws<XycSceneOperationException>(() =>
            vm.SetXycTestPosition(vm.SceneId, state.SceneRevision, id, new Vector3(21, 0, 0)));

        Assert.Equal("SCENE_MISMATCH", error.Code);
        Assert.Equal(new Vector3(20, 0, 0), vm.InspectXycSceneEntity(id).Transform.Position);
    }

    static (UiVm Vm, string Id, XycSceneState State) CreateAndMove()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var state = vm.CreateXycTestCube(vm.SceneId, vm.SceneRevision);
        var id = state.SessionEntityIds![0];
        state = vm.SetXycTestPosition(vm.SceneId, state.SceneRevision, id, new Vector3(20, 0, 0));
        return (vm, id, state);
    }
}
