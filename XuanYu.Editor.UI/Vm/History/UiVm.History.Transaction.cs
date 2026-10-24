using XuanYu.Core.History;
using XuanYu.Core.Identity;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool TryApplyHistoryAndComplete(HistorySnapshot top, bool undo)
    {
        try
        {
            if (!ApplyHistory(top.Entry, undo) ||
                !(undo ? _historyOwner.TryCompleteUndo(top) : _historyOwner.TryCompleteRedo(top)))
            {
                FenceAppliedXycHistory(top.Entry, undo, "HISTORY_COMPLETION_FAILED");
                FooterMessage = "撤销/重做未能确认；历史栈保持待核验状态。";
                return false;
            }
            return true;
        }
        catch (Exception error)
        {
            FenceAppliedXycHistory(top.Entry, undo, "HISTORY_APPLY_UNCONFIRMED:" + error.GetType().Name);
            FooterMessage = "撤销/重做异常；历史栈未移动，请检查场景状态。";
            return false;
        }
    }

    void FenceAppliedXycHistory(object entry, bool undo, string reason)
    {
        var id = entry switch
        {
            TransformHistoryEntry value => value.EntityKey,
            AddEntityHistoryEntry value => value.Snapshot.EntityKey,
            DeleteEntityHistoryEntry value => value.Snapshot.EntityKey,
            RenameEntityHistoryEntry value => value.EntityKey,
            _ => EntityId.None
        };
        if (!_xycSessionEntities.ContainsKey(id) || !HistoryTargetObserved(entry, undo)) return;
        MarkXycInDoubt(reason);
    }

    bool HistoryTargetObserved(object entry, bool undo) => entry switch
    {
        TransformHistoryEntry value => _sceneState.TryGetEntity(value.EntityKey, out var entity) &&
            entity.Transform == (undo ? value.Before : value.After),
        AddEntityHistoryEntry value => _sceneState.TryGetEntity(value.Snapshot.EntityKey, out _) != undo,
        DeleteEntityHistoryEntry value => _sceneState.TryGetEntity(value.Snapshot.EntityKey, out _) == undo,
        RenameEntityHistoryEntry value => _sceneState.TryGetEntity(value.EntityKey, out var entity) &&
            entity.Name == (undo ? value.Before : value.After),
        _ => false
    };
}
