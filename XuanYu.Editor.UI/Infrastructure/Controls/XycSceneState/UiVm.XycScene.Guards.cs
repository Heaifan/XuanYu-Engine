using XuanYu.Core.Identity;
using XuanYu.Core.History;
using XuanYu.Core.Scene;
using XuanYu.Editor.Transform;
using XuanYu.Editor.UI.XycControl;
using XuanYu.World;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool TryXycTransform(EntityId id, Vector3 position)
    {
        if (!_sceneState.TryGetEntity(id, out var entity) || !IdleXycInteraction()) return false;
        var next = entity.Transform.WithPosition(new(position.X, position.Y, position.Z));
        var command = new TransformCommand(0, id, next, _worldMutation.GetEntityMutationRevision(id));
        TransformCommit commit;
        try
        {
            if (!command.TryApply(_worldMutation, out commit) || !commit.Changed) return false;
        }
        catch
        {
            if (!_sceneState.TryGetEntity(id, out var current) || current.Transform != next) throw;
            commit = new TransformCommit(id, entity.Transform, current.Transform, true);
        }
        try { XycWorldCommittedBeforeHistoryAppendForTest?.Invoke(); }
        catch (Exception ex) { MarkXycInDoubt("WORLD_COMMITTED_HISTORY_NOT_APPENDED:" + ex.GetType().Name); throw; }
        try { RecordTransformHistory(commit); }
        catch (Exception ex)
        {
            if (!_sceneState.TryGetEntity(id, out var current) || current.Transform != next ||
                !_historyOwner.TryPeekUndoAny(out var top) || top.Entry is not TransformHistoryEntry history ||
                history.EntityKey != id || history.Before != entity.Transform || history.After != next)
            {
                MarkXycInDoubt("WORLD_COMMITTED_HISTORY_UNCONFIRMED:" + ex.GetType().Name);
                throw;
            }
        }
        TrackXycUndoTop(id);
        PublishSceneRenderSnapshot();
        return true;
    }

    void ValidateXycSession(string sceneId, long revision, bool historyConflict = false)
    {
        if (sceneId != SceneId) throw new XycSceneOperationException("SCENE_MISMATCH");
        if (_xycSessionSceneId is not null && _xycSessionSceneId != SceneId)
            throw new XycSceneOperationException("SCENE_MISMATCH");
        if (_xycSessionLifecycleRevision is not null &&
            _xycSessionLifecycleRevision != DocumentSession.LifecycleRevision)
            throw new XycSceneOperationException("SCENE_MISMATCH");
        if (_xycCommitStatus == "IN_DOUBT") throw new XycSceneOperationException("IN_DOUBT");
        if (revision != SceneRevision) throw new XycSceneOperationException("STALE_REVISION");
        if (IsRegionEditMode || IsDrawingTransactionActive || !IdleXycInteraction())
            throw new XycSceneOperationException(historyConflict ? "HISTORY_CONFLICT" : "UNSAFE_SCENE");
    }

    EntityId ValidateXycOwnedTarget(string sceneId, long revision, string id)
    {
        ValidateXycSession(sceneId, revision, historyConflict: true);
        var key = ParseXycEntityId(id);
        if (!_xycSessionEntities.TryGetValue(key, out var owned))
            throw new XycSceneOperationException("UNAUTHORIZED_ENTITY");
        if (!owned.Present || !_sceneState.TryGetEntity(key, out var entity) || entity.Transform != owned.Expected)
            throw new XycSceneOperationException("HISTORY_CONFLICT");
        return key;
    }

    bool AtTransform(EntityId id, CommittedTransform expected) =>
        _sceneState.TryGetEntity(id, out var entity) && entity.Transform == expected;

    void MarkXycInDoubt(string reason)
    {
        _xycCommitStatus = "IN_DOUBT";
        _xycRecoveryReason = reason;
    }

    void EnsureXycProjections()
    {
        var world = _sceneState.Entities.Select(x => x.EntityKey.ToString()).Order().ToArray();
        var hierarchy = BuildHierarchyItems().Where(x => x.IsEntity).Select(x => x.Key).Order().ToArray();
        var render = RenderSnapshot.Entities.Select(x => x.EntityKey.ToString()).Order().ToArray();
        if (!world.SequenceEqual(hierarchy) || !world.SequenceEqual(render))
            throw new XycSceneOperationException("PROJECTION_MISMATCH");
    }

    bool IdleXycInteraction() => _editorState.InteractionSnapshot.Phase == EditorInteractionPhase.Idle;
    static bool FiniteBounded(double value) => double.IsFinite(value) && Math.Abs(value) <= 10_000;
}
