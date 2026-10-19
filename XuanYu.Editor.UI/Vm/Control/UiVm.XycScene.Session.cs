using XuanYu.Core.Identity;
using XuanYu.Core.Scene;
using XuanYu.Editor.UI.XycControl;
using XuanYu.World;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    internal sealed class XycSessionEntity(EntityId id, CommittedTransform initial)
    {
        public EntityId Id { get; } = id;
        public CommittedTransform Initial { get; } = initial;
        public CommittedTransform Expected { get; set; } = initial;
        public bool Present { get; set; } = true;
    }

    XycSceneState CreateXycSessionCube(string sceneId, long revision)
    {
        ValidateXycCreate(sceneId, revision);
        _xycSessionSceneId ??= SceneId;
        _xycSessionLifecycleRevision ??= _documentSession.LifecycleRevision;
        XycBeforeWorldMutationForTest?.Invoke();
        var before = _sceneState.Entities.Select(x => x.EntityKey).ToHashSet();
        WorldEntitySnapshot? added = null;
        try
        {
            added = _sceneState.AddCubeEntity();
            var created = added.Value;
            var delta = _sceneState.Entities.Where(x => !before.Contains(x.EntityKey)).ToArray();
            if (delta.Length != 1 || delta[0].Type != WorldEntityTypes.Cube || delta[0].EntityKey != created.EntityKey)
                throw new XycSceneOperationException("POSTCONDITION_FAILED");
            XycWorldCommittedBeforeHistoryAppendForTest?.Invoke();
            _historyOwner.PushEntry(new AddEntityHistoryEntry(created));
            TrackXycUndoTop(created.EntityKey);
            _xycSessionEntities.Add(created.EntityKey, new(created.EntityKey, created.Transform));
            _xycSessionEntityOrder.Add(created.EntityKey);
            SelectEntity(created.EntityKey, "XYC 创建立方体");
            RaiseDocumentChanged();
            CommitXycStage(created.EntityKey, creating: true);
            EnsureXycProjections();
            _xycCommitStatus = "COMMITTED";
            _xycRecoveryReason = null;
            return XycState();
        }
        catch (Exception ex)
        {
            if (added is { } created && _sceneState.TryGetEntity(created.EntityKey, out _))
                MarkXycInDoubt("WORLD_COMMITTED_HISTORY_UNCONFIRMED:" + ex.GetType().Name);
            throw;
        }
    }

    XycSceneState SetXycSessionPosition(string sceneId, long revision, string id, Vector3 position)
    {
        var key = ValidateXycOwnedTarget(sceneId, revision, id);
        if (!FiniteBounded(position.X) || !FiniteBounded(position.Y) || !FiniteBounded(position.Z))
            throw new XycSceneOperationException("INVALID_INPUT");
        var entity = _xycSessionEntities[key];
        var current = _sceneState.TryGetEntity(key, out var found) ? found : throw new XycSceneOperationException("ENTITY_NOT_FOUND");
        var target = current.Transform.WithPosition(new(position.X, position.Y, position.Z));
        if (target == current.Transform) throw new XycSceneOperationException("NO_CHANGE");
        XycBeforeWorldMutationForTest?.Invoke();
        if (!TryXycTransform(key, position)) throw new XycSceneOperationException("TRANSFORM_REJECTED");
        entity.Expected = target;
        CommitXycStage(key, creating: false);
        EnsureXycProjections();
        _xycCommitStatus = "COMMITTED";
        _xycRecoveryReason = null;
        return XycState();
    }

    void ValidateXycCreate(string sceneId, long revision)
    {
        ValidateXycSession(sceneId, revision);
        if (_xycSessionEntities.Count == 0 && (!string.IsNullOrEmpty(CurrentScenePath) || IsSceneDirty ||
            _sceneState.Entities.Count != 0 || _historyOwner.Count != 0 || _historyOwner.RedoCount != 0))
            throw new XycSceneOperationException("UNSAFE_SCENE");
        if (_sceneState.Entities.Any(x => !_xycSessionEntities.ContainsKey(x.EntityKey)))
            throw new XycSceneOperationException("UNAUTHORIZED_SCENE_STATE");
    }

    void CommitXycStage(EntityId key, bool creating)
    {
        if (_xycSessionEntities.Count == 1 && key == _xycSessionEntityOrder[0])
        {
            if (creating) { _xycTestEntityId = key.ToString(); _xycTestStage = 1; }
            else _xycTestStage = 2;
        }
    }

}
