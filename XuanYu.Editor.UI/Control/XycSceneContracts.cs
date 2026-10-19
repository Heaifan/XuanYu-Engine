using System.Text.Json;

namespace XuanYu.Editor.UI.XycControl;

public sealed record XycEntity(string EntityId, string Type, string Name,
    XycTransform Transform);
public sealed record XycTransform(Vector3 Position, Vector3 Rotation, Vector3 Scale);
public sealed record XycInspectorField(string Label, string Value);
public sealed record XycHistorySnapshot(string OperationType, string? EntityId,
    long BeforeRevision, long AfterRevision, long CurrentRevision);
public sealed record XycSceneTransactionFailure(string Status, string RecoveryReason,
    XycSceneState? ObservedState);
public sealed record XycSceneState(string SceneId, long SceneRevision, int EntityCount,
    string[] EntityIds, string[] HierarchyEntityIds, string[] RenderEntityIds,
    XycInspectorField[] InspectorFields, string ScenePath, bool HasActiveInteraction, int UndoCount,
    int RedoCount, string? TestEntityId, string Stage,
    string[]? SessionEntityIds = null, string? InspectorEntityId = null,
    XycHistorySnapshot? UndoTop = null, XycHistorySnapshot? RedoTop = null,
    string CommitStatus = "NOT_APPLIED", string? RecoveryReason = null);
public sealed record XycSceneRequest(string SceneId, long ExpectedRevision,
    string? EntityId = null, Vector3? Position = null);

public interface IXycSceneRuntime
{
    string SceneInstanceId { get; }
    XycSceneState InspectTestScene();
    XycEntity[] ListSceneEntities();
    XycEntity InspectSceneEntity(string entityId);
    XycSceneState CreateTestCube(string expectedSceneId, long expectedRevision);
    XycSceneState SetTestPosition(string expectedSceneId, long expectedRevision,
        string entityId, Vector3 position);
    XycSceneState UndoTestOperation(string expectedSceneId, long expectedRevision,
        string entityId);
    XycSceneState RedoTestOperation(string expectedSceneId, long expectedRevision,
        string entityId);
}

public sealed record XycSceneCommandArgs(string? SceneId, long? ExpectedRevision,
    string? EntityId, Vector3? Position);
