using XuanYu.Core.Identity;
using XuanYu.Core.History;
using XuanYu.Editor.UI.XycControl;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly Dictionary<EntityId, XycSessionEntity> _xycSessionEntities = [];
    readonly List<EntityId> _xycSessionEntityOrder = [];
    readonly List<HistorySnapshot> _xycHistoryItems = [];
    string? _xycSessionSceneId;
    long? _xycSessionLifecycleRevision;
    string? _xycTestEntityId;
    int _xycTestStage;
    string _xycCommitStatus = "NOT_APPLIED";
    string? _xycRecoveryReason;
    internal Action? XycBeforeWorldMutationForTest { get; set; }
    internal Action? XycWorldCommittedBeforeHistoryAppendForTest { get; set; }
    internal Action? XycWorldAppliedBeforeHistoryCompleteForTest { get; set; }

    internal XycSceneState InspectXycTestScene() => XycState();
    internal XycEntity[] ListXycSceneEntities() => _sceneState.Entities.Select(ToXycEntity).ToArray();

    internal XycEntity InspectXycSceneEntity(string id)
    {
        var key = ParseXycEntityId(id);
        return _sceneState.TryGetEntity(key, out var entity)
            ? ToXycEntity(entity) : throw new XycSceneOperationException("ENTITY_NOT_FOUND");
    }

    internal XycSceneState CreateXycTestCube(string sceneId, long revision) =>
        CreateXycSessionCube(sceneId, revision);

    internal XycSceneState SetXycTestPosition(string sceneId, long revision,
        string id, Vector3 position) => SetXycSessionPosition(sceneId, revision, id, position);

    internal XycSceneState UndoXycTestOperation(string sceneId, long revision, string id) =>
        MoveXycHistory(sceneId, revision, id, undo: true);

    internal XycSceneState RedoXycTestOperation(string sceneId, long revision, string id) =>
        MoveXycHistory(sceneId, revision, id, undo: false);
}
