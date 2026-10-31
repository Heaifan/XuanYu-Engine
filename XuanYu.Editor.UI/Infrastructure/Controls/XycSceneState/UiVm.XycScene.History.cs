using XuanYu.Core.History;
using XuanYu.Core.Identity;
using XuanYu.Editor.UI.XycControl;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    XycSceneState MoveXycHistory(string sceneId, long revision, string id, bool undo)
    {
        ValidateXycSession(sceneId, revision, historyConflict: true);
        var key = ParseXycEntityId(id);
        if (!_xycSessionEntities.TryGetValue(key, out var owned))
            throw new XycSceneOperationException("UNAUTHORIZED_ENTITY");
        var found = undo ? _historyOwner.TryPeekUndoAny(out var top) : _historyOwner.TryPeekRedoAny(out top);
        if (!found) throw new XycSceneOperationException("INVALID_STAGE");
        if (!TryDescribeXycEntry(top.Entry, out var operation, out var entryId) || entryId != key ||
            top.CurrentRevision != _historyOwner.CurrentRevision ||
            top.CurrentRevision != (undo ? top.AfterRevision : top.BeforeRevision))
            throw new XycSceneOperationException("HISTORY_CONFLICT");
        if (!_xycHistoryItems.Any(item => item.IdentifiesSameHistoryItem(top)))
            throw new XycSceneOperationException("HISTORY_CONFLICT");
        if (!XycHistoryPrecondition(owned, top.Entry, undo))
            throw new XycSceneOperationException("HISTORY_CONFLICT");
        var wasPresent = _sceneState.TryGetEntity(key, out var before);
        try
        {
            if (!ApplyHistory(top.Entry, undo)) throw new XycSceneOperationException("HISTORY_APPLY_FAILED");
            PublishSceneRenderSnapshot();
            XycWorldAppliedBeforeHistoryCompleteForTest?.Invoke();
            EnsureXycProjections();
            if (undo ? !_historyOwner.TryCompleteUndo(top) : !_historyOwner.TryCompleteRedo(top))
                throw new XycSceneOperationException("HISTORY_COMPLETION_CONFLICT");
        }
        catch (Exception ex)
        {
            var nowPresent = _sceneState.TryGetEntity(key, out var now);
            if (wasPresent != nowPresent || (wasPresent && nowPresent && before.Transform != now.Transform))
                MarkXycInDoubt("WORLD_APPLIED_HISTORY_UNCONFIRMED:" + ex.GetType().Name);
            throw;
        }
        UpdateXycExpected(owned, top.Entry, undo);
        UpdateXycLegacyStage(key, top.Entry, undo);
        _xycCommitStatus = "COMMITTED";
        _xycRecoveryReason = null;
        return XycState();
    }

    static bool TryDescribeXycEntry(object entry, out string operation, out EntityId id)
    {
        switch (entry)
        {
            case TransformHistoryEntry transform: operation = "TRANSFORM"; id = transform.EntityKey; return true;
            case AddEntityHistoryEntry add: operation = "CREATE"; id = add.Snapshot.EntityKey; return true;
            case DeleteEntityHistoryEntry delete: operation = "DELETE"; id = delete.Snapshot.EntityKey; return true;
            case RenameEntityHistoryEntry rename: operation = "RENAME"; id = rename.EntityKey; return true;
            default: operation = "UNKNOWN"; id = EntityId.None; return false;
        }
    }

    bool XycHistoryPrecondition(XycSessionEntity entity, object entry, bool undo) => entry switch
    {
        TransformHistoryEntry t => entity.Present && AtTransform(entity.Id, undo ? t.After : t.Before),
        AddEntityHistoryEntry add => add.Snapshot.Transform == entity.Initial && entity.Present == undo,
        _ => false
    };

    void UpdateXycExpected(XycSessionEntity entity, object entry, bool undo)
    {
        if (entry is TransformHistoryEntry t) entity.Expected = undo ? t.Before : t.After;
        else if (entry is AddEntityHistoryEntry) entity.Present = !undo;
    }

    void UpdateXycLegacyStage(EntityId key, object entry, bool undo)
    {
        if (_xycSessionEntities.Count != 1 || key.ToString() != _xycTestEntityId) return;
        if (entry is TransformHistoryEntry) _xycTestStage = undo
            ? (_xycTestStage == 4 ? 5 : 3) : 4;
        else if (entry is AddEntityHistoryEntry && undo && _xycTestStage == 5)
        {
            _xycTestStage = 6;
            _xycTestEntityId = null;
        }
    }
}
