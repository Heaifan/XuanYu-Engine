using System.Text.Json;
using XuanYu.Editor.UI.XycControl;
using XuanYu.World.Tests.Xyc;

namespace XuanYu.World.Tests.Xyc;

public sealed class XycSceneCommandTests
{
    [Fact]
    public void Scene_reads_are_available_without_diagnostic_write()
    {
        var runtime = new SceneRuntime();
        var processor = new XycCommandProcessor(runtime, false);
        Assert.True(processor.Execute(Request("scene.entity.list", runtime)).Success);
        Assert.True(processor.Execute(Request("scene.test.inspect", runtime)).Success);
    }

    [Fact]
    public void Scene_writes_require_diagnostic_permission_and_current_revision()
    {
        var runtime = new SceneRuntime();
        var args = new { sceneId = "scene-test", expectedRevision = 7 };
        Assert.Equal("UNAUTHORIZED", new XycCommandProcessor(runtime, false)
            .Execute(Request("scene.entity.create-test-cube", runtime, args)).ErrorCode);
        Assert.Equal("STALE_REVISION", new XycCommandProcessor(runtime, true)
            .Execute(Request("scene.entity.create-test-cube", runtime, args with { expectedRevision = 6 })).ErrorCode);
    }

    [Fact]
    public void Invalid_or_unknown_scene_entity_is_rejected_structurally()
    {
        var runtime = new SceneRuntime();
        var processor = new XycCommandProcessor(runtime, false);
        Assert.Equal("INVALID_ENTITY_ID", processor.Execute(Request("scene.entity.inspect", runtime,
            new { entityId = "not-an-entity" })).ErrorCode);
    }

    static XycRequest Request(string command, SceneRuntime runtime, object? args = null) => new(
        "XYC-API/1", Guid.NewGuid().ToString(), runtime.InstanceId, command,
        JsonSerializer.SerializeToElement(args ?? new { }, new JsonSerializerOptions(JsonSerializerDefaults.Web)));

    sealed class SceneRuntime : FakeRuntime, IXycSceneRuntime
    {
        public string SceneInstanceId => InstanceId;
        public XycSceneState InspectTestScene() => new("scene-test", 7, 0, [], [], [], [], "", false, 0, 0, null, "0");
        public XycEntity[] ListSceneEntities() => [];
        public XycEntity InspectSceneEntity(string entityId) => throw new XycSceneOperationException("INVALID_ENTITY_ID");
        public XycSceneState CreateTestCube(string expectedSceneId, long expectedRevision) =>
            expectedRevision == 7 ? InspectTestScene() : throw new XycSceneOperationException("STALE_REVISION");
        public XycSceneState SetTestPosition(string sceneId, long revision, string entityId, Vector3 position) => InspectTestScene();
        public XycSceneState UndoTestOperation(string sceneId, long revision, string entityId) => InspectTestScene();
        public XycSceneState RedoTestOperation(string sceneId, long revision, string entityId) => InspectTestScene();
    }
}
