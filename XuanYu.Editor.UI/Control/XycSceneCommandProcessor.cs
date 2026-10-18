using System.Text.Json;

namespace XuanYu.Editor.UI.XycControl;

static class XycSceneCommandProcessor
{
    static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public static bool CanHandle(string command) =>
        command.StartsWith("scene.entity.", StringComparison.Ordinal) ||
        command.StartsWith("scene.history.", StringComparison.Ordinal) ||
        command == "scene.test.inspect";

    public static XycResponse Execute(XycRequest request, IXycRuntime runtime,
        bool diagnosticWrite, Func<bool, string?, object?, long?, long?, XycResponse> result)
    {
        if (runtime is not IXycSceneRuntime scene || scene.SceneInstanceId != runtime.InstanceId)
            return result(false, "NOT_SUPPORTED", null, null, null);
        if (request.Command is "scene.entity.list")
            return result(true, null, scene.ListSceneEntities(), runtime.InspectScene().SceneRevision, null);
        if (request.Command is "scene.test.inspect")
            return result(true, null, scene.InspectTestScene(), runtime.InspectScene().SceneRevision, null);
        if (request.Command is "scene.entity.inspect")
        {
            var read = Args(request);
            if (string.IsNullOrWhiteSpace(read.EntityId)) return result(false, "INVALID_INPUT", null, null, null);
            try { return result(true, null, scene.InspectSceneEntity(read.EntityId), runtime.InspectScene().SceneRevision, null); }
            catch (XycSceneOperationException error) { return result(false, error.Code, null, null, null); }
        }
        if (!diagnosticWrite) return result(false, "UNAUTHORIZED", null, null, null);
        var args = Args(request);
        if (string.IsNullOrWhiteSpace(args.SceneId) || args.ExpectedRevision is null)
            return result(false, "INVALID_INPUT", null, null, null);
        try
        {
            var state = request.Command switch
            {
                "scene.entity.create-test-cube" => scene.CreateTestCube(args.SceneId, args.ExpectedRevision.Value),
                "scene.entity.set-test-position" when args.EntityId is not null && args.Position is not null =>
                    scene.SetTestPosition(args.SceneId, args.ExpectedRevision.Value, args.EntityId, args.Position),
                "scene.history.undo" when args.EntityId is not null =>
                    scene.UndoTestOperation(args.SceneId, args.ExpectedRevision.Value, args.EntityId),
                "scene.history.redo" when args.EntityId is not null =>
                    scene.RedoTestOperation(args.SceneId, args.ExpectedRevision.Value, args.EntityId),
                _ => throw new XycSceneOperationException("INVALID_INPUT")
            };
            return result(true, null, state, state.SceneRevision, null);
        }
        catch (XycSceneOperationException error) { return result(false, error.Code, null, null, null); }
        catch (ArgumentException) { return result(false, "INVALID_INPUT", null, null, null); }
        catch (InvalidOperationException) { return result(false, "NOT_READY", null, null, null); }
    }

    static XycSceneCommandArgs Args(XycRequest request) =>
        request.Args.Deserialize<XycSceneCommandArgs>(Json) ?? throw new JsonException();
}

public sealed class XycSceneOperationException(string code) : Exception
{
    public string Code { get; } = code;
}
