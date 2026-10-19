using System.Reflection;
using System.Text.Json;
using XuanYu.Editor.UI;
using XuanYu.Editor.UI.XycControl;

namespace XuanYu.World.Tests.Xyc;

public sealed class XycInDoubtTests
{
    [Fact]
    public void World_commit_before_history_failure_blocks_a_new_request_id()
    {
        var vm = NewVm();
        var runtime = vm.CreateXycRuntime("xyc-test");
        var processor = new XycCommandProcessor(runtime, true);
        var state = (XycSceneState)Execute(processor, runtime.InstanceId,
            "scene.entity.create-test-cube", new { sceneId = vm.SceneId, expectedRevision = vm.SceneRevision }).ObservedState!;
        var id = state.SessionEntityIds![0];
        SetFault(vm, "XycWorldCommittedBeforeHistoryAppendForTest");
        var first = Execute(processor, runtime.InstanceId, "scene.entity.set-test-position",
            new { sceneId = vm.SceneId, expectedRevision = state.SceneRevision, entityId = id, position = new Vector3(41, 0, 0) });
        Assert.False(first.Success);
        Assert.Equal("IN_DOUBT", first.ErrorCode);
        var retry = Execute(processor, runtime.InstanceId, "scene.entity.set-test-position",
            new { sceneId = vm.SceneId, expectedRevision = state.SceneRevision, entityId = id, position = new Vector3(41, 0, 0) });
        Assert.Equal("IN_DOUBT", retry.ErrorCode);
        Assert.Equal(new Vector3(41, 0, 0), vm.InspectXycSceneEntity(id).Transform.Position);
    }

    [Fact]
    public void Failure_after_world_apply_before_history_completion_is_reported_in_doubt()
    {
        var vm = NewVm();
        var state = vm.CreateXycTestCube(vm.SceneId, vm.SceneRevision);
        var id = state.SessionEntityIds![0];
        state = vm.SetXycTestPosition(vm.SceneId, state.SceneRevision, id, new Vector3(7, 0, 0));
        SetFault(vm, "XycWorldAppliedBeforeHistoryCompleteForTest");
        Assert.ThrowsAny<Exception>(() => vm.UndoXycTestOperation(vm.SceneId, state.SceneRevision, id));
        var after = vm.InspectXycTestScene();
        Assert.Equal("IN_DOUBT", after.CommitStatus);
        Assert.False(string.IsNullOrWhiteSpace(after.RecoveryReason));
    }

    static void SetFault(UiVm vm, string name)
    {
        var property = typeof(UiVm).GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(property);
        property!.SetValue(vm, (Action)(() => throw new InvalidOperationException("injected " + name)));
    }

    static XycResponse Execute(XycCommandProcessor processor, string session, string command, object args) =>
        processor.Execute(new("XYC-API/1", Guid.NewGuid().ToString(), session, command,
            JsonSerializer.SerializeToElement(args, new JsonSerializerOptions(JsonSerializerDefaults.Web))));
    static UiVm NewVm() => new(null, () => true, seedInitialScene: false);
}
